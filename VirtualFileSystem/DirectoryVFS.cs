using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using IO.VFS.Data;

namespace IO.VFS
{
    public class DirectoryVFS
    {        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        public DirectoryVFS(string connectionString)
        {
            this.connectionString = connectionString;
            this.id = Int32.MinValue;
            this.name = String.Empty;            
            this.parentID = Int32.MinValue;
            this.parentName = String.Empty;
            this.path = String.Empty;
            this.userID = Int32.MinValue;
            this.userName = String.Empty;
            this.hasFiles = false;
            this.permissionType = PermissionType.FullAccess;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        public DirectoryVFS(string connectionString, int id)
        {
            this.connectionString = connectionString;
            this.LoadDirectory(id);
            this.permissionType = PermissionType.FullAccess;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">Uaser ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        public DirectoryVFS(string connectionString, int userID, int parentID, string directoryName)
        {
            this.connectionString = connectionString;
            this.LoadDirectory(userID, parentID, directoryName);
            this.permissionType = PermissionType.FullAccess;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">Uaser ID</param>
        /// <param name="path">Directory path</param>
        public DirectoryVFS(string connectionString, int userID, string path)
        {
            this.connectionString = connectionString;
            this.LoadDirectory(userID, path);
            this.permissionType = PermissionType.FullAccess;
        }

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
        
		private int id;
        /// <summary>
        /// Get Primary Key this Directory
        /// </summary>
		public int ID
		{
            get { return id; }
		}

        private int parentID;
        /// <summary>
        /// Get or set Primary Key Parent Directory
        /// </summary>
        public int ParentID
        {
            get { return parentID; }
            set { parentID = value; }
        }

        private string parentName;
        /// <summary>
        /// Get Parent Directory Name
        /// </summary>
        public string ParentName
        {
            get { return parentName; }
        }

        private string name;
        /// <summary>
        /// Get or set Directory Name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string path;
        /// <summary>
        /// Get Directory Path
        /// </summary>
        public string Path
        {
            get { return path; }
        }


        private DateTime createTime;
        /// <summary>
        /// Get Date & Time create Directory
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
        }

        private DateTime changeTime;
        /// <summary>
        /// Get Date & Time change Directory
        /// </summary>
        public DateTime ChangeTime
        {
            get { return changeTime; }
        }

        private int userID = Int32.MinValue;
        /// <summary>
        /// Get or set Owner User ID
        /// </summary>        
        public int UserID
        {
            get { return userID; }
            set { this.userID = value; }
        }

        private string userName;
        /// <summary>
        /// Get Owner User Name
        /// </summary>        
        public string UserName
        {
            get { return userName; }
        }

        /// <summary>
        /// Get Is Root Directory 
        /// </summary>        
        public bool IsRoot
        {
            get { return this.id == this.parentID; }
        }

        private bool hasFiles;
        /// <summary>
        /// Get Has Files
        /// </summary>        
        public bool HasFiles
        {
            get { return this.hasFiles; }
        }

        private PermissionType permissionType;
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

		#endregion

        #region Private Methods
        private void ValidateFields()
        {
            if (this.name == null)
                throw new ExceptionVFS("Directory name is empty.");
            if (this.name.Trim() == String.Empty)
                throw new Exception("Directory name is empty.");
            if (this.userID == Int32.MinValue)
                throw new ExceptionVFS("User ID is empty.");
            ValidateName(this.name);
        }

        private static void ValidateName(string name)
        {
            if (name.IndexOfAny(FileVFS.invalidChars) > -1)
            {
                throw new ExceptionVFS(String.Format("Directory Name \"{0}\" is incorrect.", name));
            }
        }

        private void LoadDirectory(SqlTransaction trans, int id)
        {
            SqlParameter[] mParams = new SqlParameter[10];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@CreateTime", SqlDbType.DateTime);
            mParams[1].Direction = ParameterDirection.Output;

            mParams[2] = new SqlParameter("@ChangeTime", SqlDbType.DateTime);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;

            mParams[4] = new SqlParameter("@ParentDirectoryName", SqlDbType.NVarChar, 127);
            mParams[4].Direction = ParameterDirection.Output;

            mParams[5] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[5].Direction = ParameterDirection.Output;

            mParams[6] = new SqlParameter("@UserName", SqlDbType.NVarChar, 127);
            mParams[6].Direction = ParameterDirection.Output;

            mParams[7] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[7].Direction = ParameterDirection.Output;

            mParams[8] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[8].Direction = ParameterDirection.Output;

            mParams[9] = new SqlParameter("@HasFiles", SqlDbType.Int);
            mParams[9].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetDirectory", mParams);

            if (mParams[3].Value.Equals(DBNull.Value))
            {
                throw new ExceptionVFS(String.Format("Directory ID {0} is not found", id.ToString()));
            }

            this.id = id;
            this.createTime = (DateTime)mParams[1].Value;
            this.changeTime = (DateTime)mParams[2].Value;
            this.parentID = (int)mParams[3].Value;
            this.parentName = (string)mParams[4].Value;
            this.userID = (int)mParams[5].Value;
            this.userName = (string)mParams[6].Value;
            this.name = (string)mParams[7].Value;
            this.path = (string)mParams[8].Value;
            this.hasFiles = (int)mParams[9].Value == 1;

            ValidateFields();
        }

        private void LoadDirectory(int id)
        {
            SqlParameter[] mParams = new SqlParameter[10];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@CreateTime", SqlDbType.DateTime);
            mParams[1].Direction = ParameterDirection.Output;

            mParams[2] = new SqlParameter("@ChangeTime", SqlDbType.DateTime);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;

            mParams[4] = new SqlParameter("@ParentDirectoryName", SqlDbType.NVarChar, 127);
            mParams[4].Direction = ParameterDirection.Output;

            mParams[5] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[5].Direction = ParameterDirection.Output;

            mParams[6] = new SqlParameter("@UserName", SqlDbType.NVarChar, 127);
            mParams[6].Direction = ParameterDirection.Output;

            mParams[7] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[7].Direction = ParameterDirection.Output;

            mParams[8] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[8].Direction = ParameterDirection.Output;

            mParams[9] = new SqlParameter("@HasFiles", SqlDbType.Int);
            mParams[9].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectory", mParams);

            if (mParams[3].Value.Equals(DBNull.Value))
            {
                throw new ExceptionVFS(String.Format("Directory ID {0} is not found", id.ToString()));
            }

            this.id = id;
            this.createTime = (DateTime)mParams[1].Value;
            this.changeTime = (DateTime)mParams[2].Value;
            this.parentID = (int)mParams[3].Value;
            this.parentName = (string)mParams[4].Value;
            this.userID = (int)mParams[5].Value;
            this.userName = (string)mParams[6].Value;
            this.name = (string)mParams[7].Value;
            this.path = (string)mParams[8].Value;
            this.hasFiles = (int)mParams[9].Value == 1;

            ValidateFields();
        }

        private void LoadDirectory(int userID, int parentID, string directoryName)
        {
            SqlParameter[] mParams = new SqlParameter[10];

            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;

            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[1].Value = parentID;

            mParams[2] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[2].Value = directoryName;

            mParams[3] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;

            mParams[4] = new SqlParameter("@CreateTime", SqlDbType.DateTime);
            mParams[4].Direction = ParameterDirection.Output;

            mParams[5] = new SqlParameter("@ChangeTime", SqlDbType.DateTime);
            mParams[5].Direction = ParameterDirection.Output;

            mParams[6] = new SqlParameter("@ParentDirectoryName", SqlDbType.NVarChar, 127);
            mParams[6].Direction = ParameterDirection.Output;

            mParams[7] = new SqlParameter("@UserName", SqlDbType.NVarChar, 127);
            mParams[7].Direction = ParameterDirection.Output;

            mParams[8] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[8].Direction = ParameterDirection.Output;

            mParams[9] = new SqlParameter("@HasFiles", SqlDbType.Int);
            mParams[9].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryByName", mParams);

            if ((int)mParams[3].Value == -1)
            {
                throw new ExceptionVFS(String.Format("Directory {0} is not found", directoryName));
            }

            this.userID = userID;
            this.parentID = parentID;
            this.name = directoryName;
            this.id = (int)mParams[3].Value;
            this.createTime = (DateTime)mParams[4].Value;
            this.changeTime = (DateTime)mParams[5].Value;
            this.parentName = (string)mParams[6].Value;
            this.userName = (string)mParams[7].Value;
            this.path = (string)mParams[8].Value;
            this.hasFiles = (int)mParams[9].Value == 1;

            ValidateFields();
        }

        private void LoadDirectory(int userID, string path)
        {
            SqlParameter[] mParams = new SqlParameter[10];

            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;

            mParams[1] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[1].Value = path;

            mParams[2] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@CreateTime", SqlDbType.DateTime);
            mParams[3].Direction = ParameterDirection.Output;

            mParams[4] = new SqlParameter("@ChangeTime", SqlDbType.DateTime);
            mParams[4].Direction = ParameterDirection.Output;

            mParams[5] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[5].Direction = ParameterDirection.Output;

            mParams[6] = new SqlParameter("@ParentDirectoryName", SqlDbType.NVarChar, 127);
            mParams[6].Direction = ParameterDirection.Output;

            mParams[7] = new SqlParameter("@UserName", SqlDbType.NVarChar, 127);
            mParams[7].Direction = ParameterDirection.Output;

            mParams[8] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[8].Direction = ParameterDirection.Output;

            mParams[9] = new SqlParameter("@HasFiles", SqlDbType.Int);
            mParams[9].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryByPath", mParams);

            if ((int)mParams[2].Value == -1)
            {
                throw new ExceptionVFS(String.Format("Directory {0} is not found", path));
            }
            this.userID = userID;
            this.path = path;
            this.id = (int)mParams[2].Value;
            this.createTime = (DateTime)mParams[3].Value;
            this.changeTime = (DateTime)mParams[4].Value;
            this.parentID = (int)mParams[5].Value;
            this.parentName = (string)mParams[6].Value;
            this.userName = (string)mParams[7].Value;
            this.name = (string)mParams[8].Value;
            this.hasFiles = (int)mParams[9].Value == 1;

            ValidateFields();
        }
        #endregion

        #region Public Method

        public override bool Equals(object obj)
        {
            if (obj is DirectoryVFS)
            {
                return (
                    (this.name.ToLower() == ((DirectoryVFS)obj).name.ToLower()) &&
                    (this.parentID == ((DirectoryVFS)obj).parentID) &&
                    (this.userID == ((DirectoryVFS)obj).userID)
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
            return this.path;
        }

        #region Create Directory
        /// <summary>
        ///  Insert Directory Information in DB
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <returns>Primary Key created Directory</returns>
        public int Create(SqlTransaction trans)
        {
            ValidateFields();

            if (DirectoryVFS.Exists(trans, this.userID, this.parentID, this.name, Int32.MinValue)) throw new ExceptionVFS(String.Format("Directory {0} alredy exists", this.name));
          
            SqlParameter[] mParams = new SqlParameter[6];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Direction = ParameterDirection.Output;

            mParams[1] = new SqlParameter("@CreateTime", SqlDbType.DateTime);
            mParams[1].Direction = ParameterDirection.Output;

            mParams[2] = new SqlParameter("@ChangeTime", SqlDbType.DateTime);
            mParams[2].Direction = ParameterDirection.Output;

            mParams[3] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            if(this.parentID == Int32.MinValue)
            {
                mParams[3].Value = DBNull.Value;
            }
            else
            {
                mParams[3].Value = this.parentID;
            }

            mParams[4] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[4].Value = this.userID;

            mParams[5] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[5].Value = this.name;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_CreateDirectory", mParams);
            this.id = (int)mParams[0].Value;
            this.createTime = (DateTime)mParams[1].Value;
            this.changeTime = (DateTime)mParams[2].Value;
            return this.id;
        }

        /// <summary>
        ///  Insert Directory Information in DB
        /// </summary>        
        /// <returns>Primary Key created Directory</returns>
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

        #region Move Directory
        /// <summary>
        /// Move directory to other parent directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="directoryID">Directory ID</param>
        public void Move(SqlTransaction trans, int directoryID)
        {
            string pathIDs = GetPathIDs(trans, directoryID);
            if(pathIDs.IndexOf(String.Format("{0};", this.id)) > -1 )
            {
                throw new ExceptionVFS("Cycled error");
            }

            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = this.id;

            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[1].Value = directoryID;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_MoveDirectory", mParams);
            this.parentID = directoryID;
        }

        /// <summary>
        /// Move directory to other parent directory
        /// </summary>
        /// <param name="directoryID">Directory ID</param>
        public void Move(int directoryID)
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    this.Move(trans, directoryID);
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

        #region Rename Directory
        /// <summary>
        /// Rename directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="directoryName">Directory Name</param>
        public void Rename(SqlTransaction trans, string directoryName)
        {
            if (DirectoryVFS.Exists(trans, this.userID, this.parentID, directoryName, this.id)) throw new ExceptionVFS(String.Format("Directory {0} alredy exists", directoryName));
            
            this.ValidateFields();

            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = this.id;

            mParams[1] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[1].Value = directoryName;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_RenameDirectory", mParams);
        }

        /// <summary>
        /// Rename directory
        /// </summary>
        /// <param name="directoryName">Directory Name</param>
        public void Rename(string directoryName)
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    this.Rename(trans, directoryName);
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

        #region Add File To Directory
        /// <summary>
        /// Add file to Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="file">File Object</param>
        /// <returns>Return primary key added file</returns>
        public int AddFile(SqlTransaction trans, FileVFS file)
        {
            return AddFile(trans, file, true);
        }

        /// <summary>
        /// Add file to Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="file">File Object</param>
        /// <param name="IsValidate"></param>
        /// <returns>Return primary key added file</returns>
        private int AddFile(SqlTransaction trans, FileVFS file, bool IsValidate)
        {
            if (IsValidate)
            {
                if (FileVFS.Exists(trans, this.id, file.FullName, this.userID))
                {
                    throw new ExceptionVFS(String.Format("File {0} is alredy exists", file.FullName));
                }
            }

            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserToFileID", SqlDbType.Int);
            mParams[0].Value = file.ID;

            mParams[1] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[1].Value = this.ID;

            mParams[2] = new SqlParameter("@RealUserToFileID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_AddFileToDirectory", mParams);

            if (mParams[2].Value.Equals(DBNull.Value))
            {
                throw new ExceptionVFS(String.Format("File {0} is not found", file.FullName));
            }

            return (int)mParams[2].Value;
        }

        /// <summary>
        /// Add file to Directory
        /// </summary>        
        /// <param name="file">File Object</param>
        /// <returns>Return primary key added file</returns>
        public int AddFile(FileVFS file)
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    int res = this.AddFile(trans, file);
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

        /// <summary>
        /// Add file to Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <returns>Return primary key added file</returns>
        public int AddFile(SqlTransaction trans, int fileID)
        {
            FileVFS file = new FileVFS(this.ConnectionString);
            file.LoadFile(trans, fileID);
            return this.AddFile(trans, file);
        }

        /// <summary>
        /// Add file to Directory
        /// </summary>        
        /// <param name="fileID">File ID</param>
        /// <returns>Return primary key added file</returns>
        public int AddFile(int fileID)
        {
            return this.AddFile(new FileVFS(this.ConnectionString, fileID));
        }
        #endregion

        #region Create File in directory
        /// <summary>
        /// Create File and Add to Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="fileContent">File Content</param>
        /// <returns>Return primary key created file</returns>
        public int CreateFile(SqlTransaction trans, string fileName, byte[] fileContent)
        {
            FileVFS file = new FileVFS(this.ConnectionString);
            file.FullName = fileName;
            file.userID = this.userID;
            file.directoryID = this.id;
            file.FileContent = fileContent;
            if(!FileVFS.Exists(trans, this.id, fileName, this.userID))
            {
                file.Create(trans);
            }
            else
            {
                file.id = FileVFS.GetFileID(trans, file.FullName, file.HashCode, file.UserID, this.id);
            }
            return this.AddFile(trans, file, false);
        }

        /// <summary>
        /// Create File and Add to Directory
        /// </summary>        
        /// <param name="fileName">File Name</param>
        /// <param name="fileContent">File Content</param>
        /// <returns>Return primary key created file</returns>
        public int CreateFile(string fileName, byte[] fileContent)
        {
            FileVFS file = new FileVFS(this.ConnectionString);
            file.FullName = fileName;
            file.userID = this.userID;
            file.directoryID = this.id;
            file.FileContent = fileContent;
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    if (!FileVFS.Exists(trans, this.id, fileName, this.userID))
                    {
                        file.Create(trans);
                    }
                    else
                    {
                        file.id = FileVFS.GetFileID(trans, file.FullName, file.HashCode, file.UserID, file.DirectoryID);
                    }
                    int res = this.AddFile(trans, file, false);
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

        /// <summary>
        /// Create File and Add to Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="stream">Stream with file content</param>
        /// <returns>Return primary key created file</returns>
        public int CreateFile(SqlTransaction trans, string fileName, Stream stream)
        {
            FileVFS file = new FileVFS(this.ConnectionString);
            file.FullName = fileName;
            file.userID = this.userID;
            file.directoryID = this.id;
            file.LoadFromStream(stream);
            if (!FileVFS.Exists(trans, this.id, fileName, this.userID))
            {
                file.Create(trans);
            }
            else
            {
                file.id = FileVFS.GetFileID(trans, file.FullName, file.HashCode, file.UserID, file.DirectoryID);
            }
            return this.AddFile(trans, file, false);
        }

        /// <summary>
        /// Create File and Add to Directory
        /// </summary>        
        /// <param name="fileName">File Name</param>
        /// <param name="stream">Stream with file content</param>
        /// <returns>Return primary key created file</returns>
        public int CreateFile(string fileName, Stream stream)
        {
            FileVFS file = new FileVFS(this.ConnectionString);
            file.FullName = fileName;
            file.userID = this.userID;
            file.directoryID = this.id;
            file.LoadFromStream(stream);
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    if (!FileVFS.Exists(trans, this.id, fileName, this.userID))
                    {
                        file.Create(trans);
                    }
                    else
                    {
                        file.id = FileVFS.GetFileID(trans, file.FullName, file.HashCode, file.UserID, file.DirectoryID);
                    }
                    int res = this.AddFile(trans, file, false);
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

        #endregion

        #region Public Static Method

        #region Create Directory
        /// <summary>
        /// Create Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryName">Directoryname</param>
        /// <returns>Created Directory</returns>
        public static DirectoryVFS Create(SqlTransaction trans, int userID, string directoryName)
        {
            if (directoryName.Trim() == "\\") directoryName = String.Empty;
            if (directoryName.Substring(0, 1) == "\\") directoryName = directoryName.Substring(1, directoryName.Length - 1);
            if (directoryName.Substring(directoryName.Length - 1, 1) == "\\") directoryName = directoryName.Substring(0, directoryName.Length - 1);
            if (directoryName.Trim() == String.Empty)
            {
                throw new ExceptionVFS("Directory name is empty");
            }
            int dirID = Int32.MinValue;
            string[] dirList = directoryName.Split('\\');
            int parentId = Int32.MinValue;
            foreach (string dirName in dirList)
            {
                if (dirName.Trim() == String.Empty)
                {
                    throw new ExceptionVFS(String.Format("Directory name {0} is invalid", directoryName));
                }
                dirID = GetDirectoryID(trans, userID, parentId, dirName);                                        
                if (dirID == -1)
                {
                    dirID = Create(trans, userID, parentId, dirName).ID;
                }
                parentId = dirID;
            }
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, dirID);
            return dir;
        }

        /// <summary>
        /// Create Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        /// <returns>Created Directory</returns>
        public static DirectoryVFS Create(SqlTransaction trans, int userID, int parentID, string directoryName)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.userID = userID;
            dir.parentID = parentID;
            dir.name = directoryName;
            int id = dir.Create(trans);
            dir.LoadDirectory(trans, id);
            return dir;
        }

        /// <summary>
        /// Create Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryName">Directoryname</param>
        /// <returns>Created Directory</returns>
        public static DirectoryVFS Create(string connectionString, int userID, string directoryName)
        {
            if (directoryName.Trim() == "\\") directoryName = String.Empty;
            if (directoryName.Substring(0, 1) == "\\") directoryName = directoryName.Substring(1, directoryName.Length - 1);
            if (directoryName.Substring(directoryName.Length - 1, 1) == "\\") directoryName = directoryName.Substring(0, directoryName.Length - 1);
            if(directoryName.Trim() == String.Empty)
            {
                throw new ExceptionVFS("Directory name is empty");
            }
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                int dirID = Int32.MinValue;
                try
                {
                    trans = connection.BeginTransaction();
                    string[] dirList = directoryName.Split('\\');                    
                    int parentId = Int32.MinValue;
                    foreach (string dirName in dirList)
                    {
                        if (dirName.Trim() == String.Empty)
                        {
                            throw new ExceptionVFS(String.Format("Directory name {0} is invalid", directoryName));
                        }
                        dirID = GetDirectoryID(trans, userID, parentId, dirName);                        
                        if (dirID == -1)
                        {
                            dirID = Create(trans, userID, parentId, dirName).ID;
                        }
                        parentId = dirID;
                    }
                    trans.Commit();
                    DirectoryVFS dir = new DirectoryVFS(connectionString, dirID);
                    return dir;
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

        /// <summary>
        /// Create Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        /// <returns>Created Directory</returns>
        public static DirectoryVFS Create(string connectionString, int userID, int parentID, string directoryName)
        {
            DirectoryVFS dir = new DirectoryVFS(connectionString);
            dir.userID = userID;
            dir.parentID = parentID;
            dir.name = directoryName;
            int id = dir.Create();
            dir.LoadDirectory(id);
            return dir;
        }


        /// <summary>
        /// Create Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Path To Directory</param>
        /// <param name="directoryName">Directoryname</param>
        /// <returns>Created Directory</returns>
        public static DirectoryVFS Create(SqlTransaction trans, int userID, string path, string directoryName)
        {
            int parentID = GetDirectoryID(trans, userID, path);
            if (parentID == -1)
            {
                throw new ExceptionVFS(String.Format("Directory path {0} is not found", path));
            }
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.userID = userID;
            dir.parentID = parentID;
            dir.name = directoryName;
            int id = dir.Create(trans);
            dir.LoadDirectory(trans, id);
            return dir;
        }

        /// <summary>
        /// Create Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Path To Directory</param>
        /// <param name="directoryName">Directoryname</param>
        /// <returns>Created Directory</returns>
        public static DirectoryVFS Create(string connectionString, int userID, string path, string directoryName)
        {
            int parentID = GetDirectoryID(connectionString, userID, path);
            if (parentID == -1)
            {
                throw new ExceptionVFS(String.Format("Directory path {0} is not found", path));
            }
            DirectoryVFS dir = new DirectoryVFS(connectionString);
            dir.userID = userID;
            dir.parentID = parentID;
            dir.name = directoryName;
            int id = dir.Create();
            dir.LoadDirectory(id);
            return dir;
        }
        #endregion

        #region Rename Directory
        /// <summary>
        /// Rename Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="id">Directory ID</param>
        /// <param name="directoryName">New Directory Name</param>
        /// <returns>Return renamed Directory</returns>
        public static DirectoryVFS Rename(SqlTransaction trans, int id, string directoryName)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, id);
            dir.Rename(trans, directoryName);
            dir.LoadDirectory(trans, id);
            return dir;
        }

        /// <summary>
        /// Rename Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        /// <param name="directoryName">New Directory Name</param>
        /// <returns>Return renamed Directory</returns>
        public static DirectoryVFS Rename(string connectionString, int id, string directoryName)
        {
            DirectoryVFS dir = new DirectoryVFS(connectionString, id);
            dir.Rename(directoryName);
            return new DirectoryVFS(connectionString, id);
        }
        #endregion

        #region Has Sub Direcory
        /// <summary>
        /// Get True If in directory has Subdirectories
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static bool HasSubDirectories(SqlTransaction trans, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_HasSubDirectoriesByID", mParams);
            return (int)mParams[1].Value == 1;
        }

        /// <summary>
        /// Get True If in directory has Subdirectories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static bool HasSubDirectories(string connectionString, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_HasSubDirectoriesByID", mParams);
            return (int)mParams[1].Value == 1;
        }

        /// <summary>
        /// Get True If in directory has Subdirectories
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryPath">Directory Path</param>
        /// <returns></returns>
        public static bool HasSubDirectories(SqlTransaction trans, int userID, string directoryPath)
        {
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[1].Value = directoryPath;
            mParams[2] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_HasSubDirectoriesByPath", mParams);
            return (int)mParams[2].Value == 1;
        }

        /// <summary>
        /// Get True If in directory has Subdirectories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryPath">Directory Path</param>
        /// <returns></returns>
        public static bool HasSubDirectories(string connectionString, int userID, string directoryPath)
        {
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[1].Value = directoryPath;
            mParams[2] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_HasSubDirectoriesByPath", mParams);
            return (int)mParams[2].Value == 1;
        }

        /// <summary>
        /// Get True If in directory has Subdirectories
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        /// <returns></returns>
        public static bool HasSubDirectories(SqlTransaction trans, int userID, int parentID, string directoryName)
        {
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[1].Value = parentID;
            mParams[2] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[2].Value = directoryName;
            mParams[3] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_HasSubDirectoriesByName", mParams);
            return (int)mParams[3].Value == 1;
        }

        /// <summary>
        /// Get True If in directory has Subdirectories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        /// <returns></returns>
        public static bool HasSubDirectories(string connectionString, int userID, int parentID, string directoryName)
        {
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[1].Value = parentID;
            mParams[2] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[2].Value = directoryName;
            mParams[3] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_HasSubDirectoriesByName", mParams);
            return (int)mParams[3].Value == 1;
        }
        #endregion

        #region Has Files in Direcory
        /// <summary>
        /// /// Get True If in directory has files
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static bool HasFilesInDirectory(SqlTransaction trans, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@HasFiles", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_HasFiles", mParams);
            return (int)mParams[1].Value == 1;
        }

        /// <summary>
        /// /// Get True If in directory has files
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static bool HasFilesInDirectory(string connectionString, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = id;
            mParams[1] = new SqlParameter("@HasFiles", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_HasFiles", mParams);
            return (int)mParams[1].Value == 1;
        }

        /// <summary>
        /// /// Get True If in directory has files
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryPath">Directory Path</param>
        /// <returns></returns>
        public static bool HasFilesInDirectory(SqlTransaction trans, int userID, string directoryPath)
        {
            int directoryID = GetDirectoryID(trans, userID, directoryPath);
            return HasFilesInDirectory(trans, directoryID);
        }

        /// <summary>
        /// /// Get True If in directory has files
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryPath">Directory Path</param>
        /// <returns></returns>
        public static bool HasFilesInDirectory(string connectionString, int userID, string directoryPath)
        {
            int directoryID = GetDirectoryID(connectionString, userID, directoryPath);
            return HasFilesInDirectory(connectionString, directoryID);
        }
        #endregion

        #region Delete Directory
        /// <summary>
        /// Delete Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="id">Directory ID</param>
        public static void Delete(SqlTransaction trans, int id)
        {
            if (DirectoryVFS.HasSubDirectories(trans, id)) throw new ExceptionVFS("This Directory have subdirectories");
            SqlParameter[] mParams = new SqlParameter[1];
            mParams[0] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[0].Value = id;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_DeleteDirectoryByID", mParams);            
        }

        /// <summary>
        /// Delete Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        public static void Delete(string connectionString, int id)
        {
            SqlConnection connection = new SqlConnection(connectionString);
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

        /// <summary>
        /// Delete Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryPath">Directory Path</param>
        public static void Delete(SqlTransaction trans, int userID, string directoryPath)
        {
            if (DirectoryVFS.HasSubDirectories(trans, userID, directoryPath)) throw new ExceptionVFS("This Directory have subdirectories");
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;            
            mParams[1] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[1].Value = directoryPath;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_DeleteDirectoryByPath", mParams);
        }

        /// <summary>
        /// Delete Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryPath">Directory Path</param>
        public static void Delete(string connectionString, int userID, string directoryPath)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    Delete(trans, userID, directoryPath);
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

        /// <summary>
        /// Delete Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        public static void Delete(SqlTransaction trans, int userID, int parentID, string directoryName)
        {
            if (DirectoryVFS.HasSubDirectories(trans, userID, parentID, directoryName)) throw new ExceptionVFS("This Directory have subdirectories");
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[1].Value = parentID;
            mParams[2] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[2].Value = directoryName;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_DeleteDirectoryByName", mParams);
        }

        /// <summary>
        /// Delete Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        public static void Delete(string connectionString, int userID, int parentID, string directoryName)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    Delete(trans, userID, parentID, directoryName);
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

        #region Get Direcory Name
        /// <summary>
        /// Get Directory Name
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static string GetName(SqlTransaction trans, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 127);
            mParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetDirectoryName", mParams);

            string res = (string)mParams[1].Value;
            if(res==String.Empty) throw new ExceptionVFS("This Directory is not found");
            return res;
        }

        /// <summary>
        /// Get Directory Name
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static string GetName(string connectionString, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 127);
            mParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryName", mParams);

            string res = (string)mParams[1].Value;
            if (res == String.Empty) throw new ExceptionVFS("This Directory is not found");
            return res;
        }
        #endregion

        #region Get Parent Directory Name
        /// <summary>
        /// Get Parent Directory Name
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static string GetParentName(SqlTransaction trans, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 127);
            mParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetParentDirectoryName", mParams);

            string res = (string)mParams[1].Value;
            if (res == String.Empty) throw new ExceptionVFS("This Directory has no parental directory");
            return res;
        }

        /// <summary>
        /// Get Parent Directory Name
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static string GetParentName(string connectionString, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 127);
            mParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetParentDirectoryName", mParams);

            string res = (string)mParams[1].Value;
            if (res == String.Empty) throw new ExceptionVFS("This Directory has no parental directory");
            return res;
        }
        #endregion

