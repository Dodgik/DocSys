using System;
using BizObj.Data;

namespace Document.UI
{
    public abstract class RestrictedPage : DocumentContentPage
    {
        protected abstract bool AccessValidate();        

        protected override void OnPreInit(EventArgs e)
        {
            if (!AccessValidate())
            {
                string url = String.Format("{0}Pages/Public/Error.aspx", RootURL);
                Session[Config.SESSION_ERROR_MESSAGE] = "Відмовлено в доступі.";
                Response.Redirect(url, true);
            }
            base.OnPreInit(e);
        }
    }
}