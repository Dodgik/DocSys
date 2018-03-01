using System;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Models
{
    public abstract class Access: IAccess
    {
        public abstract int ObjectTypeID { get; }

        protected int StateIDAll { 
            get
            {
                return ObjectTypeID * 1000 + 1;
            }
        }

        [ScriptIgnore]
        public string UserName { get; set; }

        [ScriptIgnore]
        public Guid UserId { get; set; }

        protected Access()
        {
            
        }
        protected Access(string userName)
        {
            UserName = userName;
        }
        protected Access(Guid userId)
        {
            UserId = userId;
        }

        public bool CanRead(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, ObjectTypeID * 1000 + 1);
        }
        public bool CanRead(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userId, ObjectTypeID, StateIDAll, ObjectTypeID * 1000 + 1);
        }
        public bool CanWrite(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, ObjectTypeID * 1000 + 2);
        }
        public bool CanWrite(Guid userId)
        {
            if (userId != Guid.Empty)
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userId, ObjectTypeID, StateIDAll, ObjectTypeID * 1000 + 2);
        }
    }
}
