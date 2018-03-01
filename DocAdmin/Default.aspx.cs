using System;
using System.Web.Security;

namespace Document.Pages
{
    public partial class Default : UI.PublicPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (IsLogged())
                {
                    //FormsAuthentication.SignOut();
                    //FormsAuthentication.RedirectToLoginPage();

                }
            }
        }

        public override string GetPageTitle()
        {
            return "Стартова";
        }

    }
}