using System.Collections.Generic;

namespace Document.UI
{
    public abstract class DocumentMessageList : DocumentUserControl
    {
        public new abstract void SetErrorMessage(string message);

        public new abstract void SetErrorMessage(List<string> messages);

        public new abstract void SetMessage(string message);

        public new abstract void SetMessage(List<string> messages);

        public new abstract void SetWarningMessage(string message);

        public new abstract void SetWarningMessage(List<string> messages);
    }
}
