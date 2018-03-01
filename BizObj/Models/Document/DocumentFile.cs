using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Document;

namespace BizObj.Models.Document
{
    [Serializable]
    public class DocumentFile
    {
        private struct SpNames
        {
            public const string Get = "usp_DocumentFile_Get";
            public const string Insert = "usp_DocumentFile_Insert";
            public const string Delete = "usp_DocumentFile_Delete";
            public const string DeleteFiles = "usp_DocumentFile_DeleteFiles";
            public const string List = "usp_DocumentFile_GetList";
            public const string ListDocumentIDs = "usp_DocumentFile_GetDocumentIDsList";
            public const string CountDocuments = "usp_DocumentFile_CountDocuments";
        }

        #region Properties

        public int DocumentID { get; set; }

        public int FileID { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

        public int DepartmentID { get; set; }
        public int WorkerID { get; set; }

        [ScriptIgnore]
        public Worker Worker { get; set; }

        #endregion
        
        #region Constructors
        public DocumentFile()
        {

        }

        public DocumentFile(Worker worker)
        {
            Worker = worker;
        }

        public DocumentFile(SqlTransaction trans, int fileID, Worker worker): this(worker)
        {
            if (trans == null)
                Init(fileID);
            else
                Init(trans, fileID);
        }

        public DocumentFile(int fileID, Worker worker): this(null, fileID, worker)
        {
            
        }
        #endregion
        
        #region Private Methods
        private void Init(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Init(trans, id);

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

        private void Init(SqlTransaction trans, int fileID)
        {
            FileID = fileID;
            SqlParameter[] sps = new SqlParameter[6];

            sps[0] = new SqlParameter("@FileID", SqlDbType.Int);
            sps[0].Value = fileID;

            sps[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            sps[1].Direction = ParameterDirection.Output;

            sps[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[2].Direction = ParameterDirection.Output;

            sps[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[3].Direction = ParameterDirection.Output;

            sps[4] = new SqlParameter("@FileName", SqlDbType.NVarChar, 127);
            sps[4].Direction = ParameterDirection.Output;

            sps[5] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            sps[5].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, SpNames.Get, sps);

            if (sps[1].Value != DBNull.Value)
                DocumentID = Convert.ToInt32(sps[1].Value);
            if (sps[2].Value != DBNull.Value)
                DepartmentID = Convert.ToInt32(sps[2].Value);
            if (sps[3].Value != DBNull.Value)
                WorkerID = Convert.ToInt32(sps[3].Value);
            if (sps[4].Value != DBNull.Value)
                FileName = Convert.ToString(sps[4].Value) + "." + Convert.ToString(sps[5].Value);
            if (sps[5].Value != DBNull.Value)
                Extension = Convert.ToString(sps[5].Value);
        }
        #endregion

        #region Public Methods
        public void Insert(SqlTransaction trans)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@FileID", SqlDbType.Int);
            sps[0].Value = FileID;

            sps[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            sps[1].Value = DocumentID;

            sps[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[2].Value = Worker.DepartmentID;

            sps[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[3].Value = Worker.ID;

            SPHelper.ExecuteNonQuery(trans, SpNames.Insert, sps);
        }

        public void Insert()
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
        }
        #endregion

        #region Static Public Methods
        #region Base Methods
        public static void Delete(SqlTransaction trans, int fileID, int documentID, Worker worker)
        {
            DocumentFile df = new DocumentFile(trans, fileID, worker);
            if (df.DepartmentID != worker.DepartmentID)
            {
                throw new AccessException(worker.LastName + " " + worker.FirstName + " " + worker.MiddleName, "Delete");
            }

            SqlParameter[] sps = new SqlParameter[2];

            sps[0] = new SqlParameter("@FileID", SqlDbType.Int);
            sps[0].Value = fileID;

            sps[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            sps[1].Value = documentID;

            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, sps);
        }

        public static void Delete(int fileID, int documentID, Worker worker)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans, fileID, documentID, worker);

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

        public static void DeleteFiles(SqlTransaction trans, int documentID, Worker worker)
        {
            SqlParameter[] sps = new SqlParameter[1];

            sps[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            sps[0].Value = documentID;

            SPHelper.ExecuteNonQuery(trans, SpNames.DeleteFiles, sps);
        }

        public static void DeleteFiles(int documentID, Worker worker)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DeleteFiles(trans, documentID, worker);

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

        public static List<DocumentFile> GetList(SqlTransaction trans, int documentID)
        {
            List<DocumentFile> list = new List<DocumentFile>();
            SqlParameter[] sps = new SqlParameter[1];

            sps[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            sps[0].Value = documentID;

            if (trans == null) {
                using (SqlDataReader dataReader = SPHelper.ExecuteReader(SpNames.List, sps))
                {
                    DocumentFile df;
                    while (dataReader.Read())
                    {
                        df = new DocumentFile();
                        df.DocumentID = (int) dataReader["DocumentID"];
                        df.DepartmentID = (int) dataReader["DepartmentID"];
                        df.WorkerID = (int) dataReader["WorkerID"];
                        df.FileID = (int) dataReader["FileID"];
                        df.FileName = Convert.ToString(dataReader["FileName"]) + "." +
                                      Convert.ToString(dataReader["Extension"]);
                        df.Extension = Convert.ToString(dataReader["Extension"]);
                        list.Add(df);
                    }
                }
            } else {
                using (SqlDataReader dataReader = SPHelper.ExecuteReader(trans, SpNames.List, sps))
                {
                    DocumentFile df;
                    while (dataReader.Read())
                    {
                        df = new DocumentFile();
                        df.DocumentID = (int)dataReader["DocumentID"];
                        df.DepartmentID = (int)dataReader["DepartmentID"];
                        df.WorkerID = (int)dataReader["WorkerID"];
                        df.FileID = (int)dataReader["FileID"];
                        df.FileName = Convert.ToString(dataReader["FileName"]) + "." +
                                      Convert.ToString(dataReader["Extension"]);
                        df.Extension = Convert.ToString(dataReader["Extension"]);
                        list.Add(df);
                    }
                }
            }
            return list;
        }

        public static List<DocumentFile> GetList(int documentID)
        {
            List<DocumentFile> list;
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    list = GetList(trans, documentID);

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
            return list;
        }


        public static int GetCountDocuments(SqlTransaction trans, int fileId)
        {
            SqlParameter[] sps = new SqlParameter[2];

            sps[0] = new SqlParameter("@FileID", SqlDbType.Int);
            sps[0].Value = fileId;

            sps[1] = new SqlParameter("@Count", SqlDbType.Int);
            sps[1].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, SpNames.CountDocuments, sps);

            if (sps[1].Value != DBNull.Value)
            {
                return (int) sps[1].Value;
            }
            return 0;
        }

        public static bool IsAttached(SqlTransaction trans, int fileId)
        {
            return GetCountDocuments(trans, fileId) > 0;
        }

        public static DataSet GetDocumentIDsList(SqlTransaction trans, int fileId)
        {
            SqlParameter[] sps = new SqlParameter[1];

            sps[0] = new SqlParameter("@FileID", SqlDbType.Int);
            sps[0].Value = fileId;

            return SPHelper.ExecuteDataset(trans, SpNames.ListDocumentIDs, sps);
        }

        public static List<int> GetDocumentIDsList(SqlTransaction trans, int fileId, Worker worker)
        {
            DataTable dt = GetDocumentIDsList(trans, fileId).Tables[0];

            List<int> list = new List<int>();

            foreach (DataRow r in dt.Rows)
            {
                list.Add((int) r["FileID"]);
            }

            return list;
        }
        #endregion


        #endregion
    }
}