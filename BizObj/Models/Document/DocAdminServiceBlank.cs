using System;
using System.Data.SqlClient;
using BizObj.Document;

namespace BizObj.Models.Document
{
    [Serializable]
    public class DocAdminServiceBlank : DocAdminService
    {
        #region Properties
        public Worker ReceivedWorker { get; set; }
        public Worker ReturnWorker { get; set; }
        public Department ExecutiveDepartment { get; set; }
        #endregion

        #region Constructors
        
        public DocAdminServiceBlank()
        {

        }

        public DocAdminServiceBlank(string userName): base(userName)
        {
            
        }

        public DocAdminServiceBlank(int id, string userName): base(id, userName)
        {

        }

        public DocAdminServiceBlank(SqlTransaction trans, int id, string userName): base(trans, id, userName)
        {
            ReceivedWorker = new Worker(trans, ReceivedWorkerID, userName);
            ReturnWorker = new Worker(trans, ReturnWorkerID, userName);
            ExecutiveDepartment = new Department(trans, ExecutiveDepartmentID, userName);
        }
        
        #endregion

        #region Static Public Methods


        #endregion
    }
}