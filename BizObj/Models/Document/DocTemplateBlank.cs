using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using BizObj.Document;

namespace BizObj.Models.Document
{
    [Serializable]
    public class DocTemplateBlank: DocTemplate
    {
        #region Properties

        public DocType DocType
        {
            get;
            set;
        }

        public QuestionType QuestionType
        {
            get;
            set;
        }

        public Worker Head
        {
            get;
            set;
        }

        public Worker Worker
        {
            get;
            set;
        }

        #endregion

        #region Constructors
        
        public DocTemplateBlank()
        {

        }

        public DocTemplateBlank(string userName): base(userName)
        {
            
        }

        public DocTemplateBlank(int id, string userName): base(id, userName)
        {

        }

        public DocTemplateBlank(SqlTransaction trans, int id, string userName): base(trans, id, userName)
        {
            DocType = new DocType(trans, DocTypeID, userName);
            QuestionType = new QuestionType(trans, QuestionTypeID, userName);
            Head = new Worker(trans, HeadID, userName);
            Worker = new Worker(trans, WorkerID, userName);
        }


        #endregion

        #region Static Public Methods


        #endregion
    }
}
