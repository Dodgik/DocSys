using System;
using System.Collections.Generic;

namespace BizObj.Models.Report
{
    [Serializable]
    public class DsFullStatistics
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int CountStatements { get; set; }

        public List<string> Header { get; set; }

        public List<StatisticRow> Statistic { get; set; }

        public DsFullStatistics()
        {
            Header = new List<string>();
            Statistic = new List<StatisticRow>();
        }
    }
}
