using System;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class SocialCategoryList
    {
        private struct SpNames
        {
            public const string Get = "usp_SocialCategoryList_Get";
            public const string Insert = "usp_SocialCategoryList_Insert";
            public const string Update = "usp_SocialCategoryList_Update";
            public const string Delete = "usp_SocialCategoryList_Delete";
            public const string List = "usp_SocialCategoryList_List";
            public const string DeleteList = "usp_SocialCategoryList_DeleteList";
        }

        public const int ObjectTypeID = 26;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@SocialCategoryListID", SqlDbType.Int)]
        //[FieldAttribute("SocialCategoryListID")]
        public int ID { get; set; }

        //[ParamAttribute("@CitizenID", SqlDbType.Int)]
        //[FieldAttribute("CitizenID")]
        public int CitizenID { get; set; }

        //[ParamAttribute("@SocialCategoryID", SqlDbType.Int)]
        //[FieldAttribute("SocialCategoryID")]
        public int SocialCategoryID { get; set; }

        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public SocialCategoryList()
        {
            
        }

        public SocialCategoryList(string userName)
        {
            UserName = userName;
        }

        public SocialCategoryList(int id, string userName): this(null, id, userName)
        {
            
        }

        public SocialCategoryList(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int socialCategoryListID)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@SocialCategoryListID", SqlDbType.Int);
            prms[0].Value = socialCategoryListID;

            prms[1] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@SocialCategoryID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = socialCategoryListID;
            CitizenID = (int) prms[1].Value;
            SocialCategoryID = (int) prms[2].Value;
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@SocialCategoryListID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[1].Value = CitizenID;

            prms[2] = new SqlParameter("@SocialCategoryID", SqlDbType.Int);
            prms[2].Value = SocialCategoryID;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Insert, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Insert, prms);

            ID = (int) prms[0].Value;

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
            if (!CanUpdate(UserName))
            {
                throw new AccessException(UserName, "Update");
            }

            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@SocialCategoryListID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[1].Value = CitizenID;

            prms[2] = new SqlParameter("@SocialCategoryID", SqlDbType.Int);
            prms[2].Value = SocialCategoryID;

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

        public static DataTable GetList(SqlTransaction trans, int citizenID)
        {
            if (trans == null) {
                return SPHelper.ExecuteDataset(SpNames.List, citizenID).Tables[0];
            } else {
                return SPHelper.ExecuteDataset(trans, SpNames.List, citizenID).Tables[0];
            }
        }

        public static int[] GetSocialCategoryIDList(SqlTransaction trans, int citizenID)
        {
            DataTable dtSocialCategoryList = GetList(trans, citizenID);

            int[] socialCategoryIDList = new int[dtSocialCategoryList.Rows.Count];

            int i = 0;
            foreach (DataRow rowSocialCategory in dtSocialCategoryList.Rows)
            {
                socialCategoryIDList[i] = (int) rowSocialCategory["SocialCategoryID"];
                i++;
            }

            return socialCategoryIDList;
        }

        public static void DeleteList(SqlTransaction trans, int citizenID, string userName)
        {
            if (!CanDelete(userName)) {
                throw new AccessException(userName, "Delete");
            }

            if (trans == null) {
                SPHelper.ExecuteNonQuery(SpNames.DeleteList, citizenID);
            } else {
                SPHelper.ExecuteNonQuery(trans, SpNames.DeleteList, citizenID);
            }
        }

        public static void Delete(SqlTransaction trans, int id, string userName)
        {
            if (!CanDelete(userName)) {
                throw new AccessException(userName, "Delete");
            }

            if (trans == null) {
                SPHelper.ExecuteNonQuery(SpNames.Delete, id);
            } else {
                SPHelper.ExecuteNonQuery(trans, SpNames.Delete, id);
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
            
        public static bool CanInsert(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Insert);
        }

        public static bool CanUpdate(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Update);
        }

        public static bool CanDelete(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.Delete);
        }

        public static bool CanView(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int) ActionType.View);
        }
        #endregion
        
    }
}
