using System;

namespace BizObj.Models.Report
{
    [Serializable]
    public class ControlCardReport
    {
        public string DepartmentName { get; set; }

        public string Control { get; set; }

        public string Correspondent { get; set; }

        public string Address { get; set; }

        public string InternalCreationDate { get; set;}

        public string InternalNumber { get; set;}
        
        public string ExternalCreationDate { get; set;}

        public string ExternalNumber { get; set;}
        
        public string OrganizationName { get; set;}
    
        public string Content { get; set;}

        public string Resolution { get; set;}

        public string SocialCategories { get; set;}

        public string SocialStatus { get; set;}

        public string Branches { get; set;}

        public string InputMethod { get; set;}
        public string InputDocType { get; set;}
        public string InputSubjectType { get; set;}
        public string InputSign { get; set;}
        public string DeliveryType { get; set;}

        public string Head { get; set;}
        public string EndDate { get; set;}
    }
}
