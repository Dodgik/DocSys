using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.Security;

namespace Document.UI
{
    public abstract class DocumentContentPage : Custom.UI.ContentPage
    {        

        //public abstract Boolean ValidateUserAccess(BizObj.User user);

        protected override string GetMasterPageFile()
        {
            return string.Format("~/MasterPages/MainMasterPage.master");            
        }


        protected override void OnLoad(EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddYears(-1));
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetNoServerCaching();
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            
            base.OnLoad(e);            
        }

        public bool IsLogged()
        {
            return Membership.GetUser() != null;
        }

        public MembershipUser GetCurrentUser()
        {
            return Membership.GetUser();
        }
        
        public override bool CanBrowser()
        {
            return true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            SetImageUrl(this, e);           
            base.OnPreRender(e);
        }

        protected void SetImageUrl(Control parentCntr, EventArgs e)
        {
            ImageUrlPreRender(parentCntr, e);
            SrcUrlPreRender(parentCntr, e);
            foreach (Control cntr in parentCntr.Controls)
            {
                SetImageUrl(cntr, e);
            }            
        }

        public void SetErrorMessage(string message)
        {
            GetMasterPage().SetErrorMessage(message);
        }

        public void SetErrorMessage(List<string> messages)
        {
            GetMasterPage().SetErrorMessage(messages);
        }

        public void SetMessage(string message)
        {
            GetMasterPage().SetMessage(message);
        }

        public void SetMessage(List<string> messages)
        {
            GetMasterPage().SetMessage(messages);
        }

        public void SetWarningMessage(string message)
        {
            GetMasterPage().SetWarningMessage(message);
        }

        public void SetWarningMessage(List<string> messages)
        {
            GetMasterPage().SetWarningMessage(messages);
        }

        private DocumentMasterPage GetMasterPage()
        {
            if (Master == null)
            {
                return null;
            }
            return (DocumentMasterPage)Master;
        }
    }
}