using System;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class DocStatus : SimpleDictionary
    {
        private struct SpNames
        {
            public const string Get = "usp_DocStatus_Get";
            public const string Insert = "usp_DocStatus_Insert";
            public const string Update = "usp_DocStatus_Update";
            public const string Delete = "usp_DocStatus_Delete";
            public const string Search = "usp_DocStatus_Search";
            public const string List = "usp_DocStatus_List";
        }
        
        public const int ObjectTypeID = 8;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@DocStatusID", SqlDbType.Int)]
        //[FieldAttribute("DocStatusID")]
        public new int ID { get; set; }

        //[ParamAttribute("@Name", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("Name")]
        public new string Name { get; set; }

        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public DocStatus(string userName)
        {
            UserName = userName;
        }

        public DocStatus(int id, string userName): this(userName)
        {
            Init(null, id);
        }

        public DocStatus(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int docStatusId)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocStatusID", SqlDbType.Int);
            prms[0].Value = docStatusId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            prms[1].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = docStatusId;
            Name = (string)prms[1].Value;
        }

        #endregion


        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocStatusID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            prms[1].Value = Name;

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

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocStatusID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            prms[1].Value = Name;

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

        public static DataTable GetAll()
        {
            return SPHelper.ExecuteDataset(SpNames.List).Tables[0];
        }

        public static SqlDataReader GetReader(SqlConnection conectionString)
        {
            return SPHelper.ExecuteReader(conectionString, SpNames.List);
        }

        public static DataSet Search(SqlTransaction trans, string docStatusName)
        {
            SqlParameter[] sps = new SqlParameter[1];
            sps[0] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            sps[0].Value = docStatusName;

            return SPHelper.ExecuteDataset(trans, SpNames.Search, sps);
        }

        public static DataSet Search(string docStatusName)
        {
            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = Search(trans, docStatusName);

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

            return ds;
        }
        #endregion


        #region Static Public Methods
        
        public static void Delete(SqlTransaction trans, int id)
        {
            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, id);
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
            
        public static bool CanInsert(string userName)
        {
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Insert);
        }

        public static bool CanUpdate(string userName)
        {
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Update);
        }

        public static bool CanDelete(string userName)
        {
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Delete);
        }

        public static bool CanView(string userName)
        {
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.View);
        }
        #endregion
        
    }
}