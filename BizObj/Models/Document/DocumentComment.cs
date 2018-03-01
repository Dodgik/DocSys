using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    public class DocumentComment
    {
        private struct SpNames
        {
            public const string Get = "usp_DocumentComment_Get";
            public const string Insert = "usp_DocumentComment_Insert";
            public const string Update = "usp_DocumentComment_Update";
            public const string Delete = "usp_DocumentComment_Delete";
            public const string List = "usp_DocumentComment_List";
        }
        
        #region Properties

        public int ID { get; set; }
        public int DocumentID { get; set; }
        public int WorkerID { get; set; }
        public int BehalfWorkerID { get; set; }
        public string Content { get; set; }
        public int DocumentCommentTypeID { get; set; }
        public int? ControlCardID { get; set; }
        public int? ParentDocumentCommentID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public Worker Worker { get; set; }

        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public DocumentComment()
        {
        }

        public DocumentComment(string userName)
        {
            UserName = userName;
        }

        public DocumentComment(int id, string userName): this(userName)
        {
            Init(null, id);
        }

        public DocumentComment(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int documentCommentId)
        {
            SqlParameter[] prms = new SqlParameter[10];
            prms[0] = new SqlParameter("@DocumentCommentID", SqlDbType.Int);
            prms[0].Value = documentCommentId;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;
            
            prms[2] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@BehalfWorkerID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[4].Direction = ParameterDirection.Output;
            
            prms[5] = new SqlParameter("@DocumentCommentTypeID", SqlDbType.Int);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@ParentDocumentCommentID", SqlDbType.Int);
            prms[7].Direction = ParameterDirection.Output;

            prms[8] = new SqlParameter("@CreateDate", SqlDbType.DateTime);
            prms[8].Direction = ParameterDirection.Output;
            
            prms[9] = new SqlParameter("@LastUpdateDate", SqlDbType.DateTime);
            prms[9].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = documentCommentId;
            DocumentID = (int)prms[1].Value;
            WorkerID = (int)prms[2].Value;
            BehalfWorkerID = (int)prms[3].Value;
            Content = (string)prms[4].Value;
            DocumentCommentTypeID = (int)prms[5].Value;
            ControlCardID = prms[6].Value != DBNull.Value ? (int)prms[6].Value : 0;
            ParentDocumentCommentID = prms[7].Value != DBNull.Value ? (int)prms[7].Value : 0;
            CreateDate = (DateTime)prms[8].Value;
            LastUpdateDate = (DateTime)prms[9].Value;
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            SqlParameter[] prms = new SqlParameter[8];
            prms[0] = new SqlParameter("@DocumentCommentID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[2].Value = WorkerID;

            prms[3] = new SqlParameter("@BehalfWorkerID", SqlDbType.Int);
            prms[3].Value = BehalfWorkerID;

            prms[4] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[4].Value = Content;

            prms[5] = new SqlParameter("@DocumentCommentTypeID", SqlDbType.Int);
            prms[5].Value = DocumentCommentTypeID;

            prms[6] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            if (ControlCardID != null)
                prms[6].Value = ControlCardID;
            else
                prms[6].Value = DBNull.Value;

            prms[7] = new SqlParameter("@ParentDocumentCommentID", SqlDbType.Int);
            if (ParentDocumentCommentID != null)
                prms[7].Value = ParentDocumentCommentID;
            else
                prms[7].Value = DBNull.Value;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Insert, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Insert, prms);

            ID = (int)prms[0].Value;

            return ID;
        }

        public int Insert()
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Insert(trans);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
            return ID;
        }

        public void Update(SqlTransaction trans)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentCommentID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            //prms[1].Value = Name;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Update, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Update, prms);
        }

        public void Update()
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Update(trans);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
        }


        public static List<DocumentComment> GetList(int documentId)
        {
            List<DocumentComment> comments = new List<DocumentComment>();

            DataTable dcTable = SPHelper.ExecuteDataset(SpNames.List, documentId).Tables[0];

            foreach (DataRow row in dcTable.Rows) {
                DocumentComment comment = new DocumentComment();
                comment.ID = (int)row["DocumentCommentID"];
                comment.DocumentID = (int)row["DocumentID"];
                comment.WorkerID = (int)row["WorkerID"];
                comment.BehalfWorkerID = (int)row["BehalfWorkerID"];
                comment.Content = row["Content"].ToString();
                comment.DocumentCommentTypeID = (int)row["DocumentCommentTypeID"];
                comment.ControlCardID = row["ControlCardID"] == DBNull.Value ? null : (int?)row["ControlCardID"];
                comment.ParentDocumentCommentID = row["ParentDocumentCommentID"] == DBNull.Value ? null : (int?)row["ParentDocumentCommentID"];
                comment.CreateDate = (DateTime)row["CreateDate"];
                comment.LastUpdateDate = (DateTime)row["LastUpdateDate"];
                Worker worker = new Worker();
                worker.ID = comment.WorkerID;
                worker.FirstName = row["WorkerFirstName"].ToString();
                worker.LastName = row["WorkerLastName"].ToString();
                worker.MiddleName = row["WorkerMiddleName"].ToString();
                comment.Worker = worker;
                comments.Add(comment);
            }

            return comments;
        }


        public static SqlDataReader GetReader(SqlConnection conectionString, int documentId)
        {
            return SPHelper.ExecuteReader(conectionString, SpNames.List, documentId);
        }

        #endregion
        
        #region Static Public Methods
        
        public static void Delete(SqlTransaction trans, int id)
        {
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@DocumentCommentID", SqlDbType.Int);
            prms[0].Value = id;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Delete, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Delete, prms);
        }

        public static void Delete(int id)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans, id);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
        }
        #endregion
    }
}
