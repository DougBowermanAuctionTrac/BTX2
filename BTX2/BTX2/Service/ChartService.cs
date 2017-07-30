using BTX2.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BTX2.Service
{
    public class ChartService
    {
        private List<Chart> AllCharts;
        public ChartService()
        {
            AllCharts = new List<Chart>();
            Config.Instance.BillboardChartsURL = "http://www.billboard.com/charts";
            CreateAndFillChartsTable();
            
        }
        private void CreateAndFillChartsTable()
        {
            SQLiteConnection db = new SQLiteConnection(Config.Instance.dbPath);
            db.CreateTable<Chart>();
            TableQuery<Chart> table = db.Table<Chart> ();
            if (table.Count() == 0)
            {
                FillAllChartsFromBillboard();
                FillChartsTableWithAllCharts(db);
            }
            else
            {
                foreach (var c in table) {
                    AllCharts.Add(c);
                }
            }
            db.Close();

        }
        public void FillAllChartsFromBillboard()
        {
            UriBuilder myBuilder = new UriBuilder(Config.Instance.BillboardChartsURL);
            myBuilder.Path = String.Empty;
            Uri wholeUri = myBuilder.Uri;
            string baseUrl = wholeUri.ToString();

            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(Config.Instance.BillboardChartsURL));
            request.ContentType = "application/json";
            request.Method = "GET";

            //// Send the request to the server and wait for the response:
            WebResponse response = request.GetResponse();
            Stream ReceiveStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader ReadStream = new StreamReader(ReceiveStream, encode);

            String Line;
            String ChartTag = "<a href=\"";
            int ChartNumber = 1;
            int ChartTagLen = ChartTag.Length;
            int ChartTagStart = 0;
            int ChartTagEnd = 0;
            String ChartURL;
            String ChartTitle;
            int CategoryFieldStart = 0;
            int CategoryFieldEnd = 0;
            String CurrentChartCategory = "";

            // Read the stream to a string, and write the string to the console.
            while (ReadStream.Peek() > 0)
            {
                Line = ReadStream.ReadLine();
                if (Line.Contains("id=\"id-chart-category"))
                {
                    CategoryFieldStart = Line.IndexOf(">") + 1;
                    CategoryFieldEnd = Line.IndexOf("</");
                    CurrentChartCategory = Line.Substring(CategoryFieldStart, CategoryFieldEnd - CategoryFieldStart);
                }
                if ((Line.Contains("<span class=\"field-content\">")) || (Line.Contains("<span class=\"field-content\">")))
                {
                    ChartTagStart = Line.IndexOf(ChartTag);
                    ChartTagEnd = Line.IndexOf("\">", ChartTagStart);
                    ChartURL = baseUrl + Line.Substring(ChartTagStart + ChartTagLen + 1, ChartTagEnd - ChartTagStart - ChartTagLen - 1);
                    ChartTitle = Line.Substring(ChartTagEnd + 2, Line.IndexOf("</a>") - ChartTagEnd - 2);
                    //Console.WriteLine(chartURL + ":" + chartTitle);
                    Chart NewChart = new Chart();
                    NewChart.ChartNumber = ChartNumber;
                    NewChart.LastUpdatedDateTimeString = "";
                    NewChart.ChartCategory = CurrentChartCategory;
                    NewChart.ChartURL = ChartURL;
                    NewChart.ChartTitle = WebUtility.HtmlDecode(ChartTitle);
                    NewChart.Favorite = 0;
                    NewChart.Hide = 0;
                    AllCharts.Add(NewChart);
                    ChartNumber++;
                }
            }
            ReadStream.Close();
            response.Close();

        }
        private void FillChartsTableWithAllCharts(SQLiteConnection db)
        {
            db.Query<Chart>("DELETE FROM Chart");
            foreach (Chart curChart in AllCharts)
            {
                db.Insert (curChart);
            }

        }
        public List<Chart> ChartSelect(string Filter)
        {
            if (Filter == "Fav")
            {
                return AllCharts.Where(v => v.Favorite == 1).ToList();
            }
            else if (Filter == "Vis")
            {
                return AllCharts.Where(v => v.Hide == 0).ToList();
            }
            // Covers All and anything else
            return AllCharts;
        }
        public void ChartUpdate(Chart CurChart)
        {
            Chart DstChart = AllCharts.FirstOrDefault(x => x.ChartNumber == CurChart.ChartNumber);
            if (DstChart != null)
            {
                DstChart.Favorite = CurChart.Favorite;
                DstChart.Hide = CurChart.Hide;
                ChartDBUpdate(DstChart);
            }
        }
        private void ChartDBUpdate(Chart CurChart)
        {
            SQLiteConnection db = new SQLiteConnection(Config.Instance.dbPath);
            db.Update(CurChart);
            db.Close();
        }
    }
}
