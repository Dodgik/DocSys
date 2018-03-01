using System;
using BizObj.CustomException;
using BizObj.Data;

namespace Custom.UI
{
	public class CurrentTheme
	{
		static public string Theme
		{
			get
			{
				if( System.Web.HttpContext.Current.Session[ "CurrentTheme" ] == null )
				{
                    System.Web.HttpContext.Current.Session["CurrentTheme"] = "Default";
				}
				return System.Web.HttpContext.Current.Session[ "CurrentTheme" ].ToString();
			}
		}

        public static string RootURL
        {
            get
            {
                return Config.RootURL + Config.SubFolder;
            }
        }

        public static string RootFullURL
        {
            get
            {
                Uri url = System.Web.HttpContext.Current.Request.Url;
                return String.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, url.Port == 80 ? String.Empty : String.Format(":{0}",url.Port.ToString()), RootURL == String.Empty ? "/" : RootURL);
            }
        }

        public static string ActiveThemeURL
        {
            get
            {
                if (string.IsNullOrEmpty(Theme))
                {
                    throw new CustomException("Error! Theme is invalid.");
                }
                else
                {
                    return RootURL + "App_Themes/" + Theme + "/";
                }
            }
        }

        public static string ActiveThemeFullURL
        {
            get
            {
                if (string.IsNullOrEmpty(Theme))
                {
                    throw new CustomException("Error! Theme is invalid.");
                }
                else
                {
                    return RootFullURL + "App_Themes/" + Theme + "/";
                }
            }
        }

        public static string ThemeImagesURL
        {
            get
            {
                return ActiveThemeURL + "Images/";
            }
        }

        public static string ThemeImagesFullURL
        {
            get
            {
                return ActiveThemeFullURL + "Images/";
            }
        }        

        public static string ThemeXMLURL
        {
            get
            {
                return ActiveThemeURL + "XML/";
            }
        }

        public static string ThemeXMLFullURL
        {
            get
            {
                return ActiveThemeFullURL + "XML/";
            }
        }
	}
}