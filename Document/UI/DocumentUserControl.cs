using System.Collections.Generic;
using System.Web.UI;
using System.Web.Security;

namespace Document.UI
{
    public class DocumentUserControl : Custom.UI.CustomUserControl
    {
        private const string VIEWSTATE_IS_ON_CONTROL_MESSAGES = "IsOnControlMessages";
        public bool IsOnControlMessages
        {
            get { return ViewState[VIEWSTATE_IS_ON_CONTROL_MESSAGES] == null ? true : (bool)ViewState[VIEWSTATE_IS_ON_CONTROL_MESSAGES]; }
            set
            {
                ViewState[VIEWSTATE_IS_ON_CONTROL_MESSAGES] = value;
                SetIsOnControlMessages(Controls, value);
            }
        }

        private static void SetIsOnControlMessages(ControlCollection controls, bool value)
        {
            foreach (Control control in controls)
            {
                if (control is DocumentUserControl)
                {
                    ((DocumentUserControl)control).IsOnControlMessages = value;
                }
                SetIsOnControlMessages(control.Controls, value);                
            }
        }        

        public bool IsLogged()
        {
            return Membership.GetUser() != null;
        }

        public MembershipUser GetCurrentUser()
        {
            return Membership.GetUser();
        }
        

        public override bool CanBrowser()
        {
            return true;
        }

        public void SetErrorMessage(string message)
        {
            DocumentMessageList messageList = GetMessageList(this);
            if (messageList == null)
            {
                GetDocumentContentPage().SetErrorMessage(message);
            }
            else
            {
                messageList.SetErrorMessage(message);
            }
        }

        public void SetErrorMessage(List<string> messages)
        {
            DocumentMessageList messageList = GetMessageList(this);
            if (messageList == null)
            {
                GetDocumentContentPage().SetErrorMessage(messages);
            }
            else
            {
                messageList.SetErrorMessage(messages);
            }
        }

        public void SetMessage(string message)
        {            
            DocumentMessageList messageList = GetMessageList(this);
            if (messageList == null)
            {
                GetDocumentContentPage().SetMessage(message);
            }
            else
            {
                messageList.SetMessage(message);
            }
        }

        public void SetMessage(List<string> messages)
        {
            DocumentMessageList messageList = GetMessageList(this);
            if (messageList == null)
            {
                GetDocumentContentPage().SetMessage(messages);
            }
            else
            {
                messageList.SetMessage(messages);
            }
        }

        public void SetWarningMessage(string message)
        {
            DocumentMessageList messageList = GetMessageList(this);
            if (messageList == null)
            {
                GetDocumentContentPage().SetWarningMessage(message);
            }
            else
            {
                messageList.SetWarningMessage(message);
            }
        }

        public void SetWarningMessage(List<string> messages)
        {
            DocumentMessageList messageList = GetMessageList(this);
            if (messageList == null)
            {
                GetDocumentContentPage().SetWarningMessage(messages);
            }
            else
            {
                messageList.SetWarningMessage(messages);
            }
        }

        private DocumentContentPage GetDocumentContentPage()
        {
            return (DocumentContentPage)Page;
        }

        private DocumentMessageList GetMessageList(Control control)
        {
            if (IsOnControlMessages)
            {
                DocumentMessageList res = GetMessageListParent(control);
                if (res == null)
                {
                    return GetMessageListChield(control.Controls);
                }
                else
                {
                    return res;
                }
            }
            else
            {
                return null;
            }
        }

        private static DocumentMessageList GetMessageListChield(ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (IsMessageList(control))
                {
                    return (DocumentMessageList)control;
                }
                else
                {
                    DocumentMessageList res;
                    res = GetMessageListChield(control.Controls);
                    if (res != null)
                    {
                        return res;
                    }
                }
            }
            return null;
        }

        private static DocumentMessageList GetMessageListParent(Control control)
        {
            DocumentMessageList res = null;
            while (control.Parent != null)
            {
                foreach (Control _control in control.Controls)
                {
                    if (_control.ID != control.ID)
                    {
                        if (_control is UserControl)
                        {
                            if (IsMessageList(_control))
                            {
                                return (DocumentMessageList)_control;
                            }
                            else
                            {
                                res = GetMessageListChield(_control.Controls);
                                if (res != null)
                                {
                                    return res;
                                }
                            }
                        }
                    }
                }
                control = control.Parent;
            }
            return res;
        }

        private static bool IsMessageList(Control control)
        {
            if (control is UserControl)
            {
                foreach (Control _control in control.Controls)
                {
                    if (_control.ID == "hdn5E5184CFF0B74C838E75F475FB5755C7")
                    {
                        if (control.Visible)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }
    }
}
