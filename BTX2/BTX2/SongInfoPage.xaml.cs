using BTX2.Model;
using BTX2.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BTX2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SongInfoPage : ContentPage
	{
        Account account;
        AccountStore store;
        Song CurSong;
        SongInfoService SongInfoSvc;
		public SongInfoPage (Song SelSong)
		{
            CurSong = SelSong;
            SongInfoSvc = new SongInfoService();
			InitializeComponent ();
            store = AccountStore.Create();
            account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();

		}

        private void OnGotoGPMClicked()
        {
            Device.OpenUri(new Uri(SongInfo.Text));
        }

        public void OnGLoginClick (Object sender,EventArgs e)
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constants.iOSClientId;
                    redirectUri = Constants.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = Constants.AndroidClientId;
                    redirectUri = Constants.AndroidRedirectUrl;
                    break;
            }

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                Constants.Scope,
                new Uri(Constants.AuthorizeUrl),
                new Uri(redirectUri),
                new Uri(Constants.AccessTokenUrl),
                null,
                true);

            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);

        }
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}

            User user = null;
			if (e.IsAuthenticated)
			{
				// If the user is authenticated, request their basic user data from Google
				// UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
				var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, e.Account);
				var response = await request.GetResponseAsync();
				if (response != null)
				{
					// Deserialize the data and store it in the account store
					// The users email address will be used to identify data in SimpleDB
					string userJson = await response.GetResponseTextAsync();
					user = JsonConvert.DeserializeObject<User>(userJson);
				}

				if (account != null)
				{
					store.Delete(account, Constants.AppName);
				}

                await store.SaveAsync(account = e.Account, Constants.AppName);
                await DisplayAlert("Email address", user.Email, "OK");
			}
		}

		void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}

			Debug.WriteLine("Authentication error: " + e.Message);
		}

        protected override void OnAppearing()
		{
			base.OnAppearing();
            string token = account.Properties ["access_token"];

            TitleLabel.Text = CurSong.Title;
            ArtistLabel.Text = CurSong.Artist;
            CurrentRankLabel.Text = CurSong.CurrentRank.ToString();
            PrevRankLabel.Text = CurSong.PreviousRank.ToString();
            PeakPositionLabel.Text = CurSong.PeakPosition.ToString();
            WeeksOnChartLabel.Text = CurSong.WeeksOnChart.ToString();
            String SongArtistTitle;
            SongArtistTitle = CurSong.Artist + " " + CurSong.Title;
            string VideoID = SongInfoSvc.GetFirstYouTubeSearchResult(WebUtility.HtmlEncode(SongArtistTitle.Replace(' ', '+')), token);
            string YouTubeSearchResult = "https://m.youtube.com";
            if (VideoID != "") {
                YouTubeSearchResult = YouTubeSearchResult + "/watch?v=" + VideoID;
            }
            SongInfo.Text = YouTubeSearchResult;
            //SongInfo.Text = "https://play.google.com/music/listen#/sr/" + WebUtility.HtmlEncode(SongArtistTitle.Replace(' ', '+'));
            webView.Source = YouTubeSearchResult;
        }
	}
}