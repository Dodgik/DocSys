using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BizObj.Document;

namespace BizObj.Models.Document
{
    [Serializable]
    public class DocTemplateWorkerBlank: DocTemplateBlank
    {
        #region Properties

        public List<ControlCardBlank> ControlCards
        {
            get;
            set;
        }
        
        #endregion

        #region Constructors
        
        public DocTemplateWorkerBlank()
        {

        }

        public DocTemplateWorkerBlank(string userName): base(userName)
        {
            
        }

        public DocTemplateWorkerBlank(int id, string userName): base(id, userName)
        {

        }

        public DocTemplateWorkerBlank(SqlTransaction trans, int id, int workerId, string userName): base(trans, id, userName)
        {
            ControlCards = ControlCard.GetCardsExternalToWorker(trans, DocumentID, workerId, UserName);
        }

        #endregion
    }
}