        #region Get Directory Path
        /// <summary>
        /// Get Directory Path
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static string GetPath(SqlTransaction trans, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetDirectoryPath", mParams);

            string res = (string)mParams[1].Value;
            if (res == String.Empty) throw new ExceptionVFS("This Directory is not found");
            return res;
        }

        /// <summary>
        /// Get Directory Path
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        public static string GetPath(string connectionString, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryPath", mParams);

            string res = (string)mParams[1].Value;
            if (res == String.Empty) throw new ExceptionVFS("This Directory is not found");
            return res;
        }
        #endregion

        #region Get Directory Path IDs
        /// <summary>
        /// Get Path IDs List
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        private static string GetPathIDs(SqlTransaction trans, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@PathIDs", SqlDbType.NVarChar, 4000);
            mParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetDirectoryPathIDs", mParams);

            string res = (string)mParams[1].Value;
            if (res == String.Empty) throw new ExceptionVFS("This Directory is not found");
            return res;
        }

        /// <summary>
        /// Get Path IDs List
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Directory ID</param>
        /// <returns></returns>
        private static string GetPathIDs(string connectionString, int id)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@ID", SqlDbType.Int);
            mParams[0].Value = id;

            mParams[1] = new SqlParameter("@PathIDs", SqlDbType.NVarChar, 4000);
            mParams[1].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryPathIDs", mParams);

