using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Helpers;
using BizObj.Models.JqGrid;
using BizObj.Models.Pager;
using PermissionMembership;

namespace BizObj.Document
{
    public class WorkerDocumets
    {
        private struct SpNames
        {
            public const string Page = "usp_WorkerDocuments_Page";
            public const string OutputPage = "usp_WorkerDocuments_OutputPage";
            public const string DraftPage = "usp_WorkerDocuments_DraftPage";
            public const string ReplayPage = "usp_WorkerDocuments_ReplayPage";
            public const string StatementsPage = "usp_WorkerStatements_Page";

            public const string GetCountNotOpenByDocTemplate = "usp_ControlCard_GetCountNotOpenWorkerByDocTemplate";
            public const string GetCountNotOpenByDocStatement = "usp_ControlCard_GetCountNotOpenWorkerByDocStatement";

            public const string GetCountControlledByDocTemplate = "usp_WorkerDocuments_GetCountWorkerControlledByDocTemplate";
            public const string GetCountControlledByDocStatement = "usp_WorkerDocuments_GetCountWorkerControlledByDocStatement";
        }

        #region Static Public Methods

        public static DataTable GetPage(SqlTransaction trans, PageSettings pageSettings, Guid userId, int departmentId)
        {
            Department department = new Department(trans, departmentId);
            Worker worker = new Worker(trans, userId);
            /*if (!Permission.IsUserPermission(trans, userId, department.ObjectID, (int)Department.ActionType.View))
            {
                throw new AccessException(userId.ToString(), "Get document template");
            }*/
            //worker.PostID

            SqlParameter[] sps = new SqlParameter[20];
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
                sps[5].Value = DateTime.Parse(pageSettings.Where.GetRule("CreationDate").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[5].Value = DBNull.Value;

            SqlDateTime cdStart = SqlDateTime.MinValue;
            if (pageSettings.Where.HasRule("CreationDateStart"))
                cdStart = DateTime.Parse(pageSettings.Where.GetRule("CreationDateStart").Data,
                                         CultureInfo.CurrentCulture);
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
            sps[10].Value = departmentId;

            sps[11] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            int documentCodeId;
            if (pageSettings.Where.HasRule("DocumentCodeID") &&
                Int32.TryParse(pageSettings.Where.GetRule("DocumentCodeID").Data, out documentCodeId))
            {
                sps[11].Value = documentCodeId;
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

            sps[14] = new SqlParameter("@InnerNumber", SqlDbType.NVarChar, 50);
            sps[14].Value = pageSettings.Where.HasRule("InnerNumber")
                               ? pageSettings.Where.GetRule("InnerNumber").Data
                               : String.Empty;

            sps[15] = new SqlParameter("@ParentDestinationNumber", SqlDbType.NVarChar, 50);
            sps[15].Value = pageSettings.Where.HasRule("ParentDestinationNumber")
                               ? pageSettings.Where.GetRule("ParentDestinationNumber").Data
                               : String.Empty;

            sps[16] = new SqlParameter("@Controlled", SqlDbType.Bit);
            bool controlled = false;
            if (pageSettings.Where.HasRule("Controlled")) {
                Boolean.TryParse(pageSettings.Where.GetRule("Controlled").Data, out controlled);
            }

            if (controlled) {
                sps[16].Value = true;
            } else {
                sps[16].Value = DBNull.Value;
            }

            sps[17] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[17].Value = worker.ID;

            sps[18] = new SqlParameter("@OpenWorker", SqlDbType.Bit);
            if (pageSettings.Where.HasRule("OpenWorker")) {
                bool openWorker = false;
                Boolean.TryParse(pageSettings.Where.GetRule("OpenWorker").Data, out openWorker);
                sps[18].Value = openWorker;
            } else {
                sps[18].Value = DBNull.Value;
            }

            sps[19] = new SqlParameter("@IsInput", SqlDbType.Bit);
            if (pageSettings.Where.HasRule("IsInput")) {
                bool isInput = false;
                Boolean.TryParse(pageSettings.Where.GetRule("IsInput").Data, out isInput);
                sps[19].Value = isInput;
            } else {
                sps[19].Value = DBNull.Value;
            }

            DataTable dt = SPHelper.ExecuteDataset(trans, SpNames.Page, sps).Tables[0];

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;
            
            return dt;
        }

        public static DataTable GetPage(PageSettings pageSettings, Guid userId, int departmentId)
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

                    dt = GetPage(trans, pageSettings, userId, departmentId);

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

        public static DataTable GetOutputPage(SqlTransaction trans, PageSettings pageSettings, Guid userId, int departmentId)
        {
            Department department = new Department(trans, departmentId);
            Worker worker = new Worker(trans, userId);
            /*if (!Permission.IsUserPermission(trans, userId, department.ObjectID, (int)Department.ActionType.View))
            {
                throw new AccessException(userId.ToString(), "Get document template");
            }*/
            //worker.PostID
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
                sps[5].Value = DateTime.Parse(pageSettings.Where.GetRule("CreationDate").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[5].Value = DBNull.Value;

            SqlDateTime cdStart = SqlDateTime.MinValue;
            if (pageSettings.Where.HasRule("CreationDateStart"))
                cdStart = DateTime.Parse(pageSettings.Where.GetRule("CreationDateStart").Data,
                                         CultureInfo.CurrentCulture);
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
            sps[9].Value = departmentId;

            sps[10] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            int documentCodeId;
            if (pageSettings.Where.HasRule("DocumentCodeID") &&
                Int32.TryParse(pageSettings.Where.GetRule("DocumentCodeID").Data, out documentCodeId))
            {
                sps[10].Value = documentCodeId;
            }
            else
                sps[10].Value = DBNull.Value;

            sps[11] = new SqlParameter("@Content", SqlDbType.NVarChar);
            sps[11].Value = pageSettings.Where.HasRule("Content")
                                ? pageSettings.Where.GetRule("Content").Data
                                : String.Empty;

            sps[12] = new SqlParameter("@OrganizationName", SqlDbType.NVarChar, 256);
            sps[12].Value = pageSettings.Where.HasRule("OrganizationName")
                                ? pageSettings.Where.GetRule("OrganizationName").Data
                                : String.Empty;

            sps[13] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[13].Value = worker.ID;


            DataTable dt = SPHelper.ExecuteDataset(trans, SpNames.OutputPage, sps).Tables[0];

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;

            return dt;
        }

        public static DataTable GetOutputPage(PageSettings pageSettings, Guid userId, int departmentId)
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

                    dt = GetOutputPage(trans, pageSettings, userId, departmentId);

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

        public static DataTable GetDraftPage(SqlTransaction trans, PageSettings pageSettings, Guid userId, int departmentId)
        {
            Department department = new Department(trans, departmentId);
            Worker worker = new Worker(trans, userId);
            /*if (!Permission.IsUserPermission(trans, userId, department.ObjectID, (int)Department.ActionType.View))
            {
                throw new AccessException(userId.ToString(), "Get document template");
            }*/
            //worker.PostID
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
                sps[5].Value = DateTime.Parse(pageSettings.Where.GetRule("CreationDate").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[5].Value = DBNull.Value;

            SqlDateTime cdStart = SqlDateTime.MinValue;
            if (pageSettings.Where.HasRule("CreationDateStart"))
                cdStart = DateTime.Parse(pageSettings.Where.GetRule("CreationDateStart").Data,
                                         CultureInfo.CurrentCulture);
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
            sps[9].Value = departmentId;

            sps[10] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            int documentCodeId;
            if (pageSettings.Where.HasRule("DocumentCodeID") &&
                Int32.TryParse(pageSettings.Where.GetRule("DocumentCodeID").Data, out documentCodeId))
            {
                sps[10].Value = documentCodeId;
            }
            else
                sps[10].Value = DBNull.Value;

            sps[11] = new SqlParameter("@Content", SqlDbType.NVarChar);
            sps[11].Value = pageSettings.Where.HasRule("Content")
                                ? pageSettings.Where.GetRule("Content").Data
                                : String.Empty;

            sps[12] = new SqlParameter("@OrganizationName", SqlDbType.NVarChar, 256);
            sps[12].Value = pageSettings.Where.HasRule("OrganizationName")
                                ? pageSettings.Where.GetRule("OrganizationName").Data
                                : String.Empty;

            sps[13] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[13].Value = worker.ID;


            DataTable dt = SPHelper.ExecuteDataset(trans, SpNames.DraftPage, sps).Tables[0];

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;

            return dt;
        }

        public static DataTable GetDraftPage(PageSettings pageSettings, Guid userId, int departmentId)
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

                    dt = GetDraftPage(trans, pageSettings, userId, departmentId);

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

        public static DataTable GetReplayPage(SqlTransaction trans, PageSettings pageSettings, Guid userId, int parentDocumentId)
        {
            SqlParameter[] sps = new SqlParameter[6];
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

            sps[5] = new SqlParameter("@ParentDocumentID", SqlDbType.Int);
            sps[5].Value = parentDocumentId;
            
            DataTable dt = SPHelper.ExecuteDataset(trans, SpNames.ReplayPage, sps).Tables[0];

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;

            return dt;
        }

        public static DataTable GetReplayPage(PageSettings pageSettings, Guid userId, int parentDocumentId)
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

                    dt = GetReplayPage(trans, pageSettings, userId, parentDocumentId);

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

        public static JqGridResults BuildPageResults(DataTable dataTable, PageSettings pageSettings)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            if (dataTable != null)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    bool isInput = (bool)dr["IsInput"];

                    JqGridRow row = new JqGridRow();
                    row.id = (int)dr["DocTemplateID"];
                    row.cell = new string[29];

                    row.cell[0] = dr["DocTemplateID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime)dr["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);

                    row.cell[3] = isInput ? dr["DestinationNumber"].ToString() : dr["SourceNumber"].ToString();
                    row.cell[4] = isInput ? dr["SourceNumber"].ToString() : dr["DestinationNumber"].ToString();

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

                    string destination = String.Empty;

                    if (isInput)
                    {
                        if (DBNull.Value != dr["SourceOrganizationID"] && (int)dr["SourceOrganizationID"] > 0)
                        {
                            destination = (string)dr["SourceOrganizationName"];
                        }
                        if (DBNull.Value != dr["SourceDepartmentID"] && (int)dr["SourceDepartmentID"] > 0)
                        {
                            destination = destination == String.Empty ? destination : destination + ", ";
                            destination = destination + (string)dr["SourceDepartmentName"];
                        }
                        if (DBNull.Value != dr["SourceWorkerID"] && (int)dr["SourceWorkerID"] > 0)
                        {
                            destination = destination == String.Empty ? destination : destination + ", ";
                            destination = destination +
                                          FormatHelper.FormatToLastNameAndInitials((string)dr["SourceWorkerLastName"],
                                                                                   (string)dr["SourceWorkerFirstName"],
                                                                                   (string)dr["SourceWorkerMiddleName"]);
                        }
                    }
                    else
                    {
                        if (DBNull.Value != dr["DestinationOrganizationID"] && (int)dr["DestinationOrganizationID"] > 0)
                        {
                            destination = (string)dr["DestinationOrganizationName"];
                        }
                        if (DBNull.Value != dr["DestinationDepartmentID"] && (int)dr["DestinationDepartmentID"] > 0)
                        {
                            destination = destination == String.Empty ? destination : destination + ", ";
                            destination = destination + (string)dr["DestinationDepartmentName"];
                        }
                        if (DBNull.Value != dr["DestinationWorkerID"] && (int)dr["DestinationWorkerID"] > 0)
                        {
                            destination = destination == String.Empty ? destination : destination + ", ";
                            destination = destination +
                                          FormatHelper.FormatToLastNameAndInitials((string)dr["DestinationWorkerLastName"],
                                                                                   (string)dr["DestinationWorkerFirstName"],
                                                                                   (string)dr["DestinationWorkerMiddleName"]);
                        }
                    }

                    row.cell[19] = destination;
                    row.cell[20] = dr["DepartmentID"].ToString();
                    row.cell[21] = dr["SourceDepartmentID"].ToString();
                    row.cell[22] = dr["DestinationDepartmentID"].ToString();
                    row.cell[23] = DBNull.Value != dr["InnerNumber"] ? dr["InnerNumber"].ToString() : String.Empty;
                    row.cell[24] = DBNull.Value != dr["ControlCardID"] ? dr["ControlCardID"].ToString() : String.Empty;
                    row.cell[25] = DBNull.Value != dr["ParentDestinationNumber"] ? dr["ParentDestinationNumber"].ToString() : String.Empty;

                    row.cell[26] = dr["OpenWorker"].ToString();
                    row.cell[27] = dr["HasSubCards"].ToString();
                    if (DBNull.Value == dr["ControlEndDate"]) {
                        row.cell[28] = String.Empty;
                    } else {
                        row.cell[28] = ((DateTime)dr["ControlEndDate"]).ToString("yyyy-MM-dd");
                    }

                    rows.Add(row);
                }
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


        public static DataTable GetStatementsPage(SqlTransaction trans, PageSettings pageSettings, Guid userId, int departmentId)
        {
            Department department = new Department(trans, departmentId);
            Worker worker = new Worker(trans, userId);

            SqlParameter[] sps = new SqlParameter[19];
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
                sps[5].Value = SqlDateTime.Parse(pageSettings.Where.GetRule("CreationDate").Data);
            else
                sps[5].Value = DBNull.Value;

            sps[6] = new SqlParameter("@CreationDateStart", SqlDbType.DateTime);
            sps[6].Value = pageSettings.Where.HasRule("CreationDateStart")
                               ? SqlDateTime.Parse(pageSettings.Where.GetRule("CreationDateStart").Data)
                               : SqlDateTime.MinValue;

            sps[7] = new SqlParameter("@CreationDateEnd", SqlDbType.DateTime);
            sps[7].Value = pageSettings.Where.HasRule("CreationDateEnd")
                               ? SqlDateTime.Parse(pageSettings.Where.GetRule("CreationDateEnd").Data)
                               : SqlDateTime.MaxValue;

            sps[8] = new SqlParameter("@CitizenLastName", SqlDbType.NVarChar, 50);
            sps[8].Value = pageSettings.Where.HasRule("CitizenLastName")
                               ? pageSettings.Where.GetRule("CitizenLastName").Data
                               : String.Empty;

            sps[9] = new SqlParameter("@HeadLastName", SqlDbType.NVarChar, 50);
            sps[9].Value = pageSettings.Where.HasRule("HeadLastName")
                                ? pageSettings.Where.GetRule("HeadLastName").Data
                                : String.Empty;

            sps[10] = new SqlParameter("@CityObjectName", SqlDbType.NVarChar, 50);
            sps[10].Value = pageSettings.Where.HasRule("CityObjectName")
                                ? pageSettings.Where.GetRule("CityObjectName").Data
                                : String.Empty;

            sps[11] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            sps[11].Value = pageSettings.Where.HasRule("Number")
                                ? pageSettings.Where.GetRule("Number").Data
                                : String.Empty;

            sps[12] = new SqlParameter("@ExternalNumber", SqlDbType.NVarChar, 50);
            sps[12].Value = pageSettings.Where.HasRule("ExternalNumber")
                                ? pageSettings.Where.GetRule("ExternalNumber").Data
                                : String.Empty;

            sps[13] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            sps[13].Value = departmentId;

            sps[14] = new SqlParameter("@Content", SqlDbType.NVarChar);
            sps[14].Value = pageSettings.Where.HasRule("Content")
                                ? pageSettings.Where.GetRule("Content").Data
                                : String.Empty;

            sps[15] = new SqlParameter("@Controlled", SqlDbType.Bit);
            bool controlled = false;
            if (pageSettings.Where.HasRule("Controlled")) {
                Boolean.TryParse(pageSettings.Where.GetRule("Controlled").Data, out controlled);
            }

            if (controlled) {
                sps[15].Value = true;
            } else {
                sps[15].Value = DBNull.Value;
            }

            sps[16] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[16].Value = worker.ID;

            sps[17] = new SqlParameter("@InnerNumber", SqlDbType.NVarChar, 50);
            sps[17].Value = pageSettings.Where.HasRule("InnerNumber")
                               ? pageSettings.Where.GetRule("InnerNumber").Data
                               : String.Empty;

            sps[18] = new SqlParameter("@OpenWorker", SqlDbType.Bit);
            if (pageSettings.Where.HasRule("OpenWorker")) {
                bool openWorker = false;
                Boolean.TryParse(pageSettings.Where.GetRule("OpenWorker").Data, out openWorker);
                sps[18].Value = openWorker;
            } else {
                sps[18].Value = DBNull.Value;
            }

            DataTable dt = SPHelper.ExecuteDataset(trans, SpNames.StatementsPage, sps).Tables[0];

            pageSettings.TotalRecords = sps[4].Value != DBNull.Value ? Convert.ToInt32(sps[4].Value) : 0;


            return dt;
        }

        public static DataTable GetStatementsPage(PageSettings pageSettings, Guid userId, int departmentId)
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

                    dt = GetStatementsPage(trans, pageSettings, userId, departmentId);

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
        


        public static int GetCountNotOpenByDocTemplate(SqlTransaction trans, Guid userId, int departmentId)
        {
            Department department = new Department(trans, departmentId);
            Worker worker = new Worker(trans, userId);

            SqlParameter[] sps = new SqlParameter[2];
            sps[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[0].Value = worker.ID;

            sps[1] = new SqlParameter("@Count", SqlDbType.Int);
            sps[1].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, SpNames.GetCountNotOpenByDocTemplate, sps);

            return sps[1].Value != DBNull.Value ? (int)sps[1].Value : 0;
        }

        public static int GetCountNotOpenByDocTemplate(Guid userId, int departmentId)
        {
            int count = 0;
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    count = GetCountNotOpenByDocTemplate(trans, userId, departmentId);

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

            return count;
        }

        public static int GetCountNotOpenByDocStatement(SqlTransaction trans, Guid userId, int departmentId)
        {
            Department department = new Department(trans, departmentId);
            Worker worker = new Worker(trans, userId);

            SqlParameter[] sps = new SqlParameter[2];
            sps[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[0].Value = worker.ID;

            sps[1] = new SqlParameter("@Count", SqlDbType.Int);
            sps[1].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, SpNames.GetCountNotOpenByDocStatement, sps);

            return sps[1].Value != DBNull.Value ? (int)sps[1].Value : 0;
        }

        public static int GetCountNotOpenByDocStatement(Guid userId, int departmentId)
        {
            int count = 0;
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    count = GetCountNotOpenByDocStatement(trans, userId, departmentId);

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

            return count;
        }



        public static int GetCountControlledByDocTemplate(SqlTransaction trans, Guid userId, int departmentId)
        {
            Department department = new Department(trans, departmentId);
            Worker worker = new Worker(trans, userId);

            SqlParameter[] sps = new SqlParameter[2];
            sps[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[0].Value = worker.ID;

            sps[1] = new SqlParameter("@Count", SqlDbType.Int);
            sps[1].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, SpNames.GetCountControlledByDocTemplate, sps);

            return sps[1].Value != DBNull.Value ? (int)sps[1].Value : 0;
        }

        public static int GetCountControlledByDocTemplate(Guid userId, int departmentId)
        {
            int count = 0;
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    count = GetCountControlledByDocTemplate(trans, userId, departmentId);

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

            return count;
        }

        public static int GetCountControlledByDocStatement(SqlTransaction trans, Guid userId, int departmentId)
        {
            Department department = new Department(trans, departmentId);
            Worker worker = new Worker(trans, userId);

            SqlParameter[] sps = new SqlParameter[2];
            sps[0] = new SqlParameter("@WorkerID", SqlDbType.Int);
            sps[0].Value = worker.ID;

            sps[1] = new SqlParameter("@Count", SqlDbType.Int);
            sps[1].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, SpNames.GetCountControlledByDocStatement, sps);

            return sps[1].Value != DBNull.Value ? (int)sps[1].Value : 0;
        }

        public static int GetCountControlledByDocStatement(Guid userId, int departmentId)
        {
            int count = 0;
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    count = GetCountControlledByDocStatement(trans, userId, departmentId);

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

            return count;
        }

        #endregion
    }
}
