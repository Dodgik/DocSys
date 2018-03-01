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
    public class AdminDocumets
    {
        private struct SpNames
        {
            public const string Page = "usp_AdminDocuments_Page";
            public const string ReplayPage = "usp_AdminDocuments_ReplayPage";
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
            
            SqlParameter[] sps = new SqlParameter[25];
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
                Int32.TryParse(pageSettings.Where.GetRule("DocumentCodeID").Data, out documentCodeId)) {
                sps[11].Value = documentCodeId;
            } else {
                sps[11].Value = DBNull.Value;
            }

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

            sps[15] = new SqlParameter("@Controlled", SqlDbType.Bit);
            bool controlled = false;
            if (pageSettings.Where.HasRule("Controlled")) {
                Boolean.TryParse(pageSettings.Where.GetRule("Controlled").Data, out controlled);
            }

            if (controlled) {
                sps[15].Value = true;
            } else
                sps[15].Value = DBNull.Value;

            sps[16] = new SqlParameter("@IsInput", SqlDbType.Bit);
            if (pageSettings.Where.HasRule("IsInput"))
            {
                bool isInput = false;
                Boolean.TryParse(pageSettings.Where.GetRule("IsInput").Data, out isInput);
                sps[16].Value = isInput;
            }
            else
            {
                sps[16].Value = DBNull.Value;
            }


            sps[17] = new SqlParameter("@EndDateFrom", SqlDbType.DateTime);
            sps[17].IsNullable = true;
            if (pageSettings.Where.HasRule("EndDateFrom"))
                sps[17].Value = DateTime.Parse(pageSettings.Where.GetRule("EndDateFrom").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[17].Value = DBNull.Value;

            sps[18] = new SqlParameter("@EndDateTo", SqlDbType.DateTime);
            sps[18].IsNullable = true;
            if (pageSettings.Where.HasRule("EndDateTo"))
                sps[18].Value = DateTime.Parse(pageSettings.Where.GetRule("EndDateTo").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[18].Value = DBNull.Value;

            sps[19] = new SqlParameter("@IsDepartmentOwner", SqlDbType.Bit);
            sps[19].IsNullable = true;
            if (pageSettings.Where.HasRule("IsDepartmentOwner"))
                sps[19].Value = (pageSettings.Where.GetRule("IsDepartmentOwner").Data.ToLower() == "true");
            else
                sps[19].Value = DBNull.Value;

            sps[20] = new SqlParameter("@ControlledInner", SqlDbType.Bit);
            sps[20].IsNullable = true;
            if (pageSettings.Where.HasRule("ControlledInner"))
                sps[20].Value = (pageSettings.Where.GetRule("ControlledInner").Data.ToLower() == "true");
            else
                sps[20].Value = DBNull.Value;

            sps[21] = new SqlParameter("@InnerEndDateFrom", SqlDbType.DateTime);
            sps[21].IsNullable = true;
            if (pageSettings.Where.HasRule("InnerEndDateFrom"))
                sps[21].Value = DateTime.Parse(pageSettings.Where.GetRule("InnerEndDateFrom").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[21].Value = DBNull.Value;

            sps[22] = new SqlParameter("@InnerEndDateTo", SqlDbType.DateTime);
            sps[22].IsNullable = true;
            if (pageSettings.Where.HasRule("InnerEndDateTo"))
                sps[22].Value = DateTime.Parse(pageSettings.Where.GetRule("InnerEndDateTo").Data,
                                              CultureInfo.CurrentCulture);
            else
                sps[22].Value = DBNull.Value;

            sps[23] = new SqlParameter("@LableID", SqlDbType.Int);
            int lableId;
            if (pageSettings.Where.HasRule("LableID") &&
                Int32.TryParse(pageSettings.Where.GetRule("LableID").Data, out lableId)) {
                sps[23].Value = lableId;
            } else {
                sps[23].Value = DBNull.Value;
            }

            sps[24] = new SqlParameter("@DocStatusID", SqlDbType.Int);
            int docStatusId;
            if (pageSettings.Where.HasRule("DocStatusID") &&
                Int32.TryParse(pageSettings.Where.GetRule("DocStatusID").Data, out docStatusId))
            {
                sps[24].Value = docStatusId;
            } else {
                sps[24].Value = DBNull.Value;
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

        public static JqGridResults BuildJqGridResults(DataTable dataTable, PageSettings pageSettings)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            if (dataTable != null)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    bool isInput = (bool) dr["IsInput"];

                    JqGridRow row = new JqGridRow();
                    row.id = (int) dr["DocTemplateID"];
                    row.cell = new string[25];

                    row.cell[0] = dr["DocTemplateID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime) dr["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);

                    row.cell[3] = isInput ? dr["DestinationNumber"].ToString() : dr["SourceNumber"].ToString();
                    row.cell[4] = isInput ? dr["SourceNumber"].ToString() : dr["DestinationNumber"].ToString();

                    row.cell[5] = (string) dr["Content"];
                    row.cell[6] = (string) dr["Changes"];
                    row.cell[7] = (string) dr["Notes"];
                    row.cell[8] = dr["IsControlled"].ToString();
                    row.cell[9] = dr["IsSpeciallyControlled"].ToString();
                    row.cell[10] = dr["IsIncreasedControlled"].ToString();
                    row.cell[11] = dr["IsInput"].ToString();
                    row.cell[12] = dr["DocTypeID"].ToString();
                    row.cell[13] = (string) dr["DocTypeName"];
                    row.cell[14] = dr["DocStatusID"].ToString();
                    row.cell[15] = (string) dr["DocStatusName"];
                    row.cell[16] = dr["DocumentCodeID"].ToString();
                    row.cell[17] = dr["QuestionTypeID"].ToString();
                    row.cell[18] = (string) dr["QuestionTypeName"];

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
                    } else {
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

                    rows.Add(row);
                }
            }
            result.rows = rows.ToArray();
            result.page = pageSettings.PageIndex;
            if (pageSettings.TotalRecords%pageSettings.PageSize == 0)
                result.total = pageSettings.TotalRecords/pageSettings.PageSize;
            else
                result.total = pageSettings.TotalRecords/pageSettings.PageSize + 1;
            result.records = pageSettings.TotalRecords;

            return result;
        }


        #endregion
    }
}
