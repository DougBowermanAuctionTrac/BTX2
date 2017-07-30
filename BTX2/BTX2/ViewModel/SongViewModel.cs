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
    public class SongViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Song> SongObsCol { get; set; }
        private SongService SongSvc;
        public Command LoadSongCommand { get; set; }
        public string ChartURL { get; set; }
        public string LastUpdatedDateTime { get; set; }

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

        public SongViewModel()
        {
            this.SongObsCol = new ObservableCollection<Song>();
            this.SongSvc = new SongService();
            LoadSongCommand = new Command(async () => await ExecuteLoadSongCommand());
        }
        async Task ExecuteLoadSongCommand()
		{
            if (IsBusy)
				return;

			IsBusy = true;

			try
			{
                List<Song> SongList = new List<Song>();
                string CurLastUpdatedDateTime = LastUpdatedDateTime;
                //SongList = await Task.Run(() => SongSvc.SongSelect(ChartURL, ref CurLastUpdatedDateTime));
                SongList = SongSvc.SongSelect(ChartURL, ref CurLastUpdatedDateTime);
                LastUpdatedDateTime = CurLastUpdatedDateTime;
                SongObsCol.Clear();
                foreach (Song mySong in SongList)
                {
                    SongObsCol.Add(mySong);
                }
			}
			catch (Exception AnyException)
			{
				Debug.WriteLine(AnyException.Message);

			}
			finally
			{
				IsBusy = false;
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
