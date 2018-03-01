using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using IO.VFS.Data;

namespace IO.VFS
{
    public class FileVFS
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        public FileVFS(string connectionString)
        {
            this.connectionString = connectionString;
            this.id = Int32.MinValue;
            this.name = String.Empty;
            this.extension = new ExtensionVFS(connectionString);
            this.userID = Int32.MinValue;
            this.directoryID = Int32.MinValue;
            this.userName = String.Empty;
            this.createTime = DateTime.MinValue;
            this.changeTime = DateTime.MinValue;
            this.hashCode = Int32.MinValue;
            this.permissionType = PermissionType.FullAccess;
        }

        /// <summary>
        /// Constructor, get FileVFS data
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileId">FileVFS Primary Key</param>
        public FileVFS(string connectionString, int fileId)
        {
            this.connectionString = connectionString;
            this.LoadFile(fileId);
            this.permissionType = PermissionType.FullAccess;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileName">Phisical Path + File Name</param>
        /// <param name="userID">User Owner ID</param>
        /// <param name="fileContent">File Content</param>
        public FileVFS(string connectionString, string fileName, int userID, byte[] fileContent)
        {
            this.connectionString = connectionString;
            this.LoadFile(fileName, userID, fileContent);
            this.permissionType = PermissionType.FullAccess;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileName">Phisical Path and File Name</param>
        /// <param name="userID">User Owner ID</param>
        /// <param name="stream">Stream for load content</param>
        public FileVFS(string connectionString, string fileName, int userID, Stream stream)
        {
            this.connectionString = connectionString;
            this.LoadFromStream(stream, fileName, userID);
            this.permissionType = PermissionType.FullAccess;
        }

        
        #endregion

        #region Static Properties
        internal static char[] invalidChars = { '\\', '/', ':', '*', '"', '?', '>', '<', '|' };
        #endregion
        
        #region Fields and Properties
        private string connectionString;
        /// <summary>
        /// Get Connection String for connection to Data Base.
        /// </summary>
        private string ConnectionString
        {
            get { return connectionString; }
        }
                
		internal int id;
        /// <summary>
        /// Get File Primary Key
        /// </summary>
		public int ID
		{
            get { return id; }
		}

        internal string name;
        /// <summary>
        /// Get or set File Name
        /// </summary>        
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        internal ExtensionVFS extension;
        /// <summary>
        /// Get File Extension 
        /// </summary>
        public ExtensionVFS Extension
        {
            get { return extension; }
        }

        /// <summary>
        /// Get or set Full File Name
        /// </summary>        
        public string FullName
        {
            get
            {
                if (this.extension == null || this.extension.Extension == String.Empty)
                {
                    return this.name == null ? String.Empty : this.name;
                }
                else
                {
                    return String.Format("{0}.{1}", this.name == null ? String.Empty : this.name, this.extension.Extension);
                }
            }
            set
            {
                this.name = GetName(value);
                this.extension = new ExtensionVFS(this.ConnectionString, GetExtension(value));
            }
        }

        internal int userID;
        /// <summary>
        /// Get or set Owner User ID
        /// </summary>        
        public int UserID
        {
            get { return userID; }
            set { this.userID = value; }
        }

        internal int directoryID;
        /// <summary>
        /// Get Directory ID
        /// </summary>        
        public int DirectoryID
        {
            get { return directoryID; }
        }

        private string userName;
        /// <summary>
        /// Get Owner User Name
        /// </summary>        
        public string UserName
        {
            get { return userName; }
        }

        private DateTime createTime;
        /// <summary>
        /// Get Date & Time create file
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
        }

        private DateTime changeTime;
        /// <summary>
        /// Get Date & Time change file
        /// </summary>
        public DateTime ChangeTime
        {
            get { return changeTime; }
        }
        
        private long hashCode;
        /// <summary>
        /// Get File Data Hash Code
        /// </summary>        
        public long HashCode
        {
            get { return hashCode; }
        }
        
        internal byte[] fileContent;
        /// <summary>
        /// Get or set File Content
        /// </summary>        
        public byte[] FileContent
        {
            get
            {
                return this.GetFileContent();
            }
            set
            {
                if (value == null)
                {
                    throw new ExceptionVFS("File content can not by empty");
                }
                this.fileContent = value;
                this.CalculateHashCode();
            }
        }

        public int size;
        /// <summary>
        /// Get File Size
        /// </summary>        
        public int Size
        {
            get
            {
                return this.size;                
            }
        }

        public PermissionType permissionType;
        /// <summary>
        /// Get Permission Type
        /// </summary>        
        public PermissionType PermissionType
        {
            get
            {
                return this.permissionType;
            }
        }
                
		#endregion fields and properties

        #region Private Methods

        /// <summary>
        /// Validate fields 
        /// </summary>
        private void ValidateFields()
        {
            if (this.Name == null)
                throw new ExceptionVFS("File name is empty.");
            if (this.Name.Trim() == String.Empty)
                throw new Exception("File name is empty.");            
            if (this.userID == Int32.MinValue)
                throw new ExceptionVFS("User ID is empty.");
            if (this.extension == null)
                throw new ExceptionVFS("File Extension not found.");
            ValidateName(this.name);
        }

        /// <summary>
        /// Validate File Name
        /// </summary>
        /// <param name="name"></param>
        private static void ValidateName(string name)
        {
            if (name.IndexOfAny(invalidChars) > -1)
            {
                throw new ExceptionVFS(String.Format("File Name \"{0}\" is incorrect.", name));
            }
        }

        /// <summary>
        /// Calculate hash code by file content
        /// </summary>
        private void CalculateHashCode()
        {
            long fileHashCode = Convert.ToInt64(0);
            int fileSize = Convert.ToInt32(0);
            if (this.fileContent != null)
            {
                fileHashCode = ComputeHash(this.fileContent);
                fileSize = this.fileContent.Length;
            }
            this.hashCode = fileHashCode;
            this.size = fileSize;
        }

        /// <summary>
        /// Get File Content
        /// </summary>
        /// <returns>Return File Content from DB or property fileContent</returns>
        private byte[] GetFileContent()
        {
            if (this.id == Int32.MinValue)
            {
                return this.fileContent;
            }
            else
            {
                if (this.fileContent == null)
                {
                    this.fileContent = GetContent(this.ConnectionString, this.id);                    
                    return this.fileContent;
                }
                else
                {
                    return this.fileContent;
                }
            }
        }
        
        /// <summary>
        /// Load Data from Phisical File
        /// </summary>
        /// <param name="fileName">Phisical Path + File Name</param>
        /// <param name="userID">User Owner ID</param>
        /// <param name="fileContent">File Content</param>
        private void LoadFile(string fileName, int userID, byte[] fileContent)
        {
            this.name = GetName(fileName);
            this.userID = userID;
            this.directoryID = Int32.MinValue;
            this.FileContent = fileContent;
            if (ExtensionVFS.Exists(this.ConnectionString, GetExtension(fileName)))
            {
                this.extension = new ExtensionVFS(this.ConnectionString, GetExtension(fileName));
            }
            ValidateFields();
        }        

        /// <summary>
        ///  Load File Information
        /// </summary>        
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File Primary Key</param>
        internal void LoadFile(SqlTransaction trans, int fileID)
        {           	         
            SqlParameter[] mParams = new SqlParameter[15];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = fileID;

            mParams[1] = new SqlParameter("@FileName", SqlDbType.NVarChar, 127);
            mParams[1].Direction = ParameterDirection.Output;

            mParams[2] = new SqlParameter("@HashCode", SqlDbType.BigInt);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;

            mParams[4] = new SqlParameter("@UserName", SqlDbType.NVarChar, 127);
            mParams[4].Direction = ParameterDirection.Output;

            mParams[5] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[5].Direction = ParameterDirection.Output;

            mParams[6] = new SqlParameter("@CreateTime", SqlDbType.DateTime);
            mParams[6].Direction = ParameterDirection.Output;

            mParams[7] = new SqlParameter("@ChangeTime", SqlDbType.DateTime);
            mParams[7].Direction = ParameterDirection.Output;

            mParams[8] = new SqlParameter("@ExtensionID", SqlDbType.Int);
            mParams[8].Direction = ParameterDirection.Output;

            mParams[9] = new SqlParameter("@MIMETypeID", SqlDbType.Int);
            mParams[9].Direction = ParameterDirection.Output;

            mParams[10] = new SqlParameter("@MIMEValue", SqlDbType.NVarChar, 127);
            mParams[10].Direction = ParameterDirection.Output;

            mParams[11] = new SqlParameter("@MIMEDescription", SqlDbType.NVarChar, 255);
            mParams[11].Direction = ParameterDirection.Output;

            mParams[12] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            mParams[12].Direction = ParameterDirection.Output;

            mParams[13] = new SqlParameter("@ExtensionDescription", SqlDbType.NVarChar, 255);
            mParams[13].Direction = ParameterDirection.Output;

            mParams[14] = new SqlParameter("@FileSize", SqlDbType.Int);
            mParams[14].Direction = ParameterDirection.Output;

            if (trans == null)
            {
                SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetFile", mParams);
            }
            else
            {
                SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetFile", mParams);
            }

            if (mParams[1].Value == DBNull.Value)
            {
                throw new ExceptionVFS("File not found");
            }

            this.id = fileID;
            this.name = (string)mParams[1].Value;
            this.hashCode = (long)mParams[2].Value;
            this.userID = (int)mParams[3].Value;
            this.userName = (string)mParams[4].Value;
            this.directoryID = (int)mParams[5].Value;
            this.createTime = (DateTime)mParams[6].Value;
            this.changeTime = (DateTime)mParams[7].Value;
            this.size = (int)mParams[14].Value;

            this.extension = new ExtensionVFS(this.ConnectionString,
                                                  (int)mParams[8].Value,
                                                  (int)mParams[9].Value,
                                                  (string)mParams[10].Value,
                                                  (string)mParams[11].Value,
                                                  (string)mParams[12].Value,
                                                  (string)mParams[13].Value);

        }

        /// <summary>
        ///  Load File Information
        /// </summary>        
        /// <param name="fileID">File Primary Key</param>
        internal void LoadFile(int fileID)
        {
            this.LoadFile(null, fileID);            
        }
        #endregion

        #region Public Method
        public override bool Equals(object obj)
        {
            if (obj is FileVFS)
            {
                return (
                    (this.Name.ToLower() == ((FileVFS)obj).Name.ToLower()) &&
                    (this.HashCode == ((FileVFS)obj).HashCode) &&
                    (this.userID == ((FileVFS)obj).userID) &&
                    (this.Extension.Equals(((FileVFS)obj).Extension))
                    );
            }
            else return false;
        }

        public override int GetHashCode()
        {            
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.FullName;
        }

        #region Create file
        /// <summary>
        ///  Insert File Information in DB
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <returns>Primary Key created file</returns>
        public int Create( SqlTransaction trans )
        {           
            ValidateFields();

            if (FileVFS.Exists(trans, this.FullName, this.userID, this.directoryID, Int32.MinValue))
                throw new ExceptionVFS(String.Format("File {0} alredy exists", this.FullName));
            
            SqlParameter[] mParams = new SqlParameter[8];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Direction = ParameterDirection.Output;

            mParams[1] = new SqlParameter("@CreateTime", SqlDbType.DateTime);
            mParams[1].Direction = ParameterDirection.Output;

            mParams[2] = new SqlParameter("@ChangeTime", SqlDbType.DateTime);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@FileName", SqlDbType.NVarChar, 127);
            mParams[3].Value = this.name;

            mParams[4] = new SqlParameter("@HashCode", SqlDbType.BigInt);
            mParams[4].Value = this.hashCode;

            mParams[5] = new SqlParameter("@ExtensionID", SqlDbType.Int);
            mParams[5].Value = this.extension.ID;

            mParams[6] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[6].Value = this.userID;

            mParams[7] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[7].Value = this.directoryID;            

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_CreateFile", mParams);
            this.id = (int)mParams[0].Value;
            FileContentAdapterVFS.UpdateContent(trans, this.id, this.FileContent);
            this.LoadFile(trans, this.id);
            return this.id;
        }

        /// <summary>
        ///  Insert File Information in FileVFS
        /// </summary>      
        /// <returns>Primary Key created file</returns>
        public int Create()
        {           
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    int res = this.Create(trans);
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
                connection.Close();
            }
        }
        #endregion

