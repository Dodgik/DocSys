using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using IO.VFS.Data;

namespace IO.VFS
{
    public enum PermissionType { None = 0, FullAccess = 1, Write = 2, ReadOnly = 3 }

    public class PermissionTypeVFS
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        public PermissionTypeVFS(string connectionString)
        {
            this.connectionString = connectionString;
            this.name = String.Empty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="Id">Permission Type ID</param>
        /// <param name="name">Permission Type Name</param>
        public PermissionTypeVFS(string connectionString, int id, string name)
        {
            this.connectionString = connectionString;
            this.id = id;
            this.name = name;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="Id">Permission Type ID</param>        
        public PermissionTypeVFS(string connectionString, int id)
        {
            this.connectionString = connectionString;
            this.id = id;
            this.LoadByID();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="name">Permission Type Name</param>
        public PermissionTypeVFS(string connectionString, string name)
        {
            this.connectionString = connectionString;
            this.name = name;
            this.LoadByName();
        }
        #endregion
        
        #region Fields and Properties
        private string connectionString;
        /// <summary>
        /// Connection String for connection to Data Base.
        /// </summary>
        private string ConnectionString
        {
            get { return connectionString; }
        }

        private int id;
        /// <summary>
        /// Permission Type Primary Key
        /// </summary>
        public int ID
		{
            get { return id; }            
		}

        
        /// <summary>
        /// Permission Type 
        /// </summary>
        public PermissionType PermissionType
        {
            get { return (PermissionType)id; }
        }
        
        private string name;
        /// <summary>
        /// Permission Name
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }                
        #endregion

        #region Private Methods
        /// <summary>
        ///  Load Information By ID
        /// </summary>
        private void LoadByID()
        {                        
            SqlParameter[] Params = new SqlParameter[2];
            Params[0] = new SqlParameter("@PermissionTypeID", SqlDbType.Int);
            Params[0].Value = this.id;
            Params[1] = new SqlParameter("@PermissionTypeName", SqlDbType.NVarChar, 127);
            Params[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetPermissionTypeByID", Params);
            if (Params[1].Value.Equals(DBNull.Value)) throw new ExceptionVFS(String.Format("ID: {0}. Permission Type not found", this.ID.ToString()));
            this.name = Params[1].Value.ToString();
        }

        /// <summary>
        ///  Load Information By Name
        /// </summary>
        private void LoadByName()
        {
            SqlParameter[] Params = new SqlParameter[2];
            Params[0] = new SqlParameter("@PermissionTypeName", SqlDbType.NVarChar, 127);            
            Params[0].Value = this.name;
            Params[1] = new SqlParameter("@PermissionTypeID", SqlDbType.Int);
            Params[1].Direction = ParameterDirection.Output;            

            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetPermissionTypeByName", Params);
            if (Params[1].Value.Equals(DBNull.Value)) throw new ExceptionVFS(String.Format("Permission Type: {0}. Permission Type not found", this.Name));
            this.id = (int)Params[1].Value;
        }
        #endregion

        #region Public Method
        public override bool Equals(object obj)
        {
            if (obj is PermissionTypeVFS)
            {
                return this.Name.ToLower() == ((PermissionTypeVFS)obj).Name.ToLower();
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
        #endregion               

        #region Public Static Method
        /// <summary>
        /// Get Permission Type List
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <returns></returns>
        public static List<PermissionTypeVFS> GetPermissionTypeList(string connectionString)
        {
            SqlDataReader reader = GetPermissionTypes(connectionString);
            PermissionTypeVFS permType;
            List<PermissionTypeVFS> list = new List<PermissionTypeVFS>();
            try
            {
                while (reader.Read())
                {
                    permType = new PermissionTypeVFS(connectionString, (int)reader["PermissionTypeID"], reader["PermissionTypeName"].ToString());
                    list.Add(permType);
                }
            }
            finally
            {
                if (!reader.IsClosed) reader.Close();
            }
            return list;
        }

        /// <summary>
        /// Get Permission Types
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <returns></returns>
        public static SqlDataReader GetPermissionTypes(string connectionString)
        {
            return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, "usp_vfs_GetPermissionTypes");
        }

        /// <summary>
        /// Is Permission Type Exists
        /// </summary>
        /// <param name="permissionTypeName">Permission Type Name</param>
        /// <returns>If Permission Type exists return True. Else Return False</returns>
        public static bool Exists(string connectionString, string permissionTypeName)
        {                                
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@PermissionTypeName", SqlDbType.NVarChar, 127);
            mParams[0].Value = permissionTypeName;
            mParams[1] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_PermissionTypeExists", mParams);
            return ((int)mParams[1].Value) == 1;
        }
        #endregion
    }
}