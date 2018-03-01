using System;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    public class DocumentLabel
    {
        private struct SpNames
        {
            public const string Get = "usp_DocumentLabel_Get";
            public const string Insert = "usp_DocumentLabel_Insert";
            public const string Delete = "usp_DocumentLabel_Delete";
            public const string List = "usp_DocumentLabel_List";
        }

        #region Properties

        public int ID { get; set; }
        public int LabelID { get; set; }
        public int DocumentID { get; set; }
        public int DepartmentID { get; set; }
        public int WorkerID { get; set; }
        public DateTime CreateDate { get; set; }
        
        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public DocumentLabel(string userName)
        {
            UserName = userName;
        }

        public DocumentLabel(int id, string userName): this(null, id, userName)
        {

        }

        public DocumentLabel(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
        }

        private DocumentLabel()
        {
        }

        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int documentLabelId)
        {
            SqlParameter[] prms = new SqlParameter[6];
            prms[0] = new SqlParameter("@DocumentLabelID", SqlDbType.Int);
            prms[0].Value = documentLabelId;

            prms[1] = new SqlParameter("@LabelID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@CreateDate", SqlDbType.DateTime);
            prms[5].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = documentLabelId;
            LabelID = (int)prms[1].Value;
            DocumentID = (int)prms[2].Value;
            DepartmentID = (int)prms[3].Value;
            WorkerID = (int)prms[4].Value;
            CreateDate = (DateTime)prms[5].Value;
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@DocumentLabelID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@LabelID", SqlDbType.Int);
            prms[1].Value = LabelID;

            prms[2] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[2].Value = DocumentID;

            prms[3] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[3].Value = DepartmentID;

            prms[4] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[4].Value = WorkerID;

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

        #endregion
        
        #region Static Public Methods
        public static DataTable GetList(SqlTransaction trans, int documentId, int departmentId)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentId;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[1].Value = departmentId;

            if (trans == null) {
                return SPHelper.ExecuteDataset(SpNames.List, prms).Tables[0];
            }
            return SPHelper.ExecuteDataset(trans, SpNames.List, prms).Tables[0];
        }
        public static DataTable GetList(SqlTransaction trans, int documentId)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentId;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[1].Value = DBNull.Value;

            if (trans == null)
            {
                return SPHelper.ExecuteDataset(SpNames.List, prms).Tables[0];
            }
            return SPHelper.ExecuteDataset(trans, SpNames.List, prms).Tables[0];
        }

        public static int[] GetDocumentLabelIds(SqlTransaction trans, int documentId, int departmentId)
        {
            DataTable dtLabels = GetList(trans, documentId, departmentId);

            int[] labels = new int[dtLabels.Rows.Count];

            int i = 0;
            foreach (DataRow rowBranchType in dtLabels.Rows) {
                labels[i] = (int)rowBranchType["LabelID"];
                i++;
            }

            return labels;
        }
        public static int[] GetDocumentLabelIds(SqlTransaction trans, int documentId)
        {
            DataTable dtLabels = GetList(trans, documentId);

            int[] labels = new int[dtLabels.Rows.Count];

            int i = 0;
            foreach (DataRow rowBranchType in dtLabels.Rows)
            {
                labels[i] = (int)rowBranchType["LabelID"];
                i++;
            }

            return labels;
        }

        public static DocumentLabel[] GetDocumentLabels(SqlTransaction trans, int documentId, int departmentId)
        {
            DataTable dtLabels = GetList(trans, documentId, departmentId);

            DocumentLabel[] labels = new DocumentLabel[dtLabels.Rows.Count];

            int i = 0;
            foreach (DataRow rowBranchType in dtLabels.Rows) {
                DocumentLabel label = new DocumentLabel();
                label.ID = (int)rowBranchType["DocumentLabelID"];
                label.LabelID = (int)rowBranchType["LabelID"];
                label.DocumentID = documentId;
                label.DepartmentID = departmentId;
                label.WorkerID = (int)rowBranchType["WorkerID"];
                label.CreateDate = (DateTime)rowBranchType["CreateDate"];
                labels[i] = label;
                i++;
            }

            return labels;
        }

        public static void DeleteList(SqlTransaction trans, int documentId, int? departmentId)
        {
            Delete(trans, documentId, departmentId, null);
        }

        public static void DeleteList(SqlTransaction trans, int documentId)
        {
            Delete(trans, documentId, null, null);
        }

        public static void Delete(SqlTransaction trans, int documentId, int? departmentId, int? documentLabelId)
        {
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentId;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            if (departmentId != null)
                prms[1].Value = departmentId;
            else
                prms[1].Value = DBNull.Value;

            prms[2] = new SqlParameter("@DocumentLabelID", SqlDbType.Int);
            if (documentLabelId != null)
                prms[2].Value = documentLabelId;
            else
                prms[2].Value = DBNull.Value;

            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, prms);
        }

        public static void Delete(int documentId, int? departmentId, int? documentLabelId)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    Delete(trans, documentId, departmentId, documentLabelId);

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
