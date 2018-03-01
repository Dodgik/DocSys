using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BizObj.Document;

namespace BizObj.Models.Document
{
    [Serializable]
    public class DocStatementAdminBlank : DocStatementBlank
    {
        #region Properties

        public List<ControlCardBlank> ControlCards
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        public DocStatementAdminBlank()
        {

        }

        public DocStatementAdminBlank(string userName): base(userName)
        {

        }

        public DocStatementAdminBlank(int id, string userName): base(id, userName)
        {

        }

        public DocStatementAdminBlank(SqlTransaction trans, int id, int departmentId, string userName): base(trans, id, userName)
        {
            ControlCards = ControlCard.GetCardsExternalToDepartment(trans, DocumentID, departmentId, UserName);
        }

        #endregion
    }
}
