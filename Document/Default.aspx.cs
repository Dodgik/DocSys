using System;

namespace Document.Pages
{
    public partial class Default : UI.PublicPage
    {
        public override string GetPageTitle()
        {
            return "Стартова";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (User.IsInRole("DepartmentAdmin")) {
                    Response.Redirect(String.Format("{0}main.aspx", RootURL), true);
                } else {
                    Response.Redirect(String.Format("{0}home.aspx", RootURL), true);
                }
            }
        }
    }
}