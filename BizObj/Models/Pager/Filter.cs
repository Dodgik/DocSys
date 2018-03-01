using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BizObj.Models.Pager
{
    [DataContract]
    public class Filter
    {
        [DataMember]
        public string GroupOp { get; set; }
        [DataMember]
        public List<Rule> Rules { get; set; }

        public void AddRule(Rule rule)
        {
            Rules.Add(rule);
        }

        public Rule GetRule(string field)
        {
            if (Rules != null && Rules.Count > 0)
                foreach (Rule rule in Rules)
                {
                    if (rule.Field == field)
                        return rule;
                }
            return null;
        }

        public bool HasRule(string field)
        {
            if (Rules != null && Rules.Count > 0)
                foreach (Rule rule in Rules)
                {
                    if (rule.Field == field)
                        return true;
                }
            return false;
        }
    }
}
