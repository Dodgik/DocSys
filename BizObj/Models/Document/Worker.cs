using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Document;
using BizObj.Models.JqGrid;
using BizObj.Models.Pager;

namespace BizObj.Document
{
    [Serializable]
    public sealed class Worker : ComponentData
    {
        [ScriptIgnore]
        public override int ObjectTypeID
        {
            get { return 12; }
        }

        private struct SpNames
        {
            public const string Get = "usp_Worker_Get";
            public const string GetByUserId = "usp_Worker_GetByUserId";
            public const string Insert = "usp_Worker_Insert";
            public const string Update = "usp_Worker_Update";
            public const string Delete = "usp_Worker_Delete";
            public const string List = "usp_Worker_List";
            public const string ListByPost = "usp_Worker_ListByPostID";
        }
        
        #region Properties

        //[ParamAttribute("@WorkerID", SqlDbType.Int)]
        //[FieldAttribute("WorkerID")]
        public int ID { get; set; }

        //[ParamAttribute("@FirstName", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("FirstName")]
        public string FirstName { get; set; }

        //[ParamAttribute("@MiddleName", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("MiddleName")]
        public string MiddleName { get; set; }

        //[ParamAttribute("@LastName", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("LastName")]
        public string LastName { get; set; }

        //[ParamAttribute("@IsDismissed", SqlDbType.Bit)]
        //[FieldAttribute("IsDismissed")]
        public bool IsDismissed { get; set; }

        //[ParamAttribute("@PostID", SqlDbType.Int)]
        //[FieldAttribute("PostID")]
        public int PostID { get; set; }

        public string PostName { get; set; }

        public int DepartmentID { get; set; }

        public string DepartmentName { get; set; }
        #endregion
        
        #region Constructors

        public Worker()
        {

        }

        public Worker(string userName): base(userName)
        {

        }

        public Worker(int workerId, string userName): this(null, workerId, userName)
        {

        }

        public Worker(SqlTransaction trans, int workerId, string userName): this(userName)
        {
            Init(trans, workerId);
        }
        public Worker(SqlTransaction trans, Guid userId)
        {
            InitByUserId(trans, userId);
        }
        #endregion

        #region Private Methods

