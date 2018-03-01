using System;

namespace Document.Pages
{
    public partial class Service : UI.RestrictedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        public override string GetPageTitle()
        {
            return "Сервіс";
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