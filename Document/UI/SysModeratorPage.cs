using System;
using System.Web.Security;

namespace Document.UI
{
    public abstract class SysModeratorPage : RestrictedPage
    {
        protected override bool AccessValidate()
        {
            if (this.IsLogged())
            {                
                MembershipUser membershipUser = GetCurrentUser();
                if (membershipUser.ProviderUserKey != null)
                {
                    BizObj.User user = new BizObj.User((Guid)membershipUser.ProviderUserKey);
                    return user.IsSysModerator(membershipUser.UserName);
                }
            }
            else
            {
                return false;
            }
        }                
    }
}