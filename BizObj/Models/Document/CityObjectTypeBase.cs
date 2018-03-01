using System;

namespace BizObj.Document
{
    [Serializable]
    public class CityObjectTypeBase
    {
        public short ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }
    }
}
