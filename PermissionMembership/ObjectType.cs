using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership.Data;

namespace PermissionMembership
{
    public class ObjectType
    {
        #region Private Fields

        private int id;
        private string name;
        private string connectionString;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get or set object type id
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Get or set object type name
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
        public ObjectType(string ConnectionString)
        {
            name = String.Empty;
            connectionString = ConnectionString;
        }

        /// <summary>
        /// Create object by id
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="id">Object type id</param>
        public ObjectType(string ConnectionString, int id): this(ConnectionString)
        {            
            this.id = id;
            Init();
        }

        #endregion        

        #region Private Methods

        private void Init()
        {
            string spname = "usp_Access_ObjectTypeGet";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@ObjectTypeName", SqlDbType.NVarChar, 100);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (mParams[1].Value == DBNull.Value)
            {
                ThrowException(String.Format("Object Type by id {0} not found.", id.ToString()));
            }
            else
            {
                name = (string)mParams[1].Value;
            }
        }

        private static void ThrowException(string errorMessage)
        {
            throw new PermissionMembershipException(errorMessage);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create new object type
        /// </summary>
        public void Insert()
        {            
            string spname = "usp_Access_ObjectTypeInsert";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@ObjectTypeName", SqlDbType.NVarChar, 100);
            mParams[1].Value = name;            
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Update existing object type
        /// </summary>
        public void Update()
        {
            string spname = "usp_Access_ObjectTypeUpdate";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@ObjectTypeName", SqlDbType.NVarChar, 100);
            mParams[1].Value = name;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Delete existing object type
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="id">Object type id</param>
        public static void Delete(string ConnectionString, int Id)
        {
            string spname = "usp_Access_ObjectTypeDelete";
            SqlParameter[] mParams = new SqlParameter[1];
            mParams[0] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[0].Value = Id;            
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Get object type list
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <returns></returns>
        public static DataView List(string ConnectionString)
        {            
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "usp_Access_ObjectTypeList").Tables[0].DefaultView;
        }

        #endregion
    }
}
