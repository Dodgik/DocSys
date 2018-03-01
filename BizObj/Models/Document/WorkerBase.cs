using System;

namespace BizObj.Document
{
    [Serializable]
    public class WorkerBase
    {
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int PostID { get; set; }
    }
}