            string res = (string)mParams[1].Value;
            if (res == String.Empty) throw new ExceptionVFS("This Directory is not found");
            return res;
        }
        #endregion

        #region Get Directory ID
        /// <summary>
        /// Get Directory ID
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Directory Path</param>
        /// <returns></returns>
        public static int GetDirectoryID(SqlTransaction trans, int userID, string path)
        {
            SqlParameter[] mParams = new SqlParameter[3];

            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[1].Value = path;
            mParams[2] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetDirectoryIDByPath", mParams);

            return (int)mParams[2].Value;            
        }

        /// <summary>
        /// Get Directory ID
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Directory Path</param>
        /// <returns></returns>
        public static int GetDirectoryID(string connectionString, int userID, string path)
        {
            SqlParameter[] mParams = new SqlParameter[3];

            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@Path", SqlDbType.NVarChar, 4000);
            mParams[1].Value = path;
            mParams[2] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryIDByPath", mParams);

            return (int)mParams[2].Value;
        }

        /// <summary>
        /// Get Directory ID
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="path">Directory Name</param>
        /// <returns></returns>
        public static int GetDirectoryID(SqlTransaction trans, int userID, int parentID, string directoryName)
        {
            SqlParameter[] mParams = new SqlParameter[4];

            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            if (parentID == Int32.MinValue)
            {
                mParams[1].Value = DBNull.Value;
            }
            else
            {
                mParams[1].Value = parentID;
            }
            mParams[2] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[2].Value = directoryName;
            mParams[3] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_GetDirectoryIDByName", mParams);

            return (int)mParams[3].Value;
        }


