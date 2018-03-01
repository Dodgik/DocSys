using System;
using System.Data;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using AdLib.Models.JqGrid;
using AdLib.Models.Pager;

namespace AdLib.Controllers
{
    public class UserScope
    {
        private readonly HttpContext _context;
        private Guid UserId { get; set; }

        public UserScope(HttpContext context, Guid userId)
        {
            _context = context;
            UserId = userId;
        }

        public void ParceRequest()
        {
            HttpRequest r = _context.Request;
            string type = r["type"];
            string jdata = r["jdata"];
            string rResult = String.Empty;


            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "getpage":
                        int departmentId = Convert.ToInt32(r["departmentID"]);
                        
                        PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

                        MembershipUserCollection users = Membership.GetAllUsers();

                        //users[0]

                        //DataTable dtPage = WorkerDocumets.GetPage(gridSettings, UserId, departmentId);
                        //JqGridResults jqGridResults = DocTemplate.BuildJqGridResults(dtPage, gridSettings);

                        //rResult = new JavaScriptSerializer().Serialize(jqGridResults);
                        break;
                }
                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }
    }
}
