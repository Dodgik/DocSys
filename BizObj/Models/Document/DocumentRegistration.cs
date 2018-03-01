using System;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    public class DocumentRegistration
    {
        private struct SpNames
        {
            public const string Get = "usp_DocumentRegistration_Get";
            public const string GetByDocumentIDAndDepartmentID = "usp_DocumentRegistration_GetByDocumentIDAndDepartmentID";
            public const string Insert = "usp_DocumentRegistration_Insert";
            public const string Update = "usp_DocumentRegistration_Update";
            public const string Delete = "usp_DocumentRegistration_Delete";
            public const string List = "usp_DocumentRegistration_List";
            public const string IsExist = "usp_DocumentRegistration_IsExist";
        }

        #region Properties

        public int ID { get; set; }
        public int DocumentID { get; set; }
        public int DepartmentID { get; set; }
        public int WorkerID { get; set; }
        public string Number { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        private string UserName { get; set; }
        #endregion

        #region Constructors

        public DocumentRegistration()
        {
        }

        public DocumentRegistration(string userName)
        {
            UserName = userName;
        }

        public DocumentRegistration(int id, string userName)
            : this(userName)
        {
            Init(null, id);
        }

        public DocumentRegistration(SqlTransaction trans, int id, string userName)
            : this(userName)
        {
            Init(trans, id);
        }

        public DocumentRegistration(SqlTransaction trans, int documentId, int departmentId)
        {
            Init(trans, documentId, departmentId);
        }

        public DocumentRegistration(int documentId, int departmentId)
        {
            Init(null, documentId, departmentId);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int documentRegistrationId)
        {
            SqlParameter[] prms = new SqlParameter[7];
            prms[0] = new SqlParameter("@DocumentRegistrationID", SqlDbType.Int);
            prms[0].Value = documentRegistrationId;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@CreateDate", SqlDbType.DateTime);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@LastUpdateDate", SqlDbType.DateTime);
            prms[6].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = documentRegistrationId;
            DocumentID = (int)prms[1].Value;
            DepartmentID = (int)prms[2].Value;
            WorkerID = (int)prms[3].Value;
            Number = (string)prms[4].Value;
            CreateDate = (DateTime)prms[5].Value;
            LastUpdateDate = (DateTime)prms[6].Value;
        }

        private void Init(SqlTransaction trans, int documentId, int departmentId)
        {
            SqlParameter[] prms = new SqlParameter[7];
            prms[0] = new SqlParameter("@DocumentRegistrationID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = documentId;

            prms[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[2].Value = departmentId;

            prms[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@CreateDate", SqlDbType.DateTime);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@LastUpdateDate", SqlDbType.DateTime);
            prms[6].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.GetByDocumentIDAndDepartmentID, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.GetByDocumentIDAndDepartmentID, prms);

            ID = (int)prms[0].Value;
            DocumentID = documentId;
            DepartmentID = departmentId;
            WorkerID = (int)prms[3].Value;
            Number = (string)prms[4].Value;
            CreateDate = (DateTime)prms[5].Value;
            LastUpdateDate = (DateTime)prms[6].Value;
        }

        #endregion

        #region Public Methods

        public int Insert(SqlTransaction trans)
        {
            SqlParameter[] prms = new SqlParameter[5];
            prms[0] = new SqlParameter("@DocumentRegistrationID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[2].Value = DepartmentID;

            prms[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[3].Value = WorkerID;

            prms[4] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[4].Value = Number;

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
            prms[0] = new SqlParameter("@DocumentRegistrationID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[1].Value = Number;

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


        public static DataTable GetList(int departmentId)
        {
            return SPHelper.ExecuteDataset(SpNames.List, departmentId).Tables[0];
        }

        public static DataTable GetList()
        {
            return SPHelper.ExecuteDataset(SpNames.List, DBNull.Value).Tables[0];
        }

        public static SqlDataReader GetReader(SqlConnection conectionString, int departmentId)
        {
            return SPHelper.ExecuteReader(conectionString, SpNames.List, departmentId);
        }

        #endregion

        #region Static Public Methods

        public static bool IsExist(SqlTransaction trans, int documentId, int departmentId)
        {
            bool result = false;

            if (documentId > 0)
            {
                var arParams = new SqlParameter[3];

                arParams[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
                arParams[0].Value = documentId;

                arParams[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
                arParams[1].Value = departmentId;

                arParams[2] = new SqlParameter("@Result", SqlDbType.Bit);
                arParams[2].Direction = ParameterDirection.Output;

                SPHelper.ExecuteNonQuery(trans, SpNames.IsExist, arParams);

                if (arParams[2].Value != DBNull.Value)
                {
                    result = (bool)arParams[2].Value;
                }
            }
            return result;
        }

        public static bool IsExist(int documentId, int departmentId)
        {
            bool result = false;
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    result = IsExist(trans, documentId, departmentId);

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
            return result;
        }

        public static void Delete(SqlTransaction trans, int id)
        {
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@DocumentRegistrationID", SqlDbType.Int);
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
