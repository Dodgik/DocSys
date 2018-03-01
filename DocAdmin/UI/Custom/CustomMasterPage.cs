using System;
using System.Reflection;
using System.Web.UI;


namespace Custom.UI
{
	/// <summary>
    /// Custom master page class
	/// </summary>
	public abstract class CustomMasterPage : MasterPage
	{
		public abstract string GetAboveMasterPageFile();


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

        private void UrlPreRender(object sender, EventArgs e, string propertyName)
        {
            PropertyInfo pi = sender.GetType().GetProperty(propertyName);
            if ((pi != null) && (pi.PropertyType == typeof(string)))
            {
                string value = (string)pi.GetValue(sender, null);
                if ((!string.IsNullOrEmpty(value)) && (value.IndexOf(this.ThemeImagesURL) == -1))
                {
                    value = String.Format("{0}{1}", this.ThemeImagesURL, value);
                    pi.SetValue(sender, value, null);
                }
            }            
        }

        protected void ImageUrlPreRender(object sender, EventArgs e)
        {
            this.UrlPreRender(sender, e, "ImageUrl");
        }

        protected void SrcUrlPreRender(object sender, EventArgs e)
        {
            this.UrlPreRender(sender, e, "Src");
        }                
                      
	}
}