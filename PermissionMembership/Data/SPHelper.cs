using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;

namespace PermissionMembership.Data
{
	/// <summary>
	/// The MHMSqlHelper class is intended to encapsulate high performance, scalable best practices for 
	/// common uses of SqlClient.
	/// </summary>
	public sealed class SPHelper
	{
		#region private utility methods & constructors

		//Since this class provides only static methods, make the default constructor private to prevent 
		//instances from being created with "new MHMSqlHelper()".
		private SPHelper() {}

        public static string GetConnectionString()
        {
            //string ss = ConfigurationSettings.AppSettings["ConnectString"];//Config.ConnectionString;
            //return ss;

            return Config.ConnectionString;
        }

        public static void ThrowException(Exception ex)
        {
            CustomException customException = new CustomException("System error! Error in calling database functions. Exception Message: " + ex.Message);
            throw customException;
        }

		#endregion private utility methods & constructors

		#region ExecuteNonQuery

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset and takes no parameters). 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery("PublishOrders");
		/// </remarks>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string spName)
		{
            try
            {
                return SqlHelper.ExecuteNonQuery(GetConnectionString(), CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset) 
        /// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery("PublishOrders", new SqlParameter("@prodid", 24));
		/// </remarks>		
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string spName, params SqlParameter[] spParameters)
		{
			try
            {
                return SqlHelper.ExecuteNonQuery(GetConnectionString(), CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) using the provided parameter values.  
        /// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery("PublishOrders", 24, 36);
		/// </remarks>		
		/// <param name="spName">the name of the stored prcedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(string spName, params object[] parameterValues)
		{
			try
            {
                return SqlHelper.ExecuteNonQuery(GetConnectionString(), spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, "PublishOrders");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, string spName)
		{
			try
            {
                return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, "PublishOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, string spName, params SqlParameter[] spParameters)
		{	
			try
            {
                return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
		{
			try
            {
                return SqlHelper.ExecuteNonQuery(connection, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, "PublishOrders");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName)
		{
			try
            {
                return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params SqlParameter[] spParameters)
		{
			try
            {
                return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			try
            {
                return SqlHelper.ExecuteNonQuery(transaction, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return 0;
		}


		#endregion ExecuteNonQuery

		#region ExecuteDataSet

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters).
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset("GetOrders");
		/// </remarks>		
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string spName)
		{
		    try
            {
                return SqlHelper.ExecuteDataset(GetConnectionString(), CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset("GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>		
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string spName, params SqlParameter[] spParameters)
		{
		    try
            {
                return SqlHelper.ExecuteDataset(GetConnectionString(), CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) using the provided parameter values.
        /// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset("GetOrders", 24, 36);
		/// </remarks>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string spName, params object[] parameterValues)
		{
		    try
            {
                return SqlHelper.ExecuteDataset(GetConnectionString(), spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, "GetOrders");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, string spName)
		{
		    try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}
		
		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, string spName, params SqlParameter[] spParameters)
		{
		    try
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}
		
		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
		{
            try
            {
                return SqlHelper.ExecuteDataset(connection, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, "GetOrders");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, string spName)
		{
            try
            {
                return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}
		
		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params SqlParameter[] spParameters)
		{
            try
            {
                return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}
		
		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
            try
            {
                return SqlHelper.ExecuteDataset(transaction, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		#endregion ExecuteDataSet
		
		#region ExecuteReader

        /// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters).
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader("GetOrders");
		/// </remarks>		
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string spName)
		{
		    try
            {
                return SqlHelper.ExecuteReader(GetConnectionString(), CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>		
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string spName, params SqlParameter[] spParameters)
		{
		    try
            {
                return SqlHelper.ExecuteReader(GetConnectionString(), CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) using the provided parameter values.
        /// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader("GetOrders", 24, 36);
		/// </remarks>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(string spName, params object[] parameterValues)
		{
		    try
            {
                return SqlHelper.ExecuteReader(GetConnectionString(), spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, "GetOrders");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName)
		{
		    try
            {
                return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params SqlParameter[] spParameters)
		{
		    try
            {
                return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
            try
            {
                return SqlHelper.ExecuteReader(connection, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(trans, "GetOrders");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName)
		{
            try
            {
                return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///   SqlDataReader dr = ExecuteReader(trans, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params SqlParameter[] spParameters)
		{
            try
            {
                return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  SqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a SqlDataReader containing the resultset generated by the command</returns>
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
            try
            {
                return SqlHelper.ExecuteReader(transaction, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		#endregion ExecuteReader

		#region ExecuteScalar
		
		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters). 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar("GetOrderCount");
		/// </remarks>		
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string spName)
		{
			try
            {
                return SqlHelper.ExecuteScalar(GetConnectionString(), CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar("GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>		
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string spName, params SqlParameter[] spParameters)
		{
			try
            {
                return SqlHelper.ExecuteScalar(GetConnectionString(), CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) using the provided parameter values.
        /// This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar("GetOrderCount", 24, 36);
		/// </remarks>		
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string spName, params object[] parameterValues)
		{
			try
            {
                return SqlHelper.ExecuteScalar(GetConnectionString(), spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, string spName)
		{
			try
            {
                return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, string spName, params SqlParameter[] spParameters)
		{
			try
            {
                return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
		{
			try
            {
                return SqlHelper.ExecuteScalar(connection, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, string spName)
		{
			try
            {
                return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, string spName, params SqlParameter[] spParameters)
		{
			try
            {
                return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			try
            {
                return SqlHelper.ExecuteScalar(transaction, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		#endregion ExecuteScalar	

		#region ExecuteXmlReader
        
        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters). 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders");
        /// </remarks>
        /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(string spName)
        {
            try
            {
                //create & open a SqlConnection, and dispose of it after we are done.
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    cn.Open();

                    //call the overload that takes a connection in place of the connection string
                    return ExecuteXmlReader(cn, spName);
                }                
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset).
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
        /// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(string spName, params SqlParameter[] spParameters)
        {
            try
            {
                //create & open a SqlConnection, and dispose of it after we are done.
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    cn.Open();

                    //call the overload that takes a connection in place of the connection string
                    return ExecuteXmlReader(cn, spName, spParameters);                    
                }
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(string spName, params object[] parameterValues)
        {
            try
            {
                //create & open a SqlConnection, and dispose of it after we are done.
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    cn.Open();

                    //call the overload that takes a connection in place of the connection string
                    return ExecuteXmlReader(cn, spName, parameterValues);
                }
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
        }
        
		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, "GetOrders");
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName)
		{
            try
            {
                return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
        /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params SqlParameter[] spParameters)
		{
			try
            {
                return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">a valid SqlConnection</param>
		/// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
            try
            {
                return SqlHelper.ExecuteXmlReader(connection, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, "GetOrders");
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName)
		{
			try
            {
                return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", new SqlParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
		/// <param name="spParameters">an array of SqlParamters used to execute the stored procedure</param>
		/// <returns>an XmlReader containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params SqlParameter[] spParameters)
		{
			try
            {
                return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">a valid SqlTransaction</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>a dataset containing the resultset generated by the command</returns>
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			try
            {
                return SqlHelper.ExecuteXmlReader(transaction, spName, parameterValues);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}


		#endregion ExecuteXmlReader
	}

	/// <summary>
	/// MHMSqlHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
	/// ability to discover parameters for stored procedures at run-time.
	/// </summary>
	public sealed class SPHelperParameterCache
	{
		#region private methods, variables, and constructors

		//Since this class provides only static methods, make the default constructor private to prevent 
		//instances from being created with "new SqlHelperParameterCache()".
		private SPHelperParameterCache() {}

        private static string GetConnectionString()
        {
           // return ConfigurationSettings.AppSettings["ConnectString"];  //Config.ConnectionString;
            return Config.ConnectionString;
        }

        private static void ThrowException(Exception ex)
        {
            CustomException customException = new CustomException("System error! Error in calling database functions. Exception Message: " + ex.Message);
            throw customException;   
        }
        		
		#endregion private methods, variables, and constructors

		#region caching functions

		/// <summary>
		/// add parameter array to the cache
		/// </summary>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">an array of SqlParamters to be cached</param>
		public static void CacheParameterSet(string commandText, params SqlParameter[] spParameters)
		{
            try
            {
                SqlHelperParameterCache.CacheParameterSet(GetConnectionString(), commandText, spParameters);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
		}

		/// <summary>
		/// retrieve a parameter array from the cache
		/// </summary>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="commandText">the stored procedure name or T-SQL command</param>
		/// <returns>an array of SqlParamters</returns>
		public static SqlParameter[] GetCachedParameterSet(string commandText)
		{
            try
            {
                return SqlHelperParameterCache.GetCachedParameterSet(GetConnectionString(), commandText);   
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		#endregion caching functions

		#region Parameter Discovery Functions

		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <returns>an array of SqlParameters</returns>
		public static SqlParameter[] GetSpParameterSet(string spName)
		{
            try
            {
                return SqlHelperParameterCache.GetSpParameterSet(GetConnectionString(), spName);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		/// <summary>
		/// Retrieves the set of SqlParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">a valid connection string for a SqlConnection</param>
		/// <param name="spName">the name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">a bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>an array of SqlParameters</returns>
		public static SqlParameter[] GetSpParameterSet(string spName, bool includeReturnValueParameter)
		{
            try
            {
                return SqlHelperParameterCache.GetSpParameterSet(GetConnectionString(), spName, includeReturnValueParameter);
            }
            catch (Exception ex)
            {
                ThrowException(ex);
            }
            return null;
		}

		#endregion Parameter Discovery Functions

	}
}