using System;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership;

namespace BizObj.Document
{
    public class CityObjects
    {
        
        private class CityObjectsParamsHelper : SqlParamsHelper
        {
            static CityObjectsParamsHelper()
            {
                InitBaseDictionary(typeof(CityObjectsParamsHelper));
                AddLinkedAttributes(typeof(CityObjects), typeof(CityObjectsParamsHelper));
            }

            public CityObjectsParamsHelper(object propOwner): base(propOwner)
            {
                ParamList.GetAll();
            }
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

        [ParamAttribute("@CityObjectID", SqlDbType.Int)]
        [FieldAttribute("CityObjectID")]
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

        [ParamAttribute("@CityObjectTypeID", SqlDbType.Int)]
        [FieldAttribute("CityObjectTypeID")]
        private int _cityObjectTypeID;
        public int CityObjectTypeID
        {
            get
            {
                return _cityObjectTypeID;
            }
            set
            {
                _cityObjectTypeID = value;
            }
        }

        [ParamAttribute("@CityObjectName", SqlDbType.NVarChar, 100)]
        [FieldAttribute("CityObjectName")]
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

        [ParamAttribute("@CityObjectOldName", SqlDbType.NVarChar, 100)]
        [FieldAttribute("CityObjectOldName")]
        private string _oldName;
        public string OldName
        {
            get
            {
                return _oldName;
            }
            set
            {
                _oldName = value;
            }
        }

        [ParamAttribute("@CityObjectSearchName", SqlDbType.NVarChar, 100)]
        [FieldAttribute("CityObjectSearchName")]
        private string _searchName;
        public string SearchName
        {
            get
            {
                return _searchName;
            }
            set
            {
                _searchName = value;
            }
        }

        [ParamAttribute("@IsReal", SqlDbType.Bit)]
        [FieldAttribute("IsReal")]
        private bool _isReal;
        public bool IsReal
        {
            get
            {
                return _isReal;
            }
            set
            {
                _isReal = value;
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

        public CityObjects(string userName)
        {
            this.UserName = userName;
            this._accessObject = new AccessObject(ConstantCode.CONNECTION_STRING);
        }

        public CityObjects(int id, string userName): this(userName)
        {
            this.Init(null, id);
        }

        public CityObjects(SqlTransaction trans, int id, string userName): this(userName)
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
            string spName = "usp_CityObjects_Get";
            CityObjectsParamsHelper helper = new CityObjectsParamsHelper(this);
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

            string spName = "usp_CityObjects_Insert";
            CityObjectsParamsHelper helper = new CityObjectsParamsHelper(this);
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

            string spName = "usp_CityObjects_Update";
            CityObjectsParamsHelper helper = new CityObjectsParamsHelper(this);
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
        /*
        public static DataTable GetAll(SqlConnection conectionString)
        {
            return SPHelper.ExecuteDataset(conectionString, "usp_CityObjects_List").Tables[0];
        }
        public static DataTable GetAll()
        {
            return SPHelper.ExecuteDataset("usp_CityObjects_List").Tables[0];
        }
        */
        #endregion
        
        #region Static Public Methods
        
        public static void Delete(SqlTransaction trans, int id)
        {
            SPHelper.ExecuteNonQuery(trans, "usp_CityObjects_Delete", id);
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
