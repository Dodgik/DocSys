using System;

namespace Document.Pages
{
    public partial class Main : UI.RestrictedPage
    {
        public override string GetPageTitle()
        {
            return "Головна";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!User.IsInRole("DepartmentAdmin"))
                {
                    Response.Redirect(String.Format("{0}home.aspx", RootURL), true);
                }
            }
        }

        protected override bool AccessValidate()
        {
            if (IsLogged())
            {
                return true;
            }
            return false;
        }
    }
}