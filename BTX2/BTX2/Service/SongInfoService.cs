using BTX2.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BTX2.Service
{
    public class SongInfoService
    {
        public SongInfoService()
        {
         
        }
        public string GetFirstYouTubeSearchResult(string SearchString)
        {
            string YouTubeSearchURL = "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=1";
            YouTubeSearchURL = YouTubeSearchURL + "&q=" + SearchString + "&key=AIzaSyB6xylSo9qhmTyyUtuF28OCOtcZPZMb_7o";
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(YouTubeSearchURL));
            request.ContentType = "application/json";
            request.Method = "GET";

            //// Send the request to the server and wait for the response:
            WebResponse response = request.GetResponse();
            Stream ReceiveStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader ReadStream = new StreamReader(ReceiveStream, encode);

            string Line;
            string FirstVideoID = "";
            string VideoIDTag = "\"videoId\": ";
            int FirstVideoIDTagStart = 0;
            int FirstVideoIDTagEnd = 0;
            // Read the stream to a string, and write the string to the console.
            if (ReadStream.Peek() > 0)
            {
                Line = ReadStream.ReadToEnd();
                //20Product deserializedProduct = JsonConvert.DeserializeObject<Product>(output);
                FirstVideoIDTagStart = Line.IndexOf(VideoIDTag) + VideoIDTag.Length;
                FirstVideoIDTagEnd = Line.IndexOf("}", FirstVideoIDTagStart);
                FirstVideoID = Line.Substring(FirstVideoIDTagStart, FirstVideoIDTagEnd - FirstVideoIDTagStart -4).Replace("\"", "");
            }
            ReadStream.Close();
            response.Close();
            return FirstVideoID;

        }
    }
}
