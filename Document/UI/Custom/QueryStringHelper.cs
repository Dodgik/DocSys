using System;
using System.Text;
using System.Web;

namespace Custom.UI
{
	public class QueryStringHelper
	{
		protected System.Collections.Specialized.NameValueCollection QueryString;

        private static string FIRST_PARAM = "?{0}";
        private static string OTHER_PARAM = "&{0}";

	    #region Helper methods

        public QueryStringHelper(System.Collections.Specialized.NameValueCollection queryString)
		{
			this.QueryString = queryString;
		}

		public static string CreateParameter(string parameterName, string parameterValue)
		{
			return String.Format("{0}={1}", HttpUtility.UrlEncode(parameterName), HttpUtility.UrlEncode(parameterValue));
		}

        public static string CreateParameter(string parameterName, int parameterValue)
        {
            return CreateParameter(parameterName, parameterValue.ToString());            
        }

        public static string CreateParameter(string parameterName, bool parameterValue)
        {
            return CreateParameter(parameterName, parameterValue ? "yes" : "no" );
        }

        public static string CreateURL(string pathPage, params object[] args)
        {
            StringBuilder res = new StringBuilder(pathPage);
            bool isFirst = true;
            foreach (object param in args)
            {
                if(isFirst)
                {
                    res.AppendFormat(FIRST_PARAM, param);
                }
                else
                {
                    res.AppendFormat(OTHER_PARAM, param);
                }                
                isFirst = false;
            }
            return res.ToString();
        }

		public string ExtractParameter(string parameterName)
		{
			string parameterValue = QueryString[parameterName];
			if (!string.IsNullOrEmpty(parameterValue))
            {
				return parameterValue;
			}
            else
            {
				return null;
			}
		}

		public Boolean IsParameterPresent(string parameterName)
		{
			foreach(string key in QueryString.AllKeys)
            {
				if (string.Compare(parameterName, key, true) == 0)
                {
					return true;
				}
			}
			return false;
        }

        #endregion
    }
}