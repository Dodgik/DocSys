using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership.Data;

namespace PermissionMembership
{
    public class AccessUser
    {

        #region Get User Id by User Name

        /// <summary>
        /// Get User Id by User Name
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <returns></returns>
        public static Guid GetUserId(string ConnectionString, string UserName)
        {
            string spname = "usp_Access_UserIdByUserName";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserName", SqlDbType.NChar);
            mParams[0].Value = UserName;
            mParams[1] = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, spname, mParams);
            if (mParams[1].Value == DBNull.Value)
            {
                throw new PermissionMembershipException(String.Format("User \"{0}\" not found", UserName));
            }            
            return (Guid)mParams[1].Value;
        }

        /// <summary>
        /// Get User Id by User Name
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserName">User Name</param>
        /// <returns></returns>
        public static Guid GetUserId(SqlTransaction trans, string UserName)
        {
            string spname = "usp_Access_UserIdByUserName";
            SqlParameter[] mParams = new SqlParameter[2];
            mParams[0] = new SqlParameter("@UserName", SqlDbType.NChar);
            mParams[0].Value = UserName;
            mParams[1] = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
            mParams[1].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, spname, mParams);
            if (mParams[1].Value == DBNull.Value)
            {
                throw new PermissionMembershipException(String.Format("User \"{0}\" not found", UserName));
            }
            return (Guid)mParams[1].Value;
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserId">User Id</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectTypeId">Object Type Id</param>        
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(SqlTransaction trans, Guid UserId, Guid RoleId, int ObjectTypeId)
        {
            string spname = "usp_Access_RoleAndObjectsByUserList";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = UserId;
            mParams[1] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[1].Value = RoleId == Guid.Empty ? DBNull.Value : (object)RoleId;
            mParams[2] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[2].Value = ObjectTypeId == Int32.MinValue ? DBNull.Value : (object)ObjectTypeId;
            return SqlHelper.ExecuteDataset(trans, CommandType.StoredProcedure, spname, mParams).Tables[0].DefaultView;
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserId">User Id</param>
        /// <param name="RoleId">Role Id</param>
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(SqlTransaction trans, Guid UserId, Guid RoleId)
        {
            return RoleAndObjectsByUserList(trans, UserId, RoleId, Int32.MinValue);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectTypeId">Object Type Id</param>        
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(SqlTransaction trans, Guid UserId, int ObjectTypeId)
        {
            return RoleAndObjectsByUserList(trans, UserId, Guid.Empty, ObjectTypeId);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserId">User Id</param>        
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(SqlTransaction trans, Guid UserId)
        {
            return RoleAndObjectsByUserList(trans, UserId, Guid.Empty, Int32.MinValue);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserName">User Name</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ObjectTypeId">Object Type Id</param>
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(SqlTransaction trans, string UserName, string RoleName, int ObjectTypeId)
        {
            Guid UserId = GetUserId(trans, UserName);
            Guid RoleId = RoleName == String.Empty ? Guid.Empty : AccessRole.GetRoleId(trans, RoleName);
            return RoleAndObjectsByUserList(trans, UserId, RoleId, ObjectTypeId);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserName">User Name</param>
        /// <param name="RoleName">Role Name</param>
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(SqlTransaction trans, string UserName, string RoleName)
        {
            return RoleAndObjectsByUserList(trans, UserName, RoleName, Int32.MinValue);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectTypeId">Object Type Id</param>
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(SqlTransaction trans, string UserName, int ObjectTypeId)
        {
            return RoleAndObjectsByUserList(trans, UserName, String.Empty, ObjectTypeId);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="trans">Transaction object</param>
        /// <param name="UserName">User Name</param>        
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(SqlTransaction trans, string UserName)
        {
            return RoleAndObjectsByUserList(trans, UserName, String.Empty, Int32.MinValue);
        }
        
        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectTypeId">Object Type Id</param>        
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(string ConnectionString, Guid UserId, Guid RoleId, int ObjectTypeId)
        {
            string spname = "usp_Access_RoleAndObjectsByUserList";
            SqlParameter[] mParams = new SqlParameter[3];
            mParams[0] = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
            mParams[0].Value = UserId;
            mParams[1] = new SqlParameter("@RoleId", SqlDbType.UniqueIdentifier);
            mParams[1].Value = RoleId == Guid.Empty ? DBNull.Value : (object)RoleId;
            mParams[2] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[2].Value = ObjectTypeId == Int32.MinValue ? DBNull.Value : (object)ObjectTypeId;
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, spname, mParams).Tables[0].DefaultView;
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="RoleId">Role Id</param>
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(string ConnectionString, Guid UserId, Guid RoleId)
        {
            return RoleAndObjectsByUserList(ConnectionString, UserId, RoleId, Int32.MinValue);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectTypeId">Object Type Id</param>        
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(string ConnectionString, Guid UserId, int ObjectTypeId)
        {
            return RoleAndObjectsByUserList(ConnectionString, UserId, Guid.Empty, ObjectTypeId);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>        
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(string ConnectionString, Guid UserId)
        {
            return RoleAndObjectsByUserList(ConnectionString, UserId, Guid.Empty, Int32.MinValue);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ObjectTypeId">Object Type Id</param>
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(string ConnectionString, string UserName, string RoleName, int ObjectTypeId)
        {
            Guid UserId = GetUserId(ConnectionString, UserName);
            Guid RoleId = RoleName == String.Empty ? Guid.Empty : AccessRole.GetRoleId(ConnectionString, RoleName);
            return RoleAndObjectsByUserList(ConnectionString, UserId, RoleId, ObjectTypeId);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="RoleName">Role Name</param>
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(string ConnectionString, string UserName, string RoleName)
        {
            return RoleAndObjectsByUserList(ConnectionString, UserName, RoleName, Int32.MinValue);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectTypeId">Object Type Id</param>
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(string ConnectionString, string UserName, int ObjectTypeId)
        {
            return RoleAndObjectsByUserList(ConnectionString, UserName, String.Empty, ObjectTypeId);
        }

        /// <summary>
        /// Objects for Role & User List
        /// </summary>
        /// <param name="ConnectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>        
        /// <returns></returns>
        public static DataView RoleAndObjectsByUserList(string ConnectionString, string UserName)
        {
            return RoleAndObjectsByUserList(ConnectionString, UserName, String.Empty, Int32.MinValue);
        }

        #endregion Get User Id by User Name
    }
}
