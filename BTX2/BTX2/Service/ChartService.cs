using BTX2.Model;
using HtmlAgilityPack;
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
                    //c.FavHide = c.Favorite.ToString() + "|" + c.Hide.ToString();

                    AllCharts.Add(c);
                }
            }
            db.Close();

        }
        public void FillAllChartsFromBillboard()
        {
            string ChartCategory = "";
            string ChartLink = "";
            int CategoryNum;
            int ChartNum;
            //var doc = new HtmlDocument();
            //doc.Load("D:\\Downloads\\curl_754_0\\bbcharts2.html");

            //// From String
            //var doc = new HtmlDocument();
            //doc.LoadHtml(html);

            // From Web
            var url = Config.Instance.BillboardChartsURL;
            var web = new HtmlWeb();
            web.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko";
            var doc = web.Load(url);

            CategoryNum = 0;
            ChartNum = 0;

            var titlenode = doc.DocumentNode.SelectSingleNode("//head/title");
            string ChartTitle = titlenode.InnerText;

            //var usertextnodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'usertext-body')]");
            HtmlNode articlenode = doc.DocumentNode.SelectSingleNode("//article");
                
            foreach (HtmlNode node in articlenode.Descendants())
            {
                if (node.Id.StartsWith("id-chart-category")) {
                    CategoryNum++;
                    ChartCategory = node.InnerText;
                    //outputFile.WriteLine(ChartCategory);
                    //WriteInsomniaFolder(InsommniaBillboardCharts, CategoryNum, ChartCategory);

                }
                foreach (var attrib in node.Attributes)
                {
                    if (attrib.Name == "class")
                    {
                        if (attrib.Value.StartsWith("views-row"))
                        {
                            ChartLink = "";
                            foreach(HtmlNode rowNode in node.Descendants())
                            {
                                if ((rowNode.Name == "a") && (ChartLink == ""))
                                {
                                    ChartNum++;
                                    foreach (var rowAttrib in rowNode.Attributes)
                                    {
                                        if (rowAttrib.Name == "href")
                                        {
                                            ChartLink = rowAttrib.Value;
                                            break;
                                        }
                                    }
                                    Chart NewChart = new Chart();
                                    NewChart.ChartNumber = ChartNum;
                                    NewChart.LastUpdatedDateTimeString = "";
                                    NewChart.ChartCategory = ChartCategory;
                                    NewChart.ChartURL = Config.Instance.BillboardChartsURL + ChartLink;
                                    NewChart.ChartTitle = WebUtility.HtmlDecode(ChartTitle);
                                    NewChart.Favorite = 0;
                                    NewChart.Hide = 0;
                                    //NewChart.FavHide = "0|0";
                                    AllCharts.Add(NewChart);

                                    //outputFile.WriteLine("http://www.billboard.com" + ChartLink + ":" + WebUtility.HtmlDecode(rowNode.InnerText));
                                    //WriteInsomniaRequest(InsommniaBillboardCharts, CategoryNum, ChartNum, WebUtility.HtmlDecode(rowNode.InnerText), ChartLink);
                                }
                            }
                        }
                    }
                }
            }
            // done reading charts
        }
        public void FillAllChartsFromBillboardOLD()
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
                    //NewChart.FavHide = "0|0";
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
                //curChart.FavHide = curChart.Favorite.ToString() + "|" + curChart.Hide.ToString();

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
                //DstChart.FavHide = CurChart.Favorite.ToString() + "|" + CurChart.Hide.ToString();
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
