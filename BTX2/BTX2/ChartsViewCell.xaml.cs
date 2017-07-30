using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

namespace BTX2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChartsViewCell : ViewCell
	{
		public ChartsViewCell ()
		{
			InitializeComponent ();
		}

        public void OnChartFavorite (object sender, EventArgs e)
        {
            MenuItem curMenuItem = (MenuItem)sender;
            Chart curSelectedChart = (Chart)curMenuItem.CommandParameter;
            if (curSelectedChart != null) {
                var db = new SQLiteConnection(Config.Instance.dbPath);
                if (curSelectedChart.Favorite == 0)
                    curSelectedChart.Favorite = 1;
                else
                    curSelectedChart.Favorite = 0;
                db.Update(curSelectedChart);
            }
            MessagingCenter.Send<ChartsViewCell>(this, "ChartsUpdated");
        }
        public void OnChartHide (object sender, EventArgs e)
        {
            MenuItem curMenuItem = (MenuItem)sender;
            Chart curSelectedChart = (Chart)curMenuItem.CommandParameter;
            if (curSelectedChart != null) {
                var db = new SQLiteConnection(Config.Instance.dbPath);
                if (curSelectedChart.Hide == 0)
                    curSelectedChart.Hide = 1;
                else
                    curSelectedChart.Hide = 0;
                db.Update(curSelectedChart);
            }
            MessagingCenter.Send<ChartsViewCell>(this, "ChartsUpdated");

        }
	}
}