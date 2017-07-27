using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace BTX2
{
	public partial class App : Application
	{
		public App ()
		{
            SetupDB();

            InitializeComponent();

            //MainPage = new BTX2.MainPage();
            SetMainPage();
		}
        public static void SetupDB()
        {
            Config.Instance.dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "BTXdb.db3");
        }
		public static void SetMainPage()
		{
            Current.MainPage = new TabbedPage
            {
                Children =
                {
                    new NavigationPage(new ChartsPage())
                    {
                        Title = "Charts"
                    },
                    new NavigationPage(new SongsPage())
                    {
                        Title = "Songs"
                    },
                }
            };
        }
		protected override void OnStart ()
		{
			// Handle when your app starts
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
