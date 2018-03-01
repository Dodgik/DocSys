using System;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership;

namespace BizObj.Document
{
    public class ExternalNumber
    {
        
        private class ExternalNumberParamsHelper : SqlParamsHelper
        {
            static ExternalNumberParamsHelper()
            {
                InitBaseDictionary(typeof(ExternalNumberParamsHelper));
                AddLinkedAttributes(typeof(ExternalNumber), typeof(ExternalNumberParamsHelper));
            }

            public ExternalNumberParamsHelper(object propOwner): base(propOwner)
            {
                ParamList.GetAll();
            }
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

        [ParamAttribute("@DocumentID", SqlDbType.Int)]
        [FieldAttribute("DocumentID")]
        private int _id;
        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [ParamAttribute("@CreationDate", SqlDbType.DateTime)]
        [FieldAttribute("CreationDate")]
        private DateTime _creationDate;
        public DateTime CreationDate
        {
            get
            {
                return _creationDate;
            }
            set
            {
                _creationDate = value;
            }
        }

        [ParamAttribute("@ExternalNumber", SqlDbType.NVarChar, 50)]
        [FieldAttribute("ExternalNumber")]
        private string _number;
        public string Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }
        
        private string _userName;
        private string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        #endregion
        
        #region Constructors

        public ExternalNumber(string userName)
        {
            this.UserName = userName;
        }

        public ExternalNumber(int id, string userName): this(userName)
        {
            this.Init(null, id);
        }

        public ExternalNumber(SqlTransaction trans, int id, string userName): this(userName)
        {
            this.Init(trans, id);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int id)
        {
            if (!CanView(this.UserName))
            {
                throw new AccessException(this.UserName, "Init");
            }
            this.ID = id;
            string spName = "usp_ExternalNumber_Get";
            ExternalNumberParamsHelper helper = new ExternalNumberParamsHelper(this);
            helper.InitParamsForSP(spName);
            SqlParameter[] parameters = helper.ParamList.Params;
            if (trans == null)
            {
                SPHelper.ExecuteNonQuery(spName, parameters);
            }
            else
            {
                SPHelper.ExecuteNonQuery(trans, spName, parameters);
            }
            helper.SetPropValues();
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(this.UserName))
            {
                throw new AccessException(this.UserName, "Insert");
            }

            string spName = "usp_ExternalNumber_Insert";
            ExternalNumberParamsHelper helper = new ExternalNumberParamsHelper(this);
            helper.InitParamsForSP(spName);
            SqlParameter[] parameters = helper.ParamList.Params;
            SPHelper.ExecuteNonQuery(trans, spName, parameters);
            helper.SetPropValues();
            
            return this._id;
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
                    this.Insert(trans);

                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw (e);
                }
            }
            finally
            {
                connection.Close();
            }
            return this.ID;

        }

        public void Update(SqlTransaction trans)
        {
            if (!CanUpdate(this.UserName))
            {
                throw new AccessException(this.UserName, "Update");
            }

            string spName = "usp_ExternalNumber_Update";
            ExternalNumberParamsHelper helper = new ExternalNumberParamsHelper(this);
            helper.InitParamsForSP(spName);
            SqlParameter[] parameters = helper.ParamList.Params;
            SPHelper.ExecuteNonQuery(trans, spName, parameters);
            helper.SetPropValues();
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
                    this.Update(trans);

                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw (e);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public static DataTable GetAll(SqlConnection conectionString)
        {
            return SPHelper.ExecuteDataset(conectionString, "usp_ExternalNumber_List").Tables[0];
        }
        public static DataTable GetAll()
        {
            return SPHelper.ExecuteDataset("usp_ExternalNumber_List").Tables[0];
        }
        #endregion
        
        #region Static Public Methods
        
        public static void Delete(SqlTransaction trans, int id)
        {
            SPHelper.ExecuteNonQuery(trans, "usp_ExternalNumber_Delete", id);
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
                catch (Exception e)
                {
                    trans.Rollback();
                    throw (e);
                }
            }
            finally
            {
                connection.Close();
            }
        }
            
        public static bool CanInsert(string userName)
        {
            return Permission.IsUserPermission(ConstantCode.CONNECTION_STRING, userName, ObjectTypeID, StateIDAll, (int) ActionType.Insert);
        }

        public static bool CanUpdate(string userName)
        {
            return Permission.IsUserPermission(ConstantCode.CONNECTION_STRING, userName, ObjectTypeID, StateIDAll, (int) ActionType.Update);
        }

        public static bool CanDelete(string userName)
        {
            return Permission.IsUserPermission(ConstantCode.CONNECTION_STRING, userName, ObjectTypeID, StateIDAll, (int) ActionType.Delete);
        }

        public static bool CanView(string userName)
        {
            return Permission.IsUserPermission(ConstantCode.CONNECTION_STRING, userName, ObjectTypeID, StateIDAll, (int) ActionType.View);
        }
        #endregion
        
    }
}
