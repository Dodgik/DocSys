using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership.Data;

namespace PermissionMembership
{
    public class AccessRole
    {
        #region Role in Role

        /// <summary>
        /// Add Role in Role
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ContainedRoleName">Contained Role Name</param>
        public static void RoleInRoleInsert(SqlTransaction trans, string RoleName, string ContainedRoleName)
        {
            Guid RoleId = GetRoleId(trans, RoleName);
            Guid ContainedRoleId = GetRoleId(trans, ContainedRoleName);
            RoleInRoleInsert(trans, RoleId, ContainedRoleId);
        }

        /// <summary>
        /// Add Role in Role
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ContainedRoleId">Contained Role Id</param>
        public static void RoleInRoleInsert(SqlTransaction trans, Guid RoleId, Guid ContainedRoleId)
        {
            string spname = "usp_Access_RoleInRoleInsert";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ContainedRoleId", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ContainedRoleId;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Add Role in Role
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ContainedRoleName">Contained Role Name</param>
        public static void RoleInRoleInsert(string ConnectionString, string RoleName, string ContainedRoleName)
        {
            Guid RoleId = GetRoleId(ConnectionString, RoleName);
            Guid ContainedRoleId = GetRoleId(ConnectionString, ContainedRoleName);

            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    RoleInRoleInsert(trans, RoleId, ContainedRoleId);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Add Role in Role
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ContainedRoleId">Contained Role Id</param>
        public static void RoleInRoleInsert(string ConnectionString, Guid RoleId, Guid ContainedRoleId)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    RoleInRoleInsert(trans, RoleId, ContainedRoleId);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Delete Role in Role
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ContainedRoleName">Contained Role Name</param>
        public static void RoleInRoleDelete(SqlTransaction trans, string RoleName, string ContainedRoleName)
        {
            Guid RoleId = GetRoleId(trans, RoleName);
            Guid ContainedRoleId = GetRoleId(trans, ContainedRoleName);
            RoleInRoleDelete(trans, RoleId, ContainedRoleId);
        }

        /// <summary>
        /// Delete Role in Role
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ContainedRoleId">Contained Role Id</param>
        public static void RoleInRoleDelete(SqlTransaction trans, Guid RoleId, Guid ContainedRoleId)
        {
            string spname = "usp_Access_RoleInRoleDelete";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ContainedRoleId", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ContainedRoleId;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Delete Role in Role
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ContainedRoleName">Contained Role Name</param>
        public static void RoleInRoleDelete(string ConnectionString, string RoleName, string ContainedRoleName)
        {
            Guid RoleId = GetRoleId(ConnectionString, RoleName);
            Guid ContainedRoleId = GetRoleId(ConnectionString, ContainedRoleName);

            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    RoleInRoleDelete(trans, RoleId, ContainedRoleId);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Delete Role in Role
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ContainedRoleId">Contained Role Id</param>
        public static void RoleInRoleDelete(string ConnectionString, Guid RoleId, Guid ContainedRoleId)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    RoleInRoleDelete(trans, RoleId, ContainedRoleId);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        #endregion Role in Role

        #region User in Role for Object

        /// <summary>
        /// Add User in Role for Object
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectId">Object Id</param>
        public static void UserInRoleInsert(SqlTransaction trans, string RoleName, string UserName, Guid ObjectId)
        {
            Guid RoleId = GetRoleId(trans, RoleName);
            Guid UserId = AccessUser.GetUserId(trans, UserName);
            UserInRoleInsert(trans, RoleId, UserId, ObjectId);            
        }

        /// <summary>
        /// Add User in Role for Object
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectId">Object Id</param>
        public static void UserInRoleInsert(SqlTransaction trans, Guid RoleId, Guid UserId, Guid ObjectId)
        {
            string spname = "usp_Access_UserInRoleByObjectInsert";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
            mParams[1].Value = UserId;
            mParams[2] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[2].Value = ObjectId;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Add User in Role for Object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectId">Object Id</param>        
        public static void UserInRoleInsert(string ConnectionString, string RoleName, string UserName, Guid ObjectId)
        {
            Guid RoleId = GetRoleId(ConnectionString, RoleName);
            Guid UserId = AccessUser.GetUserId(ConnectionString, UserName);

            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    UserInRoleInsert(trans, RoleId, UserId, ObjectId);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Add User in Role for Object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="UserId">User Role Id</param>
        /// <param name="ObjectId">Object Id</param>        
        public static void UserInRoleInsert(string ConnectionString, Guid RoleId, Guid UserId, Guid ObjectId)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    UserInRoleInsert(trans, RoleId, UserId, ObjectId);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Delete User in Role for Object
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectId">Object Id</param>
        public static void UserInRoleDelete(SqlTransaction trans, string RoleName, string UserName, Guid ObjectId)
        {
            Guid RoleId = GetRoleId(trans, RoleName);
            Guid UserId = AccessUser.GetUserId(trans, UserName);
            UserInRoleDelete(trans, RoleId, UserId, ObjectId);
        }

        /// <summary>
        /// Delete User in Role for Object
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectId">Object Id</param>
        public static void UserInRoleDelete(SqlTransaction trans, Guid RoleId, Guid UserId, Guid ObjectId)
        {
            string spname = "usp_Access_UserInRoleByObjectDelete";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
            mParams[1].Value = UserId;            
            mParams[2] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[2].Value = ObjectId;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Delete User in Role for Object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectId">Object Id</param>        
        public static void UserInRoleDelete(string ConnectionString, Guid RoleId, Guid UserId, Guid ObjectId)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    UserInRoleDelete(trans, RoleId, UserId, ObjectId);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        /// <summary>
        /// Delete User in Role for Object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectId">Object Id</param>        
        public static void UserInRoleDelete(string ConnectionString, string RoleName, string UserName, Guid ObjectId)
        {
            Guid RoleId = GetRoleId(ConnectionString, RoleName);
            Guid UserId = AccessUser.GetUserId(ConnectionString, UserName);
            SqlConnection connection = new SqlConnection(ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    UserInRoleDelete(trans, RoleId, UserId, ObjectId);
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
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }


        /// <summary>
        /// Role for Object List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectId">Object Id</param>
        /// <returns></returns>
        public static DataView RoleByObjectList(SqlTransaction trans, Guid RoleId, Guid ObjectId)
        {
            string spname = "usp_Access_RoleByObjectList";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ObjectId;
            return SqlHelper.ExecuteDataset(trans, CommandType.StoredProcedure, spname, mParams).Tables[0].DefaultView;
        }

        /// <summary>
        /// Role for Object List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ObjectId">Object Id</param>
        /// <returns></returns>
        public static DataView RoleByObjectList(SqlTransaction trans, string RoleName, Guid ObjectId)
        {
            Guid RoleId = GetRoleId(trans, RoleName);
            return RoleByObjectList(trans, RoleId, ObjectId);
        }

        /// <summary>
        /// Role for Object List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectId">Object Id</param>        
        /// <returns></returns>
        public static DataView RoleByObjectList(string ConnectionString, Guid RoleId, Guid ObjectId)
        {
            string spname = "usp_Access_RoleByObjectList";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ObjectId;
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, spname, mParams).Tables[0].DefaultView;
        }

        /// <summary>
        /// Role for Object List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ObjectId">Object Id</param>        
        /// <returns></returns>
        public static DataView RoleByObjectList(string ConnectionString, string RoleName, Guid ObjectId)
        {
            Guid RoleId = GetRoleId(ConnectionString, RoleName);
            return RoleByObjectList(ConnectionString, RoleId, ObjectId);
        }     
        
        /// <summary>
        /// Get User Role for Object
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectId">Object Id</param>
        /// <returns></returns>
        public static Guid RoleByObjectGet(SqlTransaction trans, Guid RoleId, Guid ObjectId)
        {
            string spname = "usp_Access_RoleByObjectList";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ObjectId;
            mParams[2] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[2].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
            if (mParams[2].Value == DBNull.Value)
            {
                return Guid.Empty;
            }
            else
            {
                return (Guid)mParams[2].Value;
            }
        }

        /// <summary>
        /// Get User Role for Object
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ObjectId">Object Id</param>
        /// <returns></returns>
        public static Guid RoleByObjectGet(SqlTransaction trans, string RoleName, Guid ObjectId)
        {
            Guid RoleId = GetRoleId(trans, RoleName);
            return RoleByObjectGet(trans, RoleId, ObjectId);
        }

        /// <summary>
        /// Get User Role for Object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectId">Object Id</param>        
        /// <returns></returns>
        public static Guid RoleByObjectGet(string ConnectionString, Guid RoleId, Guid ObjectId)
        {
            string spname = "usp_Access_RoleByObjectList";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ObjectId;
            mParams[2] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[2].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, spname, mParams);
            if (mParams[2].Value == DBNull.Value)
            {
                return Guid.Empty;
            }
            else
            {
                return (Guid)mParams[2].Value;
            }
        }

        /// <summary>
        /// Get User Role for Object
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ObjectId">Object Id</param>        
        /// <returns></returns>
        public static Guid RoleByObjectGet(string ConnectionString, string RoleName, Guid ObjectId)
        {
            Guid RoleId = GetRoleId(ConnectionString, RoleName);
            return RoleByObjectGet(ConnectionString, RoleId, ObjectId);
        }

        #endregion User in Role for Object

        #region GetRoleId

        /// <summary>
        /// Get Role Id By RoleName
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <returns></returns>
        public static Guid GetRoleId(string ConnectionString, string RoleName)
        {
            string spname = "usp_Access_RoleIdByRoleName";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@RoleName", SqlDbType.NChar);
            mParams[0].Value = RoleName;
            mParams[1] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, spname, mParams);
            if (mParams[1].Value == DBNull.Value)
            {
                throw new PermissionMembershipException(String.Format("Role \"{0}\" not found", RoleName));
            }
            return (Guid)mParams[1].Value;
        }

        /// <summary>
        /// Get Role Id By RoleName
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="RoleName">Role Name</param>
        /// <returns></returns>
        public static Guid GetRoleId(SqlTransaction trans, string RoleName)
        {
            string spname = "usp_Access_RoleIdByRoleName";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@RoleName", SqlDbType.NChar);
            mParams[0].Value = RoleName;
            mParams[1] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
            if (mParams[1].Value == DBNull.Value)
            {
                throw new PermissionMembershipException(String.Format("Role \"{0}\" not found", RoleName));
            }
            return (Guid)mParams[1].Value;
        }        

        #endregion GetRoleId

        #region IsUserInRole

        /// <summary>
        /// Get true if User in Role
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="RoleName">Role Name</param>
        /// <returns></returns>
        public static bool IsUserInRole(string connectionString, string UserName, string RoleName)
        {
            Guid RoleId = AccessRole.GetRoleId(connectionString, RoleName);
            Guid UserId = AccessUser.GetUserId(connectionString, UserName);
            return IsUserInRole(connectionString, UserId, RoleId);
        }

        /// <summary>
        /// Get true if User in Role
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="RoleId">Role Id</param>
        /// <returns></returns>
        public static bool IsUserInRole(string connectionString, string UserName, Guid RoleId)
        {
            Guid UserId = AccessUser.GetUserId(connectionString, UserName);
            return IsUserInRole(connectionString, UserId, RoleId);
        }

        /// <summary>
        /// Get true if User in Role
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="RoleName">Role Name</param>
        /// <returns></returns>
        public static bool IsUserInRole(string connectionString, Guid UserId, string RoleName)
        {
            Guid RoleId = AccessRole.GetRoleId(connectionString, RoleName);
            return IsUserInRole(connectionString, UserId, RoleId);
        }        

        /// <summary>
        /// Get true if User in Role
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="RoleId">Role Id</param>
        /// <returns></returns>
        public static bool IsUserInRole(string connectionString, Guid UserId, Guid RoleId)
        {
            string spname = "usp_Access_UserInRole";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = UserId;
            mParams[1] = new SqlParameter("@RoleID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = RoleId;
            mParams[2] = new SqlParameter("@Exists", SqlDbType.Bit);
            mParams[2].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            return (bool)mParams[2].Value;
        }

        #endregion IsUserInRole
    }
}
