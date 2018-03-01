using System;
using System.Collections.Generic;

namespace BizObj.Models.Report
{
    [Serializable]
    public class WorkerCardsReport
    {
        public List<string> Header { get; set; }
        public List<string> Footer { get; set; }

        public List<WorkerCards> Statistic { get; set; }

        public WorkerCardsReport()
        {
            Header = new List<string>();
            Footer = new List<string>();
            Statistic = new List<WorkerCards>();
        }
    }
}