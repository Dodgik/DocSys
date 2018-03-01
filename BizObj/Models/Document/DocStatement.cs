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
using BizObj.Models.Helpers;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class DocStatement
    {
        private struct SpNames
        {
            public const string Get = "usp_DocStatement_Get";
            public const string Insert = "usp_DocStatement_Insert";
            public const string Update = "usp_DocStatement_Update";
            public const string Delete = "usp_DocStatement_Delete";
        }

        public const int ObjectTypeID = 3;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;
        
        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }
        
        #region Properties

        //[ParamAttribute("@DocStatementID", SqlDbType.Int)]
        //[FieldAttribute("DocStatementID")]
        public int ID { get; set; }

        //[ParamAttribute("@DocumentID", SqlDbType.Int)]
        //[FieldAttribute("DocumentID")]
        public int DocumentID { get; set; }

        //[ParamAttribute("@CitizenID", SqlDbType.Int)]
        //[FieldAttribute("CitizenID")]
        public int CitizenID { get; set; }

        //[ParamAttribute("@HeadID", SqlDbType.Int)]
        //[FieldAttribute("HeadID")]
        public int HeadID { get; set; }

        //[ParamAttribute("@Content", SqlDbType.NVarChar, -1)]
        //[FieldAttribute("Content")]
        public string Content { get; set; }

        //[ParamAttribute("@IsReception", SqlDbType.Bit)]
        //[FieldAttribute("IsReception")]
        public bool IsReception { get; set; }

        //[ParamAttribute("@InputMethodID", SqlDbType.Int)]
        //[FieldAttribute("InputMethodID")]
        public int InputMethodID { get; set; }

        //[ParamAttribute("@InputDocTypeID", SqlDbType.Int)]
        //[FieldAttribute("InputDocTypeID")]
        public int InputDocTypeID { get; set; }

        //[ParamAttribute("@InputSubjectTypeID", SqlDbType.Int)]
        //[FieldAttribute("InputSubjectTypeID")]
        public int InputSubjectTypeID { get; set; }

        //[ParamAttribute("@InputSignID", SqlDbType.Int)]
        //[FieldAttribute("InputSignID")]
        public int InputSignID { get; set; }

        //[ParamAttribute("@DeliveryTypeID", SqlDbType.Int)]
        //[FieldAttribute("DeliveryTypeID")]
        public int DeliveryTypeID { get; set; }

        //[ParamAttribute("@IsNeedAnswer", SqlDbType.Bit)]
        //[FieldAttribute("IsNeedAnswer")]
        public bool IsNeedAnswer { get; set; }


        public Document Document { get; set; }

        public Citizen Citizen { get; set; }
        
        [ScriptIgnore]
        public string UserName { get; set; }

        public int[] Branches { get; set; }

        public Worker Head { get; set; }
        #endregion
        
        #region Constructors

        public DocStatement()
        {

        }

        public DocStatement(string userName)
        {
            UserName = userName;
        }

        public DocStatement(int id, string userName): this(null, id, userName)
        {
            
        }

        public DocStatement(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);

            Document = new Document(trans, DocumentID, userName);
            Citizen = new Citizen(trans, CitizenID, userName);
            Branches = BranchList.GetBranchTypeIDList(trans, ID);
            Head = new Worker(trans, HeadID, userName);
        }

        #endregion

        #region Private Methods
        private void Init(int id)
        {
            Init(null, id);
        }

        private void Init(SqlTransaction trans, int docStatementId)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[12];
            prms[0] = new SqlParameter("@DocStatementID", SqlDbType.Int);
            prms[0].Value = docStatementId;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@IsReception", SqlDbType.Bit);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@InputMethodID", SqlDbType.Int);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@InputDocTypeID", SqlDbType.Int);
            prms[7].Direction = ParameterDirection.Output;

            prms[8] = new SqlParameter("@InputSubjectTypeID", SqlDbType.Int);
            prms[8].Direction = ParameterDirection.Output;

            prms[9] = new SqlParameter("@InputSignID", SqlDbType.Int);
            prms[9].Direction = ParameterDirection.Output;

            prms[10] = new SqlParameter("@DeliveryTypeID", SqlDbType.Int);
            prms[10].Direction = ParameterDirection.Output;

            prms[11] = new SqlParameter("@IsNeedAnswer", SqlDbType.Bit);
            prms[11].Direction = ParameterDirection.Output;
            
            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = docStatementId;
            DocumentID = (int)prms[1].Value;
            CitizenID = (int)prms[2].Value;
            HeadID = (int)prms[3].Value;
            Content = (string)prms[4].Value;
            IsReception = (bool)prms[5].Value;
            InputMethodID = (int)prms[6].Value;
            InputDocTypeID = (int)prms[7].Value;
            InputSubjectTypeID = (int)prms[8].Value;
            InputSignID = (int)prms[9].Value;
            DeliveryTypeID = (int)prms[10].Value;
            IsNeedAnswer = (bool)prms[11].Value;
        }
        
        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[12];
            prms[0] = new SqlParameter("@DocStatementID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[2].Value = CitizenID;

            prms[3] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[3].Value = HeadID;

            prms[4] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[4].Value = Content;

            prms[5] = new SqlParameter("@IsReception", SqlDbType.Bit);
            prms[5].Value = IsReception;

            prms[6] = new SqlParameter("@InputMethodID", SqlDbType.Int);
            prms[6].Value = InputMethodID;

            prms[7] = new SqlParameter("@InputDocTypeID", SqlDbType.Int);
            prms[7].Value = InputDocTypeID;

            prms[8] = new SqlParameter("@InputSubjectTypeID", SqlDbType.Int);
            prms[8].Value = InputSubjectTypeID;

            prms[9] = new SqlParameter("@InputSignID", SqlDbType.Int);
            prms[9].Value = InputSignID;

            prms[10] = new SqlParameter("@DeliveryTypeID", SqlDbType.Int);
            prms[10].Value = DeliveryTypeID;

            prms[11] = new SqlParameter("@IsNeedAnswer", SqlDbType.Bit);
            prms[11].Value = IsNeedAnswer;

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

            SqlParameter[] prms = new SqlParameter[12];
            prms[0] = new SqlParameter("@DocStatementID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@CitizenID", SqlDbType.Int);
            prms[2].Value = CitizenID;

            prms[3] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[3].Value = HeadID;

            prms[4] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[4].Value = Content;

            prms[5] = new SqlParameter("@IsReception", SqlDbType.Bit);
            prms[5].Value = IsReception;

            prms[6] = new SqlParameter("@InputMethodID", SqlDbType.Int);
            prms[6].Value = InputMethodID;

            prms[7] = new SqlParameter("@InputDocTypeID", SqlDbType.Int);
            prms[7].Value = InputDocTypeID;

            prms[8] = new SqlParameter("@InputSubjectTypeID", SqlDbType.Int);
            prms[8].Value = InputSubjectTypeID;

            prms[9] = new SqlParameter("@InputSignID", SqlDbType.Int);
            prms[9].Value = InputSignID;

            prms[10] = new SqlParameter("@DeliveryTypeID", SqlDbType.Int);
            prms[10].Value = DeliveryTypeID;

            prms[11] = new SqlParameter("@IsNeedAnswer", SqlDbType.Bit);
            prms[11].Value = IsNeedAnswer;

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

        public static DataTable GetPage(SqlTransaction trans, PageSettings pageSettings, bool isReception, string userName, int departmentID)
        {
            Department department = new Department(trans, departmentID, userName);
            if (!Permission.IsUserPermission(trans, userName, department.ObjectID, (int) Department.ActionType.View))
            {
                throw new AccessException(userName, "Get statements");
            }

            SqlParameter[] sps = new SqlParameter[26];
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

            sps[5] = new SqlParameter("@IsReception", SqlDbType.Bit);
            sps[5].Value = isReception;

            sps[6] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            if (pageSettings.Where.HasRule("CreationDate"))
                sps[6].Value = SqlDateTime.Parse(pageSettings.Where.GetRule("CreationDate").Data);
            else
                sps[6].Value = DBNull.Value;

            sps[7] = new SqlParameter("@CreationDateStart", SqlDbType.DateTime);
            sps[7].Value = pageSettings.Where.HasRule("CreationDateStart")
                               ? SqlDateTime.Parse(pageSettings.Where.GetRule("CreationDateStart").Data)
                               : SqlDateTime.MinValue;

            sps[8] = new SqlParameter("@CreationDateEnd", SqlDbType.DateTime);
            sps[8].Value = pageSettings.Where.HasRule("CreationDateEnd")
                               ? SqlDateTime.Parse(pageSettings.Where.GetRule("CreationDateEnd").Data)
                               : SqlDateTime.MaxValue;

            sps[9] = new SqlParameter("@CitizenLastName", SqlDbType.NVarChar, 50);
            sps[9].Value = pageSettings.Where.HasRule("CitizenLastName")
                               ? pageSettings.Where.GetRule("CitizenLastName").Data
                               : String.Empty;

            sps[10] = new SqlParameter("@HeadLastName", SqlDbType.NVarChar, 50);
            sps[10].Value = pageSettings.Where.HasRule("HeadLastName")
                                ? pageSettings.Where.GetRule("HeadLastName").Data
                                : String.Empty;

            sps[11] = new SqlParameter("@CityObjectName", SqlDbType.NVarChar, 50);
            sps[11].Value = pageSettings.Where.HasRule("CityObjectName")
                                ? pageSettings.Where.GetRule("CityObjectName").Data
                                : String.Empty;
            /*
            string cityObjectName = String.Empty;
            string houseNumber = String.Empty;
            string corps = String.Empty;
            string apartmentNumber = String.Empty;
            if (pageSettings.Where.HasRule("CityObjectName"))
            {
                FormatHelper.ParseAddress(pageSettings.Where.GetRule("CityObjectName").Data, out cityObjectName, out houseNumber, out corps, out apartmentNumber);
            }

            sps[11] = new SqlParameter("@CityObjectName", SqlDbType.NVarChar, 50);
            sps[11].Value = cityObjectName;

            sps[12] = new SqlParameter("@HouseNumber", SqlDbType.NVarChar, 50);
            sps[12].Value = houseNumber;

            sps[13] = new SqlParameter("@Corps", SqlDbType.NVarChar, 50);
            sps[13].Value = corps;

            sps[14] = new SqlParameter("@ApartmentNumber", SqlDbType.NVarChar, 50);
            sps[14].Value = apartmentNumber;
            */

            sps[12] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            sps[12].Value = pageSettings.Where.HasRule("Number")
                                ? pageSettings.Where.GetRule("Number").Data
                                : String.Empty;

            sps[13] = new SqlParameter("@ExternalNumber", SqlDbType.NVarChar, 50);
            sps[13].Value = pageSettings.Where.HasRule("ExternalNumber")
                                ? pageSettings.Where.GetRule("ExternalNumber").Data
                                : String.Empty;

            sps[14] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[14].Value = departmentID;

            sps[15] = new SqlParameter("@Content", SqlDbType.NVarChar);
            sps[15].Value = pageSettings.Where.HasRule("Content")
                                ? pageSettings.Where.GetRule("Content").Data
                                : String.Empty;

            sps[16] = new SqlParameter("@InnerNumber", SqlDbType.NVarChar, 50);
            sps[16].Value = pageSettings.Where.HasRule("InnerNumber")
                               ? pageSettings.Where.GetRule("InnerNumber").Data
                               : String.Empty;

            sps[17] = new SqlParameter("@Controlled", SqlDbType.Bit);
            bool controlled = false;
            if (pageSettings.Where.HasRule("Controlled")) {
                Boolean.TryParse(pageSettings.Where.GetRule("Controlled").Data, out controlled);
            }

            if (controlled) {
                sps[17].Value = true;
            } else
                sps[17].Value = DBNull.Value;

            sps[18] = new SqlParameter("@EndDateFrom", SqlDbType.DateTime);
            sps[18].IsNullable = true;
            if (pageSettings.Where.HasRule("EndDateFrom"))
                sps[18].Value = DateTime.Parse(pageSettings.Where.GetRule("EndDateFrom").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[18].Value = DBNull.Value;

            sps[19] = new SqlParameter("@EndDateTo", SqlDbType.DateTime);
            sps[19].IsNullable = true;
            if (pageSettings.Where.HasRule("EndDateTo"))
                sps[19].Value = DateTime.Parse(pageSettings.Where.GetRule("EndDateTo").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[19].Value = DBNull.Value;

            sps[20] = new SqlParameter("@IsDepartmentOwner", SqlDbType.Bit);
            sps[20].IsNullable = true;
            if (pageSettings.Where.HasRule("IsDepartmentOwner"))
                sps[20].Value = (pageSettings.Where.GetRule("IsDepartmentOwner").Data.ToLower() == "true");
            else
                sps[20].Value = DBNull.Value;

            sps[21] = new SqlParameter("@ControlledInner", SqlDbType.Bit);
            sps[21].IsNullable = true;
            if (pageSettings.Where.HasRule("ControlledInner"))
                sps[21].Value = (pageSettings.Where.GetRule("ControlledInner").Data.ToLower() == "true");
            else
                sps[21].Value = DBNull.Value;

            sps[22] = new SqlParameter("@InnerEndDateFrom", SqlDbType.DateTime);
            sps[22].IsNullable = true;
            if (pageSettings.Where.HasRule("InnerEndDateFrom"))
                sps[22].Value = DateTime.Parse(pageSettings.Where.GetRule("InnerEndDateFrom").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[22].Value = DBNull.Value;

            sps[23] = new SqlParameter("@InnerEndDateTo", SqlDbType.DateTime);
            sps[23].IsNullable = true;
            if (pageSettings.Where.HasRule("InnerEndDateTo"))
                sps[23].Value = DateTime.Parse(pageSettings.Where.GetRule("InnerEndDateTo").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[23].Value = DBNull.Value;

            sps[24] = new SqlParameter("@LableID", SqlDbType.Int);
            int lableId;
            if (pageSettings.Where.HasRule("LableID") &&
                Int32.TryParse(pageSettings.Where.GetRule("LableID").Data, out lableId)) {
                sps[24].Value = lableId;
            } else {
                sps[24].Value = DBNull.Value;
            }

            sps[25] = new SqlParameter("@DocStatusID", SqlDbType.Int);
            int docStatusId;
            if (pageSettings.Where.HasRule("DocStatusID") &&
                Int32.TryParse(pageSettings.Where.GetRule("DocStatusID").Data, out docStatusId))
            {
                sps[25].Value = docStatusId;
            } else {
                sps[25].Value = DBNull.Value;
            }

            DataTable dt = SPHelper.ExecuteDataset(trans, "usp_DocStatement_Page", sps).Tables[0];

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;


            return dt;
        }

        public static DataTable GetPage(PageSettings pageSettings, bool isReception, string userName, int departmentID)
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

                    dt = GetPage(trans, pageSettings, isReception, userName, departmentID);

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
            if(dataTable!=null)
                foreach (DataRow dr in dataTable.Rows)
                {
                    JqGridRow row = new JqGridRow();
                    row.id = (int) dr["DocStatementID"];
                    row.cell = new string[19];

                    row.cell[0] = dr["DocStatementID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime) dr["CreationDate"]).ToString("yyyy-MM-dd");

                    row.cell[3] = FormatHelper.FormatToLastNameAndInitials((string) dr["CitizenLastName"],
                                                                           (string) dr["CitizenFirstName"],
                                                                           (string) dr["CitizenMiddleName"]);

                    row.cell[4] = String.Format("{0} {1} {2}", dr["CitizenLastName"], dr["CitizenFirstName"],
                                                dr["CitizenMiddleName"]);

                    if ((string)dr["CityObjectTypeShortName"] == "н")
                        row.cell[5] = (string)dr["Address"];
                    else
                        row.cell[5] = FormatHelper.FormatAddress((string) dr["Address"],
                                                                 (string) dr["CityObjectTypeShortName"],
                                                                 (string) dr["CityObjectName"],
                                                                 (string) dr["HouseNumber"], (string) dr["Corps"],
                                                                 (string) dr["ApartmentNumber"]);

                    row.cell[6] = FormatHelper.FormatToLastNameAndInitials((string) dr["HeadLastName"],
                                                                           (string) dr["HeadFirstName"],
                                                                           (string) dr["HeadMiddleName"]);

                    row.cell[7] = (string) dr["Number"];
                    row.cell[8] = dr["ExternalNumber"].ToString();

                    row.cell[9] = (string) dr["Content"];
                    row.cell[10] = dr["DeliveryTypeID"].ToString();
                    row.cell[11] = dr["InputDocTypeID"].ToString();
                    row.cell[12] = dr["InputMethodID"].ToString();
                    row.cell[13] = dr["InputSignID"].ToString();
                    row.cell[14] = dr["InputSubjectTypeID"].ToString();
                    row.cell[15] = dr["DepartmentID"].ToString();
                    row.cell[16] = DBNull.Value != dr["InnerNumber"] ? dr["InnerNumber"].ToString() : String.Empty;
                    row.cell[17] = DBNull.Value != dr["ControlCardID"] ? dr["ControlCardID"].ToString() : String.Empty;
                    row.cell[18] = dr["IsControlled"].ToString();
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
        
        #region Reports

        public static DataSet GetControlled(SqlTransaction trans, int departmentID, DateTime creationDate, DateTime endDate)
        {
            SqlParameter[] sps = new SqlParameter[3];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            if (creationDate >= SqlDateTime.MinValue && creationDate <= SqlDateTime.MaxValue)
                sps[1].Value = creationDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetControlled", sps);
        }

        public static DataSet GetFullStatistics(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate)
        {
            SqlParameter[] sps = new SqlParameter[3];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetFullStatistics", sps);
        }

        public static int GetCountStatements(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@Count", SqlDbType.Int);
            sps[3].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, "usp_DocStatement_GetCountStatements", sps);
            
            if (sps[3].Value != DBNull.Value)
                return Convert.ToInt32(sps[3].Value);

            return 0;
        }

        public static DataSet GetStatisticsBySocialCategory(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int socialCategoryID)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@SocialCategoryID", SqlDbType.Int);
            sps[3].Value = socialCategoryID;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsBySocialCategory", sps);
        }

        public static DataSet GetStatisticsByBranchType(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int branchTypeID)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@BranchTypeID", SqlDbType.Int);
            sps[3].Value = branchTypeID;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsByBranchType", sps);
        }

        public static DataSet GetStatisticsByInputSign(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int inputSignID)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@InputSignID", SqlDbType.Int);
            sps[3].Value = inputSignID;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsByInputSign", sps);
        }

        public static DataSet GetStatisticsByInputSubjectType(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int inputSubjectTypeID)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@InputSubjectTypeID", SqlDbType.Int);
            sps[3].Value = inputSubjectTypeID;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsByInputSubjectType", sps);
        }

        public static DataSet GetStatisticsForReceptionByHead(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int headID)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@HeadID", SqlDbType.Int);
            sps[3].Value = headID;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsForReceptionByHead", sps);
        }

        public static DataSet GetStatisticsByOrganization(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int organizationID)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@OrganizationID", SqlDbType.Int);
            sps[3].Value = organizationID;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsByOrganization", sps);
        }

        public static DataSet GetStatisticsSpeciallyControlled(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate)
        {
            SqlParameter[] sps = new SqlParameter[3];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsSpeciallyControlled", sps);
        }

        public static DataSet GetStatisticsByHead(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int headID, out int[] count)
        {
            SqlParameter[] sps = new SqlParameter[12];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@HeadID", SqlDbType.Int);
            sps[3].Value = headID;

            sps[4] = new SqlParameter("@CountAllStatements", SqlDbType.Int);
            sps[4].Direction = ParameterDirection.Output;

            sps[5] = new SqlParameter("@CountAllReceptions", SqlDbType.Int);
            sps[5].Direction = ParameterDirection.Output;

            sps[6] = new SqlParameter("@CountStatements", SqlDbType.Int);
            sps[6].Direction = ParameterDirection.Output;

            sps[7] = new SqlParameter("@CountReceptions", SqlDbType.Int);
            sps[7].Direction = ParameterDirection.Output;

            sps[8] = new SqlParameter("@CountScStatements", SqlDbType.Int);
            sps[8].Direction = ParameterDirection.Output;

            sps[9] = new SqlParameter("@CountScReceptions", SqlDbType.Int);
            sps[9].Direction = ParameterDirection.Output;

            sps[10] = new SqlParameter("@CountSccStatements", SqlDbType.Int);
            sps[10].Direction = ParameterDirection.Output;

            sps[11] = new SqlParameter("@CountSccReceptions", SqlDbType.Int);
            sps[11].Direction = ParameterDirection.Output;

            DataSet ds = SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsByHead", sps);

            count = new int[8];
            if(sps[4].Value != DBNull.Value)
            {
                count[0] = (int) sps[4].Value;
            }
            if (sps[5].Value != DBNull.Value)
            {
                count[1] = (int)sps[5].Value;
            }
            if (sps[6].Value != DBNull.Value)
            {
                count[2] = (int)sps[6].Value;
            }
            if (sps[7].Value != DBNull.Value)
            {
                count[3] = (int)sps[7].Value;
            }
            if (sps[8].Value != DBNull.Value)
            {
                count[4] = (int)sps[8].Value;
            }
            if (sps[9].Value != DBNull.Value)
            {
                count[5] = (int)sps[9].Value;
            }
            if (sps[10].Value != DBNull.Value)
            {
                count[6] = (int)sps[10].Value;
            }
            if (sps[11].Value != DBNull.Value)
            {
                count[7] = (int)sps[11].Value;
            }

            return ds;
        }

        public static DataSet GetStatisticsForStatementsByWorker(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int workerID)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate >= SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[3].Value = workerID;

            return SPHelper.ExecuteDataset(trans, "usp_DocStatement_GetStatisticsForStatementsByWorker", sps);
        }

        #endregion

        #endregion
    }
}