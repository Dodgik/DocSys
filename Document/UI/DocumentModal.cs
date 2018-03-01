using System.Web.UI.HtmlControls;
using BizObj.CustomException;

namespace Document.UI
{
    public abstract class DocumentsModal : DocumentUserControl
    {
        protected void OpenDialog(string winClientID)
        {
            HtmlInputHidden hidden = (HtmlInputHidden)FindControl("hdnDialogWindowClientID");
            if (hidden == null)
            {
                throw new DocumentException("Input Hidden для запису CleintID вікна не знайдено");
            }
            else
            {
                hidden.Value = winClientID;
            }
        }
    }
}
