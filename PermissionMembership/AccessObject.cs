using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership.Data;

namespace PermissionMembership
{
    public class AccessObject
    {
        #region Private Fields

        private Guid id;
        private int objectStateId;
        private ObjectState objectState;
        private int objectTypeId;
        private ObjectType objectType;
        private string name;
        private string connectionString;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get or set object id
        /// </summary>
        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Get or set object state id
        /// </summary>
        public int ObjectStateId
        {
            get { return objectStateId; }
            set
            {
                objectStateId = value;
                objectState = new ObjectState(connectionString, objectStateId);
            }
        }

        /// <summary>
        /// Get object state
        /// </summary>
        public ObjectState ObjectState
        {
            get { return objectState; }         
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
        /// Get or set object name
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
        public AccessObject(string ConnectionString)
        {
            name = String.Empty;
            objectState = new ObjectState(ConnectionString);
            objectType = new ObjectType(ConnectionString);
            connectionString = ConnectionString;
        }

        /// <summary>
        /// Create object by id
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="id">Object id</param>
        public AccessObject(string ConnectionString, Guid id)
            : this(ConnectionString)
        {            
            this.id = id;
            Init(null);
        }

        /// <summary>
        /// Create object by id
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="id">Object id</param>                
        public AccessObject(SqlTransaction trans, Guid id)
            : this(trans.Connection.ConnectionString)
        {
            this.id = id;
            Init(trans);
        }

        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans)
        {
            bool existsObject;
            if (trans == null)
            {
                existsObject = Exists(connectionString, id);
            }
            else
            {
                existsObject = Exists(trans, id);
            }
            if (!existsObject)
            {
                ThrowException(String.Format("Object \"{0}\" not found.", id.ToString()));
            }
            string spname = "usp_Access_ObjectGet";
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            mParams[2] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;
            mParams[3] = new SqlParameter("@ObjectName", SqlDbType.NVarChar, 100);
            mParams[3].Direction = ParameterDirection.Output;
            if (trans == null)
            {
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
            }
            if (mParams[2].Value == DBNull.Value)
            {
                ThrowException(String.Format("Object by id {0} not found.", id.ToString()));
            }
            else
            {
                ObjectStateId = (int)mParams[1].Value;
                ObjectTypeId = (int)mParams[2].Value;
                name = (string)mParams[3].Value;
            }
        }

        private void Validate()
        {
            if (!ObjectState.Exists(connectionString, objectStateId, objectTypeId))
            {
                throw new PermissionMembershipException(String.Format("Object state (id:{0}) for object type (id: {1}) not found.", objectStateId.ToString(), objectTypeId.ToString()));
            }
        }

        private static void ThrowException(string errorMessage)
        {
            throw new PermissionMembershipException(errorMessage);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create new object
        /// </summary>
        /// <param name="trans">Transaction object</param>
        public Guid Insert(SqlTransaction trans)
        {
            Validate();
            id = Guid.NewGuid();
            string spname = "usp_Access_ObjectInsert";
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[1].Value = objectStateId;
            mParams[2] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[2].Value = objectTypeId;
            mParams[3] = new SqlParameter("@ObjectName", SqlDbType.NVarChar, 100);
            mParams[3].Value = name;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
            return id;
        }

        /// <summary>
        /// Create new object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        public Guid Insert()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    Guid res = Insert(trans);
                    trans.Commit();
                    return res;
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw (e);
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Update existing object 
        /// </summary>
        /// <param name="trans">Transaction object</param>
        public void Update(SqlTransaction trans)
        {
            Validate();
            string spname = "usp_Access_ObjectUpdate";
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[1].Value = objectStateId;
            mParams[2] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[2].Value = objectTypeId;
            mParams[3] = new SqlParameter("@ObjectName", SqlDbType.NVarChar, 100);
            mParams[3].Value = name;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Update existing object 
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        public void Update()
        {
            SqlConnection connection = new SqlConnection(connectionString);
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
                catch (Exception e)
                {
                    trans.Rollback();
                    throw (e);
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Delete existing object 
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="id">Object id</param>
        public static void Delete(SqlTransaction trans, Guid Id)
        {
            string spname = "usp_Access_ObjectDelete";
            SqlParameter[] mParams = new SqlParameter[1];
            mParams[0] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = Id;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Delete existing object 
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="id">Object id</param>
        public static void Delete(string ConnectionString, Guid Id)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    Delete(trans, Id);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Exists Object
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="id">Object id</param>
        public static bool Exists(SqlTransaction trans, Guid Id)
        {
            string spname = "usp_Access_ObjectExists";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = Id;
            mParams[1] = new SqlParameter("@Exists", SqlDbType.Bit);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
            return (bool)mParams[1].Value;
        }

        /// <summary>
        /// Exists Object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="id">Object id</param>
        public static bool Exists(string ConnectionString, Guid Id)
        {
            string spname = "usp_Access_ObjectExists";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = Id;
            mParams[1] = new SqlParameter("@Exists", SqlDbType.Bit);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, spname, mParams);
            return (bool)mParams[1].Value;
        }

        #endregion
    }
}
