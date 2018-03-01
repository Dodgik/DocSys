using System;

namespace Document.Pages
{
    public partial class AdminScreens : UI.RestrictedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override string GetPageTitle()
        {
            return "Адмін-Екрани";
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