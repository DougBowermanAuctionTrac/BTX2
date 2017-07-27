using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace BTX2.Model
{
    [Table("Song")]
    public class Song
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int _id { get; set; }
        [MaxLength(256)]
        public string ChartURL { get; set; }
        public int Position { get; set; }
        public string Title { get; set; }
        [MaxLength(128)]
        public string Artist { get; set; }
        public int CurrentRank { get; set; }
        public int PreviousRank { get; set; }
        public int PeakPosition { get; set; }
        public int WeeksOnChart { get; set; }
    }
}
