using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.JqGrid;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class DocTemplate
    {
        private struct SpNames
        {
            public const string Get = "usp_DocTemplate_Get";
            public const string GetByDocumentID = "usp_DocTemplate_GetByDocumentID";
            public const string Insert = "usp_DocTemplate_Insert";
            public const string Update = "usp_DocTemplate_Update";
            public const string Delete = "usp_DocTemplate_Delete";
        }

        public const int ObjectTypeID = 4;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;

        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }

        #region Properties

        //[ParamAttribute("@DocTemplateID", SqlDbType.Int)]
        //[FieldAttribute("DocTemplateID")]
        public int ID { get; set; }

        //[ParamAttribute("@DocumentID", SqlDbType.Int)]
        //[FieldAttribute("DocumentID")]
        public int DocumentID { get; set; }

        //[ParamAttribute("@Content", SqlDbType.NVarChar, -1)]
        //[FieldAttribute("Content")]
        public string Content { get; set; }

        //[ParamAttribute("@Changes", SqlDbType.NVarChar, -1)]
        //[FieldAttribute("Changes")]
        public string Changes { get; set; }

        //[ParamAttribute("@DocTypeID", SqlDbType.Int)]
        //[FieldAttribute("DocTypeID")]
        public int DocTypeID { get; set; }

        //[ParamAttribute("@IsControlled", SqlDbType.Bit)]
        //[FieldAttribute("IsControlled")]
        public bool IsControlled { get; set; }

        //[ParamAttribute("@IsSpeciallyControlled", SqlDbType.Bit)]
        //[FieldAttribute("IsSpeciallyControlled")]
        public bool IsSpeciallyControlled { get; set; }

        //[ParamAttribute("@IsIncreasedControlled", SqlDbType.Bit)]
        //[FieldAttribute("IsIncreasedControlled")]
        public bool IsIncreasedControlled { get; set; }

        //[ParamAttribute("@IsInput", SqlDbType.Bit)]
        //[FieldAttribute("IsInput")]
        public bool IsInput { get; set; }

        //[ParamAttribute("@IsPublic", SqlDbType.Bit)]
        //[FieldAttribute("IsPublic")]
        public bool IsPublic { get; set; }

        //[ParamAttribute("@QuestionTypeID", SqlDbType.Int)]
        //[FieldAttribute("QuestionTypeID")]
        public int QuestionTypeID { get; set; }

        //[ParamAttribute("@NumberCopies", SqlDbType.Int)]
        //[FieldAttribute("NumberCopies")]
        public int NumberCopies { get; set; }

        //[ParamAttribute("@HeadID", SqlDbType.Int)]
        //[FieldAttribute("HeadID")]
        public int HeadID { get; set; }

        //[ParamAttribute("@WorkerID", SqlDbType.Int)]
        //[FieldAttribute("WorkerID")]
        public int WorkerID { get; set; }

        //[ParamAttribute("@PublicContent", SqlDbType.NVarChar, -1)]
        //[FieldAttribute("PublicContent")]
        public string PublicContent { get; set; }

        public Document Document { get; set; }

        
        [ScriptIgnore]
        public string UserName { get; set; }

        #endregion

        #region Constructors

        public DocTemplate()
        {

        }

        public DocTemplate(string userName)
        {
            UserName = userName;
        }

        public DocTemplate(int id, string userName): this(null, id, userName)
        {

        }

        public DocTemplate(SqlTransaction trans, int id, string userName): this(userName)
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
            prms[0] = new SqlParameter("@DocTemplateID", SqlDbType.Int);
            prms[0].Value = id;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@Changes", SqlDbType.NVarChar, -1);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@DocTypeID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@IsControlled", SqlDbType.Bit);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Bit);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@IsIncreasedControlled", SqlDbType.Bit);
            prms[7].Direction = ParameterDirection.Output;
            
            prms[8] = new SqlParameter("@IsInput", SqlDbType.Bit);
            prms[8].Direction = ParameterDirection.Output;

            prms[9] = new SqlParameter("@IsPublic", SqlDbType.Bit);
            prms[9].Direction = ParameterDirection.Output;

            prms[10] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            prms[10].Direction = ParameterDirection.Output;

            prms[11] = new SqlParameter("@NumberCopies", SqlDbType.Int);
            prms[11].Direction = ParameterDirection.Output;

            prms[12] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[12].Direction = ParameterDirection.Output;

            prms[13] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[13].Direction = ParameterDirection.Output;

            prms[14] = new SqlParameter("@PublicContent", SqlDbType.NVarChar, -1);
            prms[14].Direction = ParameterDirection.Output;
            
            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);


            ID = id;
            DocumentID = (int)prms[1].Value;
            Content = (string)prms[2].Value;
            Changes = (string)prms[3].Value;
            DocTypeID = (int)prms[4].Value;

            IsControlled = (bool)prms[5].Value;
            IsSpeciallyControlled = (bool)prms[6].Value;
            IsIncreasedControlled = (bool)prms[7].Value;
            IsInput = (bool)prms[8].Value;
            IsPublic = (bool)prms[9].Value;

            QuestionTypeID = (int)prms[10].Value;
            NumberCopies = (int)prms[11].Value;
            HeadID = (int)prms[12].Value;
            WorkerID = (int)prms[13].Value;
            PublicContent = (string)prms[14].Value;
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
            prms[0] = new SqlParameter("@DocTemplateID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[2].Value = Content;

            prms[3] = new SqlParameter("@Changes", SqlDbType.NVarChar, -1);
            prms[3].Value = Changes;

            prms[4] = new SqlParameter("@DocTypeID", SqlDbType.Int);
            prms[4].Value = DocTypeID;

            prms[5] = new SqlParameter("@IsControlled", SqlDbType.Bit);
            prms[5].Value = IsControlled;

            prms[6] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Bit);
            prms[6].Value = IsSpeciallyControlled;

            prms[7] = new SqlParameter("@IsIncreasedControlled", SqlDbType.Bit);
            prms[7].Value = IsIncreasedControlled;
            
            prms[8] = new SqlParameter("@IsInput", SqlDbType.Bit);
            prms[8].Value = IsInput;

            prms[9] = new SqlParameter("@IsPublic", SqlDbType.Bit);
            prms[9].Value = IsPublic;

            prms[10] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            prms[10].Value = QuestionTypeID;

            prms[11] = new SqlParameter("@NumberCopies", SqlDbType.Int);
            prms[11].Value = NumberCopies;

            prms[12] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[12].Value = HeadID;

            prms[13] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[13].Value = WorkerID;

            prms[14] = new SqlParameter("@PublicContent", SqlDbType.NVarChar, -1);
            prms[14].Value = PublicContent;
            
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
            prms[0] = new SqlParameter("@DocTemplateID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[2].Value = Content;

            prms[3] = new SqlParameter("@Changes", SqlDbType.NVarChar, -1);
            prms[3].Value = Changes;

            prms[4] = new SqlParameter("@DocTypeID", SqlDbType.Int);
            prms[4].Value = DocTypeID;

            prms[5] = new SqlParameter("@IsControlled", SqlDbType.Bit);
            prms[5].Value = IsControlled;

            prms[6] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Bit);
            prms[6].Value = IsSpeciallyControlled;

            prms[7] = new SqlParameter("@IsIncreasedControlled", SqlDbType.Bit);
            prms[7].Value = IsIncreasedControlled;
            
            prms[8] = new SqlParameter("@IsInput", SqlDbType.Bit);
            prms[8].Value = IsInput;

            prms[9] = new SqlParameter("@IsPublic", SqlDbType.Bit);
            prms[9].Value = IsPublic;

            prms[10] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            prms[10].Value = QuestionTypeID;

            prms[11] = new SqlParameter("@NumberCopies", SqlDbType.Int);
            prms[11].Value = NumberCopies;

            prms[12] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[12].Value = HeadID;

            prms[13] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[13].Value = WorkerID;

            prms[14] = new SqlParameter("@PublicContent", SqlDbType.NVarChar, -1);
            prms[14].Value = PublicContent;
            
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


            DataTable dt = SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetLastNumberList", sps).Tables[0];

            if (dt.Rows.Count > 0)
            {
                List<int> numbers = new List<int>();
                foreach (DataRow row in dt.Rows)
                {
                    string numStr = (string) row["Number"];
                    if (!String.IsNullOrEmpty(numStr) && !Regex.IsMatch(numStr, @"\D+"))
                    {
                        int num;
                        int.TryParse(numStr, out num);
                        numbers.Add(num);
                    }
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

            SqlParameter[] sps = new SqlParameter[14];
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

            sps[9] = new SqlParameter("@ExternalNumber", SqlDbType.NVarChar, 50);
            sps[9].Value = pageSettings.Where.HasRule("ExternalNumber")
                                ? pageSettings.Where.GetRule("ExternalNumber").Data
                                : String.Empty;

            sps[10] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[10].Value = departmentID;

            sps[11] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            int documentCodeID;
            if (pageSettings.Where.HasRule("DocumentCodeID") && Int32.TryParse(pageSettings.Where.GetRule("DocumentCodeID").Data, out documentCodeID))
            {
                sps[11].Value = documentCodeID;
            }
            else
                sps[11].Value = DBNull.Value;

            sps[12] = new SqlParameter("@Content", SqlDbType.NVarChar);
            sps[12].Value = pageSettings.Where.HasRule("Content")
                                ? pageSettings.Where.GetRule("Content").Data
                                : String.Empty;

            sps[13] = new SqlParameter("@OrganizationName", SqlDbType.NVarChar, 256);
            sps[13].Value = pageSettings.Where.HasRule("OrganizationName")
                                ? pageSettings.Where.GetRule("OrganizationName").Data
                                : String.Empty;

            DataTable dt = SPHelper.ExecuteDataset(trans, "usp_DocTemplate_Page", sps).Tables[0];

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
                    row.id = (int)dr["DocTemplateID"];
                    row.cell = new string[21];

                    row.cell[0] = dr["DocTemplateID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime) dr["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);

                    row.cell[3] = (string)dr["Number"];
                    row.cell[4] = dr["ExternalNumber"].ToString();

                    row.cell[5] = (string)dr["Content"];
                    row.cell[6] = (string)dr["Changes"];
                    row.cell[7] = (string)dr["Notes"];
                    row.cell[8] = dr["IsControlled"].ToString();
                    row.cell[9] = dr["IsSpeciallyControlled"].ToString();
                    row.cell[10] = dr["IsIncreasedControlled"].ToString();
                    row.cell[11] = dr["IsInput"].ToString();
                    row.cell[12] = dr["DocTypeID"].ToString();
                    row.cell[13] = (string)dr["DocTypeName"];
                    row.cell[14] = dr["DocStatusID"].ToString();
                    row.cell[15] = (string)dr["DocStatusName"];
                    row.cell[16] = dr["DocumentCodeID"].ToString();
                    row.cell[17] = dr["QuestionTypeID"].ToString();
                    row.cell[18] = (string)dr["QuestionTypeName"];
                    row.cell[19] = dr["OrganizationID"].ToString();
                    row.cell[20] = DBNull.Value == dr["OrganizationName"] ? String.Empty : (string)dr["OrganizationName"];
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


        public static DocTemplate GetByDocumentID(SqlTransaction trans, int documentID, string userName)
        {
            DocTemplate dt = null;
            if (!CanView(userName))
            {
                throw new AccessException(userName, "Init");
            }
            
            SqlParameter[] prms = new SqlParameter[15];
            prms[0] = new SqlParameter("@DocTemplateID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = documentID;

            prms[2] = new SqlParameter("@Content", SqlDbType.NVarChar, -1);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@Changes", SqlDbType.NVarChar, -1);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@DocTypeID", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@IsControlled", SqlDbType.Bit);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Bit);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@IsIncreasedControlled", SqlDbType.Bit);
            prms[7].Direction = ParameterDirection.Output;
            
            prms[8] = new SqlParameter("@IsInput", SqlDbType.Bit);
            prms[8].Direction = ParameterDirection.Output;
            prms[8].IsNullable = true;

            prms[9] = new SqlParameter("@IsPublic", SqlDbType.Bit);
            prms[9].Direction = ParameterDirection.Output;

            prms[10] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            prms[10].Direction = ParameterDirection.Output;

            prms[11] = new SqlParameter("@NumberCopies", SqlDbType.Int);
            prms[11].Direction = ParameterDirection.Output;

            prms[12] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[12].Direction = ParameterDirection.Output;

            prms[13] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[13].Direction = ParameterDirection.Output;

            prms[14] = new SqlParameter("@PublicContent", SqlDbType.NVarChar, -1);
            prms[14].Direction = ParameterDirection.Output;


            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.GetByDocumentID, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.GetByDocumentID, prms);

            if (prms[0].Value != DBNull.Value)
            {
                dt = new DocTemplate(userName);
                dt.ID = (int) prms[0].Value;
                dt.DocumentID = documentID;
                dt.Content = (string) prms[2].Value;
                dt.Changes = (string) prms[3].Value;
                dt.DocTypeID = (int) prms[4].Value;

                dt.IsControlled = (bool) prms[5].Value;
                dt.IsSpeciallyControlled = (bool)prms[6].Value;
                dt.IsIncreasedControlled = (bool)prms[7].Value;
                dt.IsInput = (bool) prms[8].Value;
                dt.IsPublic = (bool) prms[9].Value;

                dt.QuestionTypeID = (int) prms[10].Value;
                dt.NumberCopies = (int) prms[11].Value;
                dt.HeadID = (int) prms[12].Value;
                dt.WorkerID = (int) prms[13].Value;
                dt.PublicContent = (string) prms[14].Value;
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

        #region Reports

        public static DataSet GetControlled(SqlTransaction trans, int departmentID, int executiveDepartmentID, int documentCodeID, int docTypeID, int questionTypeID, DateTime currentDate, DateTime endDate)
        {
            SqlParameter[] sps = new SqlParameter[7];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[1].Value = executiveDepartmentID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[2].Value = documentCodeID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@DocTypeID", SqlDbType.Int);
            if (docTypeID > 0)
                sps[3].Value = docTypeID;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            if (questionTypeID > 0)
                sps[4].Value = questionTypeID;
            else
                sps[4].Value = DBNull.Value;

            sps[5] = new SqlParameter("@CurrentDate", SqlDbType.Date);
            if (currentDate >= SqlDateTime.MinValue && currentDate <= SqlDateTime.MaxValue)
                sps[5].Value = currentDate;
            else
                sps[5].Value = SqlDateTime.MinValue;

            sps[6] = new SqlParameter("@EndDate", SqlDbType.Date);
            if (endDate > SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[6].Value = endDate;
            else
                sps[6].Value = SqlDateTime.MaxValue;


            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetControlled", sps);
        }

        public static DataSet GetNotDone(SqlTransaction trans, int departmentID, int executiveDepartmentID, int documentCodeID, int docTypeID, int questionTypeID, DateTime currentDate)
        {
            SqlParameter[] sps = new SqlParameter[6];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[1].Value = executiveDepartmentID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[2].Value = documentCodeID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@DocTypeID", SqlDbType.Int);
            if (docTypeID > 0)
                sps[3].Value = docTypeID;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            if (questionTypeID > 0)
                sps[4].Value = questionTypeID;
            else
                sps[4].Value = DBNull.Value;

            sps[5] = new SqlParameter("@CurrentDate", SqlDbType.Date);
            if (currentDate >= SqlDateTime.MinValue && currentDate <= SqlDateTime.MaxValue)
                sps[5].Value = currentDate;
            else
                sps[5].Value = SqlDateTime.MinValue;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetNotDone", sps);
        }

        public static DataSet GetMadeLate(SqlTransaction trans, int departmentID, int executiveDepartmentID, int documentCodeID, int docTypeID, int questionTypeID, DateTime fromEndDate, DateTime toEndDate)
        {
            SqlParameter[] sps = new SqlParameter[7];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[1].Value = executiveDepartmentID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[2].Value = documentCodeID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@DocTypeID", SqlDbType.Int);
            if (docTypeID > 0)
                sps[3].Value = docTypeID;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            if (questionTypeID > 0)
                sps[4].Value = questionTypeID;
            else
                sps[4].Value = DBNull.Value;

            sps[5] = new SqlParameter("@FromEndDate", SqlDbType.Date);
            if (fromEndDate >= SqlDateTime.MinValue && fromEndDate <= SqlDateTime.MaxValue)
                sps[5].Value = fromEndDate;
            else
                sps[5].Value = SqlDateTime.MinValue;

            sps[6] = new SqlParameter("@ToEndDate", SqlDbType.Date);
            if (toEndDate > SqlDateTime.MinValue && toEndDate <= SqlDateTime.MaxValue)
                sps[6].Value = toEndDate;
            else
                sps[6].Value = SqlDateTime.MaxValue;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetMadeLate", sps);
        }

        public static DataSet GetCalendarPlan(SqlTransaction trans, int departmentID, int executiveDepartmentID, int documentCodeID, int docTypeID, int questionTypeID, int workerID, DateTime endDate)
        {
            SqlParameter[] sps = new SqlParameter[7];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[1].Value = executiveDepartmentID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[2].Value = documentCodeID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@DocTypeID", SqlDbType.Int);
            if (docTypeID > 0)
                sps[3].Value = docTypeID;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@QuestionTypeID", SqlDbType.Int);
            if (questionTypeID > 0)
                sps[4].Value = questionTypeID;
            else
                sps[4].Value = DBNull.Value;

            sps[5] = new SqlParameter("@WorkerID", SqlDbType.Int);
            if (workerID > 0)
                sps[5].Value = workerID;
            else
                sps[5].Value = DBNull.Value;

            sps[6] = new SqlParameter("@EndDate", SqlDbType.Date);
            if (endDate > SqlDateTime.MinValue && endDate < SqlDateTime.MaxValue)
                sps[6].Value = endDate;
            else
                sps[6].Value = DateTime.Now;
            
            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetCalendarPlan", sps);
        }

        public static DataSet GetStatistics(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int documentCodeID)
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
            if (endDate > SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[3].Value = documentCodeID;
            else
                sps[3].Value = DBNull.Value;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetStatistics", sps);
        }
        public static DataSet GetStatistics(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate)
        {
            return GetStatistics(trans, departmentID, startDate, endDate, 0);
        }

        public static int[] GetNumberDocuments(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int documentCodeID)
        {
            SqlParameter[] sps = new SqlParameter[6];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate > SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID >= 0)
                sps[3].Value = documentCodeID;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@Number", SqlDbType.Int);
            sps[4].Direction = ParameterDirection.Output;

            sps[5] = new SqlParameter("@NumberCopies", SqlDbType.Int);
            sps[5].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, "usp_DocTemplate_GetNumberDocuments", sps);

            return new[] {(int) sps[4].Value, (int) sps[5].Value};
        }
        public static int[] GetNumberDocuments(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate)
        {
            return GetNumberDocuments(trans, departmentID, startDate, endDate, -1);
        }

        public static int GetNumberResponse(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate, int documentCodeID)
        {
            SqlParameter[] sps = new SqlParameter[5];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            if (startDate >= SqlDateTime.MinValue && startDate <= SqlDateTime.MaxValue)
                sps[1].Value = startDate;
            else
                sps[1].Value = SqlDateTime.MinValue;

            sps[2] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            if (endDate > SqlDateTime.MinValue && endDate <= SqlDateTime.MaxValue)
                sps[2].Value = endDate;
            else
                sps[2].Value = SqlDateTime.MaxValue;

            sps[3] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID >= 0)
                sps[3].Value = documentCodeID;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@Number", SqlDbType.Int);
            sps[4].Direction = ParameterDirection.Output;


            SPHelper.ExecuteNonQuery(trans, "usp_DocTemplate_GetNumberResponseDocuments", sps);

            return (int)sps[4].Value;
        }
        public static int GetNumberResponse(SqlTransaction trans, int departmentID, DateTime startDate, DateTime endDate)
        {
            return GetNumberResponse(trans, departmentID, startDate, endDate, -1);
        }



        public static DataSet GetNumberControlledType(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime startDateFrom, DateTime startDateTo)
        {
            SqlParameter[] sps = new SqlParameter[5];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[1].Value = documentCodeID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[2].Value = executiveDepartmentID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@StartDateFrom", SqlDbType.DateTime);
            if (startDateFrom > SqlDateTime.MinValue && startDateFrom < SqlDateTime.MaxValue)
                sps[3].Value = startDateFrom;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@StartDateTo", SqlDbType.DateTime);
            if (startDateTo > SqlDateTime.MinValue && startDateTo < SqlDateTime.MaxValue)
                sps[4].Value = startDateTo;
            else
                sps[4].Value = DBNull.Value;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetNumberControlledType", sps);
        }
        public static DataSet GetNumberControlledType(SqlTransaction trans, int departmentID, int documentCodeID, DateTime startDateFrom, DateTime startDateTo)
        {
            return GetNumberControlledType(trans, departmentID, documentCodeID, 0, startDateFrom, startDateTo);
        }
        public static DataSet GetNumberControlledType(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID)
        {
            return GetNumberControlledType(trans, departmentID, documentCodeID, executiveDepartmentID, (DateTime)SqlDateTime.MinValue, (DateTime)SqlDateTime.MinValue);
        }
        public static DataSet GetNumberControlledType(SqlTransaction trans, int departmentID, int documentCodeID)
        {
            return GetNumberControlledType(trans, departmentID, documentCodeID, 0);
        }
        public static DataSet GetNumberControlledType(SqlTransaction trans, int departmentID)
        {
            return GetNumberControlledType(trans, departmentID, 0);
        }


        public static DataSet GetNumberControlled(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime startDateFrom, DateTime startDateTo)
        {
            SqlParameter[] sps = new SqlParameter[5];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[1].Value = documentCodeID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[2].Value = executiveDepartmentID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@StartDateFrom", SqlDbType.DateTime);
            if (startDateFrom > SqlDateTime.MinValue && startDateFrom < SqlDateTime.MaxValue)
                sps[3].Value = startDateFrom;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@StartDateTo", SqlDbType.DateTime);
            if (startDateTo > SqlDateTime.MinValue && startDateTo < SqlDateTime.MaxValue)
                sps[4].Value = startDateTo;
            else
                sps[4].Value = DBNull.Value;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetNumberControlled", sps);
        }
        public static DataSet GetNumberControlled(SqlTransaction trans, int departmentID, int documentCodeID, DateTime startDateFrom, DateTime startDateTo)
        {
            return GetNumberControlled(trans, departmentID, documentCodeID, 0, startDateFrom, startDateTo);
        }
        public static DataSet GetNumberControlled(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID)
        {
            return GetNumberControlled(trans, departmentID, documentCodeID, executiveDepartmentID, (DateTime)SqlDateTime.MinValue, (DateTime)SqlDateTime.MinValue);
        }
        public static DataSet GetNumberControlled(SqlTransaction trans, int departmentID, int documentCodeID)
        {
            return GetNumberControlled(trans, departmentID, documentCodeID, 0);
        }
        public static DataSet GetNumberControlled(SqlTransaction trans, int departmentID)
        {
            return GetNumberControlled(trans, departmentID, 0);
        }

        public static DataSet GetNumberControlledExecuted(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime startDateFrom, DateTime startDateTo)
        {
            SqlParameter[] sps = new SqlParameter[5];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[1].Value = documentCodeID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[2].Value = executiveDepartmentID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@StartDateFrom", SqlDbType.DateTime);
            if (startDateFrom > SqlDateTime.MinValue && startDateFrom < SqlDateTime.MaxValue)
                sps[3].Value = startDateFrom;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@StartDateTo", SqlDbType.DateTime);
            if (startDateTo > SqlDateTime.MinValue && startDateTo < SqlDateTime.MaxValue)
                sps[4].Value = startDateTo;
            else
                sps[4].Value = DBNull.Value;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetNumberControlledExecuted", sps);
        }
        public static DataSet GetNumberControlledExecuted(SqlTransaction trans, int departmentID, int documentCodeID)
        {
            return GetNumberControlledExecuted(trans, departmentID, documentCodeID, 0, (DateTime)SqlDateTime.MinValue, (DateTime)SqlDateTime.MinValue);
        }
        public static DataSet GetNumberControlledExecuted(SqlTransaction trans, int departmentID, int documentCodeID, DateTime startDateFrom, DateTime startDateTo)
        {
            return GetNumberControlledExecuted(trans, departmentID, documentCodeID, 0, startDateFrom, startDateTo);
        }
        public static DataSet GetNumberControlledExecuted(SqlTransaction trans, int departmentID)
        {
            return GetNumberControlledExecuted(trans, departmentID, 0);
        }

        public static DataSet GetNumberControlledExecuteInTime(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime startDateFrom, DateTime startDateTo, DateTime currentDate)
        {
            SqlParameter[] sps = new SqlParameter[6];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[1].Value = documentCodeID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[2].Value = executiveDepartmentID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@StartDateFrom", SqlDbType.DateTime);
            if (startDateFrom > SqlDateTime.MinValue && startDateFrom < SqlDateTime.MaxValue)
                sps[3].Value = startDateFrom;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@StartDateTo", SqlDbType.DateTime);
            if (startDateTo > SqlDateTime.MinValue && startDateTo < SqlDateTime.MaxValue)
                sps[4].Value = startDateTo;
            else
                sps[4].Value = DBNull.Value;

            sps[5] = new SqlParameter("@CurrentDate", SqlDbType.DateTime);
            if (currentDate > SqlDateTime.MinValue && currentDate < SqlDateTime.MaxValue)
                sps[5].Value = currentDate;
            else
                sps[5].Value = DateTime.Now;
            
            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetNumberControlledExecuteInTime", sps);
        }
        public static DataSet GetNumberControlledExecuteInTime(SqlTransaction trans, int departmentID, int documentCodeID, DateTime startDateFrom, DateTime startDateTo, DateTime currentDate)
        {
            return GetNumberControlledExecuteInTime(trans, departmentID, documentCodeID, 0, startDateFrom, startDateTo, currentDate);
        }
        public static DataSet GetNumberControlledExecuteInTime(SqlTransaction trans, int departmentID, DateTime startDateFrom, DateTime startDateTo, DateTime currentDate)
        {
            return GetNumberControlledExecuteInTime(trans, departmentID, 0, startDateFrom, startDateTo, currentDate);
        }
        public static DataSet GetNumberControlledExecuteInTime(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime startDateFrom, DateTime startDateTo)
        {
            return GetNumberControlledExecuteInTime(trans, departmentID, documentCodeID, executiveDepartmentID, startDateFrom, startDateTo, DateTime.Now);
        }
        public static DataSet GetNumberControlledExecuteInTime(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime currentDate)
        {
            return GetNumberControlledExecuteInTime(trans, departmentID, documentCodeID, executiveDepartmentID, (DateTime)SqlDateTime.MinValue, (DateTime)SqlDateTime.MinValue, currentDate);
        }
        public static DataSet GetNumberControlledExecuteInTime(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID)
        {
            return GetNumberControlledExecuteInTime(trans, departmentID, documentCodeID, executiveDepartmentID, DateTime.Now);
        }
        public static DataSet GetNumberControlledExecuteInTime(SqlTransaction trans, int departmentID, int documentCodeID)
        {
            return GetNumberControlledExecuteInTime(trans, departmentID, documentCodeID, 0);
        }
        public static DataSet GetNumberControlledExecuteInTime(SqlTransaction trans, int departmentID)
        {
            return GetNumberControlledExecuteInTime(trans, departmentID, 0);
        }

        public static DataSet GetNumberControlledContinued(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime startDateFrom, DateTime startDateTo)
        {
            SqlParameter[] sps = new SqlParameter[5];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[1].Value = documentCodeID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[2].Value = executiveDepartmentID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@StartDateFrom", SqlDbType.DateTime);
            if (startDateFrom > SqlDateTime.MinValue && startDateFrom < SqlDateTime.MaxValue)
                sps[3].Value = startDateFrom;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@StartDateTo", SqlDbType.DateTime);
            if (startDateTo > SqlDateTime.MinValue && startDateTo < SqlDateTime.MaxValue)
                sps[4].Value = startDateTo;
            else
                sps[4].Value = DBNull.Value;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetNumberControlledContinued", sps);
        }
        public static DataSet GetNumberControlledContinued(SqlTransaction trans, int departmentID, int documentCodeID)
        {
            return GetNumberControlledContinued(trans, departmentID, documentCodeID, 0, (DateTime)SqlDateTime.MinValue, (DateTime)SqlDateTime.MinValue);
        }
        public static DataSet GetNumberControlledContinued(SqlTransaction trans, int departmentID, int documentCodeID, DateTime startDateFrom, DateTime startDateTo)
        {
            return GetNumberControlledContinued(trans, departmentID, documentCodeID, 0, startDateFrom, startDateTo);
        }
        public static DataSet GetNumberControlledContinued(SqlTransaction trans, int departmentID)
        {
            return GetNumberControlledContinued(trans, departmentID, 0);
        }

        public static DataSet GetNumberControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime startDateFrom, DateTime startDateTo, DateTime currentDate)
        {
            SqlParameter[] sps = new SqlParameter[6];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[1].Value = documentCodeID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[2].Value = executiveDepartmentID;
            else
                sps[2].Value = DBNull.Value;

            sps[3] = new SqlParameter("@StartDateFrom", SqlDbType.DateTime);
            if (startDateFrom > SqlDateTime.MinValue && startDateFrom < SqlDateTime.MaxValue)
                sps[3].Value = startDateFrom;
            else
                sps[3].Value = DBNull.Value;

            sps[4] = new SqlParameter("@StartDateTo", SqlDbType.DateTime);
            if (startDateTo > SqlDateTime.MinValue && startDateTo < SqlDateTime.MaxValue)
                sps[4].Value = startDateTo;
            else
                sps[4].Value = DBNull.Value;

            sps[5] = new SqlParameter("@CurrentDate", SqlDbType.DateTime);
            if (currentDate > SqlDateTime.MinValue && currentDate < SqlDateTime.MaxValue)
                sps[5].Value = currentDate;
            else
                sps[5].Value = DateTime.Now;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetNumberControlledOverdue", sps);
        }
        public static DataSet GetNumberControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID, DateTime startDateFrom, DateTime startDateTo, DateTime currentDate)
        {
            return GetNumberControlledOverdue(trans, departmentID, documentCodeID, 0, startDateFrom, startDateTo, currentDate);
        }
        public static DataSet GetNumberControlledOverdue(SqlTransaction trans, int departmentID, DateTime startDateFrom, DateTime startDateTo, DateTime currentDate)
        {
            return GetNumberControlledOverdue(trans, departmentID, 0, startDateFrom, startDateTo, currentDate);
        }
        public static DataSet GetNumberControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime startDateFrom, DateTime startDateTo)
        {
            return GetNumberControlledOverdue(trans, departmentID, documentCodeID, executiveDepartmentID, startDateFrom, startDateTo, DateTime.Now);
        }
        public static DataSet GetNumberControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime currentDate)
        {
            return GetNumberControlledOverdue(trans, departmentID, documentCodeID, executiveDepartmentID, (DateTime)SqlDateTime.MinValue, (DateTime)SqlDateTime.MinValue, currentDate);
        }
        public static DataSet GetNumberControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID)
        {
            return GetNumberControlledOverdue(trans, departmentID, documentCodeID, executiveDepartmentID, DateTime.Now);
        }
        public static DataSet GetNumberControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID)
        {
            return GetNumberControlledOverdue(trans, departmentID, documentCodeID, 0);
        }
        public static DataSet GetNumberControlledOverdue(SqlTransaction trans, int departmentID)
        {
            return GetNumberControlledOverdue(trans, departmentID, 0);
        }

        public static DataSet GetControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID, DateTime currentDate)
        {
            SqlParameter[] sps = new SqlParameter[4];

            sps[0] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[0].Value = departmentID;

            sps[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            if (documentCodeID > 0)
                sps[1].Value = documentCodeID;
            else
                sps[1].Value = DBNull.Value;

            sps[2] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            if (executiveDepartmentID > 0)
                sps[2].Value = executiveDepartmentID;
            else
                sps[2].Value = DBNull.Value;
            
            sps[3] = new SqlParameter("@CurrentDate", SqlDbType.DateTime);
            if (currentDate > SqlDateTime.MinValue && currentDate < SqlDateTime.MaxValue)
                sps[3].Value = currentDate;
            else
                sps[3].Value = DateTime.Now;

            return SPHelper.ExecuteDataset(trans, "usp_DocTemplate_GetControlledOverdue", sps);
        }
        public static DataSet GetControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID, int executiveDepartmentID)
        {
            return GetControlledOverdue(trans, departmentID, documentCodeID, executiveDepartmentID, DateTime.Now);
        }
        public static DataSet GetControlledOverdue(SqlTransaction trans, int departmentID, int documentCodeID)
        {
            return GetControlledOverdue(trans, departmentID, documentCodeID, 0);
        }
        public static DataSet GetControlledOverdue(SqlTransaction trans, int departmentID)
        {
            return GetControlledOverdue(trans, departmentID, 0);
        }
        #endregion

        #endregion

    }
}