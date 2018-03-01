using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class Source
    {
        private struct SpNames
        {
            public const string Get = "usp_Source_Get";
            public const string Insert = "usp_Source_Insert";
            public const string Update = "usp_Source_Update";
            public const string Delete = "usp_Source_Delete";
            public const string IsExist = "usp_Source_IsExist";
            public const string List = "usp_Source_List";
        }
        
        public const int ObjectTypeID = 2;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        public int ID { get; set; }
        public int? CitizenID { get; set; }
        public int? OrganizationID { get; set; }
        public int? DepartmentID { get; set; }
        public int? WorkerID { get; set; }
        public DateTime CreationDate { get; set; }
        public string Number { get; set; }

        public string OrganizationName { get; set; }
        public string DepartmentName { get; set; }
        public Worker Worker { get; set; }
        public Citizen Citizen { get; set; }

        [ScriptIgnore]
        public string UserName { get; set; }
        #endregion
        
        #region Constructors

        public Source()
        {

        }

        public Source(string userName)
        {
            UserName = userName;
        }

        public Source(int id, string userName): this(null, id, userName)
        {
            
        }

        public Source(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);

            if (OrganizationID != null)
            {
                OrganizationName = new Organization(trans, (int) OrganizationID, userName).Name;
            }

            if (DepartmentID != null)
            {
                DepartmentName = new Department(trans, (int)DepartmentID, userName).Name;
            }

            if (WorkerID != null)
            {
                Worker = new Worker(trans, (int)WorkerID, userName);
            }

            if (CitizenID != null)
            {
                Citizen = new Citizen(trans, (int)CitizenID, userName);
            }
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int documentId)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }

            SqlParameter[] prms = new SqlParameter[7];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentId;

            prms[1] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[6].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = documentId;
            CitizenID = prms[1].Value != DBNull.Value ? (int?) prms[1].Value : null;
            OrganizationID = prms[2].Value != DBNull.Value ? (int?) prms[2].Value : null;
            DepartmentID = prms[3].Value != DBNull.Value ? (int?) prms[3].Value : null;
            WorkerID = prms[4].Value != DBNull.Value ? (int?) prms[4].Value : null;
            CreationDate = prms[5].Value != DBNull.Value ? (DateTime) prms[5].Value : (DateTime) SqlDateTime.MinValue;
            Number = prms[6].Value != DBNull.Value ? (string) prms[6].Value : String.Empty;
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[7];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[1].IsNullable = true;
            if (CitizenID > 0)
            {
                prms[1].Value = CitizenID;
            }
            else
            {
                prms[1].Value = DBNull.Value;
            }

            prms[2] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[2].Value = OrganizationID;

            prms[3] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[3].Value = DepartmentID;

            prms[4] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[4].Value = WorkerID;

            prms[5] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[5].Value = CreationDate;

            prms[6] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[6].Value = Number;

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
            if (!CanUpdate(UserName))
            {
                throw new AccessException(UserName, "Update");
            }

            SqlParameter[] prms = new SqlParameter[7];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[1].IsNullable = true;
            if (CitizenID > 0)
            {
                prms[1].Value = CitizenID;
            }
            else
            {
                prms[1].Value = DBNull.Value;
            }

            prms[2] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[2].IsNullable = true;
            if (OrganizationID > 0)
            {
                prms[2].Value = OrganizationID;
            }
            else
            {
                prms[2].Value = DBNull.Value;
            }

            prms[3] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[3].IsNullable = true;
            if (DepartmentID > 0)
            {
                prms[3].Value = DepartmentID;
            }
            else
            {
                prms[3].Value = DBNull.Value;
            }

            prms[4] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[4].IsNullable = true;
            if (WorkerID > 0)
            {
                prms[4].Value = WorkerID;
            }
            else
            {
                prms[4].Value = DBNull.Value;
            }

            prms[5] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[5].Value = CreationDate;

            prms[6] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[6].Value = Number;

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

        public static DataTable GetAll(SqlConnection conectionString)
        {
            return SPHelper.ExecuteDataset(conectionString, SpNames.List).Tables[0];
        }
        public static DataTable GetAll()
        {
            return SPHelper.ExecuteDataset(SpNames.List).Tables[0];
        }
        #endregion
        
        #region Static Public Methods
        
        public static bool IsExist(SqlTransaction trans, int id)
        {
            bool result = false;

            if (id > 0)
            {
                var arParams = new SqlParameter[2];

                arParams[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
                arParams[0].Value = id;

                arParams[1] = new SqlParameter("@Result", SqlDbType.Bit);
                arParams[1].Direction = ParameterDirection.Output;

                if (trans == null)
                    SPHelper.ExecuteNonQuery(SpNames.IsExist, arParams);
                else
                    SPHelper.ExecuteNonQuery(trans, SpNames.IsExist, arParams);

                if (arParams[1].Value != DBNull.Value)
                {
                    result = (bool) arParams[1].Value;
                }
            }
            return result;
        }

        public static void Delete(SqlTransaction trans, int id, string userName)
        {
            if (!CanDelete(userName))
            {
                throw new AccessException(userName, "Delete");
            }

            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, id);
        }

        public static void Delete(int id, string userName)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans, id, userName);

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
            
        public static bool CanInsert(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Insert);
        }

        public static bool CanUpdate(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Update);
        }

        public static bool CanDelete(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Delete);
        }

        public static bool CanView(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.View);
        }
        #endregion
        
    }
}
