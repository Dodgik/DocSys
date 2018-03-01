using System;
using System.Data.SqlClient;
using BizObj.CustomException;

namespace BizObj.Models.Document
{
    public abstract class Component : Access, IComponent
    {
        protected Component()
        {
            
        }
        protected Component(string userName) : base(userName)
        {

        }
        protected Component(Guid userId) : base(userId)
        {

        }


        public virtual void Init(SqlTransaction trans, int id)
        {
            if (!CanRead(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
        }

        public virtual int Insert(SqlTransaction trans)
        {
            if (!CanWrite(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            return 0;
        }

        public virtual void Update(SqlTransaction trans)
        {
            if (!CanWrite(UserName))
            {
                throw new AccessException(UserName, "Update");
            }
        }

        public virtual void Delete(SqlTransaction trans)
        {
            if (!CanWrite(UserName))
            {
                throw new AccessException(UserName, "Delete");
            }
        }
    }
}
