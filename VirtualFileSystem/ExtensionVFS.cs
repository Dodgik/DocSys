using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using IO.VFS.Data;

namespace IO.VFS
{
    public class ExtensionVFS
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        public ExtensionVFS(string connectionString)
        {
            this.connectionString = connectionString;
            this.extension = String.Empty;
            this.description = String.Empty;
            this.mimeType = new MIMETypeVFS(connectionString);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Extension ID</param>
        /// <param name="mimeTypeID">MIME Type ID</param>
        /// <param name="mimeValue">MIME Type Value</param>
        /// <param name="mimeDescription">MIME Type Description</param>
        /// <param name="extension">Extension Value</param>
        /// <param name="description">Description</param>
        public ExtensionVFS(string connectionString, int id, int mimeTypeID, string mimeValue, string mimeDescription, string extension, string description)
        {
            this.connectionString = connectionString;
            this.id = id;
            this.mimeTypeID = mimeTypeID;
            this.mimeType = new MIMETypeVFS(connectionString, mimeTypeID, mimeValue, mimeDescription);
            this.extension = extension;
            this.description = description;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="id">Extension ID</param>
        public ExtensionVFS(string connectionString, int id)
        {
            this.connectionString = connectionString;
            this.id = id;
            this.LoadByID();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="extension">Extension Value</param>
        public ExtensionVFS(string connectionString, string extension)
        {
            this.connectionString = connectionString;
            this.extension = extension;
            this.LoadByExtension();
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
        /// Get Extension Primary Key
        /// </summary>
        public int ID
		{
            get { return id; }
		}
        
        private int mimeTypeID;

        private MIMETypeVFS mimeType;
        /// <summary>
        /// Get MIME Type
        /// </summary>
        public MIMETypeVFS MIMEType
        {
            get { return mimeType; }
        }

        private string extension;
        /// <summary>
        /// Get File Extension
        /// </summary>
        public string Extension
        {
            get { return extension; }
        }
        
        private string description;
        /// <summary>
        /// Get Extension Description
        /// </summary>
        public string Description
        {
            get { return description; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        ///  Load File Information By ID
        /// </summary>
        private void LoadByID()
        {
            SqlParameter[] Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@ExtensionID", SqlDbType.Int);
            Params[0].Value = this.id;
            Params[1] = new SqlParameter("@MIMETypeID", SqlDbType.Int);
            Params[1].Direction = ParameterDirection.Output;
            Params[2] = new SqlParameter("@MIMEValue", SqlDbType.NVarChar, 127);
            Params[2].Direction = ParameterDirection.Output;
            Params[3] = new SqlParameter("@MIMEDescription", SqlDbType.NVarChar, 255);
            Params[3].Direction = ParameterDirection.Output;
            Params[4] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            Params[4].Direction = ParameterDirection.Output;
            Params[5] = new SqlParameter("@Description", SqlDbType.NVarChar, 255);
            Params[5].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetExtensionByID", Params);
            if (Params[1].Value.Equals(DBNull.Value)) throw new ExceptionVFS(String.Format("ID: {0}. Extension not found", this.ID.ToString()));
            this.mimeTypeID = (int)Params[1].Value;
            this.extension = Params[4].Value.ToString();
            this.description = Params[5].Value.ToString();
            this.mimeType = new MIMETypeVFS(this.ConnectionString, (int)Params[1].Value, Params[2].Value.ToString(), Params[3].Value.ToString());
        }

        /// <summary>
        ///  Load File Information By Extension
        /// </summary>
        private void LoadByExtension()
        {
            SqlParameter[] Params = new SqlParameter[6];
            Params[0] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            Params[0].Value = this.extension;
            Params[1] = new SqlParameter("@ExtensionID", SqlDbType.Int);
            Params[1].Direction = ParameterDirection.Output;
            Params[2] = new SqlParameter("@MIMETypeID", SqlDbType.Int);
            Params[2].Direction = ParameterDirection.Output;
            Params[3] = new SqlParameter("@MIMEValue", SqlDbType.NVarChar, 127);
            Params[3].Direction = ParameterDirection.Output;
            Params[4] = new SqlParameter("@MIMEDescription", SqlDbType.NVarChar, 255);
            Params[4].Direction = ParameterDirection.Output;
            Params[5] = new SqlParameter("@Description", SqlDbType.NVarChar, 255);
            Params[5].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetExtensionByExtension", Params);
            if (Params[1].Value.Equals(DBNull.Value)) throw new ExceptionVFS(String.Format("Extension Value: {0}. Extension not found", this.Extension));
            this.id = (int)Params[1].Value;
            this.mimeTypeID = (int)Params[2].Value;
            this.description = Params[5].Value.ToString();
            this.mimeType = new MIMETypeVFS(this.ConnectionString, (int)Params[2].Value, Params[3].Value.ToString(), Params[4].Value.ToString());
        }
        #endregion

        #region Public Method
        public override bool Equals(object obj)
        {
            if (obj is ExtensionVFS)
            {
                return (
                    (this.MIMEType.Equals(((ExtensionVFS)obj).MIMEType)) &&
                    (this.Extension.ToLower() == ((ExtensionVFS)obj).Extension.ToLower()) &&
                    (this.Description.ToLower() == ((ExtensionVFS)obj).Description.ToLower())
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
            return this.Extension;
        }
        #endregion               

        #region Public Static Method
        /// <summary>
        /// Get Existing Extension List
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <returns>Return Generic object List<ExtensionVFS></returns>
        public static List<ExtensionVFS> GetExtensionList(string connectionString)
        {
            SqlDataReader reader = GetExtensions(connectionString);
            ExtensionVFS extension;
            List<ExtensionVFS> list = new List<ExtensionVFS>();
            try
            {
                while (reader.Read())
                {
                    extension = new ExtensionVFS(connectionString, (int)reader["ExtensionID"],
                                                                   (int)reader["MIMETypeID"],
                                                                   reader["MIMEValue"].ToString(),
                                                                   reader["MIMEDescription"].ToString(),
                                                                   reader["Extension"].ToString(),
                                                                   reader["Description"].ToString());
                    list.Add(extension);
                }
            }
            finally
            {
                if (!reader.IsClosed) reader.Close();
            }
            return list;
        }        

        /// <summary>
        /// Get Existing Extension List
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <returns>Return SqlDataReader object</returns>
        public static SqlDataReader GetExtensions(string connectionString)
        {
            return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, "usp_vfs_GetExtensions");
        }

        /// <summary>
        /// Is Exists Extension
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="extension">Extension Value</param>
        /// <returns>If Extension existr then return True. Else Return False</returns>
        public static bool Exists(string connectionString, string extension)
        {
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@Extension", SqlDbType.NVarChar, 50);
            mParams[0].Value = extension;
            mParams[1] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_ExtensionExists", mParams);
            return ((int)mParams[1].Value) == 1;
        }        
        #endregion
    }
}