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
using System.Diagnostics;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace BTX2.Service
{
    public class SongInfoService
    {
        public SongInfoService()
        {

        }
        public string GetFirstYouTubeSearchResult(string SearchString, string BearerToken)
        {

        }
        private async Task Run()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                  GoogleClientSecrets.Load(stream).Secrets,
        // This OAuth 2.0 access scope allows for full read/write access to the
        // authenticated user's account.
                        new[] { YouTubeService.Scope.Youtube },
                  "user",
                  CancellationToken.None,
                  new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            // Create a new, private playlist in the authorized user's channel.
            var newPlaylist = new Playlist();
            newPlaylist.Snippet = new PlaylistSnippet();
            newPlaylist.Snippet.Title = "Test Playlist";
            newPlaylist.Snippet.Description = "A playlist created with the YouTube API v3";
            newPlaylist.Status = new PlaylistStatus();
            newPlaylist.Status.PrivacyStatus = "public";
            newPlaylist = await youtubeService.Playlists.Insert(newPlaylist, "snippet,status").ExecuteAsync();

            // Add a video to the newly created playlist.
            var newPlaylistItem = new PlaylistItem();
            newPlaylistItem.Snippet = new PlaylistItemSnippet();
            newPlaylistItem.Snippet.PlaylistId = newPlaylist.Id;
            newPlaylistItem.Snippet.ResourceId = new ResourceId();
            newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
            newPlaylistItem.Snippet.ResourceId.VideoId = "GNRMeaz6QRI";
            newPlaylistItem = await youtubeService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

            Console.WriteLine("Playlist item id {0} was added to playlist id {1}.", newPlaylistItem.Id, newPlaylist.Id);
        }
    }
    public string GetFirstYouTubeSearchResultOLD(string SearchString, string BearerToken)
    {
        string YouTubeSearchURL = "https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=1";
        YouTubeSearchURL = YouTubeSearchURL + "&q=" + SearchString;// + "&key=AIzaSyB6xylSo9qhmTyyUtuF28OCOtcZPZMb_7o";
        string FirstVideoID = "";

        try
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(YouTubeSearchURL));
            request.ContentType = "application/json";
            request.Headers.Add("Authorization: Bearer " + BearerToken);
            request.Method = "GET";

            //// Send the request to the server and wait for the response:
            WebResponse response = request.GetResponse();
            Stream ReceiveStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader ReadStream = new StreamReader(ReceiveStream, encode);

            string Line;
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
                FirstVideoID = Line.Substring(FirstVideoIDTagStart, FirstVideoIDTagEnd - FirstVideoIDTagStart - 4).Replace("\"", "");
            }
            ReadStream.Close();
            response.Close();
        }
        catch (Exception AnyException)
        {
            Debug.WriteLine(AnyException.Message);
        }
        return FirstVideoID;

    }
}
}
