using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.JqGrid;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class DocAdminService
    {
                private struct SpNames
        {
            public const string Get = "usp_DocAdminService_Get";
            public const string GetByDocumentID = "usp_DocAdminService_GetByDocumentID";
            public const string Insert = "usp_DocAdminService_Insert";
            public const string Update = "usp_DocAdminService_Update";
            public const string Delete = "usp_DocAdminService_Delete";
        }

        public const int ObjectTypeID = 31;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        public int ID { get; set; }
        public int DocumentID { get; set; }

        public string Content { get; set; }
        public string SubjectRequest { get; set; }
        
        public string ServiceName { get; set; }
        public string ObjectForService { get; set; }
        public int ExecutiveDepartmentID { get; set; }

        public DateTime DateReceivedToDepartment { get; set; }
        public int ReceivedWorkerID { get; set; }
        public DateTime DateReturnFromDepartment { get; set; }
        public int ReturnWorkerID { get; set; }
        public string ServiceResult { get; set; }
        public DateTime DateResponseToClient { get; set; }
        public string ResponseClientInfo { get; set; }
        public bool IsControlled { get; set; }

        public Document Document { get; set; }

        
        [ScriptIgnore]
        public string UserName { get; set; }

        #endregion

        #region Constructors

        public DocAdminService()
        {

        }

        public DocAdminService(string userName)
        {
            UserName = userName;
        }

        public DocAdminService(int id, string userName): this(null, id, userName)
        {

        }

        public DocAdminService(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);

            Document = new Document(trans, DocumentID, userName);
        }

        #endregion

        #region Private Methods
        private void Init(int id)
        {
            Init(null, id);
        }

        private void Init(SqlTransaction trans, int id)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[15];
            prms[0] = new SqlParameter("@DocAdminServiceID", SqlDbType.Int);
            prms[0].Value = id;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@SubjectRequest", SqlDbType.NVarChar, -1);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@ServiceName", SqlDbType.NVarChar, -1);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@ObjectForService", SqlDbType.NVarChar, -1);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@DateReceivedToDepartment", SqlDbType.DateTime);
            prms[7].Direction = ParameterDirection.Output;
            
            prms[8] = new SqlParameter("@ReceivedWorkerID", SqlDbType.Int);
            prms[8].Direction = ParameterDirection.Output;

            prms[9] = new SqlParameter("@DateReturnFromDepartment", SqlDbType.DateTime);
            prms[9].Direction = ParameterDirection.Output;

            prms[10] = new SqlParameter("@ReturnWorkerID", SqlDbType.Int);
            prms[10].Direction = ParameterDirection.Output;

            prms[11] = new SqlParameter("@ServiceResult", SqlDbType.NVarChar, -1);
            prms[11].Direction = ParameterDirection.Output;
            
            prms[12] = new SqlParameter("@DateResponseToClient", SqlDbType.DateTime);
            prms[12].Direction = ParameterDirection.Output;

            prms[13] = new SqlParameter("@ResponseClientInfo", SqlDbType.NVarChar, -1);
            prms[13].Direction = ParameterDirection.Output;

            prms[14] = new SqlParameter("@IsControlled", SqlDbType.Bit);
            prms[14].Direction = ParameterDirection.Output;
            
            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);
            
            ID = id;
            DocumentID = (int)prms[1].Value;
            Content = (string)prms[2].Value;
            SubjectRequest = (string)prms[3].Value;
            ServiceName = (string)prms[4].Value;

            ObjectForService = (string)prms[5].Value;
            ExecutiveDepartmentID = (int)prms[6].Value;
            DateReceivedToDepartment = (DateTime)prms[7].Value;
            ReceivedWorkerID = (int)prms[8].Value;
            DateReturnFromDepartment = (DateTime)prms[9].Value;

            ReturnWorkerID = (int)prms[10].Value;
            ServiceResult = (string)prms[11].Value;
            DateResponseToClient = (DateTime)prms[12].Value;
            ResponseClientInfo = (string)prms[13].Value;
            IsControlled = (bool)prms[14].Value;
        }

        #endregion

        #region Public Methods

        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[15];
            prms[0] = new SqlParameter("@DocAdminServiceID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[2].Value = Content;

            prms[3] = new SqlParameter("@SubjectRequest", SqlDbType.NVarChar, -1);
            prms[3].Value = SubjectRequest;

            prms[4] = new SqlParameter("@ServiceName", SqlDbType.NVarChar, -1);
            prms[4].Value = ServiceName;

            prms[5] = new SqlParameter("@ObjectForService", SqlDbType.NVarChar, -1);
            prms[5].Value = ObjectForService;

            prms[6] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            prms[6].Value = ExecutiveDepartmentID;

            prms[7] = new SqlParameter("@DateReceivedToDepartment", SqlDbType.DateTime);
            if (DateReceivedToDepartment > (DateTime)SqlDateTime.MinValue)
                prms[7].Value = DateReceivedToDepartment;
            else
                prms[7].Value = (DateTime)SqlDateTime.MinValue;

            prms[8] = new SqlParameter("@ReceivedWorkerID", SqlDbType.Int);
            prms[8].Value = ReceivedWorkerID;

            prms[9] = new SqlParameter("@DateReturnFromDepartment", SqlDbType.DateTime);
            if (DateReturnFromDepartment > (DateTime)SqlDateTime.MinValue)
                prms[9].Value = DateReturnFromDepartment;
            else
                prms[9].Value = (DateTime)SqlDateTime.MinValue;

            prms[10] = new SqlParameter("@ReturnWorkerID", SqlDbType.Int);
            prms[10].Value = ReturnWorkerID;

            prms[11] = new SqlParameter("@ServiceResult", SqlDbType.NVarChar, -1);
            prms[11].Value = ServiceResult;

            prms[12] = new SqlParameter("@DateResponseToClient", SqlDbType.DateTime);
            if (DateResponseToClient > (DateTime)SqlDateTime.MinValue)
                prms[12].Value = DateResponseToClient;
            else
                prms[12].Value = (DateTime)SqlDateTime.MinValue;

            prms[13] = new SqlParameter("@ResponseClientInfo", SqlDbType.NVarChar, -1);
            prms[13].Value = ResponseClientInfo;

            prms[14] = new SqlParameter("@IsControlled", SqlDbType.Bit);
            prms[14].Value = IsControlled;
            
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

            SqlParameter[] prms = new SqlParameter[15];
            prms[0] = new SqlParameter("@DocAdminServiceID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[2].Value = Content;

            prms[3] = new SqlParameter("@SubjectRequest", SqlDbType.NVarChar, -1);
            prms[3].Value = SubjectRequest;

            prms[4] = new SqlParameter("@ServiceName", SqlDbType.NVarChar, -1);
            prms[4].Value = ServiceName;

            prms[5] = new SqlParameter("@ObjectForService", SqlDbType.NVarChar, -1);
            prms[5].Value = ObjectForService;

            prms[6] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            prms[6].Value = ExecutiveDepartmentID;

            prms[7] = new SqlParameter("@DateReceivedToDepartment", SqlDbType.DateTime);
            if (DateReceivedToDepartment > (DateTime)SqlDateTime.MinValue)
                prms[7].Value = DateReceivedToDepartment;
            else
                prms[7].Value = (DateTime)SqlDateTime.MinValue;

            prms[8] = new SqlParameter("@ReceivedWorkerID", SqlDbType.Int);
            prms[8].Value = ReceivedWorkerID;

            prms[9] = new SqlParameter("@DateReturnFromDepartment", SqlDbType.DateTime);
            if (DateReturnFromDepartment > (DateTime)SqlDateTime.MinValue)
                prms[9].Value = DateReturnFromDepartment;
            else
                prms[9].Value = (DateTime)SqlDateTime.MinValue;

            prms[10] = new SqlParameter("@ReturnWorkerID", SqlDbType.Int);
            prms[10].Value = ReturnWorkerID;

            prms[11] = new SqlParameter("@ServiceResult", SqlDbType.NVarChar, -1);
            prms[11].Value = ServiceResult;

            prms[12] = new SqlParameter("@DateResponseToClient", SqlDbType.DateTime);
            if (DateResponseToClient > (DateTime)SqlDateTime.MinValue)
                prms[12].Value = DateResponseToClient;
            else
                prms[12].Value = (DateTime)SqlDateTime.MinValue;

            prms[13] = new SqlParameter("@ResponseClientInfo", SqlDbType.NVarChar, -1);
            prms[13].Value = ResponseClientInfo;

            prms[14] = new SqlParameter("@IsControlled", SqlDbType.Bit);
            prms[14].Value = IsControlled;
            
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

        #region Base Methods
        
        public static int GetNextNumber(SqlTransaction trans, string userName, int departmentID, int documentCodeID)
        {
            Department department = new Department(trans, departmentID, userName);
            if (!Permission.IsUserPermission(trans, userName, department.ObjectID, (int)Department.ActionType.View))
            {
                throw new AccessException(userName, "Get document template");
            }

            int number = 1;
            DateTime starYearDate = new DateTime(DateTime.Now.Year, 1, 1, 1, 0, 0);
            DateTime endYearDate = new DateTime(DateTime.Now.Year, 12, 31, 23, 0, 0);

            SqlParameter[] sps = new SqlParameter[5];
            sps[0] = new SqlParameter("@Count", SqlDbType.Int);
            sps[0].Value = 300;

            sps[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            sps[1].Value = documentCodeID;

            sps[2] = new SqlParameter("@CreationDateStart", SqlDbType.DateTime);
            sps[2].Value = starYearDate;

            sps[3] = new SqlParameter("@CreationDateEnd", SqlDbType.DateTime);
            sps[3].Value = endYearDate;

            sps[4] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[4].Value = departmentID;


            DataTable dt = SPHelper.ExecuteDataset(trans, "usp_DocAdminService_GetLastNumberList", sps).Tables[0];

            if (dt.Rows.Count > 0)
            {
                List<int> numbers = new List<int>();
                foreach (DataRow row in dt.Rows)
                {
                    numbers.Add(Convert.ToInt32((string)row["Number"]));
                }
                numbers.Sort();
                number = numbers[numbers.Count - 1] + 1;
            }

            return number;
        }

        public static int GetNextNumber(string userName, int departmentID, int documentCodeID)
        {
            int number;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    number = GetNextNumber(trans, userName, departmentID, documentCodeID);

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

            return number;
        }
        
        public static DataTable GetPage(SqlTransaction trans, PageSettings pageSettings, string userName, int departmentID)
        {
            Department department = new Department(trans, departmentID, userName);
            if (!Permission.IsUserPermission(trans, userName, department.ObjectID, (int)Department.ActionType.View))
            {
                throw new AccessException(userName, "Get document template");
            }

            SqlParameter[] sps = new SqlParameter[15];
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

            sps[5] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            if (pageSettings.Where.HasRule("CreationDate"))
                sps[5].Value = DateTime.Parse(pageSettings.Where.GetRule("CreationDate").Data, CultureInfo.CurrentCulture);
            else
                sps[5].Value = DBNull.Value;

            SqlDateTime cdStart = SqlDateTime.MinValue;
            if (pageSettings.Where.HasRule("CreationDateStart"))
                cdStart = DateTime.Parse(pageSettings.Where.GetRule("CreationDateStart").Data, CultureInfo.CurrentCulture);
            sps[6] = new SqlParameter("@CreationDateStart", SqlDbType.DateTime);
            sps[6].Value = cdStart;

            SqlDateTime cdEnd = SqlDateTime.MaxValue;
            if (pageSettings.Where.HasRule("CreationDateEnd"))
                cdEnd = DateTime.Parse(pageSettings.Where.GetRule("CreationDateEnd").Data, CultureInfo.CurrentCulture);
            sps[7] = new SqlParameter("@CreationDateEnd", SqlDbType.DateTime);
            sps[7].Value = cdEnd;

            sps[8] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            sps[8].Value = pageSettings.Where.HasRule("Number")
                                ? pageSettings.Where.GetRule("Number").Data
                                : String.Empty;
            
            sps[9] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[9].Value = departmentID;
            /*
            sps[10] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            int documentCodeID;
            if (pageSettings.Where.HasRule("DocumentCodeID") && Int32.TryParse(pageSettings.Where.GetRule("DocumentCodeID").Data, out documentCodeID))
            {
                sps[10].Value = documentCodeID;
            }
            else
                sps[10].Value = DBNull.Value;
            */
            sps[10] = new SqlParameter("@DocumentCode", SqlDbType.NVarChar);
            sps[10].Value = pageSettings.Where.HasRule("DocumentCode")
                                ? pageSettings.Where.GetRule("DocumentCode").Data
                                : String.Empty;

            sps[11] = new SqlParameter("@Content", SqlDbType.NVarChar);
            sps[11].Value = pageSettings.Where.HasRule("Content")
                                ? pageSettings.Where.GetRule("Content").Data
                                : String.Empty;

            sps[12] = new SqlParameter("@SubjectRequest", SqlDbType.NVarChar);
            sps[12].Value = pageSettings.Where.HasRule("SubjectRequest")
                                ? pageSettings.Where.GetRule("SubjectRequest").Data
                                : String.Empty;

            sps[13] = new SqlParameter("@ServiceName", SqlDbType.NVarChar);
            sps[13].Value = pageSettings.Where.HasRule("ServiceName")
                                ? pageSettings.Where.GetRule("ServiceName").Data
                                : String.Empty;

            sps[14] = new SqlParameter("@ObjectForService", SqlDbType.NVarChar);
            sps[14].Value = pageSettings.Where.HasRule("ObjectForService")
                                ? pageSettings.Where.GetRule("ObjectForService").Data
                                : String.Empty;


            DataTable dt = SPHelper.ExecuteDataset(trans, "usp_DocAdminService_Page", sps).Tables[0];

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;
            
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
                    row.id = (int)dr["DocAdminServiceID"];
                    row.cell = new string[20];

                    row.cell[0] = dr["DocAdminServiceID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime) dr["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);

                    row.cell[3] = (string)dr["Number"];
                    row.cell[4] = dr["DocumentCode"].ToString();

                    row.cell[5] = (string)dr["SubjectRequest"];
                    row.cell[6] = (string)dr["ServiceName"];
                    row.cell[7] = (string)dr["ObjectForService"];
                    row.cell[8] = (string)dr["Content"];
                    row.cell[9] = (string)dr["Notes"];
                    row.cell[10] = dr["IsControlled"].ToString();
                    row.cell[11] = dr["ExecutiveDepartmentID"].ToString();
                    row.cell[12] = (string)dr["ExecutiveDepartmentName"];

                    if ((DateTime)dr["DateReceivedToDepartment"] > (DateTime)SqlDateTime.MinValue)
                        row.cell[13] = ((DateTime)dr["DateReceivedToDepartment"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);
                    else
                        row.cell[13] = String.Empty;
                    row.cell[14] = dr["ReceivedWorkerID"].ToString();

                    if ((DateTime)dr["DateReturnFromDepartment"] > (DateTime)SqlDateTime.MinValue)
                        row.cell[15] = ((DateTime)dr["DateReturnFromDepartment"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);
                    else
                        row.cell[15] = String.Empty;

                    row.cell[16] = dr["ReturnWorkerID"].ToString();

                    row.cell[17] = (string)dr["ServiceResult"];

                    if ((DateTime)dr["DateResponseToClient"] > (DateTime)SqlDateTime.MinValue)
                        row.cell[18] = ((DateTime)dr["DateResponseToClient"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);
                    else
                        row.cell[18] = String.Empty;

                    row.cell[19] = (string)dr["ResponseClientInfo"];
                    /*
                    row.cell[20] = dr["DocStatusID"].ToString();
                    row.cell[21] = (string)dr["DocStatusName"];
                    */
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


        public static DocAdminService GetByDocumentID(SqlTransaction trans, int documentID, string userName)
        {
            DocAdminService dt = null;
            if (!CanView(userName))
            {
                throw new AccessException(userName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[15];
            prms[0] = new SqlParameter("@DocAdminServiceID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = documentID;

            prms[2] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@SubjectRequest", SqlDbType.NVarChar, -1);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@ServiceName", SqlDbType.NVarChar, -1);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@ObjectForService", SqlDbType.NVarChar, -1);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@DateReceivedToDepartment", SqlDbType.DateTime);
            prms[7].Direction = ParameterDirection.Output;

            prms[8] = new SqlParameter("@ReceivedWorkerID", SqlDbType.Int);
            prms[8].Direction = ParameterDirection.Output;

            prms[9] = new SqlParameter("@DateReturnFromDepartment", SqlDbType.DateTime);
            prms[9].Direction = ParameterDirection.Output;

            prms[10] = new SqlParameter("@ReturnWorkerID", SqlDbType.Int);
            prms[10].Direction = ParameterDirection.Output;

            prms[11] = new SqlParameter("@ServiceResult", SqlDbType.NVarChar, -1);
            prms[11].Direction = ParameterDirection.Output;

            prms[12] = new SqlParameter("@DateResponseToClient", SqlDbType.DateTime);
            prms[12].Direction = ParameterDirection.Output;

            prms[13] = new SqlParameter("@ResponseClientInfo", SqlDbType.NVarChar, -1);
            prms[13].Direction = ParameterDirection.Output;

            prms[14] = new SqlParameter("@IsControlled", SqlDbType.Bit);
            prms[14].Direction = ParameterDirection.Output;


            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.GetByDocumentID, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.GetByDocumentID, prms);

            if (prms[0].Value != DBNull.Value)
            {
                dt = new DocAdminService(userName);
                dt.ID = (int) prms[0].Value;
                dt.DocumentID = documentID;
                dt.Content = (string) prms[2].Value;

                dt.SubjectRequest = (string)prms[3].Value;
                dt.ServiceName = (string)prms[4].Value;

                dt.ObjectForService = (string)prms[5].Value;
                dt.ExecutiveDepartmentID = (int)prms[6].Value;
                dt.DateReceivedToDepartment = (DateTime)prms[7].Value;
                dt.ReceivedWorkerID = (int)prms[8].Value;
                dt.DateReturnFromDepartment = (DateTime)prms[9].Value;

                dt.ReturnWorkerID = (int)prms[10].Value;
                dt.ServiceResult = (string)prms[11].Value;
                dt.DateResponseToClient = (DateTime)prms[12].Value;
                dt.ResponseClientInfo = (string)prms[13].Value;
                dt.IsControlled = (bool)prms[14].Value;
            }

            return dt;
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
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int)ActionType.Update);
        }

        public static bool CanDelete(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int)ActionType.Delete);
        }

        public static bool CanView(string userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }
            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int)ActionType.View);
        }

        #endregion


        #endregion
    }
}