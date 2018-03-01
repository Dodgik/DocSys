using System.Runtime.Serialization;

namespace AdLib.Models.Pager
{
    [DataContract]
    public class Rule
    {
        [DataMember]
        public string Field { get; set; }
        [DataMember]
        public string Op { get; set; }
        [DataMember]
        public string Data { get; set; }
    }
}
