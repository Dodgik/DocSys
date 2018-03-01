using System;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    public class QuestionType
    {
        private struct SpNames
        {
            public const string Get = "usp_QuestionType_Get";
            public const string Insert = "usp_QuestionType_Insert";
            public const string Update = "usp_QuestionType_Update";
            public const string Delete = "usp_QuestionType_Delete";
            public const string Search = "usp_QuestionType_Search";
            public const string List = "usp_QuestionType_List";
        }
        
        public const int ObjectTypeID = 13;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@QuestionTypeID", SqlDbType.Int)]
        //[FieldAttribute("QuestionTypeID")]
        public int ID { get; set; }

        //[ParamAttribute("@Name", SqlDbType.NVarChar, 100)]
        //[FieldAttribute("Name")]
        public string Name { get; set; }

        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public QuestionType(string userName)
        {
            UserName = userName;
        }

        public QuestionType(int id, string userName): this(userName)
        {
            Init(null, id);
        }

        public QuestionType(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int questionTypeId)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            prms[0].Value = questionTypeId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
            prms[1].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = questionTypeId;
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
            prms[0] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
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
            prms[0] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
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

        
        public static SqlDataReader GetReader(SqlConnection conectionString)
        {
            return SPHelper.ExecuteReader(conectionString, SpNames.List);
        }
        
        public static DataSet Search(SqlTransaction trans, string organizationName)
        {
            SqlParameter[] sps = new SqlParameter[1];
            sps[0] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
            sps[0].Value = organizationName;

            return SPHelper.ExecuteDataset(trans, SpNames.Search, sps);
        }

        public static DataSet Search(string organizationName)
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

                    ds = Search(trans, organizationName);

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
