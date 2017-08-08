using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.ComponentModel;

namespace BTX2.Model
{
    [Table("Chart")]
    public class Chart : INotifyPropertyChanged
    {
        private int _chartNumber;
        private string _lastUpdatedDateTimeString;
        private string _chartCategory;
        private string _chartURL;
        private string _chartTitle;
        private int _favorite;
        private int _hide;
        private string _favHide;

        [PrimaryKey, Column("ChartNumber")]
        public int ChartNumber
        {
            get { return _chartNumber; }
            set
            {
                if (_chartNumber == value)
                    return;

                _chartNumber = value;
                OnPropertyChanged("ChartNumber");
            }
        }
        [MaxLength(30)]
        public string LastUpdatedDateTimeString
        {
            get { return _lastUpdatedDateTimeString; }
            set
            {
                if (_lastUpdatedDateTimeString == value)
                    return;

                _lastUpdatedDateTimeString = value;
                OnPropertyChanged("LastUpdatedDateTimeString");
            }
        }
        [MaxLength(128)]
        public string ChartCategory
        {
            get { return _chartCategory; }
            set
            {
                if (_chartCategory == value)
                    return;

                _chartCategory = value;
                OnPropertyChanged("ChartCategory");
            }
        }
        [MaxLength(256)]
        public string ChartURL
        {
            get { return _chartURL; }
            set
            {
                if (_chartURL == value)
                    return;

                _chartURL = value;
                OnPropertyChanged("ChartURL");
            }
        }
        [MaxLength(128)]
        public string ChartTitle
        {
            get { return _chartTitle; }
            set
            {
                if (_chartTitle == value)
                    return;

                _chartTitle = value;
                OnPropertyChanged("ChartTitle");
            }
        }
        public int Favorite
        {
            get { return _favorite; }
            set
            {
                if (_favorite == value)
                    return;

                _favorite = value;
                _favHide = (_favorite == 0 ? "*" : "F") +"|" + (_hide == 0 ? "V" : "H");

                OnPropertyChanged("Favorite");
                OnPropertyChanged("FavHide");
            }
        }
        public int Hide
        {
            get { return _hide; }
            set
            {
                if (_hide == value)
                    return;

                _hide = value;
                _favHide = (_favorite == 0 ? "*" : "F") +"|" + (_hide == 0 ? "V" : "H");
                
                OnPropertyChanged("Hide");
                OnPropertyChanged("FavHide");
            }
        }

        [Ignore]
        public string FavHide
        {
            get { if (_favHide == null) _favHide = (_favorite == 0 ? "*" : "F") +"|" + (_hide == 0 ? "V" : "H"); return _favHide; }
            //set
            //{
            //    if (_favHide == value)
            //        return;

            //    _favHide = value;
            //    OnPropertyChanged("FavHide");
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }

}
