using BTX2.Model;
using BTX2.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BTX2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SongInfoPage : ContentPage
	{
        Song CurSong;
        SongInfoService SongInfoSvc;
		public SongInfoPage (Song SelSong)
		{
            CurSong = SelSong;
            SongInfoSvc = new SongInfoService();
			InitializeComponent ();
		}

        private void OnGotoGPMClicked()
        {
            Device.OpenUri(new Uri(SongInfo.Text));
        }

        protected override void OnAppearing()
		{
			base.OnAppearing();

            TitleLabel.Text = CurSong.Title;
            ArtistLabel.Text = CurSong.Artist;
            CurrentRankLabel.Text = CurSong.CurrentRank.ToString();
            PrevRankLabel.Text = CurSong.PreviousRank.ToString();
            PeakPositionLabel.Text = CurSong.PeakPosition.ToString();
            WeeksOnChartLabel.Text = CurSong.WeeksOnChart.ToString();
            String SongArtistTitle;
            SongArtistTitle = CurSong.Artist + " " + CurSong.Title;
            string VideoID = SongInfoSvc.GetFirstYouTubeSearchResult(WebUtility.HtmlEncode(SongArtistTitle.Replace(' ', '+')));
            string YouTubeSearchResult = "https://m.youtube.com" + "/watch?v=" + VideoID;
            SongInfo.Text = YouTubeSearchResult;
            //SongInfo.Text = "https://play.google.com/music/listen#/sr/" + WebUtility.HtmlEncode(SongArtistTitle.Replace(' ', '+'));
            webView.Source = YouTubeSearchResult;
        }
	}
}