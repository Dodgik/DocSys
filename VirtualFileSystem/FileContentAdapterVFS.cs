using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using IO.VFS.Data;

namespace IO.VFS
{
    public class FileContentAdapterVFS
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
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            
            mParams[1] = new SqlParameter("@ContentBaseID", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;

            mParams[2] = new SqlParameter("@ContentID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@FileID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;


            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetFileContentInfo", mParams);
            if (mParams[1].Value == DBNull.Value)
            {
                throw new ExceptionVFS("File not found");
            }

            int contentBaseID = (int)mParams[1].Value;
            int contentID = (int)mParams[2].Value;

            return GetFileContent(contentBaseID, contentID);
        }

        /// <summary>
        /// Get File Content
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <returns>Return File Content by File ID</returns>
        public static byte[] GetContent(string connectionString, int fileID)
        {
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;

            mParams[1] = new SqlParameter("@ContentBaseID", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;

            mParams[2] = new SqlParameter("@ContentID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@FileID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;


            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetFileContentInfo", mParams);

            if ((mParams[2].Value == DBNull.Value) || (mParams[1].Value == DBNull.Value))
            {
                throw new ExceptionVFS("File not found");
            }

            int contentBaseID = (int)mParams[1].Value;
            int contentID = (int)mParams[2].Value;

            return GetFileContent(contentBaseID, contentID);
        }


        public static byte[] GetFileContent(int contentBaseID, int contentID)
        {
            byte[] content = null;
            SqlConnection connection = new SqlConnection(getConnectionString(contentBaseID));

            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ContentID", SqlDbType.Int);
            mParams[0].Value = contentID;
            mParams[1] = new SqlParameter("@Content", SqlDbType.VarBinary, -1);
            mParams[1].Direction = ParameterDirection.Output; 

            try
            {
                connection.Open();
                try
                {
                    SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "usp_GetFileContent", mParams);
                    if (mParams[1].Value != DBNull.Value)
                    {
                        content = (byte[])mParams[1].Value;
                    }

                }
                catch (Exception)
                {
                    content = null;
                }
            }
            finally
            {
                connection.Close();
            }

            return content;
        }

        #endregion

        #region Update File Content
        /// <summary>
        /// Update File Content
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <param name="fileContent">File Content</param>
        public static void UpdateContent(SqlTransaction trans, int userToFileId, byte[] fileContent)
        {
            
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = userToFileId;
            
            mParams[1] = new SqlParameter("@ContentBaseID", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;

            mParams[2] = new SqlParameter("@ContentID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@FileID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;


            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetFileContentInfo", mParams);

            int contentBaseID = -1;
            int contentID = -1;
            int fileID = -1;

            if (mParams[3].Value == DBNull.Value)
            {
                throw new ExceptionVFS("File not found");
            }

            fileID = (int)mParams[3].Value;

            if ((mParams[1].Value != DBNull.Value) && (mParams[2].Value != DBNull.Value))
            {
                contentBaseID = (int)mParams[1].Value;
                contentID = (int)mParams[2].Value;
            }
            else
            {
                contentBaseID = getContentBaseID();
            }
        
            SqlConnection connection = new SqlConnection(getConnectionString(contentBaseID));
            try
            {
                connection.Open();
                SqlTransaction transContent = null;
                try
                {
                    transContent = connection.BeginTransaction();

                    mParams = new SqlParameter[2];
                    mParams[0] = new SqlParameter("@ContentID", SqlDbType.Int);
                    mParams[0].Direction = ParameterDirection.InputOutput;
                   if (contentID != -1)
                   {
                    mParams[0].Value = contentID;
                   }

                    mParams[1] = new SqlParameter("@Content", SqlDbType.VarBinary, -1);
                    mParams[1].Value = fileContent;

                    SqlHelper.ExecuteNonQuery(transContent, CommandType.StoredProcedure, "usp_UpdateFileContent", mParams);

                    if ((mParams[0].Value != DBNull.Value))
                    {
                        transContent.Commit();
                        contentID = (int)mParams[0].Value;
                    }
                    else
                    {
                        contentID = -1;
                        if (transContent != null)
                            transContent.Rollback();
                    }

                }
                catch (Exception)
                {
                    contentID = -1;
                    if (transContent != null)
                        transContent.Rollback();
                }
            }
            catch (Exception e)
            {
                throw;
            }

            finally
            {
                connection.Close();
            }

            if (contentID == -1)
            {
                throw new ExceptionVFS("Save file error");
            }

            mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@FileID", SqlDbType.Int);
            mParams[0].Value = fileID;

            mParams[1] = new SqlParameter("@ContentBaseID", SqlDbType.Int);
            mParams[1].Value = contentBaseID;

            mParams[2] = new SqlParameter("@ContentID", SqlDbType.Int);
            mParams[2].Value = contentID;            

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_UpdateFileContent", mParams);
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

        public static string getConnectionString(int contentBaseID)
        {
            string fileConectingStringTemplate = (string)new AppSettingsReader().GetValue("FileContentConnectStringTemplate", typeof(string));
            string fileBase =  string.Format("F{0}",contentBaseID.ToString());
            string res = string.Format(fileConectingStringTemplate, fileBase);
            return res;
        }

        public static int getContentBaseID()
        {
            return Int32.Parse((string)new AppSettingsReader().GetValue("NewFileContentCurrentDB", typeof(string)));
            Random rnd = new Random();
            return rnd.Next(1, 3);
        }
    }
}
