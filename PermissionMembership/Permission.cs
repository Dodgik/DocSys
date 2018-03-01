using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using PermissionMembership.Data;

namespace PermissionMembership
{
    public class Permission
    {
        #region Get Permission
        /// <summary>
        /// Get True if role have permission to action Type for object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to DB</param>
        /// <param name="roleName">Role Name</param>
        /// <param name="objectId">Object Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if role have permission to action Type for object. Else returt false</returns>
        public static bool IsRolePermission(string connectionString, string roleName, Guid objectId, int actionTypeId)
        {
            string spname = "usp_Access_IsRolePermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@RoleName", SqlDbType.NVarChar, 256);
            mParams[0].Value = roleName;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = objectId;
            mParams[2] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[2].Value = actionTypeId;
            mParams[3] = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 256);
            mParams[3].Direction = ParameterDirection.Output;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (((string)mParams[3].Value) != String.Empty)
            {
                ThrowException((string)mParams[3].Value);
            }
            return (bool)mParams[4].Value;
        }

        /// <summary>
        /// Get True if role have permission to action Type for object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to DB</param>
        /// <param name="roleList">Role List</param>
        /// <param name="objectId">Object Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if role have permission to action Type for object. Else returt false</returns>
        public static bool IsRolePermission(string connectionString, List<string> roleList, Guid objectId, int actionTypeId)
        {
            foreach (string roleName in roleList)
            {
                if (IsRolePermission(connectionString, roleName, objectId, actionTypeId))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get True if role have permission to action Type for object type and state id
        /// </summary>
        /// <param name="connectionString">Connection string for connect to DB</param>
        /// <param name="roleName">Role Name</param>
        /// <param name="objectTypeId">Object Type Id</param>
        /// <param name="stateId">State Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if role have permission to action Type for object. Else returt false</returns>
        public static bool IsRolePermission(string connectionString, string roleName, int objectTypeId, int stateId, int actionTypeId)
        {
            string spname = "usp_Access_IsRoleToObjectTypePermission";
            SqlParameter[] mParams = new SqlParameter[6];
            mParams[0] = new SqlParameter("@RoleName", SqlDbType.NVarChar, 256);
            mParams[0].Value = roleName;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = objectTypeId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = stateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = actionTypeId;
            mParams[4] = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 256);
            mParams[4].Direction = ParameterDirection.Output;
            mParams[5] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[5].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (((string)mParams[4].Value) != String.Empty)
            {
                ThrowException((string)mParams[4].Value);
            }
            return (bool)mParams[5].Value;
        }

        /// <summary>
        /// Get True if role have permission to action Type for object type and state id
        /// </summary>
        /// <param name="connectionString">Connection string for connect to DB</param>
        /// <param name="roleList">Role List</param>
        /// <param name="objectTypeId">Object Type Id</param>
        /// <param name="stateId">State Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if role have permission to action Type for object. Else returt false</returns>
        public static bool IsRolePermission(string connectionString, List<string> roleList, int objectTypeId, int stateId, int actionTypeId)
        {
            foreach (string roleName in roleList)
            {
                if (IsRolePermission(connectionString, roleName, objectTypeId, stateId, actionTypeId))
                {
                    return true;
                }
            }
            return false;
        }        

        /// <summary>
        /// Get True if user have permission to action Type for object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to DB</param>
        /// <param name="userId">User Id</param>
        /// <param name="objectId">Object Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if user have permission to action Type for object. Else returt false</returns>
        public static bool IsUserPermission(string connectionString, Guid userId, Guid objectId, int actionTypeId)
        {
            string spname = "usp_Access_IsUserPermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = userId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = objectId;
            mParams[2] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[2].Value = actionTypeId;
            mParams[3] = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 256);
            mParams[3].Direction = ParameterDirection.Output;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (((string)mParams[3].Value) != String.Empty)
            {
                ThrowException((string)mParams[3].Value);
            }
            return (bool)mParams[4].Value;
        }

        /// <summary>
        /// Get True if user have permission to action Type for object
        /// </summary>
        /// <param name="transaction">Sql transaction to DB</param>
        /// <param name="userId">User Id</param>
        /// <param name="objectId">Object Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if user have permission to action Type for object. Else returt false</returns>
        public static bool IsUserPermission(SqlTransaction transaction, Guid userId, Guid objectId, int actionTypeId)
        {
            string spname = "usp_Access_IsUserPermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = userId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = objectId;
            mParams[2] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[2].Value = actionTypeId;
            mParams[3] = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 256);
            mParams[3].Direction = ParameterDirection.Output;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spname, mParams);
            if (((string)mParams[3].Value) != String.Empty)
            {
                ThrowException((string)mParams[3].Value);
            }
            return (bool)mParams[4].Value;
        }

        /// <summary>
        /// Get True if user have permission to action Type for object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to DB</param>
        /// <param name="userName">User Name</param>
        /// <param name="objectId">Object Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if user have permission to action Type for object. Else returt false</returns>
        public static bool IsUserPermission(string connectionString, string userName, Guid objectId, int actionTypeId)
        {
            string spname = "usp_Access_IsUserPermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 256);
            mParams[0].Value = userName;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = objectId;
            mParams[2] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[2].Value = actionTypeId;
            mParams[3] = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 256);
            mParams[3].Direction = ParameterDirection.Output;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (((string)mParams[3].Value) != String.Empty)
            {
                ThrowException((string)mParams[3].Value);
            }
            return (bool)mParams[4].Value;
        }

        /// <summary>
        /// Get True if user have permission to action Type for object
        /// </summary>
        /// <param name="transaction">Sql transaction to DB</param>
        /// <param name="userName">User Name</param>
        /// <param name="objectId">Object Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if user have permission to action Type for object. Else returt false</returns>
        public static bool IsUserPermission(SqlTransaction transaction, string userName, Guid objectId, int actionTypeId)
        {
            string spname = "usp_Access_IsUserPermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 256);
            mParams[0].Value = userName;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = objectId;
            mParams[2] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[2].Value = actionTypeId;
            mParams[3] = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 256);
            mParams[3].Direction = ParameterDirection.Output;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spname, mParams);
            if (((string)mParams[3].Value) != String.Empty)
            {
                ThrowException((string)mParams[3].Value);
            }
            return (bool)mParams[4].Value;
        }

        /// <summary>
        /// Get True if user have permission to action Type for object type and state id
        /// </summary>
        /// <param name="connectionString">Connection string for connect to DB</param>
        /// <param name="userId">User Id</param>
        /// <param name="objectTypeId">Object Type Id</param>
        /// <param name="stateId">State Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if user have permission to action Type for object. Else returt false</returns>
        public static bool IsUserPermission(string connectionString, Guid userId, int objectTypeId, int stateId, int actionTypeId)
        {
            string spname = "usp_Access_IsUserToObjectTypePermission";
            SqlParameter[] mParams = new SqlParameter[6];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = userId;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = objectTypeId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = stateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = actionTypeId;
            mParams[4] = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 256);
            mParams[4].Direction = ParameterDirection.Output;
            mParams[5] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[5].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (((string)mParams[4].Value) != String.Empty)
            {
                ThrowException((string)mParams[4].Value);
            }
            return (bool)mParams[5].Value;
        }

        /// <summary>
        /// Get True if user have permission to action Type for object type and state id
        /// </summary>
        /// <param name="connectionString">Connection string for connect to DB</param>
        /// <param name="userName">User Name</param>
        /// <param name="objectTypeId">Object Type Id</param>
        /// <param name="stateId">State Id</param>
        /// <param name="actionTypeId">Action Id</param>
        /// <returns>Return true if user have permission to action Type for object. Else returt false</returns>
        public static bool IsUserPermission(string connectionString, string userName, int objectTypeId, int stateId, int actionTypeId)
        {
            string spname = "usp_Access_IsUserToObjectTypePermission";
            SqlParameter[] mParams = new SqlParameter[6];
            mParams[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 256);
            mParams[0].Value = userName;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = objectTypeId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = stateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = actionTypeId;
            mParams[4] = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 256);
            mParams[4].Direction = ParameterDirection.Output;
            mParams[5] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[5].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (((string)mParams[4].Value) != String.Empty)
            {
                ThrowException((string)mParams[4].Value);
            }
            return (bool)mParams[5].Value;
        }

        #endregion Get Permission

        #region Set Permission

        /// <summary>
        /// Set Permission Role to object 
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="Rolename">Role Name</param>
        /// <param name="ObjectId">Object Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void SetRoleToObjectPermission(string connectionString, string RoleName, Guid ObjectId, int StateId, int ActionTypeId)
        {
            Guid RoleId = AccessRole.GetRoleId(connectionString, RoleName);
            SetRoleToObjectPermission(connectionString, RoleId, ObjectId, StateId, ActionTypeId);
        }

        /// <summary>
        /// Set Permission Role to object 
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectId">Object Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void SetRoleToObjectPermission(string connectionString, Guid RoleId, Guid ObjectId, int StateId, int ActionTypeId)
        {
            string spname = "usp_Access_SetRolePermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@RoleID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ObjectId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = StateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = ActionTypeId;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (!(bool)mParams[4].Value)
            {
                ThrowException(String.Format("Setting role error. RoleId: {0}, ObjectId: {1}, StateId: {2}, ActionTypeId: {3}", RoleId.ToString(), ObjectId.ToString(), StateId.ToString(), ActionTypeId.ToString()));
            }            
        }

        /// <summary>
        /// Remove Permission Role to object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="Rolename">Role Name</param>
        /// <param name="ObjectId">Object Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void RemoveRoleToObjectPermission(string connectionString, string RoleName, Guid ObjectId, int StateId, int ActionTypeId)
        {
            Guid RoleId = AccessRole.GetRoleId(connectionString, RoleName);
            RemoveRoleToObjectPermission(connectionString, RoleId, ObjectId, StateId, ActionTypeId);            
        }

        /// <summary>
        /// Remove Permission Role to object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectId">Object Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void RemoveRoleToObjectPermission(string connectionString, Guid RoleId, Guid ObjectId, int StateId, int ActionTypeId)
        {
            string spname = "usp_Access_RemoveRolePermission";
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@RoleID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ObjectId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = StateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = ActionTypeId;            
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);            
        }


        /// <summary>
        /// Set Permission Role to object type
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ObjectTypeId">Object type Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void SetRoleToObjectTypePermission(string connectionString, string RoleName, int ObjectTypeId, int StateId, int ActionTypeId)
        {
            Guid RoleId = AccessRole.GetRoleId(connectionString, RoleName);
            SetRoleToObjectTypePermission(connectionString, RoleId, ObjectTypeId, StateId, ActionTypeId);
        }

        /// <summary>
        /// Set Permission Role to object type
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectTypeId">Object type Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void SetRoleToObjectTypePermission(string connectionString, Guid RoleId, int ObjectTypeId, int StateId, int ActionTypeId)
        {
            string spname = "usp_Access_SetRoleObjectTypePermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@RoleID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = ObjectTypeId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = StateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = ActionTypeId;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (!(bool)mParams[4].Value)
            {
                ThrowException(String.Format("Setting role error. RoleId: {0}, ObjectTypeId: {1}, StateId: {2}, ActionTypeId: {3}", RoleId.ToString(), ObjectTypeId.ToString(), StateId.ToString(), ActionTypeId.ToString()));
            }
        }

        /// <summary>
        /// Remove Permission Role to object type
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="RoleName">Role Name</param>
        /// <param name="ObjectTypeId">Object type Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void RemoveRoleToObjectTypePermission(string connectionString, string RoleName, int ObjectTypeId, int StateId, int ActionTypeId)
        {
            Guid RoleId = AccessRole.GetRoleId(connectionString, RoleName);
            RemoveRoleToObjectTypePermission(connectionString, RoleId, ObjectTypeId, StateId, ActionTypeId);            
        }

        /// <summary>
        /// Remove Permission Role to object type
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="RoleId">Role Id</param>
        /// <param name="ObjectTypeId">Object type Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void RemoveRoleToObjectTypePermission(string connectionString, Guid RoleId, int ObjectTypeId, int StateId, int ActionTypeId)
        {
            string spname = "usp_Access_RemoveRoleObjectTypePermission";
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@RoleID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = RoleId;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = ObjectTypeId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = StateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = ActionTypeId;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Set Permission User to object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectId">Object Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void SetUserToObjectPermission(string connectionString, string UserName, Guid ObjectId, int StateId, int ActionTypeId)
        {
            Guid UserId = AccessUser.GetUserId(connectionString, UserName);
            SetUserToObjectPermission(connectionString, UserId, ObjectId, StateId, ActionTypeId);
        }

        /// <summary>
        /// Set Permission User to object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectId">Object Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void SetUserToObjectPermission(string connectionString, Guid UserId, Guid ObjectId, int StateId, int ActionTypeId)
        {
            string spname = "usp_Access_SetUserPermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = UserId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ObjectId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = StateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = ActionTypeId;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (!(bool)mParams[4].Value)
            {
                ThrowException(String.Format("Setting role error. UserId: {0}, ObjectId: {1}, StateId: {2}, ActionTypeId: {3}", UserId.ToString(), ObjectId.ToString(), StateId.ToString(), ActionTypeId.ToString()));
            }
        }

        /// <summary>
        /// Remove Permission User to object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectId">Object Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void RemoveUserToObjectPermission(string connectionString, string UserName, Guid ObjectId, int StateId, int ActionTypeId)
        {
            Guid UserId = AccessUser.GetUserId(connectionString, UserName);
            RemoveUserToObjectPermission(connectionString, UserId, ObjectId, StateId, ActionTypeId);
        }

        /// <summary>
        /// Remove Permission User to object
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectId">Object Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void RemoveUserToObjectPermission(string connectionString, Guid UserId, Guid ObjectId, int StateId, int ActionTypeId)
        {
            string spname = "usp_Access_RemoveUserPermission";
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = UserId;
            mParams[1] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            mParams[1].Value = ObjectId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = StateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = ActionTypeId;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
        }

        /// <summary>
        /// Set Permission User to object type
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectTypeId">Object type Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void SetUserToObjectTypePermission(string connectionString, string UserName, int ObjectTypeId, int StateId, int ActionTypeId)
        {
            Guid UserId = AccessUser.GetUserId(connectionString, UserName);
            SetUserToObjectTypePermission(connectionString, UserId, ObjectTypeId, StateId, ActionTypeId);
        }

        /// <summary>
        /// Set Permission User to object type
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectTypeId">Object type Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void SetUserToObjectTypePermission(string connectionString, Guid UserId, int ObjectTypeId, int StateId, int ActionTypeId)
        {
            string spname = "usp_Access_SetUserObjectTypePermission";
            SqlParameter[] mParams = new SqlParameter[5];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = UserId;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = ObjectTypeId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = StateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = ActionTypeId;
            mParams[4] = new SqlParameter("@IsPermission", SqlDbType.Bit);
            mParams[4].Direction = ParameterDirection.Output;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
            if (!(bool)mParams[4].Value)
            {
                ThrowException(String.Format("Setting role error. UserId: {0}, ObjectTypeId: {1}, StateId: {2}, ActionTypeId: {3}", UserId.ToString(), ObjectTypeId.ToString(), StateId.ToString(), ActionTypeId.ToString()));
            }
        }

        /// <summary>
        /// Remove Permission User to object type
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="UserName">User Name</param>
        /// <param name="ObjectTypeId">Object type Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void RemoveUserToObjectTypePermission(string connectionString, string UserName, int ObjectTypeId, int StateId, int ActionTypeId)
        {
            Guid UserId = AccessUser.GetUserId(connectionString, UserName);
            RemoveUserToObjectTypePermission(connectionString, UserName, ObjectTypeId, StateId, ActionTypeId);
        }

        /// <summary>
        /// Remove Permission User to object type
        /// </summary>
        /// <param name="connectionString">Connection string for connect to data base</param>
        /// <param name="UserId">User Id</param>
        /// <param name="ObjectTypeId">Object type Id</param>
        /// <param name="StateId">State Id</param>
        /// <param name="ActionTypeId">Action Type id</param>
        public static void RemoveUserToObjectTypePermission(string connectionString, Guid UserId, int ObjectTypeId, int StateId, int ActionTypeId)
        {
            string spname = "usp_Access_RemoveUserObjectTypePermission";
            SqlParameter[] mParams = new SqlParameter[4];
            mParams[0] = new SqlParameter("@UserID", SqlDbType.UniqueIdentifier);
            mParams[0].Value = UserId;
            mParams[1] = new SqlParameter("@ObjectTypeID", SqlDbType.Int);
            mParams[1].Value = ObjectTypeId;
            mParams[2] = new SqlParameter("@StateID", SqlDbType.Int);
            mParams[2].Value = StateId;
            mParams[3] = new SqlParameter("@ActionTypeID", SqlDbType.Int);
            mParams[3].Value = ActionTypeId;
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spname, mParams);
        }

        #endregion Set Permission

        #region Private Static Methods

        private static void ThrowException(string errorMessage)
        {
            throw new PermissionMembershipException(errorMessage);
        }

        #endregion Private Static Methods
    }
}
