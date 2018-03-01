using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BizObj.Document;

namespace BizObj.Models.Document
{
    [Serializable]
    public class DocStatementWorkerBlank : DocStatement
    {
        #region Properties

        public List<ControlCardBlank> ControlCards
        {
            get;
            set;
        }
        
        #endregion

        #region Constructors
        
        public DocStatementWorkerBlank()
        {

        }

        public DocStatementWorkerBlank(string userName): base(userName)
        {
            
        }

        public DocStatementWorkerBlank(int id, string userName): base(id, userName)
        {

        }

        public DocStatementWorkerBlank(SqlTransaction trans, int id, int workerId, string userName): base(trans, id, userName)
        {
            ControlCards = ControlCard.GetCardsExternalToWorker(trans, DocumentID, workerId, UserName);
        }

        #endregion
    }
}
