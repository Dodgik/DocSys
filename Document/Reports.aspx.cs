using System;
using BizObj.Document;

namespace Document.Pages
{
    public partial class Reports : UI.RestrictedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlSocialCategory.DataSource = SocialCategory.GetAll();
                ddlSocialCategory.DataBind();

                ddlBranchType.DataSource = BranchType.GetAll();
                ddlBranchType.DataBind();
            }
        }

        public override string GetPageTitle()
        {
            return "Звіти";
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