        /// <summary>
        /// Get Directory ID
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="path">Directory Name</param>
        /// <returns></returns>
        public static int GetDirectoryID(string connectionString, int userID, int parentID, string directoryName)
        {
            SqlParameter[] mParams = new SqlParameter[4];

            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            if (parentID == Int32.MinValue)
            {
                mParams[1].Value = DBNull.Value;
            }
            else
            {
                mParams[1].Value = parentID;
            }
            mParams[2] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[2].Value = directoryName;
            mParams[3] = new SqlParameter("@DirectoryID", SqlDbType.Int);
            mParams[3].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_GetDirectoryIDByName", mParams);

            return (int)mParams[3].Value;
        }
        #endregion

        #region Directory Exists
        /// <summary>
        /// Get exists directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        /// <param name="exludeDirectoryID">Exclude Directory Name</param>
        /// <returns></returns>
        private static bool Exists(SqlTransaction trans, int userID, int parentID, string directoryName, int exludeDirectoryID)
        {
            SqlParameter[] mParams = new SqlParameter[5];

            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[1].Value = parentID;
            mParams[2] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[2].Value = directoryName;
            mParams[3] = new SqlParameter("@ExcludeDirectoryID", SqlDbType.Int);
            mParams[3].Value = exludeDirectoryID;
            mParams[4] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[4].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "usp_vfs_ExistsDirectory", mParams);
            return ((int)mParams[4].Value) == 1;
        }

        /// <summary>
        /// Get exists directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>
        /// <param name="exludeDirectoryID">Exclude Directory Name</param>
        /// <returns></returns>
        private static bool Exists(string connectionString, int userID, int parentID, string directoryName, int exludeDirectoryID)
        {
            SqlParameter[] mParams = new SqlParameter[5];

            mParams[0] = new SqlParameter("@UserID", SqlDbType.Int);
            mParams[0].Value = userID;
            mParams[1] = new SqlParameter("@ParentDirectoryID", SqlDbType.Int);
            mParams[1].Value = parentID;
            mParams[2] = new SqlParameter("@DirectoryName", SqlDbType.NVarChar, 127);
            mParams[2].Value = directoryName;
            mParams[3] = new SqlParameter("@ExcludeDirectoryID", SqlDbType.Int);
            mParams[3].Value = exludeDirectoryID;
            mParams[4] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[4].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_ExistsDirectory", mParams);
            return ((int)mParams[4].Value) == 1;
        }

        /// <summary>
        /// Get exists directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>        
        /// <returns></returns>
        public static bool Exists(string connectionString, int userID, int parentID, string directoryName)
        {
            return Exists(connectionString, userID, parentID, directoryName, Int32.MinValue);
        }

        /// <summary>
        /// Get exists directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="parentID">Parent Directory ID</param>
        /// <param name="directoryName">Directory Name</param>        
        /// <returns></returns>
        public static bool Exists(SqlTransaction trans, int userID, int parentID, string directoryName)
        {
            return Exists(trans, userID, parentID, directoryName, Int32.MinValue);
        }
        #endregion

        #region Get Sub Directories
        /// <summary>
        /// Get Sub Directories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static SqlDataReader GetSubDirectories(string connectionString, int directoryID)
        {
            return SqlHelper.ExecuteReader(connectionString, "usp_vfs_GetSubDirectories", directoryID);
        }

        /// <summary>
        /// Get Sub Directories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryPath">Directory Path</param>
        /// <returns></returns>
        public static SqlDataReader GetSubDirectories(string connectionString, int userID, string directoryPath)
        {
            int directoryID = GetDirectoryID(connectionString, userID, directoryPath);
            return GetSubDirectories(connectionString, directoryID);
        }

        /// <summary>
        /// Load from SqlDataReader To List<DirectoryVFS>
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static List<DirectoryVFS> LoadFromReader(string connectionString, SqlDataReader reader)
        {
            List<DirectoryVFS> list = new List<DirectoryVFS>();
            try
            {
                while (reader.Read())
                {
                    DirectoryVFS dir = new DirectoryVFS(connectionString);
                    dir.id = (int)reader["DirectoryID"];
                    dir.createTime = (DateTime)reader["CreateTime"];
                    dir.changeTime = (DateTime)reader["ChangeTime"];
                    dir.parentID = (int)reader["ParentDirectoryID"];
                    dir.parentName = (string)reader["ParentDirectoryName"];
                    dir.userID = (int)reader["UserID"];
                    dir.userName = (string)reader["UserName"];
                    dir.name = (string)reader["DirectoryName"];
                    dir.path = (string)reader["Path"];
                    dir.hasFiles = (int)reader["HasFiles"] == 1;
                    try
                    {
                        dir.permissionType = (PermissionType)reader["PermissionTypeID"];
                    }
                    catch { }
                    list.Add(dir);
                }
            }
            finally
            {
                if (!reader.IsClosed) reader.Close();
            }
            return list;
        }
        #endregion

        #region Get Sub Directories List
        /// <summary>
        /// Get Sub Directories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="directoryID">Directory ID</param>        
        /// <returns></returns>
        public static List<DirectoryVFS> GetSubDirectoryList(string connectionString, int directoryID)
        {
            return LoadFromReader(connectionString, GetSubDirectories(connectionString, directoryID));
        }

        /// <summary>
        /// Get Sub Directories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="directoryPath">Directory Path</param>
        /// <returns></returns>
        public static List<DirectoryVFS> GetSubDirectoryList(string connectionString, int userID, string directoryPath)
        {
            int directoryID = GetDirectoryID(connectionString, userID, directoryPath);
            return GetSubDirectoryList(connectionString, directoryID);
        }
        #endregion

        #region Get Root Directories
        /// <summary>
        /// Get Root Directories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static SqlDataReader GetRootDirectories(string connectionString, int userID)
        {
            return SqlHelper.ExecuteReader(connectionString, "usp_vfs_GetRootDirectories", userID);
        }
        #endregion

        #region Get Root Directories List
        /// <summary>
        /// Get Root Directories
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public static List<DirectoryVFS> GetRootDirectoryList(string connectionString, int userID)
        {
            return LoadFromReader(connectionString, GetRootDirectories(connectionString, userID));
        }
        #endregion

        #region Copy File To Directory
        /// <summary>
        /// Copy File to directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <param name="directoryID">Direcory ID</param>
        /// <returns>Return copied file id</returns>
        public static int CopyFile(SqlTransaction trans, int fileID, int directoryID)
        {
            FileVFS file = new FileVFS(trans.Connection.ConnectionString);
            file.LoadFile(trans, fileID);
            return CopyFile(trans, file, directoryID);
        }

        /// <summary>
        /// Copy File to directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="file">File Object</param>
        /// <param name="directoryID">Direcory ID</param>
        /// <returns>Return copied file id</returns>
        public static int CopyFile(SqlTransaction trans, FileVFS file, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            file.directoryID = directoryID;
            file.userID = dir.userID;
            file.Create(trans);
            return dir.AddFile(trans, file, false);
        }

        /// <summary>
        /// Copy File to directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileID">File ID</param>
        /// <param name="path">Direcory Path</param>
        /// <returns>Return copied file id</returns>
        public static int CopyFile(SqlTransaction trans, int userID, int fileID, string path)
        {
            FileVFS file = new FileVFS(trans.Connection.ConnectionString);
            file.LoadFile(trans, fileID);
            return CopyFile(trans, file, DirectoryVFS.GetDirectoryID(trans, userID, path));
        }

        /// <summary>
        /// Copy File to directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="file">File Object</param>
        /// <param name="path">Direcory Path</param>
        /// <returns>Return copied file id</returns>
        public static int CopyFile(SqlTransaction trans, int userID, FileVFS file, string path)
        {
            return CopyFile(trans, file, DirectoryVFS.GetDirectoryID(trans, userID, path));
        }

        /// <summary>
        /// Copy File to directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="file">File Object</param>
        /// <param name="directoryID">Direcory ID</param>
        /// <returns>Return copied file id</returns>
        public static int CopyFile(string connectionString, FileVFS file, int directoryID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    int res = CopyFile(trans, file, directoryID);
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

        /// <summary>
        /// Copy File to directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <param name="directoryID">Direcory ID</param>
        /// <returns>Return copied file id</returns>
        public static int CopyFile(string connectionString, int fileID, int directoryID)
        {
            FileVFS file = new FileVFS(connectionString);
            file.LoadFile(fileID);
            return CopyFile(connectionString, file, directoryID);
        }

        /// <summary>
        /// Copy File to directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="file">File Object</param>
        /// <param name="path">Direcory Path</param>
        /// <returns>Return copied file id</returns>
        public static int CopyFile(string connectionString, int userID, FileVFS file, string path)
        {
            return CopyFile(connectionString, file, DirectoryVFS.GetDirectoryID(connectionString, userID, path));
        }

        /// <summary>
        /// Copy File to directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileID">File ID</param>
        /// <param name="path">Direcory Path</param>
        /// <returns>Return copied file id</returns>
        public static int CopyFile(string connectionString, int userID, int fileID, string path)
        {
            FileVFS file = new FileVFS(connectionString);
            file.LoadFile(fileID);
            return CopyFile(connectionString, file, DirectoryVFS.GetDirectoryID(connectionString, userID, path));
        }
        #endregion

        #region Delete File From Directory
        /// <summary>
        /// Delete File From Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        public static void DeleteFile(SqlTransaction trans, int fileID)
        {
            FileVFS.Delete(trans, fileID);
        }

        /// <summary>
        /// Delete File From Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="file">File Object</param>
        public static void DeleteFile(SqlTransaction trans, FileVFS file)
        {
            FileVFS.Delete(trans, file.ID);
        }

        /// <summary>
        /// Delete File From Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="hashCode">Hash Code of File Content</param>
        /// <param name="directoryID">Directory ID</param>
        public static void DeleteFile(SqlTransaction trans, string fileName, long hashCode, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            int fileID = FileVFS.GetFileID(trans, fileName, hashCode, dir.userID, directoryID);
            FileVFS.Delete(trans, fileID);
        }        

        /// <summary>
        /// Delete File From Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileName">File Name</param>
        /// <param name="hashCode">Hash Code of File Content</param>
        /// <param name="path">Directory path</param>
        public static void DeleteFile(SqlTransaction trans, int userID, string fileName, long hashCode, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(trans, userID, path);
            DeleteFile(trans, fileName, hashCode, directoryID);
        }

        /// <summary>
        /// Delete File From Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        public static void DeleteFile(string connectionString, int fileID)
        {
            FileVFS.Delete(connectionString, fileID);
        }

        /// <summary>
        /// Delete File From Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="file">File Object</param>
        public static void DeleteFile(string connectionString, FileVFS file)
        {
            FileVFS.Delete(connectionString, file.ID);
        }

        /// <summary>
        /// Delete File From Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileName">File Name</param>
        /// <param name="hashCode">Hash Code of File Content</param>
        /// <param name="directoryID">Directory ID</param>
        public static void DeleteFile(string connectionString, string fileName, long hashCode, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(connectionString, directoryID);
            int fileID = FileVFS.GetFileID(connectionString, fileName, hashCode, dir.userID, directoryID);
            FileVFS.Delete(connectionString, fileID);
        }

        /// <summary>
        /// Delete File From Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileName">File Name</param>
        /// <param name="hashCode">Hash Code of File Content</param>
        /// <param name="path">Directory path</param>
        public static void DeleteFile(string connectionString, int userID, string fileName, long hashCode, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(connectionString, userID, path);
            DeleteFile(connectionString, fileName, hashCode, directoryID);
        }
        #endregion

        #region Move File to Directory
        /// <summary>
        /// Move File to Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <param name="directoryID">Directory ID</param>
        public static void MoveFile(SqlTransaction trans, int fileID, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            FileVFS.Move( directoryID, trans, fileID, dir.UserID);
        }

        /// <summary>
        /// Move File to Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="file">File Object</param>
        /// <param name="directoryID">Directory ID</param>
        public static void MoveFile(SqlTransaction trans, FileVFS file, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            FileVFS.Move(directoryID, trans, file.ID, dir.UserID);
        }

        /// <summary>
        /// Move File to Directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Directory Path</param>
        public static void MoveFile(SqlTransaction trans, int fileID, int userID, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(trans, userID, path);
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            FileVFS.Move(trans, fileID, dir.UserID, directoryID);
        }

        /// <summary>
        /// Move File to Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <param name="directoryID">Directory ID</param>
        public static void MoveFile(string connectionString, int fileID, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(connectionString, directoryID);
            FileVFS.Move(connectionString, fileID, dir.UserID, directoryID);
        }

        /// <summary>
        /// Move File to Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="file">File Object</param>
        /// <param name="directoryID">Directory ID</param>
        public static void MoveFile(string connectionString, FileVFS file, int directoryID)
        {
            MoveFile(connectionString, file.ID, directoryID);
        }

        /// <summary>
        /// Move File to Directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Directory Path</param>
        public static void MoveFile(string connectionString, int fileID, int userID, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(connectionString, userID, path);
            DirectoryVFS dir = new DirectoryVFS(connectionString, directoryID);            
            FileVFS.Move(connectionString, fileID, dir.UserID, directoryID);
        }
        #endregion

        #region Rename File
        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileID">File ID</param>
        /// <param name="fileName">New File name</param>
        /// <param name="directoryID">Directory ID</param>
        public static void RenameFile(SqlTransaction trans, int fileID, string fileName, int directoryID)
        {
            FileVFS.Rename(trans, fileID, fileName, directoryID);
        }

        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="file">File Object</param>
        /// <param name="fileName">New File name</param>
        /// <param name="directoryID">Directory ID</param>
        public static void RenameFile(SqlTransaction trans, FileVFS file, string fileName, int directoryID)
        {
            FileVFS.Rename(trans, file.ID, fileName, directoryID);
        }

        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="oldFileName">Old File Name</param>
        /// <param name="newFileName">New File Name</param>
        /// <param name="hashCode">Hash Code of File Content</param>
        /// <param name="directoryID">Directory ID</param>
        public static void RenameFile(SqlTransaction trans, string oldFileName, string newFileName, long hashCode, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            int fileID = FileVFS.GetFileID(trans, oldFileName, hashCode, dir.userID, directoryID);
            FileVFS.Rename(trans, fileID, newFileName, directoryID);
        }

        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="oldFileName">Old File Name</param>
        /// <param name="newFileName">New File Name</param>
        /// <param name="hashCode">Hash Code of File Content</param>
        /// <param name="path">Directory Path</param>
        public static void RenameFile(SqlTransaction trans, int userID, string oldFileName, string newFileName, long hashCode, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(trans, userID, path);
            RenameFile(trans, oldFileName, newFileName, hashCode, directoryID);
        }

        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileID">File ID</param>
        /// <param name="fileName">New File name</param>
        /// <param name="directoryID">Directory ID</param>
        public static void RenameFile(string connectionString, int fileID, string fileName, int directoryID)
        {
            FileVFS.Rename(connectionString, fileID, fileName, directoryID);
        }

        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="file">File Object</param>
        /// <param name="fileName">New File name</param>
        /// <param name="directoryID">Directory ID</param>
        public static void RenameFile(string connectionString, FileVFS file, string fileName, int directoryID)
        {
            FileVFS.Rename(connectionString, file.ID, fileName, directoryID);
        }

        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="oldFileName">Old File Name</param>
        /// <param name="newFileName">New File Name</param>
        /// <param name="hashCode">Hash Code of File Content</param>
        /// <param name="directoryID">Directory ID</param>
        public static void RenameFile(string connectionString, string oldFileName, string newFileName, long hashCode, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(connectionString, directoryID);
            int fileID = FileVFS.GetFileID(connectionString, oldFileName, hashCode, dir.userID, directoryID);
            FileVFS.Rename(connectionString, fileID, newFileName, directoryID);
        }

        /// <summary>
        /// Rename File
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="oldFileName">Old File Name</param>
        /// <param name="newFileName">New File Name</param>
        /// <param name="hashCode">Hash Code of File Content</param>
        /// <param name="path">Directory Path</param>
        public static void RenameFile(string connectionString, int userID, string oldFileName, string newFileName, long hashCode, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(connectionString, userID, path);
            RenameFile(connectionString, oldFileName, newFileName, hashCode, directoryID);
        }
        #endregion

        #region File Exists
        /// <summary>
        /// Get True if file exists in directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="fileName">File Name</param>
        /// <param name="directoryID">File Directory</param>
        /// <returns></returns>
        public static bool FileExists(SqlTransaction trans, string fileName, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            return FileVFS.Exists(trans, directoryID, fileName, dir.UserID);
        }

        /// <summary>
        /// Get True if file exists in directory
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileName">File Name</param>
        /// <param name="path">Directory Path</param>
        /// <returns></returns>
        public static bool FileExists(SqlTransaction trans, int userID, string fileName, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(trans, userID, path);
            return FileExists(trans, fileName, directoryID);
        }

        /// <summary>
        /// Get True if file exists in directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="fileName">File Name</param>
        /// <param name="directoryID">File Directory</param>
        /// <returns></returns>
        public static bool FileExists(string connectionString, string fileName, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(connectionString, directoryID);
            return FileVFS.Exists(connectionString, directoryID, fileName, dir.UserID);
        }

        /// <summary>
        /// Get True if file exists in directory
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="fileName">File Name</param>
        /// <param name="path">Directory Path</param>
        /// <returns></returns>
        public static bool FileExists(string connectionString, int userID, string fileName, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(connectionString, userID, path);
            return FileExists(connectionString, fileName, directoryID);
        }
        #endregion

        #region Get Files From Directory
        /// <summary>
        /// Get Directory Files
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static SqlDataReader GetFiles(SqlTransaction trans, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            return FileVFS.GetFiles(trans, dir.UserID, directoryID);
        }

        /// <summary>
        /// Get Directory Files
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Directory Path</param>
        /// <returns></returns>
        public static SqlDataReader GetFiles(SqlTransaction trans, int userID, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(trans, userID, path);
            return GetFiles(trans, directoryID);
        }

        /// <summary>
        /// Get Directory Files
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static SqlDataReader GetFiles(string connectionString, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(connectionString, directoryID);
            return FileVFS.GetFiles(connectionString, dir.UserID, directoryID);
        }

        /// <summary>
        /// Get Directory Files
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Directory Path</param>
        /// <returns></returns>
        public static SqlDataReader GetFiles(string connectionString, int userID, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(connectionString, userID, path);
            return GetFiles(connectionString, directoryID);
        }
        #endregion

        #region Get Files From Directory List
        /// <summary>
        /// Get Directory Files
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static List<FileVFS> GetFileList(SqlTransaction trans, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(trans.Connection.ConnectionString);
            dir.LoadDirectory(trans, directoryID);
            return FileVFS.GetFileList(trans, dir.UserID, directoryID);
        }

        /// <summary>
        /// Get Directory Files
        /// </summary>
        /// <param name="trans">Transaction</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Directory Path</param>
        /// <returns></returns>
        public static List<FileVFS> GetFileList(SqlTransaction trans, int userID, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(trans, userID, path);            
            return GetFileList(trans, directoryID);
        }

        /// <summary>
        /// Get Directory Files
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="directoryID">Directory ID</param>
        /// <returns></returns>
        public static List<FileVFS> GetFileList(string connectionString, int directoryID)
        {
            DirectoryVFS dir = new DirectoryVFS(connectionString, directoryID);            
            return FileVFS.GetFileList(connectionString, dir.UserID, directoryID);
        }

        /// <summary>
        /// Get Directory Files
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="userID">User ID</param>
        /// <param name="path">Directory Path</param>
        /// <returns></returns>
        public static List<FileVFS> GetFileList(string connectionString, int userID, string path)
        {
            int directoryID = DirectoryVFS.GetDirectoryID(connectionString, userID, path);
            return GetFileList(connectionString, directoryID);
        }
        #endregion

        #endregion
    }
}