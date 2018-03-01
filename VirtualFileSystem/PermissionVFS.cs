using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using IO.VFS.Data;

namespace IO.VFS
{
    public class PermissionVFS
    {                                            
        #region Add permission To File
        /// <summary>
        /// Add Permissions To File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileID">File ID</param>
        /// <param name="permissionType">Permission Type</param>
        public static void AddToFile(SqlTransaction trans, int userID, int fileID, PermissionType permissionType)
        {
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;
            mParams[2] = new SqlParameter("@PermissionTypeID", SqlDbType.Int);
            mParams[2].Value = (int)permissionType;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_AddPermissionToFile", mParams);
        }

        /// <summary>
        /// Add Permissions To File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileID">File ID</param>
        /// <param name="permissionType">Permission Type</param>
        public static void AddToFile(string connectionString, int userID, int fileID, PermissionType permissionType)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    AddToFile(trans, userID, fileID, permissionType);
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
        #endregion

        #region Delete Permission From File
        /// <summary>
        /// Delete Permissions To File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileID">File ID</param>        
        public static void DeleteToFile(SqlTransaction trans, int userID, int fileID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;            

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_DeletePermissionToFile", mParams);
        }

        /// <summary>
        /// Delete Permissions To File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileID">File ID</param>        
        public static void DeleteToFile(string connectionString, int userID, int fileID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    DeleteToFile(trans, userID, fileID);
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
        #endregion

        #region Add Permission To Directory
        /// <summary>
        /// Add Permissions To Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <param name="permissionType">Permission Type</param>
        public static void AddToDirectory(SqlTransaction trans, int userID, int directoryID, PermissionType permissionType)
        {
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = directoryID;
            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;
            mParams[2] = new SqlParameter("@PermissionTypeID", SqlDbType.Int);
            mParams[2].Value = (int)permissionType;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_AddPermissionToDirectory", mParams);
        }

        /// <summary>
        /// Add Permissions To Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <param name="permissionType">Permission Type</param>
        public static void AddToDirectory(string connectionString, int userID, int directoryID, PermissionType permissionType)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    AddToDirectory(trans, userID, directoryID, permissionType);
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
        #endregion

        #region Delete Permission From Directory
        /// <summary>
        /// Delete Permissions To Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>        
        public static void DeleteToDirectory(SqlTransaction trans, int userID, int directoryID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = directoryID;
            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;            

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_DeletePermissionToDirectory", mParams);
        }

        /// <summary>
        /// Delete Permissions To Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>        
        public static void DeleteToDirectory(string connectionString, int userID, int directoryID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    DeleteToDirectory(trans, userID, directoryID);
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
        #endregion

        #region Get Shared User List
        /// <summary>
        /// Get Shared User list 
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedUsers(SqlTransaction trans, int userID)
        {
            return SqlHelper.ExecuteReader(trans, "usp_vfs_GetPermissionUsers", userID);
        }

        /// <summary>
        /// Get Shared User list 
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedUsers(string connectionString, int userID)
        {
            return SqlHelper.ExecuteReader(connectionString, "usp_vfs_GetPermissionUsers", userID);
        }
        #endregion

        #region Get Shared Files
        /// <summary>
        /// Get Shared Files 
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedFiles(SqlTransaction trans, int userID)
        {
            return GetSharedFiles(trans, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Shared Files 
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userAcceptorID">User Acceptor ID</param>
        /// <param name="userDonorID">User Donor ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedFiles(SqlTransaction trans, int userAcceptorID, int userDonorID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserAcceptorID", SqlDbType.Int);
            mParams[0].Value = userAcceptorID;
            mParams[1] = new SqlParameter("@UserDonorID", SqlDbType.Int);
            if (userDonorID == Int32.MinValue)
            {
                mParams[1].Value = DBNull.Value;
            }
            else
            {
                mParams[1].Value = userDonorID;
            }
            return SqlHelper.ExecuteReader(trans, CommandType.StoredProcedure, "usp_vfs_GetFilePermission", mParams);
        }

        /// <summary>
        /// Get Shared Files 
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedFiles(string connectionString, int userID)
        {
            return GetSharedFiles(connectionString, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Shared Files 
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userAcceptorID">User Acceptor ID</param>
        /// <param name="userDonorID">User Donor ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedFiles(string connectionString, int userAcceptorID, int userDonorID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserAcceptorID", SqlDbType.Int);
            mParams[0].Value = userAcceptorID;
            mParams[1] = new SqlParameter("@UserDonorID", SqlDbType.Int);
            if (userDonorID == Int32.MinValue)
            {
                mParams[1].Value = DBNull.Value;
            }
            else
            {
                mParams[1].Value = userDonorID;
            }
            return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, "usp_vfs_GetFilePermission", mParams);
        }
        #endregion

        #region Get Shared File List
        /// <summary>
        /// Get Shared Files 
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static List<FileVFS> GetSharedFileList(SqlTransaction trans, int userID)
        {
            return GetSharedFileList(trans, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Shared Files 
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userAcceptorID">User Acceptor ID</param>
        /// <param name="userDonorID">User Donor ID</param>
        /// <returns></returns>
        public static List<FileVFS> GetSharedFileList(SqlTransaction trans, int userAcceptorID, int userDonorID)
        {
            List<FileVFS> list = new List<FileVFS>();
            SqlDataReader reader = GetSharedFiles(trans, userAcceptorID, userDonorID);
            try
            {
                while (reader.Read())
                {
                    FileVFS file = FileVFS.GetFileFromReader(trans.Connection.ConnectionString, reader);
                    list.Add(file);
                }
            }
            finally
            {
                if (!reader.IsClosed) reader.Close();
            }
            return list;
        }

        /// <summary>
        /// Get Shared Files 
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static List<FileVFS> GetSharedFileList(string connectionString, int userID)
        {
            return GetSharedFileList(connectionString, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Shared Files 
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userAcceptorID">User Acceptor ID</param>
        /// <param name="userDonorID">User Donor ID</param>
        /// <returns></returns>
        public static List<FileVFS> GetSharedFileList(string connectionString, int userAcceptorID, int userDonorID)
        {
            List<FileVFS> list = new List<FileVFS>();
            SqlDataReader reader = GetSharedFiles(connectionString, userAcceptorID, userDonorID);
            try
            {
                while (reader.Read())
                {
                    FileVFS file = FileVFS.GetFileFromReader(connectionString, reader);
                    list.Add(file);
                }
            }
            finally
            {
                if (!reader.IsClosed) reader.Close();
            }
            return list;
        }
        #endregion

        #region Get Shared Directories
        /// <summary>
        /// Get Shared Directories
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedDirectories(SqlTransaction trans, int userID)
        {
            return GetSharedDirectories(trans, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Shared Directories 
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userAcceptorID">User Acceptor ID</param>
        /// <param name="userDonorID">User Donor ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedDirectories(SqlTransaction trans, int userAcceptorID, int userDonorID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserAcceptorID", SqlDbType.Int);
            mParams[0].Value = userAcceptorID;
            mParams[1] = new SqlParameter("@UserDonorID", SqlDbType.Int);
            if (userDonorID == Int32.MinValue)
            {
                mParams[1].Value = DBNull.Value;
            }
            else
            {
                mParams[1].Value = userDonorID;
            }
            return SqlHelper.ExecuteReader(trans, CommandType.StoredProcedure, "usp_vfs_GetDirectoryPermission", mParams);
        }

        /// <summary>
        /// Get Shared Directories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedDirectories(string connectionString, int userID)
        {
            return GetSharedDirectories(connectionString, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Shared Directories 
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userAcceptorID">User Acceptor ID</param>
        /// <param name="userDonorID">User Donor ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSharedDirectories(string connectionString, int userAcceptorID, int userDonorID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserAcceptorID", SqlDbType.Int);
            mParams[0].Value = userAcceptorID;
            mParams[1] = new SqlParameter("@UserDonorID", SqlDbType.Int);
            if (userDonorID == Int32.MinValue)
            {
                mParams[1].Value = DBNull.Value;
            }
            else
            {
                mParams[1].Value = userDonorID;
            }
            return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryPermission", mParams);
        }
        #endregion

        #region Get Shared Directory List
        /// <summary>
        /// Get Shared Directories
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static List<DirectoryVFS> GetSharedDirectoryList(SqlTransaction trans, int userID)
        {
            return GetSharedDirectoryList(trans, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Shared Directories 
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userAcceptorID">User Acceptor ID</param>
        /// <param name="userDonorID">User Donor ID</param>
        /// <returns></returns>
        public static List<DirectoryVFS> GetSharedDirectoryList(SqlTransaction trans, int userAcceptorID, int userDonorID)
        {
            return DirectoryVFS.LoadFromReader(trans.Connection.ConnectionString, GetSharedDirectories(trans, userAcceptorID, userDonorID));
        }

        /// <summary>
        /// Get Shared Directories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static List<DirectoryVFS> GetSharedDirectoryList(string connectionString, int userID)
        {
            return GetSharedDirectoryList(connectionString, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Shared Directories 
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userAcceptorID">User Acceptor ID</param>
        /// <param name="userDonorID">User Donor ID</param>
        /// <returns></returns>
        public static List<DirectoryVFS> GetSharedDirectoryList(string connectionString, int userAcceptorID, int userDonorID)
        {
            return DirectoryVFS.LoadFromReader(connectionString, GetSharedDirectories(connectionString, userAcceptorID, userDonorID));
        }
        #endregion

        #region Get File Permission
        /// <summary>
        /// Get File Permission
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileID">File ID</param>
        /// <returns></returns>
        public static PermissionType GetFilePermission(SqlTransaction trans, int userID, int fileID)
        {
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;
            mParams[2] = new SqlParameter("@PermissionTypeID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetFilePermissionType", mParams);

            return (PermissionType)mParams[2].Value;
        }

        /// <summary>
        /// Get File Permission
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileID">File ID</param>
        /// <returns></returns>
        public static PermissionType GetFilePermission(string connectionString, int userID, int fileID)
        {
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;
            mParams[2] = new SqlParameter("@PermissionTypeID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetFilePermissionType", mParams);

            return (PermissionType)mParams[2].Value;
        }
        #endregion

        #region Get Directory Permission
        /// <summary>
        /// Get Directory Permission
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static PermissionType GetDirectoryPermission(SqlTransaction trans, int userID, int directoryID)
        {
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = directoryID;
            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;
            mParams[2] = new SqlParameter("@PermissionTypeID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetDirectoryPermissionType", mParams);

            return (PermissionType)mParams[2].Value;
        }

        /// <summary>
        /// Get Directory Permission
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static PermissionType GetDirectoryPermission(string connectionString, int userID, int directoryID)
        {
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = directoryID;
            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;
            mParams[2] = new SqlParameter("@PermissionTypeID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryPermissionType", mParams);

            return (PermissionType)mParams[2].Value;
        }
        #endregion

        #region File Permissions
        /// <summary>
        /// If File have permission ReadOnly - Return true
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>>
        /// <param name="fileID">File ID</param>
        /// <returns></returns>
        public static bool IsFileReadOnly(SqlTransaction trans, int userID, int fileID)
        {
            PermissionType perm = GetFilePermission(trans, userID, fileID);
            return perm == PermissionType.ReadOnly || perm == PermissionType.FullAccess;
        }

        /// <summary>
        /// If File have permission ReadOnly - Return true
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>>
        /// <param name="fileID">File ID</param>
        /// <returns></returns>
        public static bool IsFileReadOnly(string connectionString, int userID, int fileID)
        {
            PermissionType perm = GetFilePermission(connectionString, userID, fileID);
            return perm == PermissionType.ReadOnly || perm == PermissionType.FullAccess;
        }

        /// <summary>
        /// If File have permission Write - Return true
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>>
        /// <param name="fileID">File ID</param>
        /// <returns></returns>
        public static bool IsFileWrite(SqlTransaction trans, int userID, int fileID)
        {
            PermissionType perm = GetFilePermission(trans, userID, fileID);
            return perm == PermissionType.Write || perm == PermissionType.FullAccess;
        }

        /// <summary>
        /// If File have permission Write - Return true
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>>
        /// <param name="fileID">File ID</param>
        /// <returns></returns>
        public static bool IsFileWrite(string connectionString, int userID, int fileID)
        {
            PermissionType perm = GetFilePermission(connectionString, userID, fileID);
            return perm == PermissionType.Write || perm == PermissionType.FullAccess;
        }
        #endregion

        #region Directory Permissions
        /// <summary>
        /// If Directory have permission ReadOnly - Return true
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static bool IsDirectoryReadOnly(SqlTransaction trans, int userID, int directoryID)
        {
            PermissionType perm = GetDirectoryPermission(trans, userID, directoryID);
            return perm == PermissionType.ReadOnly || perm == PermissionType.FullAccess;
        }

        /// <summary>
        /// If Directory have permission ReadOnly - Return true
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static bool IsDirectoryReadOnly(string connectionString, int userID, int directoryID)
        {
            PermissionType perm = GetDirectoryPermission(connectionString, userID, directoryID);
            return perm == PermissionType.ReadOnly || perm == PermissionType.FullAccess;
        }

        /// <summary>
        /// If Directory have permission Write - Return true
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static bool IsDirectoryWrite(SqlTransaction trans, int userID, int directoryID)
        {
            PermissionType perm = GetDirectoryPermission(trans, userID, directoryID);
            return perm == PermissionType.Write || perm == PermissionType.FullAccess;
        }

        /// <summary>
        /// If Directory have permission Write - Return true
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static bool IsDirectoryWrite(string connectionString, int userID, int directoryID)
        {
            PermissionType perm = GetDirectoryPermission(connectionString, userID, directoryID);
            return perm == PermissionType.Write || perm == PermissionType.FullAccess;
        }
        #endregion
    }
}