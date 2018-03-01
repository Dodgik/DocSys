using System;

namespace Document.Pages
{
    public partial class Home : UI.RestrictedPage
    {
        public override string GetPageTitle()
        {
            return "Домашня";
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override bool AccessValidate()
        {
            if (IsLogged())
            {
                return true;
            }
            return false;
        }

        protected override string GetMasterPageFile()
        {
            return string.Format("~/MasterPages/WorkerMasterPage.master");
        }
    }
}