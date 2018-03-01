using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using IO.VFS.Data;

namespace IO.VFS
{
    public class MIMETypeVFS
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        public MIMETypeVFS(string connectionString)
        {
            this.connectionString = connectionString;
            this.value = String.Empty;
            this.description = String.Empty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="Id">MIME Type ID</param>
        /// <param name="value">MIME Type Value</param>
        /// <param name="description">MIME Type Description</param>
        public MIMETypeVFS(string connectionString, int id, string value, string description)
        {
            this.connectionString = connectionString;
            this.id = id;
            this.value = value;
            this.description = description;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="Id">MIME Type ID</param>        
        public MIMETypeVFS(string connectionString, int id)
        {
            this.connectionString = connectionString;
            this.id = id;
            this.LoadByID();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="value">MIME Type Value</param>
        public MIMETypeVFS(string connectionString, string value)
        {
            this.connectionString = connectionString;
            this.value = value;
            this.LoadByValue();
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
        /// Get MIME Type Primary Key
        /// </summary>
        public int ID
		{
            get { return id; }            
		}
        
        private string value;
        /// <summary>
        /// Get MIME Value
        /// </summary>
        public string Value
        {
            get { return this.value; }            
        }
        
        private string description;
        /// <summary>
        /// Get MIME Description
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
            SqlParameter[] Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@MIMETypeID", SqlDbType.Int);
            Params[0].Value = this.id;
            Params[1] = new SqlParameter("@MIMEValue", SqlDbType.NVarChar, 127);
            Params[1].Direction = ParameterDirection.Output;
            Params[2] = new SqlParameter("@Description", SqlDbType.NVarChar, 255);
            Params[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetMIMETypeByID", Params);
            if (Params[1].Value.Equals(DBNull.Value)) throw new ExceptionVFS(String.Format("ID: {0}. MIME Type not found", this.ID.ToString()));
            this.value = Params[1].Value.ToString();
            this.description = Params[2].Value.ToString();
        }

        /// <summary>
        ///  Load File Information By Value
        /// </summary>
        private void LoadByValue()
        {
            SqlParameter[] Params = new SqlParameter[3];
            Params[0] = new SqlParameter("@MIMEValue", SqlDbType.NVarChar, 127);            
            Params[0].Value = this.value;
            Params[1] = new SqlParameter("@MIMETypeID", SqlDbType.Int);
            Params[1].Direction = ParameterDirection.Output;
            Params[2] = new SqlParameter("@Description", SqlDbType.NVarChar, 255);
            Params[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, "usp_vfs_GetMIMETypeByValue", Params);
            if (Params[1].Value.Equals(DBNull.Value)) throw new ExceptionVFS(String.Format("MIME Value: {0}. MIME Type not found", this.Value));
            this.id = (int)Params[1].Value;
            this.description = Params[2].Value.ToString();
        }
        #endregion

        #region Public Method
        public override bool Equals(object obj)
        {
            if (obj is MIMETypeVFS)
            {
                return (
                    (this.Value.ToLower() == ((MIMETypeVFS)obj).Value.ToLower()) &&
                    (this.Description.ToLower() == ((MIMETypeVFS)obj).Description.ToLower())
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
            return this.Value;
        }
        #endregion               

        #region Public Static Method
        /// <summary>
        /// Get All MIME Types
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <returns></returns>
        public static List<MIMETypeVFS> GetMIMEList(string connectionString)
        {
            SqlDataReader reader = GetMIMEs(connectionString);
            MIMETypeVFS mimeType;
            List<MIMETypeVFS> list = new List<MIMETypeVFS>();
            try
            {
                while (reader.Read())
                {
                    mimeType = new MIMETypeVFS(connectionString, (int)reader["MIMETypeID"], reader["MIMEValue"].ToString(), reader["Description"].ToString());
                    list.Add(mimeType);
                }
            }
            finally
            {
                if (!reader.IsClosed) reader.Close();
            }
            return list;
        }

        /// <summary>
        /// Get All MIME Types
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <returns></returns>
        public static SqlDataReader GetMIMEs(string connectionString)
        {
            return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, "usp_vfs_GetMIMETypes");
        }

        /// <summary>
        /// Is MIME Type Exists
        /// </summary>
        /// <param name="connectionString">Connection String for connection to Data Base</param>
        /// <param name="mimeTypeValue">MIME Type Value</param>
        /// <returns>If MIME Type exists return True. Else Return False</returns>
        public static bool Exists(string connectionString, string mimeTypeValue)
        {                                
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@MIMEValue", SqlDbType.NVarChar, 127);
            mParams[0].Value = mimeTypeValue;
            mParams[1] = new SqlParameter("@IsExists", SqlDbType.Int);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "usp_vfs_MIMETypeExists", mParams);
            return ((int)mParams[1].Value) == 1;
        }
        #endregion
    }
}