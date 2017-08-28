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
	public partial class ChartViewCell : ViewCell
	{
		public ChartViewCell ()
		{
			InitializeComponent ();
		}
        //protected override void OnBindingContextChanged()
        //{
        //    base.OnBindingContextChanged();
        //    var ChartBC = BindingContext as Chart;
        //    if (ChartBC != null)
        //    {
        //        ChartNumber.Text = ChartBC.ChartNumber.ToString();
        //        ChartTitle.Text = ChartBC.ChartTitle;
        //        FavHide.Text = (ChartBC.Favorite == 0 ? "*" : "F") + "|" + (ChartBC.Hide == 0 ? "V" : "H");
        //        ChartURL.Text = ChartBC.ChartURL;
        //    }
        //    else
        //    {
        //        ChartNumber.Text = "";
        //        ChartTitle.Text = "";
        //        FavHide.Text = "";
        //        ChartURL.Text = "";
        //    }
        //}

        public void OnChartFavorite (object sender, EventArgs e)
        {
            MenuItem curMenuItem = (MenuItem)sender;
            Chart SelChart = (Chart)curMenuItem.CommandParameter;
            ChartViewModel curViewModel = (ChartViewModel)this.Parent.BindingContext;
            if (SelChart != null) {
                //ParentPage.SetChartFav(SelChart);
                SelChart.Favorite = (SelChart.Favorite == 1 ? 0 : 1);
                //SelChart.FavHide = SelChart.Favorite.ToString() + "|" + SelChart.Hide.ToString();

                curViewModel.ChartUpdate(SelChart);
            }
            MessagingCenter.Send<ChartViewCell>(this, "ChartUpdated");
        }
        public void OnChartHide (object sender, EventArgs e)
        {
            MenuItem curMenuItem = (MenuItem)sender;
            Chart SelChart = (Chart)curMenuItem.CommandParameter;
            ChartViewModel curViewModel = (ChartViewModel)this.Parent.BindingContext;
            if (SelChart != null) {
                //ParentPage.SetChartHide(SelChart);
                SelChart.Hide = (SelChart.Hide == 1 ? 0 : 1);
                //SelChart.FavHide = SelChart.Favorite.ToString() + "|" + SelChart.Hide.ToString();

                curViewModel.ChartUpdate(SelChart);
            }
            MessagingCenter.Send<ChartViewCell>(this, "ChartUpdated");
        }
        public void OnChartUpdate (object sender, EventArgs e)
        {
            MenuItem curMenuItem = (MenuItem)sender;
            Chart SelChart = (Chart)curMenuItem.CommandParameter;
            ChartViewModel curViewModel = (ChartViewModel)this.Parent.BindingContext;
            if (SelChart != null) {
                SelChart.LastUpdatedDateTimeString = "";
                curViewModel.ChartUpdate(SelChart);
                //curViewModel.LoadChart(SelChart.ChartURL, SelChart.LastUpdatedDateTimeString, SelChart.ChartTitle);

            }
            MessagingCenter.Send<ChartViewCell>(this, "ChartUpdated");
        }

	}
}