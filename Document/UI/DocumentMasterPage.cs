using System;
using System.Collections.Generic;
using System.Web.Security;


namespace Document.UI
{
    /// <summary>
    /// Document master page abstract class
    /// </summary>
    public abstract class DocumentMasterPage : Custom.UI.CustomMasterPage
    {
        public override string GetAboveMasterPageFile()
        {
            return String.Empty;
        }

        public bool IsLogged()
        {
            return Membership.GetUser() != null;
        }


        public abstract void SetErrorMessage(string message);
 
        public abstract void SetErrorMessage(List<string> messages);
 
        public abstract void SetMessage(string message);
 
        public abstract void SetMessage(List<string> messages);
 
        public abstract void SetWarningMessage(string message);

        public abstract void SetWarningMessage(List<string> messages);
     }        
}