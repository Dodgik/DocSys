using System;
using System.Reflection;

namespace Custom.UI
{
	public abstract class ContentPage: System.Web.UI.Page
	{
		protected abstract string GetMasterPageFile();

        protected  string  RootURL
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
                if ((!string.IsNullOrEmpty(value)) && (value.IndexOf("://") == -1) && (value.IndexOf(this.ThemeImagesURL) == -1))
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

		protected override void OnPreInit(EventArgs e)
		{
			this.Theme = CurrentTheme.Theme;            
            if(this.CanBrowser())            
            {
                if (System.Web.HttpContext.Current.Request.QueryString["MasterPageFile"] == null)
                {
                    this.MasterPageFile = this.GetMasterPageFile();
                }
                else
                {
                    this.MasterPageFile = Server.HtmlDecode(System.Web.HttpContext.Current.Request.QueryString["MasterPageFile"]);
                }
                if (this.Master is CustomMasterPage)
                {
                    System.Web.UI.MasterPage ms = this.Master;
                    do
                    {
                        ms.MasterPageFile = ((CustomMasterPage)ms).GetAboveMasterPageFile();
                        ms = ms.Master;
                    } while (ms is CustomMasterPage);
                }
            }
            else
            {
                this.MasterPageFile = this.GetBlankMasterPageFile();
            }
			base.OnPreInit(e);
		}

        public virtual bool CanBrowser()
        {
            return true;
        }

        protected virtual string GetBlankMasterPageFile()
        {
            return String.Empty;
        }			

        protected override void OnPreRender(EventArgs e)
        {
            if (Master != null) Master.Page.Title = GetPageTitle();
            base.OnPreRender(e);
        }

		public abstract string GetPageTitle();
	}
}