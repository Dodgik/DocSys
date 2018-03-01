using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership.Data;

namespace PermissionMembership
{
    public class ActionType
    {
        #region Private Fields

        private int id;
        private int objectTypeId;
        private ObjectType objectType;
        private string name;
        private string connectionString;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get or set action type id
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Get or set object type id
        /// </summary>
        public int ObjectTypeId
        {
            get { return objectTypeId; }
            set
            {
                objectTypeId = value;
                objectType = new ObjectType(connectionString, objectTypeId);
            }
        }

        /// <summary>
        /// Get object type
        /// </summary>
        public ObjectType ObjectType
        {
            get { return objectType; }         
        }

        /// <summary>
        /// Get or set action type name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create not initialize object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        public ActionType(string ConnectionString)
        {
            name = String.Empty;
            objectType = new ObjectType(ConnectionString);
            connectionString = ConnectionString;
        }

        /// <summary>
        /// Create object by id
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="id">Action type id</param>
        public ActionType(string ConnectionString, int id): this(ConnectionString)
        {            
            this.id = id;
            Init();
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            string spname = "usp_Access_ActionTypeGet";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            mParams[2] = new SqlParameter("@ActionName", SqlDbType.NVarChar, 100);
            mParams[2].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (mParams[2].Value == DBNull.Value)
            {
                ThrowException(String.Format("Action Type by id {0} not found.", id.ToString()));
            }
            else
            {
                ObjectTypeId = (int)mParams[1].Value;
                name = (string)mParams[2].Value;
            }
        }

        private static void ThrowException(string errorMessage)
        {
            throw new PermissionMembershipException(errorMessage);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create new action type
        /// </summary>
        public void Insert()
        {
            string spname = "usp_Access_ActionTypeInsert";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = objectTypeId;
            mParams[2] = new SqlParameter("@ActionName", SqlDbType.NVarChar, 100);
            mParams[2].Value = name;            
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Update existing action type
        /// </summary>
        public void Update()
        {
            string spname = "usp_Access_ActionTypeUpdate";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = objectTypeId;
            mParams[2] = new SqlParameter("@ActionName", SqlDbType.NVarChar, 100);
            mParams[2].Value = name;  
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Delete existing action type
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="id">Action type id</param>
        public static void Delete(string ConnectionString, int Id)
        {
            string spname = "usp_Access_ActionTypeDelete";
            SqlParameter[] mParams = new SqlParameter[1];
            mParams[0] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[0].Value = Id;            
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Get action type list
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <returns></returns>
        public static DataView List(string ConnectionString, int ObjectTypeId)
        {
            string spname = "usp_Access_ActionTypeList";
            SqlParameter[] mParams = new SqlParameter[1];
            mParams[0] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[0].Value = ObjectTypeId;
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, spname, mParams).Tables[0].DefaultView;
        }

        #endregion
    }
}
