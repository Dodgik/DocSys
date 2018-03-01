using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BizObj.Document;

namespace BizObj.Models.Document
{
    [Serializable]
    public class DocTemplateAdminBlank : DocTemplateBlank
    {
        #region Properties

        public List<ControlCardBlank> ControlCards
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        public DocTemplateAdminBlank()
        {

        }

        public DocTemplateAdminBlank(string userName): base(userName)
        {

        }

        public DocTemplateAdminBlank(int id, string userName): base(id, userName)
        {

        }

        public DocTemplateAdminBlank(SqlTransaction trans, int id, int departmentId, string userName): base(trans, id, userName)
        {
            ControlCards = ControlCard.GetCardsExternalToDepartment(trans, DocumentID, departmentId, UserName);
        }

        #endregion
    }
}
