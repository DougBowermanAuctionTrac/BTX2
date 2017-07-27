using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace BTX2.Model
{
    [Table("Chart")]
    public class Chart
    {
        [PrimaryKey, Column("ChartNumber")]
        public int ChartNumber { get; set; }
        [MaxLength(30)]
        public string LastUpdatedDateTimeString { get; set; }
        [MaxLength(128)]
        public string ChartCategory { get; set; }
        [MaxLength(256)]
        public string ChartURL { get; set; }
        [MaxLength(128)]
        public string ChartTitle { get; set; }
        public int Favorite { get; set; }
        public int Hide { get; set; }
    }
}
