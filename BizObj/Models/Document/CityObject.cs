using System;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    public class CityObject
    {
        private struct SpNames
        {
            public const string Get = "usp_CityObjects_Get";
            public const string Insert = "usp_CityObjects_Insert";
            public const string Update = "usp_CityObjects_Update";
            public const string Delete = "usp_CityObjects_Delete";
            public const string Search = "usp_CityObjects_Search";
            public const string List = "usp_CityObjects_List";
        }
        
        public const int ObjectTypeID = 16;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@CityObjectID", SqlDbType.Int)]
        //[FieldAttribute("CityObjectID")]
        public int ID { get; set; }

        //[ParamAttribute("@CityObjectTypeID", SqlDbType.SmallInt)]
        //[FieldAttribute("CityObjectTypeID")]
        public short TypeID { get; set; }

        //[ParamAttribute("@CityObjectName", SqlDbType.NVarChar, 100)]
        //[FieldAttribute("CityObjectName")]
        public string Name { get; set; }

        //[ParamAttribute("@CityObjectOldName", SqlDbType.NVarChar, 100)]
        //[FieldAttribute("CityObjectOldName")]
        public string OldName { get; set; }

        //[ParamAttribute("@CityObjectSearchName", SqlDbType.NVarChar, 100)]
        //[FieldAttribute("CityObjectSearchName")]
        public string SearchName { get; set; }

        //[ParamAttribute("@IsReal", SqlDbType.Bit)]
        //[FieldAttribute("IsReal")]
        public bool IsReal { get; set; }

        public string TypeName { get; set; }

        public string TypeShortName { get; set; }

        
        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public CityObject(string userName)
        {
            UserName = userName;
        }

        public CityObject(int id, string userName): this(userName)
        {
            Init(null, id);
        }

        public CityObject(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int cityObjectId)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[6];
            prms[0] = new SqlParameter("@CityObjectID", SqlDbType.Int);
            prms[0].Value = cityObjectId;

            prms[1] = new SqlParameter("@CityObjectTypeID", SqlDbType.SmallInt);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@CityObjectName", SqlDbType.NVarChar, 100);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@CityObjectOldName", SqlDbType.NVarChar, 100);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@CityObjectSearchName", SqlDbType.NVarChar, 100);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@IsReal", SqlDbType.Bit);
            prms[5].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = cityObjectId;
            TypeID = (short)prms[1].Value;
            Name = (string)prms[2].Value;
            if (prms[3].Value != DBNull.Value) {
                OldName = (string)prms[3].Value;
            }
            SearchName = (string)prms[4].Value;
            IsReal = (bool)prms[5].Value;
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[6];
            prms[0] = new SqlParameter("@CityObjectID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@CityObjectTypeID", SqlDbType.SmallInt);
            prms[1].Value = TypeID;

            prms[2] = new SqlParameter("@CityObjectName", SqlDbType.NVarChar, 100);
            prms[2].Value = Name;

            prms[3] = new SqlParameter("@CityObjectOldName", SqlDbType.NVarChar, 100);
            prms[3].Value = OldName;

            prms[4] = new SqlParameter("@CityObjectSearchName", SqlDbType.NVarChar, 100);
            prms[4].Value = SearchName;

            prms[5] = new SqlParameter("@IsReal", SqlDbType.Bit);
            prms[5].Value = IsReal;

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

            SqlParameter[] prms = new SqlParameter[6];
            prms[0] = new SqlParameter("@CityObjectID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@CityObjectTypeID", SqlDbType.SmallInt);
            prms[1].Value = TypeID;

            prms[2] = new SqlParameter("@CityObjectName", SqlDbType.NVarChar, 100);
            prms[2].Value = Name;

            prms[3] = new SqlParameter("@CityObjectOldName", SqlDbType.NVarChar, 100);
            prms[3].Value = OldName;

            prms[4] = new SqlParameter("@CityObjectSearchName", SqlDbType.NVarChar, 100);
            prms[4].Value = SearchName;

            prms[5] = new SqlParameter("@IsReal", SqlDbType.Bit);
            prms[5].Value = IsReal;

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
        
        public static DataSet Search(SqlTransaction trans, string cityObjectName)
        {
            SqlParameter[] sps = new SqlParameter[1];
            sps[0] = new SqlParameter("@CityObjectName", SqlDbType.NVarChar, 100);
            sps[0].Value = cityObjectName;

            return SPHelper.ExecuteDataset(trans, SpNames.Search, sps);
        }

        public static DataSet Search(string cityObjectName)
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

                    ds = Search(trans, cityObjectName);

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