using System;
using System.Collections.Generic;
using System.Text;
using BTX2.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using BTX2.Service;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BTX2.ViewModel
{
    public class ChartViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Chart> ChartObsCol { get; set; }
        private ChartService ChartSvc;
        public Command LoadChartCommand { get; set; }
        public string ChartFilter { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool IsBusyPriv;
        public bool IsBusy
        {
	        get { return IsBusyPriv; }
	        set 
	        {
		        if (IsBusyPriv == value)
			        return;
    	
	        	IsBusyPriv = value;
		        OnPropertyChanged ("IsBusy");
	        }
        }

        public ChartViewModel()
        {
            this.ChartObsCol = new ObservableCollection<Chart>();
            this.ChartSvc = new ChartService();
            LoadChartCommand = new Command(async () => await ExecuteLoadChartCommand());
        }
        async Task ExecuteLoadChartCommand()
		{
            if (IsBusy)
				return;

			IsBusy = true;

			try
			{
                List<Chart> ChartList = new List<Chart>();
                ChartList = await Task.Run(() => ChartSvc.ChartSelect(ChartFilter));
                ChartObsCol.Clear();
                foreach (Chart myChart in ChartList)
                {
                    ChartObsCol.Add(myChart);
                }
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);

			}
			finally
			{
				IsBusy = false;
			}
		}

        public void ChartUpdate(Chart CurChart)
        {
            ChartSvc.ChartUpdate(CurChart);
            for (int i = 0; i < ChartObsCol.Count; i++)
            {
                if (ChartObsCol[i].ChartNumber == CurChart.ChartNumber)
                {
                    ChartObsCol[i].Favorite = CurChart.Favorite;
                    ChartObsCol[i].Hide = CurChart.Hide;
                    OnPropertyChanged("ChartObsCol");
                    OnPropertyChanged("Favorite");
                    OnPropertyChanged("Hide");
                    break;
                }
            }
        }
        protected virtual void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

    }
}
