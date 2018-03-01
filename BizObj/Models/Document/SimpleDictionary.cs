using System;

namespace BizObj.Document
{
    [Serializable]
    public class SimpleDictionary: ISimpleDictionary
    {
        #region Properties

        public int ID { get; set; }

        public string Name { get; set; }

        #endregion
    }
}