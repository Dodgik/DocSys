using System;
using System.Collections.Generic;

namespace BizObj.Models.Report
{
    [Serializable]
    public class WorkerCards
    {
        public string Worker { get; set; }

        public List<CardInfo> Cards { get; set; }

        public WorkerCards()
        {
            Cards = new List<CardInfo>();
        }
    }
}
