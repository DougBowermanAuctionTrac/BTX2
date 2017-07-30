using BTX2.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BTX2.Service
{
    public class SongService
    {
        private List<Song> AllSongs;
        public SongService()
        {
            AllSongs = new List<Song>();
            CreateAndFillSongTable();
        }
        private void CreateAndFillSongTable()
        {
            SQLiteConnection db = new SQLiteConnection(Config.Instance.dbPath);
            db.CreateTable<Song>();
            TableQuery<Song> table = db.Table<Song> ();
            if (table.Count() != 0)
            {
                foreach (var s in table) {
                    AllSongs.Add(s);
                }
            }
            db.Close();

        }
        public List<Song> SongSelect(string ChartURL, ref string LastUpdatedDateTime)
        {
            List<Song> CurSongs = new List<Song>();
            CurSongs = AllSongs.Where(x => x.ChartURL == ChartURL).ToList();

            DateTime LastUpdated;
            if (DateTime.TryParse(LastUpdatedDateTime, out LastUpdated))
            {
                // Has been updated in last 6 hours
                if (LastUpdated > DateTime.Now.AddHours(-6))
                {
                    if (CurSongs.Count == 0)
                    {
                        FillCurSongsFromBillboardURL(ChartURL, ref CurSongs);
                        FillAllSongsWithCurSongs(ChartURL, CurSongs);
                        FillSongTableWithCurSongs(ChartURL, CurSongs);
                        LastUpdatedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                }
                else
                {
                    FillCurSongsFromBillboardURL(ChartURL, ref CurSongs);
                    FillAllSongsWithCurSongs(ChartURL, CurSongs);
                    FillSongTableWithCurSongs(ChartURL, CurSongs);
                    LastUpdatedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
            }
            if (CurSongs.Count == 0)
            {
                FillCurSongsFromBillboardURL(ChartURL, ref CurSongs);
                FillAllSongsWithCurSongs(ChartURL, CurSongs);
                FillSongTableWithCurSongs(ChartURL, CurSongs);
                LastUpdatedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }

            return CurSongs;
        }
        private void FillCurSongsFromBillboardURL(string ChartURL, ref List<Song> CurSongs)
        {
            CurSongs.Clear();
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(ChartURL));
            request.ContentType = "application/json";
            request.Method = "GET";

            //// Send the request to the server and wait for the response:
            WebResponse response = request.GetResponse();
            Stream ReceiveStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader ReadStream = new StreamReader(ReceiveStream, encode);

            int SongNum = 0;
            string Line;
            string CurrentRank = "";
            string PreviousRank = "";
            string PeakRank = "";
            string WeeksOnChart = "";

            string TitleTag = "data-songtitle=\"";
            string ArtistTag = ">";
            string Title = "";
            string Artist = "";

            bool bGotTitle = false;
            bool bGotArtist = false;
            bool bGotChartRowRank = false;
            bool bGotRowStats = false;


            // Read the stream to a string, and write the string to the console.
            while (ReadStream.Peek() > 0)
            {
                bGotTitle = false;
                bGotArtist = false;
                bGotChartRowRank = false;
                bGotRowStats = false;
                Title = "";
                Artist = "";
                CurrentRank = "0";
                PreviousRank = "0";
                PeakRank = "0";
                WeeksOnChart = "0";

                Line = ReadStream.ReadLine();
                if ((Line == null) || (Line == ""))
                    continue;
                //if (Line.StartsWith(ChartTitleTag))
                //{
                //    ChartTitle = Line.Substring(Line.IndexOf(ChartTitleTag) + ChartTitleTag.Length, Line.Length - (ChartTitleTag.Length * 2) - 1);
                //}
                if (Line.Contains("<article class=\"chart-row"))
                {
                    //Console.WriteLine(Line);
                    int TitlePos = Line.IndexOf(TitleTag) + TitleTag.Length;
                    Title = Line.Substring(TitlePos, Line.IndexOf('"', TitlePos) - TitlePos);
                    bGotTitle = true;
                    SongNum++;
                    while (Line != "</article>")
                    {
                        try
                        {
                            Line = ReadStream.ReadLine();
                            if ((Line == null) || (Line == ""))
                                break;
                            if (Line.Contains("class=\"chart-row__current-week\""))
                            {
                                CurrentRank = Line.Substring(Line.IndexOf(">") + 1, Line.IndexOf("</") - Line.IndexOf(">") - 1);
                                bGotChartRowRank = true;
                            }
                            if (Line.Contains("class=\"chart-row__artist\""))
                            {
                                // The artist is on the next line, and is the WHOLE line
                                Artist = ReadStream.ReadLine();
                                bGotArtist = true;
                            }
                            if (Line.Contains("class=\"chart-row__stats\""))
                            {
                                bGotRowStats = true;
                            }
                            if ((bGotRowStats) && (Line.Contains("class=\"chart-row__last-week\"")))
                            {
                                Line = ReadStream.ReadLine(); // Label
                                Line = ReadStream.ReadLine(); // Value
                                PreviousRank = Line.Substring(Line.IndexOf(">") + 1, Line.IndexOf("</") - Line.IndexOf(">") - 1);
                            }
                            if ((bGotRowStats) && (Line.Contains("class=\"chart-row__top-spot\"")))
                            {
                                Line = ReadStream.ReadLine(); // Label
                                Line = ReadStream.ReadLine(); // Value
                                PeakRank = Line.Substring(Line.IndexOf(">") + 1, Line.IndexOf("</") - Line.IndexOf(">") - 1);
                            }
                            if ((bGotRowStats) && (Line.Contains("class=\"chart-row__weeks-on-chart\"")))
                            {
                                Line = ReadStream.ReadLine(); // Label
                                Line = ReadStream.ReadLine(); // Value
                                WeeksOnChart = Line.Substring(Line.IndexOf(">") + 1, Line.IndexOf("</") - Line.IndexOf(">") - 1);
                            }
                        }
                        catch (Exception AnyException)
                        {
                            Debug.WriteLine(AnyException.Message);
                            break;
                        }
                    }
                    // At end of article then fill song
                    Song mySong = new Song();
                    mySong.ChartURL = ChartURL;
                    mySong.Position = SongNum;
                    mySong.CurrentRank = SongNum;
                    mySong.PreviousRank = 0;
                    mySong.PeakPosition = 0;
                    mySong.WeeksOnChart = 0;
                    if (bGotTitle)
                    {
                        mySong.Title = WebUtility.HtmlDecode(Title);
                    }
                    if (bGotArtist)
                    {
                        mySong.Artist = WebUtility.HtmlDecode(Artist);
                    }
                    if (bGotChartRowRank)
                    {
                        try
                        {
                            mySong.CurrentRank = Int32.Parse(CurrentRank);
                        }
                        catch (Exception AnyException)
                        {
                            //Console.WriteLine(AnyException.Message);
                        }
                    }
                    if (bGotRowStats)
                    {
                        try
                        {
                            mySong.PreviousRank = Int32.Parse(PreviousRank);
                        }
                        catch (Exception AnyException)
                        {
                            //Console.WriteLine(AnyException.Message);
                        }
                        try
                        {
                            mySong.PeakPosition = Int32.Parse(PeakRank);
                        }
                        catch (Exception AnyException)
                        {
                            //Console.WriteLine(AnyException.Message);
                        }
                        try
                        {
                            mySong.WeeksOnChart = Int32.Parse(WeeksOnChart);
                        }
                        catch (Exception AnyException)
                        {
                            //Console.WriteLine(AnyException.Message);
                        }
                    }
                    CurSongs.Add(mySong);
                }
            }

        }


        private void FillAllSongsWithCurSongs(string ChartURL, List<Song> CurSongs)
        {
            AllSongs.RemoveAll(s => s.ChartURL == ChartURL);
            foreach(Song CurSong in CurSongs)
            {
                AllSongs.Add(CurSong);
            }
        }

        private void FillSongTableWithCurSongs(string ChartURL, List<Song> CurSongs)
        {
            try
            {
                SQLiteConnection db = new SQLiteConnection(Config.Instance.dbPath);
                db.Query<Song>("DELETE FROM Song WHERE ChartURL = '" + ChartURL + "'");
                foreach (Song curSong in CurSongs)
                {
                    db.Insert (curSong);
                }
                db.Close();

            }
            catch (Exception AnyException)
            {
                Debug.WriteLine(AnyException.Message);
            }
        }
    }
}
