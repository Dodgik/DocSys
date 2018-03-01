using System;

namespace Document.Pages
{
    public partial class Main : UI.RestrictedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override string GetPageTitle()
        {
            return "Головна";
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