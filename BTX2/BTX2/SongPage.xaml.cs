using BTX2.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BTX2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SongPage : ContentPage
	{
        SongViewModel myViewModel;
		public SongPage ()
		{
            myViewModel = new SongViewModel();
			InitializeComponent ();
            listView.BindingContext = myViewModel;
            listView.ItemsSource = myViewModel.SongObsCol;
            listView.SetBinding(ListView.IsRefreshingProperty,
                            new Binding("IsBusy", BindingMode.OneWay));
            listView.SetBinding(ListView.RefreshCommandProperty, new Binding("LoadSongCommand"));

		}
        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return; // has been set to null, do not 'process' tapped event
            //Chart SelChart = (Chart)e.SelectedItem;
            //AppSongPage.ChartURL = SelChart.ChartURL;
            //SongsPage.ChartURL = SelChart.ChartURL;
            //ChartDB.Instance.SetCurSelectedChart(selChart.ChartNumber);
            //SongDB.Instance.FillFromURL(selChart.ChartURL);
            MessagingCenter.Send<SongPage>(this, "SongSelected");
            ((ListView)sender).SelectedItem = null; // de-select the row
        }

        public void LoadChart(string SelChartURL, string LastUpdatedDateTime)
        {
            myViewModel.ChartURL = SelChartURL;
            myViewModel.LastUpdatedDateTime = LastUpdatedDateTime;
            myViewModel.LoadSongCommand.Execute(null);
        }
	}
}