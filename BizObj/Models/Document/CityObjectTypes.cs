using System;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership;

namespace BizObj.Document
{
    public class CityObjectTypes
    {
        
        private class CityObjectTypesParamsHelper : SqlParamsHelper
        {
            static CityObjectTypesParamsHelper()
            {
                InitBaseDictionary(typeof(CityObjectTypesParamsHelper));
                AddLinkedAttributes(typeof(CityObjectTypes), typeof(CityObjectTypesParamsHelper));
            }

            public CityObjectTypesParamsHelper(object propOwner): base(propOwner)
            {
                ParamList.GetAll();
            }
        }

        public const int ObjectTypeID = 15;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        [ParamAttribute("@CityObjectTypeID", SqlDbType.Int)]
        [FieldAttribute("CityObjectTypeID")]
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

        [ParamAttribute("@CityObjectTypeName", SqlDbType.NVarChar, 25)]
        [FieldAttribute("CityObjectTypeName")]
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        [ParamAttribute("@CityObjectTypeShortName", SqlDbType.NVarChar, 25)]
        [FieldAttribute("CityObjectTypeShortName")]
        private string _shortName;
        public string ShortName
        {
            get
            {
                return _shortName;
            }
            set
            {
                _shortName = value;
            }
        }

        [ParamAttribute("@ObjectID", SqlDbType.UniqueIdentifier)]
        [FieldAttribute("ObjectID")]
        private Guid _objectID;
        private AccessObject _accessObject;
        private AccessObject Access_Object
        {
            get
            {
                return _accessObject;
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

        public CityObjectTypes(string userName)
        {
            this.UserName = userName;
            this._accessObject = new AccessObject(ConstantCode.CONNECTION_STRING);
        }

        public CityObjectTypes(int id, string userName): this(userName)
        {
            this.Init(null, id);
        }

        public CityObjectTypes(SqlTransaction trans, int id, string userName): this(userName)
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
            string spName = "usp_CityObjectTypes_Get";
            CityObjectTypesParamsHelper helper = new CityObjectTypesParamsHelper(this);
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
            this._accessObject = new AccessObject(ConstantCode.CONNECTION_STRING, this._objectID);
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(this.UserName))
            {
                throw new AccessException(this.UserName, "Insert");
            }

            AccessObject aObject = new AccessObject(ConstantCode.CONNECTION_STRING);
            aObject.Id = Guid.NewGuid();
            aObject.Name = this._name;
            aObject.ObjectTypeId = ObjectTypeID;
            aObject.ObjectStateId = StateIDAll;
            this._objectID = aObject.Insert(trans);

            string spName = "usp_CityObjectTypes_Insert";
            CityObjectTypesParamsHelper helper = new CityObjectTypesParamsHelper(this);
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

            AccessObject aObject = new AccessObject(trans, this._objectID);
            aObject.Name = this._name;
            aObject.Update(trans);

            string spName = "usp_CityObjectTypes_Update";
            CityObjectTypesParamsHelper helper = new CityObjectTypesParamsHelper(this);
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
            return SPHelper.ExecuteDataset(conectionString, "usp_CityObjectTypes_List").Tables[0];
        }
        public static DataTable GetAll()
        {
            return SPHelper.ExecuteDataset("usp_CityObjectTypes_List").Tables[0];
        }
        #endregion
        
        #region Static Public Methods
        
        public static void Delete(SqlTransaction trans, int id)
        {
            SPHelper.ExecuteNonQuery(trans, "usp_CityObjectTypes_Delete", id);
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
