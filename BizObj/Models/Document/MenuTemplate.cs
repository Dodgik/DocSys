using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class MenuTemplate
    {
        private struct SpNames
        {
            public const string Get = "usp_MenuTemplate_Get";
            public const string GetByObjectID = "usp_MenuTemplate_GetByObjectID";
            public const string Insert = "usp_MenuTemplate_Insert";
            public const string Update = "usp_MenuTemplate_Update";
            public const string Delete = "usp_MenuTemplate_Delete";
            public const string List = "usp_MenuTemplate_List";
        }

        public const int ObjectTypeID = 30;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        public enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@MenuTemplateID", SqlDbType.Int)]
        //[FieldAttribute("MenuTemplateID")]
        public int ID { get; set; }

        //[ParamAttribute("@Name", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("Name")]
        public string Name { get; set; }

        //[ParamAttribute("@SystemName", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("SystemName")]
        public string SystemName { get; set; }

        //[ParamAttribute("@ObjectID", SqlDbType.UniqueIdentifier)]
        //[FieldAttribute("ObjectID")]
        public Guid ObjectID { get; set; }


        private string UserName { get; set; }
        #endregion

        #region Constructors

        public MenuTemplate()
        {

        }

        public MenuTemplate(string userName)
        {
            UserName = userName;
        }

        public MenuTemplate(SqlTransaction trans, int id, string userName)
            : this(userName)
        {
            Init(trans, id);
        }

        public MenuTemplate(SqlTransaction trans, Guid objectID, string userName)
            : this(userName)
        {
            Init(trans, objectID);
        }

        public MenuTemplate(int id, string userName)
            : this(null, id, userName)
        {

        }

        public MenuTemplate(Guid objectID, string userName)
            : this(null, objectID, userName)
        {

        }

        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int menuTemplateId)
        {
            if (!CanView(UserName, ObjectID))
            {
                throw new AccessException(UserName, "Init");
            }

            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@MenuTemplateID", SqlDbType.Int);
            prms[0].Value = menuTemplateId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@SystemName", SqlDbType.NVarChar, 50);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = menuTemplateId;
            Name = (string)prms[1].Value;
            SystemName = (string)prms[2].Value;
            ObjectID = (Guid)prms[3].Value;
        }

        private void Init(SqlTransaction trans, Guid objectId)
        {
            if (!CanView(UserName, objectId))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@MenuTemplateID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@SystemName", SqlDbType.NVarChar, 50);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Value = objectId;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.GetByObjectID, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.GetByObjectID, prms);

            ID = (int)prms[0].Value;
            Name = (string)prms[1].Value;
            SystemName = (string)prms[2].Value;
            ObjectID = objectId;
        }
        #endregion

        #region Public Methods

        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName, ObjectID))
            {
                throw new AccessException(UserName, "Insert");
            }

            AccessObject accessObject = new AccessObject(trans.Connection.ConnectionString);
            accessObject.Id = Guid.NewGuid();
            accessObject.Name = Name;
            accessObject.ObjectTypeId = ObjectTypeID;
            accessObject.ObjectStateId = StateIDAll;
            ObjectID = accessObject.Insert(trans);

            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@MenuTemplateID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@SystemName", SqlDbType.NVarChar, 50);
            prms[2].Value = SystemName;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Value = ObjectID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Insert, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Insert, prms);

            ID = (int)prms[0].Value;

            return ID;
        }

        public int Insert()
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Insert(trans);

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
            return ID;

        }

        public void Update(SqlTransaction trans)
        {
            if (!CanUpdate(UserName, ObjectID))
            {
                throw new AccessException(UserName, "Update");
            }

            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@MenuTemplateID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@SystemName", SqlDbType.NVarChar, 50);
            prms[2].Value = SystemName;

            prms[3] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[3].Value = ObjectID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Update, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Update, prms);
        }

        public void Update()
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
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

        #endregion

        #region Static Public Methods

        public static List<MenuTemplate> GetList(SqlTransaction trans, string userName)
        {
            /*if (!CanView(userName))
            {
                throw new AccessException(userName, "Get list");
            }*/
            List<MenuTemplate> dl = new List<MenuTemplate>();
            DataTable dt;
            if (trans == null)
            {
                dt = SPHelper.ExecuteDataset(SpNames.List).Tables[0];
            }
            else
            {
                dt = SPHelper.ExecuteDataset(trans, SpNames.List).Tables[0];
            }

            foreach (DataRow dr in dt.Rows)
            {
                MenuTemplate menuTemplate = new MenuTemplate(userName);
                menuTemplate.ID = (int)dr["MenuTemplateID"];
                menuTemplate.Name = (string)dr["Name"];
                menuTemplate.SystemName = (string)dr["SystemName"];
                menuTemplate.ObjectID = (Guid)dr["ObjectID"];

                dl.Add(menuTemplate);
            }

            return dl;
        }

        public static List<MenuTemplate> GetList(string userName)
        {
            return GetList(null, userName);
        }

        public static void Delete(SqlTransaction trans, int id, string userName)
        {
            MenuTemplate dep = new MenuTemplate(trans, id, userName);
            if (!CanDelete(userName, dep.ObjectID))
            {
                throw new AccessException(userName, "Delete");
            }

            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, id);
        }

        public static void Delete(int id, string userName)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans, id, userName);

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

        public static bool CanInsert(string userName, Guid objectID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, objectID, (int)ActionType.Insert);
        }

        public static bool CanUpdate(string userName, Guid objectID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, objectID, (int)ActionType.Update);
        }

        public static bool CanDelete(string userName, Guid objectID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, objectID, (int)ActionType.Delete);
        }

        public static bool CanView(string userName, Guid objectID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, objectID, (int)ActionType.View);
        }
        #endregion
    }
}
