using System;

namespace BizObj.Models.Report
{
    [Serializable]
    public class CardInfo
    {
        public string Number { get; set; }

        public string CreationDate { get; set; }

        public string EndDate { get; set; }

        public string Citizen { get; set; }

        public string Content { get; set; }

        public string Worker { get; set; }
    }
}
