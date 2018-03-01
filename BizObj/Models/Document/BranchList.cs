using System;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class BranchList
    {
        private struct SpNames
        {
            public const string Get = "usp_BranchList_Get";
            public const string Insert = "usp_BranchList_Insert";
            public const string Update = "usp_BranchList_Update";
            public const string Delete = "usp_BranchList_Delete";
            public const string DeleteList = "usp_BranchList_DeleteList";
            public const string List = "usp_BranchList_List";
        }
        
        public const int ObjectTypeID = 24;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@BranchListID", SqlDbType.Int)]
        //[FieldAttribute("BranchListID")]
        public int ID { get; set; }

        //[ParamAttribute("@DocStatementID", SqlDbType.Int)]
        //[FieldAttribute("DocStatementID")]
        public int DocStatementID { get; set; }

        //[ParamAttribute("@BranchTypeID", SqlDbType.Int)]
        //[FieldAttribute("BranchTypeID")]
        public int BranchTypeID { get; set; }

        
        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public BranchList(string userName)
        {
            UserName = userName;
        }

        public BranchList(int id, string userName): this(null, id, userName)
        {

        }

        public BranchList(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int branchListId)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@BranchListID", SqlDbType.Int);
            prms[0].Value = branchListId;

            prms[1] = new SqlParameter("@DocStatementID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@BranchTypeID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = branchListId;
            DocStatementID = (int)prms[1].Value;
            BranchTypeID = (int)prms[2].Value;
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
            prms[0] = new SqlParameter("@BranchListID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocStatementID", SqlDbType.Int);
            prms[1].Value = DocStatementID;

            prms[2] = new SqlParameter("@BranchTypeID", SqlDbType.Int);
            prms[2].Value = BranchTypeID;

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
            if (!CanUpdate(UserName))
            {
                throw new AccessException(UserName, "Update");
            }

            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@BranchListID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@DocStatementID", SqlDbType.Int);
            prms[1].Value = DocStatementID;

            prms[2] = new SqlParameter("@BranchTypeID", SqlDbType.Int);
            prms[2].Value = BranchTypeID;

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
        public static DataTable GetList(SqlTransaction trans, int docStatementID)
        {
            if (trans == null)
            {
                return SPHelper.ExecuteDataset(SpNames.List, docStatementID).Tables[0];
            }
            return SPHelper.ExecuteDataset(trans, SpNames.List, docStatementID).Tables[0];
        }
        
        public static int[] GetBranchTypeIDList(SqlTransaction trans, int docStatementID)
        {
            DataTable dtBranchTypes = GetList(trans, docStatementID);

            int[] branchTypeIDList = new int[dtBranchTypes.Rows.Count];

            int i = 0;
            foreach (DataRow rowBranchType in dtBranchTypes.Rows)
            {
                branchTypeIDList[i] = (int)rowBranchType["BranchTypeID"];
                i++;
            }

            return branchTypeIDList;
        }

        public static void DeleteList(SqlTransaction trans, int docStatementID, string userName)
        {
            if (!CanDelete(userName))
            {
                throw new AccessException(userName, "Delete");
            }

            SPHelper.ExecuteNonQuery(trans, SpNames.DeleteList, docStatementID);
        }

        public static void Delete(SqlTransaction trans, int id, string userName)
        {
            if (!CanDelete(userName))
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
            
        public static bool CanInsert(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int)ActionType.Insert);
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