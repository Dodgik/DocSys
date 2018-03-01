using System;
using System.Data.SqlClient;
using BizObj.Document;

namespace BizObj.Models.Document
{
    [Serializable]
    public class DocStatementBlank : DocStatement
    {
        #region Properties


        #endregion

        #region Constructors
        
        public DocStatementBlank()
        {

        }

        public DocStatementBlank(string userName): base(userName)
        {
            
        }

        public DocStatementBlank(int id, string userName): base(id, userName)
        {

        }

        public DocStatementBlank(SqlTransaction trans, int id, string userName): base(trans, id, userName)
        {
            
        }


        #endregion

        #region Static Public Methods


        #endregion
    }
}
