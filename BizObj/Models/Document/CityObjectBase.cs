using System;

namespace BizObj.Document
{
    [Serializable]
    public class CityObjectBase
    {
        public int ID { get; set; }

        public short TypeID { get; set; }

        public string Name { get; set; }

        public string OldName { get; set; }

        public string SearchName { get; set; }

        public bool IsReal { get; set; }
    }
}
