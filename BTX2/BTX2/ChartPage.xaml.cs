using BTX2.Model;
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
	public partial class ChartPage : ContentPage
	{
        ChartViewModel myViewModel;
        private SongPage AppSongPage;
		public ChartPage (SongPage CurSongPage)
		{
            AppSongPage = CurSongPage;
            myViewModel = new ChartViewModel();
			InitializeComponent ();
            listView.BindingContext = myViewModel;
            listView.ItemsSource = myViewModel.ChartObsCol;
            listView.SetBinding(ListView.IsRefreshingProperty,
                            new Binding("IsBusy", BindingMode.OneWay));
            listView.SetBinding(ListView.RefreshCommandProperty, new Binding("LoadChartCommand"));
            
		}
        public void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return; // has been set to null, do not 'process' tapped event
            Chart SelChart = (Chart)e.SelectedItem;
            AppSongPage.LoadChart(SelChart.ChartURL, SelChart.LastUpdatedDateTimeString);
            MessagingCenter.Send<ChartPage>(this, "ChartSelected");
            ((ListView)sender).SelectedItem = null; // de-select the row
        }
        public void OnAllClick (Object sender,EventArgs e)
        {
            myViewModel.ChartFilter = "All";
            myViewModel.LoadChartCommand.Execute(sender);
        }
        public void OnFavClick (Object sender,EventArgs e)
        {
            myViewModel.ChartFilter = "Fav";
            myViewModel.LoadChartCommand.Execute(sender);
        }
        public void OnVisClick (Object sender,EventArgs e)
        {
            myViewModel.ChartFilter = "Vis";
            myViewModel.LoadChartCommand.Execute(sender);
        }
        public void SetChartFav(Chart SelChart)
        {
            SelChart.Favorite = (SelChart.Favorite == 1 ? 0 : 1);
        }

        public void SetChartHide(Chart SelChart)
        {
            SelChart.Hide = (SelChart.Hide == 1 ? 0 : 1);
        }
        protected override void OnAppearing()
		{
			base.OnAppearing();

			if (myViewModel.ChartObsCol.Count == 0)
				myViewModel.LoadChartCommand.Execute(null);
		}
	}
}