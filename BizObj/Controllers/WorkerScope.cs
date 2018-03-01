using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using BizObj.Data;
using BizObj.Document;
using BizObj.Models.Document;
using BizObj.Models.Helpers;
using BizObj.Models.JqGrid;
using BizObj.Models.Pager;
using Newtonsoft.Json;

namespace BizObj.Controllers
{
    public class WorkerScope
    {
        private readonly HttpContext _context;
        private string UserName { get; set; }//TODO: нужно заменить использование UserName на UserID
        private Worker Worker { get; set; }
        private Guid UserId { get; set; }

        public WorkerScope(HttpContext context, Guid userId, Worker worker, string userName)
        {
            _context = context;
            UserId = userId;
            Worker = worker;
            UserName = userName;
        }

        public void ParceRequest()
        {
            HttpRequest r = _context.Request;
            HttpResponse res = _context.Response;
            string type = r["type"];
            string jdata = r["jdata"];
            string rResult = String.Empty;


            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "getblank":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                rResult = GetDocTemplateBlank(id);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "jdata");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("jdata", "Inuput id is not valid");
                        }
                        break;
                    case "getpage":
                        int departmentId = Convert.ToInt32(r["departmentID"]);
                        
                        PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

                        DataTable dtPage = WorkerDocumets.GetPage(gridSettings, UserId, Worker.DepartmentID);
                        JqGridResults jqGridResults = WorkerDocumets.BuildPageResults(dtPage, gridSettings);

                        rResult = new JavaScriptSerializer().Serialize(jqGridResults);
                        break;
                    case "getoutputpage":
                        rResult = GetOutputPage(r);
                        break;
                    case "getdraftpage":
                        rResult = GetDraftPage(r);
                        break;
                    case "getreplaypage":
                        rResult = GetReplayPage(r);
                        break;
                    case "draft":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            DocTemplate docTemplate = null;
                            try
                            {
                                docTemplate = JsonConvert.DeserializeObject<DocTemplate>(jdata, new DateTimeConvertorSql());
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }

