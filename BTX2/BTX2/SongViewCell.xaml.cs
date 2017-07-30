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
	public partial class SongViewCell : ViewCell
	{
		public SongViewCell ()
		{
			InitializeComponent ();
		}
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var SongBC = BindingContext as Song;
            if (SongBC != null)
            {
                Position.Text = SongBC.Position.ToString();
                PrevRank.Text = SongBC.PreviousRank.ToString();
                Title.Text = SongBC.Title;
                PeakPosition.Text = SongBC.Position.ToString();
                WeeksOnChart.Text = SongBC.WeeksOnChart.ToString();
                Artist.Text = SongBC.Artist;
            }
            else
            {
                Position.Text = "";
                PrevRank.Text = "";
                Title.Text = "";
                PeakPosition.Text = "";
                WeeksOnChart.Text = "";
                Artist.Text = "";
            }
        }

	}
}