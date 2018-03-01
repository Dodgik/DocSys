using System;
using System.Reflection;
using System.Web.UI;
using BizObj.Data;

namespace Custom.UI
{
    [PartialCaching(0, null, "ClientID", "browser", true)]
	public class CustomUserControl : UserControl
	{
        protected static string VIEWSTATE_ISCACHE = "IsCache";

        protected string RootURL
        {
            get
            {
                return CurrentTheme.RootURL;
            }
        }

        protected string RootFullURL
        {
            get
            {
                return CurrentTheme.RootFullURL;
            }
        }


        protected string ActiveThemeURL
        {
            get
            {
                return CurrentTheme.ActiveThemeURL;
            }
        }

        protected string ActiveThemeFullURL
        {
            get
            {
                return CurrentTheme.ActiveThemeFullURL;
            }
        }

        protected string ThemeImagesURL
        {
            get
            {
                return CurrentTheme.ThemeImagesURL;
            }
        }

        protected string ThemeImagesFullURL
        {
            get
            {
                return CurrentTheme.ThemeImagesFullURL;
            }
        }

        protected string ThemeXMLURL
        {
            get
            {
                return CurrentTheme.ThemeXMLURL;
            }
        }

        protected string ThemeXMLFullURL
        {
            get
            {
                return CurrentTheme.ThemeXMLFullURL;
            }
        }
        
        public bool IsCache
        {
            set
            {
                ViewState[VIEWSTATE_ISCACHE] = value;
            }
            get
            {
                return ViewState[VIEWSTATE_ISCACHE] == null ? false : (bool)ViewState[VIEWSTATE_ISCACHE];
            }
        }

		protected void ImageUrlPreRender(object sender, EventArgs e)
		{
            UrlPreRender(sender, e, "ImageUrl");			
		}

        protected void LogoutImageUrlPreRender(object sender, EventArgs e)
        {
            UrlPreRender(sender, e, "LogoutImageUrl");
        }

        protected void SrcUrlPreRender(object sender, EventArgs e)
        {
            UrlPreRender(sender, e, "Src");
        }

        private void UrlPreRender(object sender, EventArgs e, string propertyName)
        {
            PropertyInfo pi = sender.GetType().GetProperty(propertyName);
            if ((pi != null) && (pi.PropertyType == typeof(string)))
            {
                string value = (string)pi.GetValue(sender, null);
                if ((!string.IsNullOrEmpty(value)) && (value.IndexOf(ThemeImagesURL) == -1))
                {
                    value = String.Format("{0}{1}", ThemeImagesURL, value);
                    pi.SetValue(sender, value, null);
                }
            }
        }

		protected void ImagesBaseUrlPreRender(object sender, EventArgs e)
		{
			PropertyInfo pi = sender.GetType().GetProperty("ImagesBaseUrl");
			if ((pi != null) && (pi.PropertyType == typeof(string)))
			{
				pi.SetValue(sender, ThemeImagesURL, null);
			}
		}        

        public virtual bool CanBrowser()
        {
            return true;
        }

        public void SetVisibleForIndexation()
        {
            Visible = CanBrowser();
        }

        protected virtual bool IsCached()
        {
            return false;
        }

        private void SetCachedInstance()
        {
            if ((IsCached() || IsCache) && Config.CACHE_MINUTES > 0)
            {
                CachePolicy.Duration = TimeSpan.FromMinutes(Config.CACHE_MINUTES);
                CachePolicy.SetSlidingExpiration(Config.CACHE_SLIDING);
            }
            else
            {
                CachePolicy.Cached = false;                
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SetCachedInstance();
        }
	}
}