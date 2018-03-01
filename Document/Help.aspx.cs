using System;

namespace Document.Pages
{
    public partial class Help : UI.RestrictedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override string GetPageTitle()
        {
            return "Довідка";
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