using System;
using System.Web;
using System.Web.Script.Serialization;
using BizObj.Models.Pager;

namespace BizObj.Models.JqGrid
{
    public class JqGridSettings
    {
        public static PageSettings GetPageSettings(HttpContext context)
        {
            try
            {
                var request = context.Request;
                return new PageSettings
                {
                    IsSearch = bool.Parse(request["_search"] ?? "false"),
                    PageIndex = int.Parse(request["page"] ?? "1"),
                    PageSize = int.Parse(request["rows"] ?? "50"),
                    SortColumn = request["sidx"] ?? "",
                    SortOrder = request["sord"] ?? "asc",
                    Where = GetFilter(request["filters"] ?? "")
                };
            }
            catch
            {
                return null;
            }
        }

        public static Filter GetFilter(string jsonObject)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(jsonObject))
                    return new Filter();

                return new JavaScriptSerializer().Deserialize<Filter>(jsonObject);
            }
            catch
            {
                return new Filter();
            }
        }
    }
}