        public override void Init(SqlTransaction trans, int workerId)
        {
            base.Init(trans, workerId);
            
            SqlParameter[] sps = new SqlParameter[9];
            sps[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[0].Value = workerId;

            sps[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            sps[1].Direction = ParameterDirection.Output;

            sps[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            sps[2].Direction = ParameterDirection.Output;

            sps[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            sps[3].Direction = ParameterDirection.Output;

            sps[4] = new SqlParameter("@IsDismissed", SqlDbType.Bit);
            sps[4].Direction = ParameterDirection.Output;

            sps[5] = new SqlParameter("@PostID", SqlDbType.Int);
            sps[5].Direction = ParameterDirection.Output;

            sps[6] = new SqlParameter("@PostName", SqlDbType.NVarChar, 256);
            sps[6].Direction = ParameterDirection.Output;

            sps[7] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[7].Direction = ParameterDirection.Output;

            sps[8] = new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 100);
            sps[8].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, sps);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, sps);

            if (sps[1].Value != DBNull.Value)
            {
                ID = workerId;
                FirstName = (string) sps[1].Value;
                MiddleName = (string) sps[2].Value;
                LastName = (string) sps[3].Value;
                IsDismissed = (bool)sps[4].Value;
                PostID = (int)sps[5].Value;
                PostName = (string)sps[6].Value;
                DepartmentID = (int)sps[7].Value;
                DepartmentName = (string)sps[8].Value;
            }
            else
            {
                throw new NotFoundException("Worker", workerId.ToString());
            }
        }

        public void InitByUserId(SqlTransaction trans, Guid userId)
        {
            SqlParameter[] sps = new SqlParameter[10];
            sps[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[0].Direction = ParameterDirection.Output;

            sps[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            sps[1].Direction = ParameterDirection.Output;

            sps[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            sps[2].Direction = ParameterDirection.Output;

            sps[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            sps[3].Direction = ParameterDirection.Output;
            
            sps[4] = new SqlParameter("@IsDismissed", SqlDbType.Bit);
            sps[4].Direction = ParameterDirection.Output;

            sps[5] = new SqlParameter("@PostID", SqlDbType.Int);
            sps[5].Direction = ParameterDirection.Output;

            sps[6] = new SqlParameter("@PostName", SqlDbType.NVarChar, 256);
            sps[6].Direction = ParameterDirection.Output;

            sps[7] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[7].Direction = ParameterDirection.Output;

            sps[8] = new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 100);
            sps[8].Direction = ParameterDirection.Output;

            sps[9] = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier);
            sps[9].Value = userId;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.GetByUserId, sps);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.GetByUserId, sps);

            if (sps[1].Value != DBNull.Value)
            {
                ID = (int)sps[0].Value;
                FirstName = (string)sps[1].Value;
                MiddleName = (string)sps[2].Value;
                LastName = (string)sps[3].Value;
                IsDismissed = (bool)sps[4].Value;
                PostID = (int)sps[5].Value;
                PostName = (string)sps[6].Value;
                DepartmentID = (int)sps[7].Value;
                DepartmentName = (string)sps[8].Value;
            }
        }
        #endregion


        #region Public Methods
        public override int Insert(SqlTransaction trans)
        {
            base.Insert(trans);
            
            SqlParameter[] prms = new SqlParameter[6];
            prms[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            prms[1].Value = FirstName;

            prms[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            prms[2].Value = MiddleName;

            prms[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            prms[3].Value = LastName;

            prms[4] = new SqlParameter("@PostID", SqlDbType.Int);
            prms[4].Value = PostID;

            prms[5] = new SqlParameter("@IsDismissed", SqlDbType.Bit);
            prms[5].Value = IsDismissed;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Insert, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Insert, prms);

            ID = (int)prms[0].Value;
            
            return ID;
        }
        
        public override void Update(SqlTransaction trans)
        {
            base.Update(trans);

            SqlParameter[] prms = new SqlParameter[6];
            prms[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            prms[1].Value = FirstName;

            prms[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            prms[2].Value = MiddleName;

            prms[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            prms[3].Value = LastName;

            prms[4] = new SqlParameter("@PostID", SqlDbType.Int);
            prms[4].Value = PostID;

            prms[5] = new SqlParameter("@IsDismissed", SqlDbType.Bit);
            prms[5].Value = IsDismissed;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Update, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Update, prms);
        }
        #endregion


        #region Static Public Methods

        public static SqlDataReader GetReader(int departmentID)
        {
            SqlParameter[] sps = new SqlParameter[1];
            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            return SPHelper.ExecuteReader(SpNames.List, sps);
        }

        public static DataTable GetAll(int departmentID, int postID)
        {
            SqlParameter[] sps = new SqlParameter[2];
            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@PostID", SqlDbType.Int);
            sps[1].Value = postID;

            return SPHelper.ExecuteDataset(SpNames.ListByPost, sps).Tables[0];
        }

        public static DataSet Search(SqlTransaction trans, int? departmentId, string lastName)
        {
            SqlParameter[] sps = new SqlParameter[4];
            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            if (departmentId != null)
                sps[0].Value = departmentId;
            else
                sps[0].Value = DBNull.Value;

            sps[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            sps[1].Value = String.Empty;

            sps[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            sps[2].Value = String.Empty;

            sps[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            sps[3].Value = lastName;

            return SPHelper.ExecuteDataset(trans, "usp_Worker_Search", sps);
        }

        public static DataSet Search(int? departmentId, string lastName)
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

                    ds = Search(trans, departmentId, lastName);

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

        public static Worker GetByLastName(SqlTransaction trans, string lastName, string userName)
        {
            Worker worker = null;

            SqlParameter[] sps = new SqlParameter[9];
            sps[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[0].Direction = ParameterDirection.Output;

            sps[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            sps[1].Direction = ParameterDirection.Output;

            sps[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            sps[2].Direction = ParameterDirection.Output;

            sps[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            sps[3].Direction = ParameterDirection.InputOutput;
            sps[3].Value = lastName;

            sps[4] = new SqlParameter("@IsDismissed", SqlDbType.Bit);
            sps[4].Direction = ParameterDirection.Output;

            sps[5] = new SqlParameter("@PostID", SqlDbType.Int);
            sps[5].Direction = ParameterDirection.Output;

            sps[6] = new SqlParameter("@PostName", SqlDbType.NVarChar, 256);
            sps[6].Direction = ParameterDirection.Output;

            sps[7] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[7].Direction = ParameterDirection.Output;

            sps[8] = new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 100);
            sps[8].Direction = ParameterDirection.Output;
            
            SPHelper.ExecuteNonQuery(trans, "usp_Worker_GetByLastName", sps);

            if (sps[0].Value != DBNull.Value)
            {
                worker = new Worker(userName);
                worker.ID = (int) sps[0].Value;
                worker.FirstName = (string) sps[1].Value;
                worker.MiddleName = (string) sps[2].Value;
                worker.LastName = (string) sps[3].Value;
                worker.IsDismissed = (bool)sps[4].Value;
                worker.PostID = (int)sps[5].Value;
                worker.PostName = (string)sps[6].Value;
                worker.DepartmentID = (int)sps[7].Value;
                worker.DepartmentName = (string)sps[8].Value;
            }
            return worker;
        }

        public static DataTable GetPage(SqlTransaction trans, PageSettings pageSettings, string userName, int departmentID)
        {
            SqlParameter[] sps = new SqlParameter[5];
            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            sps[1].Value = pageSettings.Where.HasRule("LastName")
                                ? pageSettings.Where.GetRule("LastName").Data
                                : String.Empty;

            sps[2] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            sps[2].Value = pageSettings.Where.HasRule("FirstName")
                                ? pageSettings.Where.GetRule("FirstName").Data
                                : String.Empty;

            sps[3] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            sps[3].Value = pageSettings.Where.HasRule("MiddleName")
                                ? pageSettings.Where.GetRule("MiddleName").Data
                                : String.Empty;

            sps[4] = new SqlParameter("@PostName", SqlDbType.NVarChar, 256);
            sps[4].Value = pageSettings.Where.HasRule("PostName")
                                ? pageSettings.Where.GetRule("PostName").Data
                                : String.Empty;

            DataTable dt = SPHelper.ExecuteDataset(trans, "usp_Worker_List", sps).Tables[0];

            pageSettings.TotalRecords = dt.Rows.Count;

            return dt;
        }

        public static DataTable GetPage(PageSettings pageSettings, string userName, int departmentID)
        {
            DataTable dt;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    dt = GetPage(trans, pageSettings, userName, departmentID);

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

            return dt;
        }

        public static JqGridResults BuildJqGridResults(DataTable dataTable, PageSettings pageSettings)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            if (dataTable != null)
                foreach (DataRow dr in dataTable.Rows)
                {
                    JqGridRow row = new JqGridRow();
                    row.id = (int)dr["WorkerID"];
                    row.cell = new string[9];

                    row.cell[0] = dr["WorkerID"].ToString();
                    row.cell[1] = (string)dr["LastName"];
                    row.cell[2] = (string)dr["FirstName"];
                    row.cell[3] = (string)dr["MiddleName"];

                    row.cell[4] = dr["PostID"].ToString();
                    row.cell[5] = (string)dr["PostName"];

                    row.cell[6] = dr["DepartmentID"].ToString();
                    row.cell[7] = (string)dr["DepartmentName"];
                    row.cell[8] = dr["IsDismissed"].ToString();

                    rows.Add(row);
                }
            result.rows = rows.ToArray();
            result.page = pageSettings.PageIndex;
            if (pageSettings.TotalRecords % pageSettings.PageSize == 0)
                result.total = pageSettings.TotalRecords / pageSettings.PageSize;
            else
                result.total = pageSettings.TotalRecords / pageSettings.PageSize + 1;
            result.records = pageSettings.TotalRecords;

            return result;
        }


        public override void Delete(SqlTransaction trans)
        {
            base.Update(trans);

            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, ID);
        }

        public static void Delete(SqlTransaction trans, int workerId, string userName)
        {
            /*if (!CanDelete(trans, workerId, userName))
            {
                throw new AccessException(userName, "Delete");
            }
            */
            SPHelper.ExecuteNonQuery(trans, SpNames.Delete, workerId);
        }

        public static void Delete(int workerId, string userName)
        {
            SqlConnection connection = new SqlConnection(SPHelper.GetConnectionString());
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Delete(trans, workerId, userName);

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


        public static int GetPostID(SqlTransaction trans, int workerId)
        {
            SqlParameter[] prms = new SqlParameter[6];
            prms[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[0].Value = workerId;

            prms[1] = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@MiddleName", SqlDbType.NVarChar, 50);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@PostID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@IsDismissed", SqlDbType.Bit);
            prms[5].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            return (int)prms[4].Value;
        }
        #endregion
    }
}