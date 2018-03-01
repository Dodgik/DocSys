using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class ExternalSource
    {
        private struct SpNames
        {
            public const string Get = "usp_ExternalSource_Get";
            public const string Insert = "usp_ExternalSource_Insert";
            public const string Update = "usp_ExternalSource_Update";
            public const string Delete = "usp_ExternalSource_Delete";
            public const string IsExist = "usp_ExternalSource_IsExist";
            public const string List = "usp_ExternalSource_List";
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

        //[ParamAttribute("@DocumentID", SqlDbType.Int)]
        //[FieldAttribute("DocumentID")]
        public int ID { get; set; }

        //[ParamAttribute("@CreationDate", SqlDbType.DateTime)]
        //[FieldAttribute("CreationDate")]
        public DateTime CreationDate { get; set; }

        //[ParamAttribute("@ExternalNumber", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("ExternalNumber")]
        public string Number { get; set; }

        //[ParamAttribute("@OrganizationID", SqlDbType.Int)]
        //[FieldAttribute("OrganizationID")]
        public int OrganizationID { get; set; }

        public string OrganizationName { get; set; }

        [ScriptIgnore]
        public string UserName { get; set; }
        #endregion
        
        #region Constructors

        public ExternalSource()
        {

        }

        public ExternalSource(string userName)
        {
            UserName = userName;
        }

        public ExternalSource(int id, string userName): this(null, id, userName)
        {
            
        }

        public ExternalSource(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);

            Organization organization = new Organization(trans, OrganizationID, userName);
            OrganizationName = organization.Name;
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int documentId)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentId;

            prms[1] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@ExternalNumber", SqlDbType.NVarChar, 50);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = documentId;
            CreationDate = (DateTime)prms[1].Value;
            Number = (string)prms[2].Value;
            OrganizationID = (int)prms[3].Value;
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[1].Value = CreationDate;

            prms[2] = new SqlParameter("@ExternalNumber", SqlDbType.NVarChar, 50);
            prms[2].Value = Number;

            prms[3] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[3].Value = OrganizationID;

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

            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[1].Value = CreationDate;

            prms[2] = new SqlParameter("@ExternalNumber", SqlDbType.NVarChar, 50);
            prms[2].Value = Number;

            prms[3] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            prms[3].Value = OrganizationID;

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
