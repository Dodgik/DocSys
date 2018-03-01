using System;
using System.Data.SqlClient;
using BizObj.Data;

namespace BizObj.Models.Document
{
    public abstract class ComponentData : Component
    {
        protected ComponentData() 
        {
            
        }
        protected ComponentData(string userName) : base(userName)
        {
            
        }
        protected ComponentData(Guid userId) : base(userId)
        {

        }
        
        public int Insert()
        {
            int id;
            var connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    id = Insert(trans);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
            return id;
        }

        public void Update()
        {
            var connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Update(trans);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public void Delete()
        {
            var connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans);

                    trans.Commit();
                }
                catch (Exception)
                {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