        #region Update file content
        /// <summary>
        ///  Update Content File 
        /// </summary>
        /// <param name="trans">Transaction</param>    
        public void Update(SqlTransaction trans)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = this.id;            
            mParams[1] = new SqlParameter("@HashCode", SqlDbType.BigInt);
            mParams[1].Value = this.hashCode;                        
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_UpdateContent", mParams);
            FileContentAdapterVFS.UpdateContent(trans, this.id, this.FileContent);
        }

        /// <summary>
        ///  Update Content File 
        /// </summary>
        public void Update()
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
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
        #endregion

        #region Copy File
        /// <summary>
        ///  Copy File to other user
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User</param>
        /// <returns>Primary Key copied file</returns>
        public int Copy(SqlTransaction trans, int userID)
        {
            if (FileVFS.Exists(trans, this.FullName, userID, this.directoryID, this.ID)) throw new ExceptionVFS(String.Format("File {0} alredy exists", this.FullName));

            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = this.id;

            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;

            mParams[2] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[2].Value = this.directoryID;

            mParams[3] = new SqlParameter("@NewUserToFileID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_CopyFile", mParams);

            return (int)mParams[3].Value;
        }        

        /// <summary>
        ///  Copy File to other user
        /// </summary>      
        /// <param name="userID">User</param>
        /// <returns>Primary Key copied file</returns>
        public int Copy(int userID)
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    int res = this.Copy(trans, userID);
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
                connection.Close();
            }
        }
        #endregion

        #region Rename File
        /// <summary>
        ///  Rename File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>        
        public void Rename(SqlTransaction trans, string fileName)
        {
            if (FileVFS.Exists(trans, fileName, this.userID, this.directoryID, this.ID)) throw new ExceptionVFS(String.Format("File {0} alredy exists", fileName));

            this.name = GetName(fileName);
            if (ExtensionVFS.Exists(this.ConnectionString, GetExtension(fileName)))
            {
                this.extension = new ExtensionVFS(this.ConnectionString, GetExtension(fileName));
            }
            this.ValidateFields();

            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = this.id;

            mParams[1] = new SqlParameter("@FileName", SqlDbType.NVarChar, 127);
            mParams[1].Value = this.name;

            mParams[2] = new SqlParameter("@ExtensionID", SqlDbType.Int);
            mParams[2].Value = this.extension.ID;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_RenameFile", mParams);
        }

        /// <summary>
        ///  Rename File
        /// </summary>
        /// <param name="fileName">File Name</param>        
        public void Rename(string fileName)
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    this.Rename(trans, fileName);
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

        #region Move File
        /// <summary>
        ///  Move File to other user
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User</param>
        public void Move(SqlTransaction trans, int userID)
        {
            this.Move(Int32.MinValue, trans, userID);
        }

        /// <summary>
        ///  Move File to other user
        /// </summary>
        /// <param name="directoryID">Directory</param>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User</param>        
        internal void Move(int directoryID, SqlTransaction trans, int userID)
        {
            if (FileVFS.Exists(trans, this.FullName, userID, this.directoryID, this.ID)) throw new ExceptionVFS(String.Format("File {0} alredy exists", this.FullName));

            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = this.id;

            mParams[1] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[1].Value = userID;

            mParams[2] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[2].Value = directoryID;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_MoveFile", mParams);

            this.userID = userID;
        }

        /// <summary>
        ///  Move File to other user
        /// </summary>
        /// <param name="userID">User</param>        
        public void Move(int userID)
        {
            this.Move(userID, Int32.MinValue);
        }

        /// <summary>
        ///  Move File to other user
        /// </summary>      
        /// <param name="userID">User</param>
        /// <param name="directoryID">Directory</param>
        internal void Move(int userID, int directoryID)
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    this.Move(directoryID, trans, userID);
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

        #region Save Content
        /// <summary>
        /// Save file content to stream
        /// </summary>
        /// <param name="stream">Stream for save file content</param>
        public void SaveToStream(Stream stream)
        {
            if (this.FileContent != null)
            {
                stream.Write(this.FileContent, 0, this.FileContent.Length);
            }
        }

        /// <summary>
        /// Save file content to file
        /// </summary>
        /// <param name="fileName">File name for save file content</param>
        public void SaveToFile(string fileName)
        {
            fileName = fileName.Replace("/", "\\");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            try
            {
                this.SaveToStream(file);
            }
            finally
            {
                file.Close();
            }
        }
        #endregion

        #region Load Content
        /// <summary>
        /// Load file content from stream
        /// </summary>
        /// <param name="stream">Stream for load content</param>
        public void LoadFromStream(Stream stream)
        {
            long straemPosition = stream.Position;
            try
            {
                stream.Position = 0;
                byte[] fileContent = new byte[(int)stream.Length];
                stream.Read(fileContent, 0, (int)stream.Length);
                this.FileContent = fileContent;
            }
            finally
            {
                stream.Position = straemPosition;
            }
        }

        /// <summary>
        /// Load file content from stream
        /// </summary>
        /// <param name="stream">Stream for load content</param>
        /// <param name="fileName">File name</param>
        /// <param name="userID">User ID</param>
        public void LoadFromStream(Stream stream, string fileName, int userID)
        {
            this.LoadFromStream(stream);
            this.name = GetName(fileName);            
            if (ExtensionVFS.Exists(this.ConnectionString, GetExtension(fileName)))
            {
                this.extension = new ExtensionVFS(this.ConnectionString, GetExtension(fileName));
            }
            this.userID = userID;
            this.directoryID = Int32.MinValue;
            this.ValidateFields();
        }

        /// <summary>
        /// Load file content from file
        /// </summary>
        /// <param name="fileName">File name</param>
        public void LoadFromFile(string fileName)
        {
            fileName = fileName.Replace("/", "\\");
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            try
            {
                this.LoadFromStream(file);
            }
            finally
            {
                file.Close();
            }
        }
        #endregion
        #endregion

        #region Public Static Method

        #region Delete File
        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File Primary Key</param>
        public static void Delete( SqlTransaction trans, int fileID)
        {
            FileContentAdapterVFS.DeleteContent(trans, fileID);
            SqlHelper.ExecuteNonQuery( trans, "usp_vfs_DeleteFile", fileID);            
        }

        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File Primary Key</param>
        public static void Delete(string connectionString, int fileID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    Delete(trans, fileID);
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

        #region Get Files
        /// <summary>
        /// Get Files for user
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns>Retun File List by UserId</returns>
        public static SqlDataReader GetFiles(string connectionString, int userID)
        {
            return GetFiles(connectionString, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Files by user
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns>Retun File List by UserId</returns>
        internal static SqlDataReader GetFiles(string connectionString, int userID, int directoryID)
        {
            return SqlHelper.ExecuteReader(connectionString, "usp_vfs_GetFiles", userID, directoryID);
        }

        public static SqlDataReader GetFiles(SqlTransaction trans, int userID)
        {
            return GetFiles(trans, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Files by user
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <returns>Retun File List by UserId</returns>
        internal static SqlDataReader GetFiles(SqlTransaction trans, int userID, int directoryID)
        {
            return SqlHelper.ExecuteReader(trans, "usp_vfs_GetFiles", userID, directoryID);
        }
        #endregion

        #region Get File List
        /// <summary>
        /// Get Files by user
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <returns>Retun File List by UserId</returns>
        public static List<FileVFS> GetFileList(SqlTransaction trans, int userID)
        {
            return GetFileList(trans, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Files by user
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns>Retun File List by UserId</returns>
        public static List<FileVFS> GetFileList(string connectionString, int userID)
        {
            return GetFileList(connectionString, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get Files by user
        /// </summary>        
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns>Retun File List by UserId</returns>
        internal static List<FileVFS> GetFileList(SqlTransaction trans, int userID, int directoryID)
        {
            List<FileVFS> list = new List<FileVFS>();
            SqlDataReader reader = FileVFS.GetFiles(trans, userID, directoryID);
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
        /// Get Files by user
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns>Retun File List by UserId</returns>
        internal static List<FileVFS> GetFileList(string connectionString, int userID, int directoryID)
        {
            List<FileVFS> list = new List<FileVFS>();
            SqlDataReader reader = FileVFS.GetFiles(connectionString, userID, directoryID);
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

        /// <summary>
        /// Get FileVFS Object Filled Values
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="reader">Files Values Reder </param>
        /// <returns>FileVFS Object</returns>
        internal static FileVFS GetFileFromReader(string connectionString, SqlDataReader reader)
        {
            FileVFS file = new FileVFS(connectionString);
            file.id = (int)reader["UserToFileID"];
            file.name = (string)reader["FileName"];
            file.userID = (int)reader["UserID"];
            file.userName = (string)reader["UserName"];
            file.directoryID = (int)reader["DirectoryID"];
            file.createTime = (DateTime)reader["CreateTime"];
            file.changeTime = (DateTime)reader["ChangeTime"];
            file.hashCode = (long)reader["HashCode"];
            file.size = (int)reader["FileSize"];
            try
            {
                file.permissionType = (PermissionType)reader["PermissionTypeID"];
            }
            catch { }
            file.extension = new ExtensionVFS(connectionString,
                                              (int)reader["ExtensionID"],
                                              (int)reader["MIMETypeID"],
                                              (string)reader["MIMEValue"],
                                              (string)reader["MIMEDescription"],
                                              (string)reader["Extension"],
                                              (string)reader["ExtensionDescription"]);
            file.ValidateFields();
            return file;
        }
        #endregion

        #region Save content
        /// <summary>
        /// Save file content to stream
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">file ID</param>
        /// <param name="stream">Stream for save file content</param>
        public static void SaveToStream(string connectionString, int fileID, Stream stream)
        {
            FileVFS file = new FileVFS(connectionString, fileID);
            file.SaveToStream(stream);
        }

        /// <summary>
        /// Save file content to file
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">file ID</param>
        /// <param name="fileName">File name for save file content</param>
        public static void SaveToFile(string connectionString, int fileID, string fileName)
        {
            FileVFS file = new FileVFS(connectionString, fileID);
            file.SaveToFile(fileName);
        }
        #endregion

        #region Move File
        /// <summary>
        ///  Move File to other user
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">file ID</param>
        /// <param name="userID">User</param>
        /// <param name="directoryID">Directory ID</param>
        internal static void Move(SqlTransaction trans, int fileID, int userID, int directoryID)
        {
            FileVFS file = new FileVFS(trans.Connection.ConnectionString);
            file.LoadFile(trans, fileID);
            file.Move(directoryID, trans, userID);
        }

        /// <summary>
        ///  Move File to other user
        /// </summary>
        /// <param name="directoryID">Directory ID</param>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">file ID</param>
        /// <param name="userID">User</param>        
        internal static void Move(int directoryID, SqlTransaction trans, int fileID, int userID)
        {
            FileVFS file = new FileVFS(trans.Connection.ConnectionString);
            file.LoadFile(trans, fileID);
            file.Move(directoryID, trans, userID);
        }

        /// <summary>
        ///  Move File to other user
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">file ID</param>
        /// <param name="userID">User</param>
        public static void Move(SqlTransaction trans, int fileID, int userID)
        {
            FileVFS file = new FileVFS(trans.Connection.ConnectionString);
            file.LoadFile(trans, fileID);
            file.Move(trans, userID);
        }

        /// <summary>
        ///  Move File to other user
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">file ID</param>
        /// <param name="userID">User</param>
        /// <param name="directoryID">Directory ID</param>
        internal static void Move(string connectionString, int fileID, int userID, int directoryID)
        {
            FileVFS file = new FileVFS(connectionString, fileID);
            file.Move(userID, directoryID);
        }

        /// <summary>
        ///  Move File to other user
        /// </summary>      
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">file ID</param>
        /// <param name="userID">User ID</param>
        public static void Move(string connectionString, int fileID, int userID)
        {
            FileVFS file = new FileVFS(connectionString, fileID);
            file.Move(userID);
        }
        #endregion

        #region Copy File
        /// <summary>
        ///  Copy File to other user
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">file ID</param>
        /// <param name="userID">User ID</param>
        /// <returns>Primary Key copied file</returns>
        public static int Copy(SqlTransaction trans, int fileID, int userID)
        {
            FileVFS file = new FileVFS(trans.Connection.ConnectionString);
            file.LoadFile(trans, fileID);
            return file.Copy(trans, userID);
        }

        /// <summary>
        ///  Copy File to other user
        /// </summary>      
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">file ID</param>
        /// <param name="userID">User ID</param>
        /// <returns>Primary Key copied file</returns>
        public static int Copy(string connectionString, int fileID, int userID)
        {
            FileVFS file = new FileVFS(connectionString, fileID);            
            return file.Copy(userID);
        }
        #endregion

        #region Exists File
        /// <summary>
        /// Return True if file exists. Else return False.
        /// </summary>        
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="userID">User ID</param>
        /// <param name="exludeFileID">Exclude File ID</param>
        /// <returns>Return True if file exists. Else return False.</returns>
        private static bool Exists(SqlTransaction trans, string fileName, int userID, int exludeFileID)
        {
            return Exists(trans, fileName, userID, Int32.MinValue, exludeFileID);
        }
        
        /// <summary>
        /// Return True if file exists. Else return False.
        /// </summary>        
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <param name="exludeFileID">Exclude File ID</param>
        /// <returns>Return True if file exists. Else return False.</returns>
        private static bool Exists(SqlTransaction trans, string fileName, int userID, int directoryID, int exludeFileID)
        {
            SqlParameter[] mParams = new SqlParameter[6];

            mParams[0] = new SqlParameter("@FileName", SqlDbType.NVarChar, 127);
            mParams[0].Value = GetName(fileName);

            mParams[1] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            mParams[1].Value = GetExtension(fileName);

            mParams[2] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[2].Value = userID;

            mParams[3] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[3].Value = directoryID;

            mParams[4] = new SqlParameter("@ExcludeUserToFileID", SqlDbType.Int);
            mParams[4].Value = exludeFileID;

            mParams[5] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[5].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_ExistsFile", mParams);
            return ((int)mParams[5].Value) == 1;        
        }

        /// <summary>
        /// Return True if file exists. Else return False.
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileName">File Name</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <param name="exludeFileID">Exclude File ID</param>
        /// <returns>Return True if file exists. Else return False.</returns>
        private static bool Exists(string connectionString, string fileName, int userID, int directoryID, int exludeFileID)
        {
            SqlParameter[] mParams = new SqlParameter[6];

            mParams[0] = new SqlParameter("@FileName", SqlDbType.NVarChar, 127);
            mParams[0].Value = GetName(fileName);

            mParams[1] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            mParams[1].Value = GetExtension(fileName);

            mParams[2] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[2].Value = userID;

            mParams[3] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[3].Value = directoryID;

            mParams[4] = new SqlParameter("@ExcludeUserToFileID", SqlDbType.Int);
            mParams[4].Value = exludeFileID;

            mParams[5] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[5].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_ExistsFile", mParams);
            return ((int)mParams[5].Value) == 1;        
        }

        /// <summary>
        /// Return True if file exists. Else return False.
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileName">File Name</param>
        /// <param name="userID">User ID</param>
        /// <param name="exludeFileID">Exclude File ID</param>
        /// <returns>Return True if file exists. Else return False.</returns>
        private static bool Exists(string connectionString, string fileName, int userID, int exludeFileID)
        {
            return Exists(connectionString, fileName, userID, Int32.MinValue, exludeFileID);
        }        

        /// <summary>
        /// Return True if file exists. Else return False.
        /// </summary>        
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="userID">User ID</param>
        /// <returns>Return True if file exists. Else return False.</returns>
        public static bool Exists(SqlTransaction trans, string fileName, int userID)
        {
            return Exists(trans, fileName, userID, Int32.MinValue);
        }

        /// <summary>
        /// Return True if file exists. Else return False.
        /// </summary>        
        /// <param name="trans">Transaction</param>
        /// <param name="directoryID">Directory ID</param>
        /// <param name="fileName">File Name</param>
        /// <param name="userID">User ID</param>
        /// <returns>Return True if file exists. Else return False.</returns>
        internal static bool Exists(SqlTransaction trans, int directoryID, string fileName, int userID)
        {
            return Exists(trans, fileName, userID, directoryID, Int32.MinValue);
        }

        public static bool Exists(string connectionString, string fileName, int userID)
        {
            return Exists(connectionString, Int32.MinValue, fileName, userID);
        }

        /// <summary>
        /// Return True if file exists. Else return False.
        /// </summary>        
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="directoryID">Directory ID</param>
        /// <param name="fileName">File Name</param>
        /// <param name="userID">User ID</param>
        /// <returns>Return True if file exists. Else return False.</returns>
        internal static bool Exists(string connectionString, int directoryID, string fileName, int userID)
        {
            return Exists(connectionString, fileName, userID, directoryID, Int32.MinValue);
        }
        #endregion

        #region Get File ID
        /// <summary>
        /// Get File Primary Key
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="hashCode">Hash Code</param>
        /// <param name="userID">User ID</param>
        /// <returns>Return File Primaty Key</returns>
        public static int GetFileID(SqlTransaction trans, string fileName, long hashCode, int userID)
        {
            return GetFileID(trans, fileName, hashCode, userID, Int32.MinValue);
        }

        /// <summary>
        /// Get File Primary Key
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="hashCode">Hash Code</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns>Return File Primaty Key</returns>
        internal static int GetFileID(SqlTransaction trans, string fileName, long hashCode, int userID, int directoryID)
        {
            SqlParameter[] mParams = new SqlParameter[6];

            mParams[0] = new SqlParameter("@FileName", SqlDbType.NVarChar, 127);
            mParams[0].Value = GetName(fileName);

            mParams[1] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            mParams[1].Value = GetExtension(fileName);

            mParams[2] = new SqlParameter("@HashCode", SqlDbType.BigInt);
            mParams[2].Value = hashCode;

            mParams[3] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[3].Value = directoryID;

            mParams[4] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[4].Value = userID;


            mParams[5] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[5].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetFileID", mParams);

            if (mParams[5].Value.Equals(DBNull.Value))
            {
                return 0;
                //throw new ExceptionVFS(String.Format("File {0} is not found", fileName));
            }

            return (int)mParams[5].Value;
        }

        /// <summary>
        /// Get File Primary Key
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileName">File Name</param>
        /// <param name="hashCode">Hash Code</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns>Return File Primaty Key</returns>
        internal static int GetFileID(string connectionString, string fileName, long hashCode, int userID, int directoryID)
        {
            SqlParameter[] mParams = new SqlParameter[6];

            mParams[0] = new SqlParameter("@FileName", SqlDbType.NVarChar, 127);
            mParams[0].Value = GetName(fileName);

            mParams[1] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            mParams[1].Value = GetExtension(fileName);

            mParams[2] = new SqlParameter("@HashCode", SqlDbType.BigInt);
            mParams[2].Value = hashCode;

            mParams[3] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[3].Value = directoryID;

            mParams[4] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[4].Value = userID;


            mParams[5] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[5].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetFileID", mParams);

            if (mParams[5].Value.Equals(DBNull.Value))
            {
                throw new ExceptionVFS(String.Format("File {0} is not found", fileName));
            }

            return (int)mParams[5].Value;
        }

        /// <summary>
        /// Get File Primary Key
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileName">File Name</param>
        /// <param name="hashCode">Hash Code</param>
        /// <param name="userID">User ID</param>
        /// <returns>Return File Primaty Key</returns>
        public static int GetFileID(string connectionString, string fileName, long hashCode, int userID)
        {
            return GetFileID(connectionString, fileName, hashCode, userID, Int32.MinValue);
        }


        public static long ComputeHash(byte[] content)
        {
            long fileHashCode = Convert.ToInt64(0);
            if (content != null)
            {
                for (long i = 0; i <= content.Length - 1; i++)
                {
                    fileHashCode += content[i] + i;
                }
                fileHashCode += content.Length;
            }
            return fileHashCode;
        }
        #endregion

        #region Rename File
        /// <summary>
        ///  Rename File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <param name="fileName">File Name</param>        
        public static void Rename(SqlTransaction trans, int fileID, string fileName)
        {
            Rename(trans, fileID, fileName, Int32.MinValue);
        }

        /// <summary>
        ///  Rename File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <param name="fileName">File Name</param>        
        /// <param name="directoryID">Directory ID</param>
        internal static void Rename(SqlTransaction trans, int fileID, string fileName, int directoryID)
        {
            FileVFS file = new FileVFS(trans.Connection.ConnectionString);
            file.LoadFile(trans, fileID);
            file.directoryID = directoryID;
            file.Rename(trans, fileName);
        }

        /// <summary>
        ///  Rename File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <param name="fileName">File Name</param>        
        public static void Rename(string connectionString, int fileID, string fileName)
        {
            Rename(connectionString, fileID, fileName, Int32.MinValue);
        }

        /// <summary>
        ///  Rename File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <param name="fileName">File Name</param>
        internal static void Rename(string connectionString, int fileID, string fileName, int directoryID)
        {
            FileVFS file = new FileVFS(connectionString, fileID);
            file.directoryID = directoryID;
            file.Rename(fileName);
        }
        #endregion

        #region Get File Content
        /// <summary>
        /// Get File Content
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <returns>Return File Content by File ID</returns>
        public static byte[] GetContent(SqlTransaction trans, int fileID)
        {
            return FileContentAdapterVFS.GetContent(trans, fileID);
        }

        /// <summary>
        /// Get File Content
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <returns>Return File Content by File ID</returns>
        public static byte[] GetContent(string connectionString, int fileID)
        {
            return FileContentAdapterVFS.GetContent(connectionString, fileID);
        }
        #endregion

        private static string GetName(string fileName)
        {
            ValidateName(fileName);
            FileInfo info = new FileInfo(fileName);
            return info.Name.Substring(0, info.Name.Length - info.Extension.Length);
        }

        private static string GetExtension(string fileName)
        {
            ValidateName(fileName);
            FileInfo info = new FileInfo(fileName);
            return info.Extension == String.Empty?String.Empty:info.Extension.Substring(1, info.Extension.Length - 1);
        }

        public static Guid GetGuidIDByID(string connectionString, int fileID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@FileID", SqlDbType.Int);
            mParams[0].Value = fileID;
            mParams[1] = new SqlParameter("@GuidFileID", SqlDbType.UniqueIdentifier);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetGuidIDByID", mParams);
            return (Guid)mParams[1].Value;
        }

        public static int GetIDByGuidID(string connectionString, Guid guidFileID)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@GuidFileID", SqlDbType.UniqueIdentifier);            
            mParams[0].Value = guidFileID;
            mParams[1] = new SqlParameter("@FileID", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetIDByGuidID", mParams);
            return (int)mParams[1].Value;
        }

        
        #endregion
    }
}
