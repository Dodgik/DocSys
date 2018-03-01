using System;
using System.Data;
using System.Data.SqlClient;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    public class Label
    {
        private struct SpNames
        {
            public const string Get = "usp_Label_Get";
            public const string Insert = "usp_Label_Insert";
            public const string Update = "usp_Label_Update";
            public const string Delete = "usp_Label_Delete";
            public const string List = "usp_Label_List";
            public const string Page = "usp_Label_Page";
        }
        
        #region Properties

        public int ID { get; set; }
        public string Name { get; set; }
        public int DepartmentID { get; set; }
        public int WorkerID { get; set; }
        
        private string UserName { get; set; }
        #endregion
        
        #region Constructors

        public Label()
        {
        }

        public Label(string userName)
        {
            UserName = userName;
        }

        public Label(int id, string userName): this(userName)
        {
            Init(null, id);
        }

        public Label(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
        }
        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int labelId)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@LabelID", SqlDbType.Int);
            prms[0].Value = labelId;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = labelId;
            Name = (string)prms[1].Value;
        }

        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@LabelID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Value = Name;

            prms[2] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[2].Value = DepartmentID;

            prms[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[3].Value = WorkerID;

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
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@LabelID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            prms[1].Value = Name;

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


        public static DataTable GetList(int departmentId)
        {
            return SPHelper.ExecuteDataset(SpNames.List, departmentId).Tables[0];
        }

        public static DataTable GetList()
        {
            return SPHelper.ExecuteDataset(SpNames.List, DBNull.Value).Tables[0];
        }

        public static SqlDataReader GetReader(SqlConnection conectionString, int departmentId)
        {
            return SPHelper.ExecuteReader(conectionString, SpNames.List, departmentId);
        }


        public static DataSet GetPage(SqlTransaction trans, PageSettings pageSettings)
        {
            SqlParameter[] sps = new SqlParameter[7];
            sps[0] = new SqlParameter("@PageIndex", SqlDbType.Int);
            sps[0].Value = pageSettings.PageIndex;

            sps[1] = new SqlParameter("@SortColumnName", SqlDbType.VarChar, 50);
            sps[1].Value = pageSettings.SortColumn;

            sps[2] = new SqlParameter("@SortOrderBy", SqlDbType.VarChar, 4);
            sps[2].Value = pageSettings.SortOrder;

            sps[3] = new SqlParameter("@NumberOfRows", SqlDbType.Int);
            sps[3].Value = pageSettings.PageSize;

            sps[4] = new SqlParameter("@TotalRecords", SqlDbType.Int);
            sps[4].Direction = ParameterDirection.Output;

            sps[5] = new SqlParameter("@Name", SqlDbType.NVarChar, 256);
            sps[5].Value = pageSettings.Where.HasRule("Name")
                                ? pageSettings.Where.GetRule("Name").Data
                                : String.Empty;

            sps[6] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            int departmentID;
            if (pageSettings.Where.HasRule("DepartmentID") && Int32.TryParse(pageSettings.Where.GetRule("DepartmentID").Data, out departmentID)) {
                sps[6].Value = departmentID;
            } else {
                sps[6].Value = DBNull.Value;
            }

            DataSet ds = SPHelper.ExecuteDataset(trans, SpNames.Page, sps);

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;

            return ds;
        }

        public static DataSet GetPage(PageSettings pageSettings)
        {
            DataSet ds;
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = GetPage(trans, pageSettings);

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

            return ds;
        }

        #endregion
        
        #region Static Public Methods
        
        public static void Delete(SqlTransaction trans, int id)
        {
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@LabelID", SqlDbType.Int);
            prms[0].Value = id;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Delete, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Delete, prms);
        }

        public static void Delete(int id)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans, id);

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
    }
}
