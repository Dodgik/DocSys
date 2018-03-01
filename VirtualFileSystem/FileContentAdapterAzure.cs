using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using IO.VFS.Data;

namespace IO.VFS
{
    public class FileContentAdapterVFS_Azure
    {
        #region Get File Content
        /// <summary>
        /// Get File Content
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <returns>Return File Content by File ID</returns>
        public static byte[] GetContent(SqlTransaction trans, int fileID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            mParams[1] = new SqlParameter("@FileContent", SqlDbType.VarBinary, -1);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetFileContent", mParams);
            if (mParams[1].Value == DBNull.Value)
            {
                throw new ExceptionVFS("File not found");
            }
            return (byte[])mParams[1].Value;
        }

        /// <summary>
        /// Get File Content
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <returns>Return File Content by File ID</returns>
        public static byte[] GetContent(string connectionString, int fileID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            mParams[1] = new SqlParameter("@FileContent", SqlDbType.VarBinary, -1);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetFileContent", mParams);
            if (mParams[1].Value == DBNull.Value)
            {
                throw new ExceptionVFS("File not found");
            }
            return (byte[])mParams[1].Value;
        }
        #endregion

        #region Update File Content
        /// <summary>
        /// Update File Content
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <param name="fileContent">File Content</param>
        public static void UpdateContent(SqlTransaction trans, int fileID, byte[] fileContent)
        {            
            //
            string azure_URL = Guid.NewGuid().ToString();
            //File.Create(
            //
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            mParams[1] = new SqlParameter("@AzureStorageURL", SqlDbType.NVarChar, 512);
            mParams[1].Value = azure_URL;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_AZURE_Update", mParams);
        }
        #endregion

        #region Delete File
        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File Primary Key</param>
        public static void DeleteContent(SqlTransaction trans, int fileID)
        {
            
        }
        #endregion
    }
}
