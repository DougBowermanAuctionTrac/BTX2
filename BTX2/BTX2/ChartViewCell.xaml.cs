using BTX2.Model;
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
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var ChartBC = BindingContext as Chart;
            if (ChartBC != null)
            {
                ChartNumber.Text = ChartBC.ChartNumber.ToString();
                ChartTitle.Text = ChartBC.ChartTitle;
                Favorite.Text = ChartBC.Favorite.ToString();
                ChartURL.Text = ChartBC.ChartURL;
            }
            else
            {
                ChartNumber.Text = "";
                ChartTitle.Text = "";
                Favorite.Text = "";
                ChartURL.Text = "";
            }
        }

        public void OnChartFavorite (object sender, EventArgs e)
        {
            MenuItem curMenuItem = (MenuItem)sender;
            Chart SelChart = (Chart)curMenuItem.CommandParameter;
            if (SelChart != null) {
                ChartPage Parent = (ChartPage)this.Parent;
                Parent.SetChartFav(SelChart);
                //var db = new SQLiteConnection(Config.Instance.dbPath);
                //if (curSelectedChart.Favorite == 0)
                //    curSelectedChart.Favorite = 1;
                //else
                //    curSelectedChart.Favorite = 0;
                //db.Update(curSelectedChart);
            }
            MessagingCenter.Send<ChartViewCell>(this, "ChartUpdated");
        }
        public void OnChartHide (object sender, EventArgs e)
        {
            MenuItem curMenuItem = (MenuItem)sender;
            Chart SelChart = (Chart)curMenuItem.CommandParameter;
            if (SelChart != null) {
                ChartPage Parent = (ChartPage)this.Parent;
                Parent.SetChartHide(SelChart);
                //var db = new SQLiteConnection(Config.Instance.dbPath);
                //if (curSelectedChart.Hide == 0)
                //    curSelectedChart.Hide = 1;
                //else
                //    curSelectedChart.Hide = 0;
                //db.Update(curSelectedChart);
            }
            MessagingCenter.Send<ChartViewCell>(this, "ChartUpdated");

        }

	}
}