                            if (docTemplate.ID == 0)
                            {
                                ProcessingResult pr = InsertDocTemplate(docTemplate);
                                rResult = new JavaScriptSerializer().Serialize(pr);
                            }
                            else
                            {
                                ProcessingResult pr = UpdateDocTemplate(docTemplate);
                                rResult = new JavaScriptSerializer().Serialize(pr);
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("jdata", "Inuput id is not valid");
                        }
                        break;
                    case "del":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                            {
                                ProcessingResult pr = DeleteDocTemplate(id);
                                rResult = new JavaScriptSerializer().Serialize(pr);
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "jdata");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("jdata", "Inuput id is not valid");
                        }
                        break;
                    case "getstpage":
                        rResult = GetStatementsPage(r);
                        break;
                    case "getstblank":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                rResult = GetDocStatementBlank(id);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "jdata");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("jdata", "Inuput id is not valid");
                        }
                        break;
                    case "getstcount":
                        int departmentId2 = Convert.ToInt32(r["dep"]);
                        rResult = WorkerDocumets.GetCountNotOpenByDocStatement(UserId, departmentId2).ToString();
                        break;
                    case "getdtcount":
                        int departmentId3 = Convert.ToInt32(r["dep"]);
                        rResult = WorkerDocumets.GetCountNotOpenByDocTemplate(UserId, departmentId3).ToString();
                        break;
                    case "getstconcount":
                        int departmentId4 = Convert.ToInt32(r["dep"]);
                        rResult = WorkerDocumets.GetCountControlledByDocStatement(UserId, departmentId4).ToString();
                        break;
                    case "getdtconcount":
                        int departmentId5 = Convert.ToInt32(r["dep"]);
                        rResult = WorkerDocumets.GetCountControlledByDocTemplate(UserId, departmentId5).ToString();
                        break;
                    case "openworker":
                        int controlCardId = 0;
                        int documentId = 0;
                        if (String.IsNullOrWhiteSpace(r["DocumentID"])) {
                            throw new ArgumentNullException("DocumentID", "Inuput controlCardId is not valid");
                        } else {
                            documentId = Convert.ToInt32(r["DocumentID"]);
                        }
                        if (!String.IsNullOrWhiteSpace(r["ControlCardID"])) {
                            controlCardId = Convert.ToInt32(r["ControlCardID"]);
                        }

                        if (documentId > 0) {
                            rResult = SetOpenWorker(documentId, controlCardId).ToString();
                        } else {
                            throw new ArgumentException("Inuput DocumentID is not valid", "DocumentID");
                        }
                        break;
                }
                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    res.Write(rResult);
                }
            }
        }
        

        private string GetDocTemplateBlank(int id)
        {
            DocTemplateWorkerBlank docTemplate;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Worker worker = new Worker(trans, UserId);

                    docTemplate = new DocTemplateWorkerBlank(trans, id, worker.ID, UserName);

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

            return new JavaScriptSerializer().Serialize(docTemplate);
        }

        private string GetOutputPage(HttpRequest r)
        {
            int departmentId = Convert.ToInt32(r["departmentID"]);

            PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);
                        
            DataTable dtPage = WorkerDocumets.GetOutputPage(gridSettings, UserId, departmentId);

            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            if (dtPage != null)
                foreach (DataRow dr in dtPage.Rows)
                {
                    JqGridRow row = new JqGridRow();
                    row.id = (int)dr["DocTemplateID"];
                    row.cell = new string[20];

                    row.cell[0] = dr["DocTemplateID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime)dr["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);

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

                    string destination = String.Empty;

                    if (DBNull.Value != dr["OrganizationID"] && (int)dr["OrganizationID"] > 0)
                    {
                        destination = (string)dr["OrganizationName"];
                    }
                    if (DBNull.Value != dr["DepartmentID"] && (int)dr["DepartmentID"] > 0)
                    {
                        destination = destination == String.Empty ? destination : ", ";
                        destination = destination + (string)dr["DepartmentName"];
                    }
                    if (DBNull.Value != dr["WorkerID"] && (int)dr["WorkerID"] > 0)
                    {
                        destination = destination == String.Empty ? destination : ", ";
                        destination = destination + FormatHelper.FormatToLastNameAndInitials((string)dr["WorkerLastName"],
                                                                                  (string)dr["WorkerFirstName"],
                                                                                  (string)dr["WorkerMiddleName"]);
                    }

                    row.cell[19] = destination;

                    rows.Add(row);
                }
            result.rows = rows.ToArray();
            result.page = gridSettings.PageIndex;
            if (gridSettings.TotalRecords % gridSettings.PageSize == 0)
                result.total = gridSettings.TotalRecords / gridSettings.PageSize;
            else
                result.total = gridSettings.TotalRecords / gridSettings.PageSize + 1;
            result.records = gridSettings.TotalRecords;

            return new JavaScriptSerializer().Serialize(result);
        }

        private string GetDraftPage(HttpRequest r)
        {
            int departmentId = Convert.ToInt32(r["departmentID"]);

            PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

            DataTable dtPage = WorkerDocumets.GetDraftPage(gridSettings, UserId, departmentId);

            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            if (dtPage != null)
                foreach (DataRow dr in dtPage.Rows)
                {
                    JqGridRow row = new JqGridRow();
                    row.id = (int)dr["DocTemplateID"];
                    row.cell = new string[20];

                    row.cell[0] = dr["DocTemplateID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime)dr["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);

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

                    string destination = String.Empty;

                    if (DBNull.Value != dr["OrganizationID"] && (int) dr["OrganizationID"] > 0)
                    {
                        destination = (string) dr["OrganizationName"];
                    }
                    if (DBNull.Value != dr["DepartmentID"] && (int)dr["DepartmentID"] > 0)
                    {
                        destination = destination == String.Empty ? destination : ", ";
                        destination = destination + (string)dr["DepartmentName"];
                    }
                    if (DBNull.Value != dr["WorkerID"] && (int)dr["WorkerID"] > 0)
                    {
                        destination = destination == String.Empty ? destination : ", ";
                        destination = destination + FormatHelper.FormatToLastNameAndInitials((string)dr["WorkerLastName"],
                                                                                  (string)dr["WorkerFirstName"],
                                                                                  (string)dr["WorkerMiddleName"]);
                    }

                    row.cell[19] = destination;

                    rows.Add(row);
                }
            result.rows = rows.ToArray();
            result.page = gridSettings.PageIndex;
            if (gridSettings.TotalRecords % gridSettings.PageSize == 0)
                result.total = gridSettings.TotalRecords / gridSettings.PageSize;
            else
                result.total = gridSettings.TotalRecords / gridSettings.PageSize + 1;
            result.records = gridSettings.TotalRecords;

            return new JavaScriptSerializer().Serialize(result);
        }

        private string GetReplayPage(HttpRequest r)
        {
            int documentId = Convert.ToInt32(r["documentId"]);

            PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

            DataTable dtPage = WorkerDocumets.GetReplayPage(gridSettings, UserId, documentId);

            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            if (dtPage != null)
                foreach (DataRow dr in dtPage.Rows)
                {
                    JqGridRow row = new JqGridRow();
                    row.id = (int)dr["DocTemplateID"];
                    row.cell = new string[20];

                    row.cell[0] = dr["DocTemplateID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime)dr["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);

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

                    string destination = String.Empty;

                    if (DBNull.Value != dr["OrganizationID"] && (int)dr["OrganizationID"] > 0)
                    {
                        destination = (string)dr["OrganizationName"];
                    }
                    if (DBNull.Value != dr["DepartmentID"] && (int)dr["DepartmentID"] > 0)
                    {
                        destination = destination == String.Empty ? destination : ", ";
                        destination = destination + (string)dr["DepartmentName"];
                    }
                    if (DBNull.Value != dr["WorkerID"] && (int)dr["WorkerID"] > 0)
                    {
                        destination = destination == String.Empty ? destination : ", ";
                        destination = destination + FormatHelper.FormatToLastNameAndInitials((string)dr["WorkerLastName"],
                                                                                  (string)dr["WorkerFirstName"],
                                                                                  (string)dr["WorkerMiddleName"]);
                    }

                    row.cell[19] = destination;

                    rows.Add(row);
                }
            result.rows = rows.ToArray();
            result.page = gridSettings.PageIndex;
            if (gridSettings.TotalRecords % gridSettings.PageSize == 0)
                result.total = gridSettings.TotalRecords / gridSettings.PageSize;
            else
                result.total = gridSettings.TotalRecords / gridSettings.PageSize + 1;
            result.records = gridSettings.TotalRecords;

            return new JavaScriptSerializer().Serialize(result);
        }

        private string GetStatementsPage(HttpRequest r)
        {
            int documentId = Convert.ToInt32(r["documentId"]);

            PageSettings pageSettings = JqGridSettings.GetPageSettings(_context);

            DataTable dataTable = WorkerDocumets.GetStatementsPage(pageSettings, UserId, Worker.DepartmentID);

            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            if (dataTable != null)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    JqGridRow row = new JqGridRow();
                    row.id = (int) dr["DocStatementID"];
                    row.cell = new string[20];

                    row.cell[0] = dr["DocStatementID"].ToString();
                    row.cell[1] = dr["DocumentID"].ToString();

                    row.cell[2] = ((DateTime) dr["CreationDate"]).ToString("yyyy-MM-dd");

                    row.cell[3] = FormatHelper.FormatToLastNameAndInitials((string) dr["CitizenLastName"],
                        (string) dr["CitizenFirstName"],
                        (string) dr["CitizenMiddleName"]);

                    row.cell[4] = String.Format("{0} {1} {2}", dr["CitizenLastName"], dr["CitizenFirstName"],
                        dr["CitizenMiddleName"]);

                    if ((string) dr["CityObjectTypeShortName"] == "н")
                        row.cell[5] = (string) dr["Address"];
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

                    row.cell[15] = dr["ControlCardID"].ToString();
                    row.cell[16] = dr["OpenWorker"].ToString();
                    row.cell[17] = dr["HasSubCards"].ToString();
                    row.cell[18] = dr["IsControlled"].ToString();
                    if (DBNull.Value == dr["ControlEndDate"]) {
                        row.cell[19] = String.Empty;
                    } else {
                        row.cell[19] = ((DateTime)dr["ControlEndDate"]).ToString("yyyy-MM-dd");
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

            return new JavaScriptSerializer().Serialize(result);
        }

        private ProcessingResult InsertDocTemplate(DocTemplate docTemplate)
        {
            ProcessingResult pr = new ProcessingResult();
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Worker worker = new Worker(trans, UserId);
                    Post post = new Post(trans, worker.PostID, UserName);

                    docTemplate.Document.UserName = UserName;
                    docTemplate.Document.OriginatorID = UserId;
                    docTemplate.Document.OwnerID = UserId;
                    docTemplate.Document.RequestorID = UserId;

                    docTemplate.Document.CreationDate = DateTime.Now;
                    docTemplate.Document.RequestDate = DateTime.Now;
                    docTemplate.Document.DocStatusID = 0;
                    docTemplate.Document.Insert(trans);

                    
                    docTemplate.Document.Destination.UserName = UserName;
                    docTemplate.Document.Destination.ID = docTemplate.Document.ID;
                    if (docTemplate.Document.Destination.OrganizationID <= 0 && !String.IsNullOrWhiteSpace(docTemplate.Document.Destination.OrganizationName))
                    {
                        Organization org = new Organization(UserName);
                        org.Name = docTemplate.Document.Destination.OrganizationName;
                        org.OrganizationTypeID = 2;
                        org.DepartmentID = Worker.DepartmentID;
                        org.WorkerID = Worker.ID;
                        org.Insert(trans);
                        docTemplate.Document.Destination.OrganizationID = org.ID;

                    }
                    DateTime ddt = docTemplate.Document.Destination.CreationDate;
                    if (docTemplate.Document.Destination != null && (ddt < (DateTime)SqlDateTime.MinValue || ddt > (DateTime)SqlDateTime.MaxValue))
                    {
                        docTemplate.Document.Destination.CreationDate = DateTime.Now;
                    }
                    docTemplate.Document.Destination.Insert(trans);


                    docTemplate.Document.Source.UserName = UserName;
                    docTemplate.Document.Source.ID = docTemplate.Document.ID;
                    docTemplate.Document.Source.WorkerID = worker.ID;
                    docTemplate.Document.Source.DepartmentID = post.DepartmentID;
                    DateTime sdt = docTemplate.Document.Source.CreationDate;
                    if (docTemplate.Document.Source != null && (sdt < (DateTime)SqlDateTime.MinValue || sdt > (DateTime)SqlDateTime.MaxValue))
                    {
                        docTemplate.Document.Source.CreationDate = DateTime.Now;
                    }
                    docTemplate.Document.Source.Insert(trans);

                    docTemplate.UserName = UserName;
                    docTemplate.DocumentID = docTemplate.Document.ID;
                    docTemplate.Insert(trans);

                    foreach (DocumentFile df in docTemplate.Document.Files)
                    {
                        if (df.FileID > 0)
                        {
                            df.DocumentID = docTemplate.DocumentID;
                            df.Worker = Worker;
                            df.Insert(trans);
                        }
                    }

                    trans.Commit();
                    pr.Success = true;
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.Rollback();
                    pr.Message = e.Message;
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
            pr.Data = docTemplate;
            return pr;
        }

        private ProcessingResult UpdateDocTemplate(DocTemplate docTemplate)
        {
            ProcessingResult pr = new ProcessingResult();
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    
                    Worker worker = new Worker(trans, UserId);
                    Post post = new Post(trans, worker.PostID, UserName);

                    docTemplate.Document.UserName = UserName;
                    docTemplate.Document.OriginatorID = UserId;
                    docTemplate.Document.OwnerID = UserId;
                    docTemplate.Document.RequestorID = UserId;

                    docTemplate.Document.CreationDate = DateTime.Now;
                    docTemplate.Document.RequestDate = DateTime.Now;
                    docTemplate.Document.Update(trans);


                    docTemplate.Document.Destination.UserName = UserName;
                    if (docTemplate.Document.Destination.OrganizationID <= 0 && !String.IsNullOrWhiteSpace(docTemplate.Document.Destination.OrganizationName))
                    {
                        Organization org = new Organization(UserName);
                        org.Name = docTemplate.Document.Destination.OrganizationName;
                        org.OrganizationTypeID = 2;
                        org.DepartmentID = Worker.DepartmentID;
                        org.WorkerID = Worker.ID;
                        org.Insert(trans);
                        docTemplate.Document.Destination.OrganizationID = org.ID;
                    }
                    DateTime ddt = docTemplate.Document.Destination.CreationDate;
                    if (docTemplate.Document.Destination != null && (ddt < (DateTime)SqlDateTime.MinValue || ddt > (DateTime)SqlDateTime.MaxValue))
                    {
                        docTemplate.Document.Destination.CreationDate = DateTime.Now;
                    }
                    if (docTemplate.Document.DocStateID == 0)
                    {
                        docTemplate.Document.Destination.CreationDate = DateTime.Now;
                    }
                    docTemplate.Document.Destination.Update(trans);


                    docTemplate.Document.Source.UserName = UserName;
                    DateTime sdt = docTemplate.Document.Source.CreationDate;
                    if (docTemplate.Document.Source != null && (sdt < (DateTime)SqlDateTime.MinValue || sdt > (DateTime)SqlDateTime.MaxValue))
                    {
                        docTemplate.Document.Source.CreationDate = DateTime.Now;
                    }
                    if (docTemplate.Document.DocStateID == 0)
                    {
                        docTemplate.Document.Source.CreationDate = DateTime.Now;
                    }
                    docTemplate.Document.Source.Update(trans);

                    docTemplate.UserName = UserName;
                    docTemplate.Update(trans);

                    foreach (DocumentFile df in docTemplate.Document.Files)
                    {
                        if (df.FileID > 0)
                        {
                            df.DocumentID = docTemplate.DocumentID;
                            df.Worker = Worker;
                            df.Insert(trans);
                        }
                    }

                    trans.Commit();
                    pr.Success = true;
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.Rollback();
                    pr.Message = e.Message;
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
            pr.Data = docTemplate;
            return pr;
        }

        private ProcessingResult DeleteDocTemplate(int id)
        {
            ProcessingResult pr = new ProcessingResult();
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DocTemplate docTemplate = new DocTemplate(trans, id, UserName);

                    DocTemplate.Delete(trans, docTemplate.ID, UserName);

                    Destination.Delete(trans, docTemplate.DocumentID, UserName);
                    Source.Delete(trans, docTemplate.DocumentID, UserName);

                    DocumentFile.DeleteFiles(trans, docTemplate.DocumentID, Worker);

                    Document.Document.Delete(trans, docTemplate.DocumentID, UserName);

                    trans.Commit();

                    pr.Success = true;
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.Rollback();

                    pr.Message = e.Message;
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }
            return pr;
        }



        private string GetDocStatementBlank(int id)
        {
            DocStatementWorkerBlank docStatement;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Worker worker = new Worker(trans, UserId);

                    docStatement = new DocStatementWorkerBlank(trans, id, worker.ID, UserName);

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

            return new JavaScriptSerializer().Serialize(docStatement);
        }

        private string SetOpenWorker(int documentId, int controlCardId)
        {
            ControlCard card;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    if (documentId > 0) {

                        SqlParameter[] prms = new SqlParameter[4];
                        prms[0] = new SqlParameter("@DocumentActionID", SqlDbType.Int);
                        prms[0].Direction = ParameterDirection.Output;

                        prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
                        prms[1].Value = documentId;

                        prms[2] = new SqlParameter("@WorkerID", SqlDbType.Int);
                        prms[2].Value = Worker.ID;

                        prms[3] = new SqlParameter("@DocumentActionTypeID", SqlDbType.Int);
                        prms[3].Value = 1;

                        SPHelper.ExecuteNonQuery("usp_DocumentAction_Insert", prms);
                    }
                    else
                    {
                        throw new CustomException.CustomException("Null documentId");
                    }


                    if (controlCardId > 0) {
                        card = new ControlCard(trans, controlCardId, UserName);

                        if (card.WorkerID == Worker.ID || card.FixedWorkerID == Worker.ID) {
                            ControlCard.SetOpenWorker(card.ID, Worker.ID, UserName);
                        } else {
                            //throw new CustomException.CustomException("Not worker card");
                        }
                    } else {
                        card = new ControlCard();
                    }

                    trans.Commit();
                } catch (Exception) {
                    if (trans != null)
                        trans.Rollback();
                    throw;
                }
            }
            finally
            {
                connection.Close();
            }

            return new JavaScriptSerializer().Serialize(card);
        }
    }
}
