using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace BTX2
{
	public partial class App : Application
	{
		public App ()
		{
            SetupDB();
            SetupBillboardURL();

            InitializeComponent();

            //MainPage = new BTX2.MainPage();
            SetMainPage();
		}
        public static void SetupDB()
        {
            Config.Instance.dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "BTXdb.db3");
        }
        public static void SetupBillboardURL()
        {
            Config.Instance.BillboardChartsURL = "http://www.billboard.com/charts";
        }
		public static void SetMainPage()
		{
            SongPage CurSongPage = new SongPage();
            
            Current.MainPage = new TabbedPage
            {
                Children =
                {
                    new NavigationPage(new ChartPage(CurSongPage))
                    {
                        Title = "Charts"
                    },
                    new NavigationPage(CurSongPage)
                    {
                        Title = "Songs"
                    },
                }
            };
        }
		protected override void OnStart ()
		{
			// Handle when your app starts
                    MobileCenter.Start("android=d088e7de-5cd1-41ee-a623-66c735d5c787;",
                   typeof(Analytics), typeof(Crashes));
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
