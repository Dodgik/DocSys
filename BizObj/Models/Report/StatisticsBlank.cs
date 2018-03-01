using System;
using System.Collections.Generic;

namespace BizObj.Models.Report
{
    [Serializable]
    public class StatisticsBlank
    {
        public List<string> Headers { get; set; }

        public string[] ColumnsWidth { get; set; }
        public List<string[]> Statistics { get; set; }

        public StatisticsBlank()
        {
            Headers = new List<string>();
            Statistics = new List<string[]>();
        }
    }
}
