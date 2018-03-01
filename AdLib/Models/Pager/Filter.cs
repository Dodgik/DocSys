using System.Runtime.Serialization;

namespace AdLib.Models.Pager
{
    [DataContract]
    public class Filter
    {
        [DataMember]
        public string GroupOp { get; set; }
        [DataMember]
        public Rule[] Rules { get; set; }

        public Rule GetRule(string field)
        {
            if (Rules != null && Rules.Length > 0)
                foreach (Rule rule in Rules)
                {
                    if (rule.Field == field)
                        return rule;
                }
            return null;
        }

        public bool HasRule(string field)
        {
            if (Rules != null && Rules.Length > 0)
                foreach (Rule rule in Rules)
                {
                    if (rule.Field == field)
                        return true;
                }
            return false;
        }
    }
}
