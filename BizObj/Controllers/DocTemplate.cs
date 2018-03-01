using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;
using BizObj.Data;
using BizObj.Document;
using BizObj.Models.Document;
using BizObj.Models.Helpers;
using BizObj.Models.JqGrid;
using BizObj.Models.Pager;
using BizObj.Models.Report;
using ESCommon;
using ESCommon.Rtf;
using Newtonsoft.Json;

namespace BizObj.Controllers
{
    public class DocTemplateController
    {
        private readonly HttpContext _context;
        private Guid UserId { get; set; }
        private Worker Worker { get; set; }
        private string UserName { get; set; }

        public DocTemplateController(HttpContext context, Guid userId, Worker worker)
        {
            _context = context;
            UserId = userId;
            Worker = worker;
        }
        public DocTemplateController(HttpContext context, Guid userId, Worker worker, string userName)
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

            DocTemplate docTemplate = null;

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetDocTemplateObjAsync(id);
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
                    case "getblank":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetDocTemplateBlank(id);
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
                    case "getadminblank":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetDocTemplateAdminBlank(id);
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
                    case "getlist":

                        bool isReception = Convert.ToBoolean(r["isReception"]);
                        int departmentID = Convert.ToInt32(r["departmentID"]);
                        GetDocTemplateDataAsync(isReception, departmentID);
                        break;
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            try
                            {
                                //JavaScriptSerializer serializer2 = new JavaScriptSerializer();
                                //JavaScriptSerializer serializer2 = ExtendedJavaScriptConverter<DocTemplate>.GetSerializer();
                                //docTemplate = serializer2.Deserialize<DocTemplate>(jdata);
                                docTemplate = JsonConvert.DeserializeObject<DocTemplate>(jdata, new DateTimeConvertorCustome());
                                //serializer2.RegisterConverters(new[] {new DateTimeJavaScriptConverter()});
                                //docTemplate = new JavaScriptSerializer().Deserialize<DocTemplate>(jdata);
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            string errorMsg;
                            if (IsValidDocTemplate(docTemplate, out errorMsg))
                            {
                                int docTemplateID = InsertDocTemplateAsync(docTemplate);
                                res.Write(docTemplateID);
                                res.Flush();
                            }
                            else
                            {
                                throw new CustomException.CustomException(errorMsg);
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("jdata", "Inuput id is not valid");
                        }
                        break;
                    case "upd":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            try
                            {
                                //docTemplate = new JavaScriptSerializer().Deserialize<DocTemplate>(jdata);
                                docTemplate = JsonConvert.DeserializeObject<DocTemplate>(jdata, new DateTimeConvertorCustome());
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            string errorMsg;
                            if (IsValidDocTemplate(docTemplate, out errorMsg))
                            {
                                ProcessingResult pr = UpdateDocTemplateAsync(docTemplate);
                                rResult = new JavaScriptSerializer().Serialize(pr);
                            }
                            else
                            {
                                throw new CustomException.CustomException(errorMsg);
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
                                ProcessingResult pr = DeleteDocTemplateAsync(id);
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
                    case "rep":
                        ProcessDocTReportAsync();
                        break;
                    case "getnextnumber":
                        int depID = Convert.ToInt32(r["dep"]);
                        int documentCodeID = Convert.ToInt32(r["code"]);
                        int number = DocTemplate.GetNextNumber(UserName, depID, documentCodeID);
                        rResult = number.ToString();
                        break;
                }
                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }

        private bool IsValidDocTemplate(DocTemplate docTemplate, out string msg)
        {
            msg = String.Empty;
            if (docTemplate == null)
                return false;

            if (docTemplate.Document == null)
                return false;

            if (String.IsNullOrWhiteSpace(docTemplate.Document.Number))
            {
                msg = "Не вказаний номер документа";
                return false;
            }
            DateTime dcd = docTemplate.Document.CreationDate;
            if (dcd < (DateTime)SqlDateTime.MinValue || dcd > (DateTime)SqlDateTime.MaxValue)
            {
                msg = "Не вказана коректна дата документа";
                return false;
            }
            if (docTemplate.Document.Source != null && String.IsNullOrWhiteSpace(docTemplate.Document.Source.Number) && docTemplate.IsInput)
            {
                msg = "Не вказаний номер документа";
                return false;
            }
            DateTime sDate = docTemplate.Document.Source.CreationDate;
            if ((sDate < (DateTime)SqlDateTime.MinValue || sDate > (DateTime)SqlDateTime.MaxValue) && docTemplate.IsInput)
            {
                msg = "Не вказана коректна дата документа в організації";
                return false;
            }
            return true;
        }

        private void GetDocTemplateObjAsync(int id)
        {
            HttpResponse response = _context.Response;
            DocTemplate docTemplate;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    docTemplate = new DocTemplate(trans, id, UserName);

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

            string output = new JavaScriptSerializer().Serialize(docTemplate);

            response.Write(output);
        }

        private void GetDocTemplateBlank(int id)
        {
            HttpResponse response = _context.Response;
            DocTemplateBlank docTemplateB;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    docTemplateB = new DocTemplateBlank(trans, id, UserName);

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

            string output = new JavaScriptSerializer().Serialize(docTemplateB);

            response.Write(output);
        }

        private void GetDocTemplateAdminBlank(int id)
        {
            HttpResponse response = _context.Response;
            DocTemplateAdminBlank docTemplateB;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    Worker worker = new Worker(trans, UserId);
                    int departmentId = Post.GetDepartmentID(trans, worker.PostID);

                    docTemplateB = new DocTemplateAdminBlank(trans, id, departmentId, UserName);

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

            string output = new JavaScriptSerializer().Serialize(docTemplateB);

            response.Write(output);
        }

        private void GetDocTemplateDataAsync(bool isReception, int departmentID)
        {
            HttpResponse response = _context.Response;

            PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

            DataTable dtPage = DocTemplate.GetPage(gridSettings, UserName, departmentID);
            JqGridResults jqGridResults = DocTemplate.BuildJqGridResults(dtPage, gridSettings);

            string output = new JavaScriptSerializer().Serialize(jqGridResults);
            response.Write(output);
        }

        private int InsertDocTemplateAsync(DocTemplate docTemplate)
        {
            int docTemplateID = 0;
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    MembershipUser mu = Membership.GetUser();
                    docTemplate.Document.UserName = UserName;
                    if (mu != null)
                    {
                        if (mu.ProviderUserKey != null)
                        {
                            docTemplate.Document.OriginatorID = (Guid)mu.ProviderUserKey;
                            docTemplate.Document.OwnerID = (Guid)mu.ProviderUserKey;
                            docTemplate.Document.RequestorID = (Guid)mu.ProviderUserKey;
                        }
                    }
                    docTemplate.Document.RequestDate = DateTime.Now;
                    docTemplate.Document.DocStatusID = 0;
                    docTemplate.Document.Insert(trans);

                    foreach (int labelId in docTemplate.Document.Labels)
                    {
                        DocumentLabel label = new DocumentLabel(UserName);
                        label.LabelID = labelId;
                        label.DocumentID = docTemplate.Document.ID;
                        label.DepartmentID = docTemplate.Document.DepartmentID;
                        label.WorkerID = Worker.ID;
                        label.Insert(trans);
                    }

                    DateTime sDate = docTemplate.Document.Source.CreationDate;
                    if (docTemplate.Document.Source != null && (sDate < (DateTime)SqlDateTime.MinValue || sDate > (DateTime)SqlDateTime.MaxValue))
                    {
                        docTemplate.Document.Source.CreationDate = (DateTime)SqlDateTime.MinValue;
                    }
                    if (docTemplate.Document.Source.OrganizationID == 0 && !String.IsNullOrWhiteSpace(docTemplate.Document.Source.OrganizationName))
                    {
                        Organization org = new Organization(UserName);
                        org.Name = docTemplate.Document.Source.OrganizationName;
                        org.OrganizationTypeID = 2;
                        org.DepartmentID = Worker.DepartmentID;
                        org.WorkerID = Worker.ID;
                        org.Insert(trans);
                        docTemplate.Document.Source.OrganizationID = org.ID;
                    }
                    docTemplate.Document.Source.UserName = UserName;
                    docTemplate.Document.Source.ID = docTemplate.Document.ID;
                    docTemplate.Document.Source.Insert(trans);

                    DateTime desDate = docTemplate.Document.Destination.CreationDate;
                    if ((desDate < (DateTime)SqlDateTime.MinValue || desDate > (DateTime)SqlDateTime.MaxValue))
                    {
                        docTemplate.Document.Destination.CreationDate = DateTime.Now;
                    }
                    docTemplate.Document.Destination.UserName = UserName;
                    docTemplate.Document.Destination.ID = docTemplate.Document.ID;
                    docTemplate.Document.Destination.Insert(trans);



                    docTemplate.UserName = UserName;
                    docTemplate.DocumentID = docTemplate.Document.ID;
                    docTemplateID = docTemplate.Insert(trans);

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
            return docTemplateID;
        }

        private ProcessingResult UpdateDocTemplateAsync(DocTemplate docTemplate)
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

                    MembershipUser mu = Membership.GetUser();
                    docTemplate.Document.UserName = UserName;
                    if (mu != null)
                    {
                        if (mu.ProviderUserKey != null)
                        {
                            docTemplate.Document.OriginatorID = (Guid)mu.ProviderUserKey;
                            docTemplate.Document.OwnerID = (Guid)mu.ProviderUserKey;
                            docTemplate.Document.RequestorID = (Guid)mu.ProviderUserKey;
                        }
                    }

                    docTemplate.Document.ID = docTemplate.DocumentID;
                    Document.Document doc = new Document.Document(trans, docTemplate.DocumentID, UserName);
                    docTemplate.Document.ObjectID = doc.ObjectID;

                    docTemplate.Document.RequestDate = DateTime.Now;
                    docTemplate.Document.Update(trans);


                    DocumentLabel[] documentLabels = DocumentLabel.GetDocumentLabels(trans, docTemplate.Document.ID,
                                                                           docTemplate.Document.DepartmentID);
                    foreach (int labelId in docTemplate.Document.Labels)
                    {
                        bool isNewLabel = true;
                        foreach (DocumentLabel documentLabel in documentLabels)
                        {
                            if (documentLabel.LabelID == labelId)
                            {
                                isNewLabel = false;
                            }
                        }
                        if (isNewLabel)
                        {
                            DocumentLabel label = new DocumentLabel(UserName);
                            label.LabelID = labelId;
                            label.DocumentID = docTemplate.Document.ID;
                            label.DepartmentID = docTemplate.Document.DepartmentID;
                            label.WorkerID = Worker.ID;
                            label.Insert(trans);
                        }
                    }
                    foreach (DocumentLabel documentLabel in documentLabels)
                    {
                        bool isExist = false;
                        foreach (int labelId in docTemplate.Document.Labels)
                        {
                            if (documentLabel.LabelID == labelId)
                            {
                                isExist = true;
                            }
                        }
                        if (!isExist)
                        {
                            DocumentLabel.Delete(trans, docTemplate.Document.ID, docTemplate.Document.DepartmentID, documentLabel.ID);
                        }
                    }


                    DateTime sDate = docTemplate.Document.Source.CreationDate;
                    if (docTemplate.Document.Source != null && (sDate < (DateTime)SqlDateTime.MinValue || sDate > (DateTime)SqlDateTime.MaxValue))
                    {
                        docTemplate.Document.Source.CreationDate = (DateTime)SqlDateTime.MinValue;
                    }
                    if (docTemplate.Document.Source.OrganizationID == 0 && !String.IsNullOrWhiteSpace(docTemplate.Document.Source.OrganizationName))
                    {
                        Organization org = new Organization(UserName);
                        org.Name = docTemplate.Document.Source.OrganizationName;
                        org.OrganizationTypeID = 2;
                        org.DepartmentID = Worker.DepartmentID;
                        org.WorkerID = Worker.ID;
                        org.Insert(trans);
                        docTemplate.Document.Source.OrganizationID = org.ID;
                    }
                    docTemplate.Document.Source.UserName = UserName;
                    docTemplate.Document.Source.ID = docTemplate.Document.ID;
                    docTemplate.Document.Source.Update(trans);


                    DateTime desDate = docTemplate.Document.Destination.CreationDate;
                    if ((desDate < (DateTime)SqlDateTime.MinValue || desDate > (DateTime)SqlDateTime.MaxValue))
                    {
                        docTemplate.Document.Destination.CreationDate = (DateTime)SqlDateTime.MinValue;
                    }
                    if (docTemplate.Document.Destination.OrganizationID == 0 && !String.IsNullOrWhiteSpace(docTemplate.Document.Destination.OrganizationName))
                    {
                        Organization org = new Organization(UserName);
                        org.Name = docTemplate.Document.Destination.OrganizationName;
                        org.OrganizationTypeID = 2;
                        org.DepartmentID = Worker.DepartmentID;
                        org.WorkerID = Worker.ID;
                        org.Insert(trans);
                        docTemplate.Document.Destination.OrganizationID = org.ID;
                    }
                    docTemplate.Document.Destination.UserName = UserName;
                    docTemplate.Document.Destination.ID = docTemplate.Document.ID;
                    docTemplate.Document.Destination.Update(trans);


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
            return pr;
        }

        private ProcessingResult DeleteDocTemplateAsync(int id)
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

                    DocumentLabel.DeleteList(trans, docTemplate.DocumentID);
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


        #region [ DocTemplate Reports ]
        public void ProcessDocTReportAsync()
        {
            HttpRequest request = _context.Request;
            int dep = Convert.ToInt32(request["dep"]);
            string rType = request["r"];
            string cd = request["cd"];
            string ed = request["ed"];
            int documentCodeID = 0;
            if (!String.IsNullOrWhiteSpace(request["dcid"]))
                documentCodeID = Convert.ToInt32(request["dcid"]);

            if (dep > 0 && !String.IsNullOrWhiteSpace(rType))
            {
                switch (rType)
                {
                    case "dc":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime creationDate;
                            DateTime endDate;
                            if (DateTime.TryParse(cd, out creationDate) && DateTime.TryParse(ed, out endDate))
                                GetDocTControlledReport(dep, creationDate, endDate);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("cd", "Inuput id is not valid");
                        }
                        break;
                    case "nd":
                        if (!String.IsNullOrWhiteSpace(cd))
                        {
                            DateTime creationDate;
                            if (DateTime.TryParse(cd, out creationDate))
                                GetDocTNotDoneReport(dep, creationDate);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("cd", "Inuput id is not valid");
                        }
                        break;
                    case "ml":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime fromEndDate;
                            DateTime toEndDate;
                            if (DateTime.TryParse(cd, out fromEndDate) && DateTime.TryParse(ed, out toEndDate))
                                GetDocTMadeLateReport(dep, fromEndDate, toEndDate);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("cd", "Inuput id is not valid");
                        }
                        break;
                    case "cpw":
                        GetDocTCalendarPlanReport(dep);
                        break;
                    case "qs":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime creationDate,
                                endDate;
                            if (DateTime.TryParse(cd, out creationDate) && DateTime.TryParse(ed, out endDate))
                                GetDocTStatisticsReport(dep, creationDate, endDate, documentCodeID);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("cd", "Inuput id is not valid");
                        }
                        break;
                    case "dv":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime creationDate,
                                endDate;
                            if (DateTime.TryParse(cd, out creationDate) && DateTime.TryParse(ed, out endDate))
                                GetDocTStatisticsReportDocumentsVolume(dep, creationDate, endDate);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("cd", "Inuput id is not valid");
                        }
                        break;
                    case "dcn":
                        if (!String.IsNullOrWhiteSpace(cd))
                        {
                            DateTime currentDate;
                            if (DateTime.TryParse(cd, out currentDate))
                                GetDocTStatisticsReportControlledDocuments(dep, documentCodeID, currentDate);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("cd", "Inuput id is not valid");
                        }
                        break;
                    case "do":
                        if (!String.IsNullOrWhiteSpace(cd))
                        {
                            DateTime currentDate;
                            int executiveDepartmentID = 0;
                            if (!String.IsNullOrWhiteSpace(request["edid"]))
                                executiveDepartmentID = Convert.ToInt32(request["edid"]);

                            if (DateTime.TryParse(cd, out currentDate))
                                GetDocTStatisticsReportControlledOverdueDocuments(dep, documentCodeID, executiveDepartmentID, currentDate);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("cd", "Inuput id is not valid");
                        }
                        break;
                }
            }
        }

        public void GetDocTControlledReport(int departmentID, DateTime currentDate, DateTime endDate)
        {
            HttpRequest request = _context.Request;

            int executiveDepartmentID = 0,
                documentCodeID = 0,
                docTypeID = 0,
                questionTypeID = 0;

            if (!String.IsNullOrWhiteSpace(request["edid"]))
                executiveDepartmentID = Convert.ToInt32(request["edid"]);
            if (!String.IsNullOrWhiteSpace(request["dcid"]))
                documentCodeID = Convert.ToInt32(request["dcid"]);
            if (!String.IsNullOrWhiteSpace(request["dtid"]))
                docTypeID = Convert.ToInt32(request["dtid"]);
            if (!String.IsNullOrWhiteSpace(request["qtid"]))
                questionTypeID = Convert.ToInt32(request["qtid"]);

            _context.Response.Clear();

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocTemplate.GetControlled(trans, departmentID, executiveDepartmentID, documentCodeID, docTypeID, questionTypeID, currentDate, endDate);

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

            RtfDocument rtf = new RtfDocument();
            rtf.FontTable.Add(new RtfFont("Times New Roman"));

            RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
            header.AppendText("Список документів, які стоять на контролі за виконавцями");
            header.AppendParagraph();

            string[] cols = { "WorkerID", "WorkerFirstName", "WorkerMiddleName", "WorkerLastName" };
            DataTable workerTable = ds.Tables[0].DefaultView.ToTable(true, cols);

            RtfTable tbl = new RtfTable(RtfTableAlign.Center, 7, ds.Tables[0].Rows.Count + workerTable.Rows.Count + 1);
            tbl.Width = TwipConverter.ToTwip(489, MetricUnit.Point);
            tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Center));
            tbl.Columns[0].Width = TwipConverter.ToTwip(40, MetricUnit.Point);
            tbl.Columns[1].Width = TwipConverter.ToTwip(10, MetricUnit.Point);
            tbl.Columns[2].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
            tbl.Columns[3].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
            tbl.Columns[4].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
            tbl.Columns[5].Width = TwipConverter.ToTwip(100, MetricUnit.Point);
            tbl.Columns[5].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));
            tbl.Columns[6].Width = TwipConverter.ToTwip(149, MetricUnit.Point);
            tbl.Columns[6].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));

            foreach (RtfTableRow row in tbl.Rows)
            {
                row.Height = TwipConverter.ToTwip(20, MetricUnit.Point);
            }
            tbl.Rows[0].Height = TwipConverter.ToTwip(30, MetricUnit.Point);
            tbl.Rows[0].Cells[5].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Center);

            tbl[0, 0].AppendText("Шифр");
            tbl[1, 0].AppendText("K");
            tbl[2, 0].AppendText("Номер документа");
            tbl[3, 0].AppendText("Дата створення");
            tbl[4, 0].AppendText("Виконати до");
            tbl[5, 0].AppendText("Організація");
            tbl[6, 0].AppendText("Зміст");
            int numRow = 1;

            foreach (DataRow worker in workerTable.Rows)
            {
                ds.Tables[0].DefaultView.RowFilter = String.Format("WorkerID = {0}", worker["WorkerID"]);

                tbl.MergeCellsHorizontally(0, numRow, 6);
                tbl[0, numRow].AppendText("Виконавець: " + FormatHelper.FormatToLastNameAndInitials((string)worker["WorkerLastName"], (string)worker["WorkerFirstName"], (string)worker["WorkerMiddleName"]));
                tbl.Rows[numRow].Cells[0].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Left);
                numRow++;

                DataTable cardTable = ds.Tables[0].DefaultView.ToTable();
                foreach (DataRow card in cardTable.Rows)
                {
                    tbl[0, numRow].AppendText(card["DocumentCodeID"].ToString());
                    tbl[1, numRow].AppendText(((bool)card["IsSpeciallyControlled"]) ? "k" : String.Empty);
                    tbl[2, numRow].AppendText((string)card["Number"]);
                    tbl[3, numRow].AppendText(((DateTime)card["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[4, numRow].AppendText(((DateTime)card["EndDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[5, numRow].AppendText((string)card["OrganizationName"]);
                    tbl[6, numRow].AppendText((string)card["Content"]);

                    numRow++;
                }
            }

            RtfFormattedParagraph footer = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Left));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Список станом на {0}", currentDate.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Всього документів: {0}", ds.Tables[0].Rows.Count));
            footer.AppendParagraph();
            footer.AppendText("Виконав ______________________");

            rtf.Contents.AddRange(new RtfDocumentContentBase[] { header, tbl, footer });


            RtfWriter rtfWriter = new RtfWriter();
            StringBuilder sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                rtfWriter.Write(writer, rtf);
            }

            string fileName = String.Format("ds_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(sb.ToString());
        }

        public void GetDocTNotDoneReport(int departmentID, DateTime currentDate)
        {
            HttpRequest request = _context.Request;

            int executiveDepartmentID = 0,
                documentCodeID = 0,
                docTypeID = 0,
                questionTypeID = 0;

            if (!String.IsNullOrWhiteSpace(request["edid"]))
                executiveDepartmentID = Convert.ToInt32(request["edid"]);
            if (!String.IsNullOrWhiteSpace(request["dcid"]))
                documentCodeID = Convert.ToInt32(request["dcid"]);
            if (!String.IsNullOrWhiteSpace(request["dtid"]))
                docTypeID = Convert.ToInt32(request["dtid"]);
            if (!String.IsNullOrWhiteSpace(request["qtid"]))
                questionTypeID = Convert.ToInt32(request["qtid"]);

            _context.Response.Clear();

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocTemplate.GetNotDone(trans, departmentID, executiveDepartmentID, documentCodeID, docTypeID, questionTypeID, currentDate);

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

            RtfDocument rtf = new RtfDocument();
            rtf.FontTable.Add(new RtfFont("Times New Roman"));

            RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
            header.AppendText("Список невиконаних документів");
            header.AppendParagraph();

            string[] cols = { "WorkerID", "WorkerFirstName", "WorkerMiddleName", "WorkerLastName" };
            DataTable workerTable = ds.Tables[0].DefaultView.ToTable(true, cols);

            RtfTable tbl = new RtfTable(RtfTableAlign.Center, 5, ds.Tables[0].Rows.Count + workerTable.Rows.Count + 1);
            tbl.Width = TwipConverter.ToTwip(489, MetricUnit.Point);
            tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Center));
            tbl.Columns[0].Width = TwipConverter.ToTwip(40, MetricUnit.Point);
            tbl.Columns[1].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
            tbl.Columns[2].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
            tbl.Columns[3].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
            tbl.Columns[4].Width = TwipConverter.ToTwip(249, MetricUnit.Point);
            tbl.Columns[4].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));

            foreach (RtfTableRow row in tbl.Rows)
            {
                row.Height = TwipConverter.ToTwip(20, MetricUnit.Point);
            }
            tbl.Rows[0].Height = TwipConverter.ToTwip(30, MetricUnit.Point);
            tbl.Rows[0].Cells[4].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Center);

            tbl[0, 0].AppendText("Шифр");
            tbl[1, 0].AppendText("Номер документа");
            tbl[2, 0].AppendText("Дата створення");
            tbl[3, 0].AppendText("Виконати до");
            tbl[4, 0].AppendText("Зміст");
            int numRow = 1;

            foreach (DataRow worker in workerTable.Rows)
            {
                ds.Tables[0].DefaultView.RowFilter = String.Format("WorkerID = {0}", worker["WorkerID"]);

                tbl.MergeCellsHorizontally(0, numRow, 5);
                tbl[0, numRow].AppendText("Виконавець: " + FormatHelper.FormatToLastNameAndInitials((string)worker["WorkerLastName"], (string)worker["WorkerFirstName"], (string)worker["WorkerMiddleName"]));
                tbl.Rows[numRow].Cells[0].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Left);
                numRow++;

                DataTable cardTable = ds.Tables[0].DefaultView.ToTable();
                foreach (DataRow card in cardTable.Rows)
                {
                    tbl[0, numRow].AppendText(card["DocumentCodeID"].ToString());
                    tbl[1, numRow].AppendText((string)card["Number"]);
                    tbl[2, numRow].AppendText(((DateTime)card["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[3, numRow].AppendText(((DateTime)card["EndDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[4, numRow].AppendText((string)card["Content"]);

                    numRow++;
                }
            }

            RtfFormattedParagraph footer = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Left));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Список станом на {0}", currentDate.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Всього документів: {0}", ds.Tables[0].Rows.Count));
            footer.AppendParagraph();
            footer.AppendText("Виконав ______________________");

            rtf.Contents.AddRange(new RtfDocumentContentBase[] { header, tbl, footer });


            RtfWriter rtfWriter = new RtfWriter();
            StringBuilder sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                rtfWriter.Write(writer, rtf);
            }

            string fileName = String.Format("nd_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(sb.ToString());
        }

        public void GetDocTMadeLateReport(int departmentID, DateTime fromEndDate, DateTime toEndDate)
        {
            HttpRequest request = _context.Request;

            int executiveDepartmentID = 0,
                documentCodeID = 0,
                docTypeID = 0,
                questionTypeID = 0;

            if (!String.IsNullOrWhiteSpace(request["edid"]))
                executiveDepartmentID = Convert.ToInt32(request["edid"]);
            if (!String.IsNullOrWhiteSpace(request["dcid"]))
                documentCodeID = Convert.ToInt32(request["dcid"]);
            if (!String.IsNullOrWhiteSpace(request["dtid"]))
                docTypeID = Convert.ToInt32(request["dtid"]);
            if (!String.IsNullOrWhiteSpace(request["qtid"]))
                questionTypeID = Convert.ToInt32(request["qtid"]);

            _context.Response.Clear();

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocTemplate.GetMadeLate(trans, departmentID, executiveDepartmentID, documentCodeID, docTypeID, questionTypeID, fromEndDate, toEndDate);

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

            RtfDocument rtf = new RtfDocument();
            rtf.FontTable.Add(new RtfFont("Times New Roman"));

            RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
            header.AppendText("Список документів, виконаних із запізненням");
            header.AppendParagraph();

            string[] cols = { "WorkerID", "WorkerFirstName", "WorkerMiddleName", "WorkerLastName" };
            DataTable workerTable = ds.Tables[0].DefaultView.ToTable(true, cols);

            RtfTable tbl = new RtfTable(RtfTableAlign.Center, 6, ds.Tables[0].Rows.Count + workerTable.Rows.Count + 1);
            tbl.Width = TwipConverter.ToTwip(489, MetricUnit.Point);
            tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Center));
            tbl.Columns[0].Width = TwipConverter.ToTwip(40, MetricUnit.Point);
            tbl.Columns[1].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
            tbl.Columns[2].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
            tbl.Columns[3].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
            tbl.Columns[4].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
            tbl.Columns[5].Width = TwipConverter.ToTwip(189, MetricUnit.Point);
            tbl.Columns[5].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));

            foreach (RtfTableRow row in tbl.Rows)
            {
                row.Height = TwipConverter.ToTwip(20, MetricUnit.Point);
            }
            tbl.Rows[0].Height = TwipConverter.ToTwip(30, MetricUnit.Point);
            tbl.Rows[0].Cells[5].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Center);

            tbl[0, 0].AppendText("Шифр");
            tbl[1, 0].AppendText("Номер документа");
            tbl[2, 0].AppendText("Дата створення");
            tbl[3, 0].AppendText("Виконати до");
            tbl[4, 0].AppendText("Виконано");
            tbl[5, 0].AppendText("Зміст");
            int numRow = 1;

            foreach (DataRow worker in workerTable.Rows)
            {
                ds.Tables[0].DefaultView.RowFilter = String.Format("WorkerID = {0}", worker["WorkerID"]);

                tbl.MergeCellsHorizontally(0, numRow, 6);
                tbl[0, numRow].AppendText("Виконавець: " + FormatHelper.FormatToLastNameAndInitials((string)worker["WorkerLastName"], (string)worker["WorkerFirstName"], (string)worker["WorkerMiddleName"]));
                tbl.Rows[numRow].Cells[0].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Left);
                numRow++;

                DataTable cardTable = ds.Tables[0].DefaultView.ToTable();
                foreach (DataRow card in cardTable.Rows)
                {
                    tbl[0, numRow].AppendText(card["DocumentCodeID"].ToString());
                    tbl[1, numRow].AppendText((string)card["Number"]);
                    tbl[2, numRow].AppendText(((DateTime)card["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[3, numRow].AppendText(((DateTime)card["EndDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[4, numRow].AppendText(((DateTime)card["ControlResponseDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[5, numRow].AppendText((string)card["Content"]);

                    numRow++;
                }
            }

            RtfFormattedParagraph footer = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Left));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Список станом на {0}", toEndDate.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Всього документів: {0}", ds.Tables[0].Rows.Count));
            footer.AppendParagraph();
            footer.AppendText("Виконав ______________________");

            rtf.Contents.AddRange(new RtfDocumentContentBase[] { header, tbl, footer });


            RtfWriter rtfWriter = new RtfWriter();
            StringBuilder sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                rtfWriter.Write(writer, rtf);
            }

            string fileName = String.Format("ml_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(sb.ToString());
        }

        public void GetDocTCalendarPlanReport(int departmentID)
        {
            HttpRequest request = _context.Request;

            int executiveDepartmentID = 0,
                documentCodeID = 0,
                docTypeID = 0,
                questionTypeID = 0,
                workerID = 0;

            DateTime currentDate = DateTime.Now;

            if (!String.IsNullOrWhiteSpace(request["edid"]))
                executiveDepartmentID = Convert.ToInt32(request["edid"]);
            if (!String.IsNullOrWhiteSpace(request["dcid"]))
                documentCodeID = Convert.ToInt32(request["dcid"]);
            if (!String.IsNullOrWhiteSpace(request["dtid"]))
                docTypeID = Convert.ToInt32(request["dtid"]);
            if (!String.IsNullOrWhiteSpace(request["qtid"]))
                questionTypeID = Convert.ToInt32(request["qtid"]);
            if (!String.IsNullOrWhiteSpace(request["wk"]))
                questionTypeID = Convert.ToInt32(request["wk"]);

            if (!String.IsNullOrWhiteSpace(request["cd"]))
                DateTime.TryParse(request["cd"], out currentDate);


            _context.Response.Clear();

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocTemplate.GetCalendarPlan(trans, departmentID, executiveDepartmentID, documentCodeID, docTypeID, questionTypeID, workerID, currentDate);

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

            RtfDocument rtf = new RtfDocument();
            rtf.FontTable.Add(new RtfFont("Times New Roman"));

            RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
            header.AppendText("Календарний план на " + currentDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture));
            header.AppendParagraph();

            string[] cols = { "WorkerID", "WorkerFirstName", "WorkerMiddleName", "WorkerLastName" };
            DataTable workerTable = ds.Tables[0].DefaultView.ToTable(true, cols);

            RtfTable tbl = new RtfTable(RtfTableAlign.Center, 5, ds.Tables[0].Rows.Count + workerTable.Rows.Count + 1);
            tbl.Width = TwipConverter.ToTwip(489, MetricUnit.Point);
            tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Center));
            tbl.Columns[0].Width = TwipConverter.ToTwip(40, MetricUnit.Point);
            tbl.Columns[1].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
            tbl.Columns[2].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
            tbl.Columns[3].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
            tbl.Columns[4].Width = TwipConverter.ToTwip(249, MetricUnit.Point);
            tbl.Columns[4].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));

            foreach (RtfTableRow row in tbl.Rows)
            {
                row.Height = TwipConverter.ToTwip(20, MetricUnit.Point);
            }
            tbl.Rows[0].Height = TwipConverter.ToTwip(30, MetricUnit.Point);
            tbl.Rows[0].Cells[4].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Center);

            tbl[0, 0].AppendText("Шифр");
            tbl[1, 0].AppendText("Номер документа");
            tbl[2, 0].AppendText("Дата створення");
            tbl[3, 0].AppendText("Виконати до");
            tbl[4, 0].AppendText("Зміст");
            int numRow = 1;

            foreach (DataRow worker in workerTable.Rows)
            {
                ds.Tables[0].DefaultView.RowFilter = String.Format("WorkerID = {0}", worker["WorkerID"]);

                tbl.MergeCellsHorizontally(0, numRow, 5);
                tbl[0, numRow].AppendText("Виконавець: " + FormatHelper.FormatToLastNameAndInitials((string)worker["WorkerLastName"], (string)worker["WorkerFirstName"], (string)worker["WorkerMiddleName"]));
                tbl.Rows[numRow].Cells[0].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Left);
                numRow++;

                DataTable cardTable = ds.Tables[0].DefaultView.ToTable();
                foreach (DataRow card in cardTable.Rows)
                {
                    tbl[0, numRow].AppendText(card["DocumentCodeID"].ToString());
                    tbl[1, numRow].AppendText((string)card["Number"]);
                    tbl[2, numRow].AppendText(((DateTime)card["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[3, numRow].AppendText(((DateTime)card["EndDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                    tbl[4, numRow].AppendText((string)card["Content"]);

                    numRow++;
                }
            }

            RtfFormattedParagraph footer = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Left));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Список станом на {0}", currentDate.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Всього документів: {0}", ds.Tables[0].Rows.Count));
            footer.AppendParagraph();
            footer.AppendText("Виконав ______________________");

            rtf.Contents.AddRange(new RtfDocumentContentBase[] { header, tbl, footer });


            RtfWriter rtfWriter = new RtfWriter();
            StringBuilder sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                rtfWriter.Write(writer, rtf);
            }

            string fileName = String.Format("cpw_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(sb.ToString());
        }

        public void GetDocTStatisticsReport(int departmentID, DateTime startDate, DateTime endDate, int documentCodeID)
        {
            _context.Response.Clear();

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocTemplate.GetStatistics(trans, departmentID, startDate, endDate, documentCodeID);

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

            StatisticsReport sReport = new StatisticsReport();

            sReport.Header.Add("Звіт по кількості надходження документів");
            if (documentCodeID > 0)
                sReport.Header.Add("з шифром " + documentCodeID);
            sReport.Header.Add(String.Format("за період з {0} по {1}", startDate.ToString("dd.MM.yyyy"), endDate.ToString("dd.MM.yyyy")));


            StatisticsReport.TableForm table0 = new StatisticsReport.TableForm();
            table0.Columns.Add(5000);
            table0.Columns.Add(5800);

            StatisticsReport.TableForm table = new StatisticsReport.TableForm();
            table.Columns.Add(4400);
            table.Columns.Add(4920);
            table.Columns.Add(5440);

            List<string> headerRow = new List<string>();
            headerRow.Add("Джерело надходження");
            headerRow.Add("(Н)");
            headerRow.Add("(К)");

            List<int> dsID = new List<int>();
            foreach (DataRow status in ds.Tables[0].Rows)
            {
                if ((int)status["DocStatusID"] > 0)
                {
                    table0.Rows.Add(new List<string> { String.Format("({0}) - {1}", dsID.Count, status["Name"]), ((int)status["Number"]).ToString(CultureInfo.CurrentCulture) });
                    headerRow.Add(String.Format("({0})", dsID.Count));
                    dsID.Add((int)status["DocStatusID"]);
                    table.Columns.Add(table.Columns[table.Columns.Count - 1] + 490);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                headerRow.Add(String.Format("({0})", dsID.Count + i));
                table.Columns.Add(table.Columns[table.Columns.Count - 1] + 490);
            }
            table.Rows.Add(headerRow);


            int[] numbList = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            if (ds.Tables[3].Rows.Count > 0)
            {
                numbList[0] = (int)ds.Tables[3].Rows[0]["NumberDecisionOfSession"];
                numbList[1] = (int)ds.Tables[3].Rows[0]["NumberDecisionOfExecutiveCommittee"];
                numbList[2] = (int)ds.Tables[3].Rows[0]["NumberOrderOfHeader"];
                numbList[3] = (int)ds.Tables[3].Rows[0]["NumberActionWorkPlan"];
                numbList[4] = (int)ds.Tables[3].Rows[0]["NumberSendResponse"];
                numbList[5] = (int)ds.Tables[3].Rows[0]["NumberSendInterimResponse"];
                numbList[6] = (int)ds.Tables[3].Rows[0]["NumberLeftToWorker"];
                numbList[7] = (int)ds.Tables[3].Rows[0]["NumberAcquaintedWorker"];
            }

            table0.Rows.Add(new List<string> { String.Format("({0}) - Рішення сесії", dsID.Count), numbList[0].ToString(CultureInfo.CurrentCulture) });
            table0.Rows.Add(new List<string> { String.Format("({0}) - Рішення виконкому", dsID.Count + 1), numbList[1].ToString(CultureInfo.CurrentCulture) });
            table0.Rows.Add(new List<string> { String.Format("({0}) - Розпорядження голови", dsID.Count + 2), numbList[2].ToString(CultureInfo.CurrentCulture) });
            table0.Rows.Add(new List<string> { String.Format("({0}) - Заходи, робочий план", dsID.Count + 3), numbList[3].ToString(CultureInfo.CurrentCulture) });
            table0.Rows.Add(new List<string> { String.Format("({0}) - Направлена відповідь (інформація)", dsID.Count + 4), numbList[4].ToString(CultureInfo.CurrentCulture) });
            table0.Rows.Add(new List<string> { String.Format("({0}) - Направлена проміжна відповідь", dsID.Count + 5), numbList[5].ToString(CultureInfo.CurrentCulture) });
            table0.Rows.Add(new List<string> { String.Format("({0}) - Залишено для роботи у виконавця", dsID.Count + 6), numbList[6].ToString(CultureInfo.CurrentCulture) });
            table0.Rows.Add(new List<string> { String.Format("({0}) - Виконавець ознайомлений", dsID.Count + 7), numbList[7].ToString(CultureInfo.CurrentCulture) });
            sReport.Tables.Add(table0);


            StatisticsReport.TableForm table3 = new StatisticsReport.TableForm();
            table3.Columns.Add(6000);
            table3.Columns.Add(7600);
            table3.Columns.Add(9200);
            DataTable dtTable = ds.Tables[4];
            table3.Rows.Add(new List<string> { "Найменування документа", "Кількість", "Контрольні" });
            foreach (DataRow quest in dtTable.Rows)
            {
                List<string> row = new List<string>();
                row.Add((string)quest["Name"]);
                row.Add(((int)quest["Number"]).ToString(CultureInfo.CurrentCulture));
                row.Add(((int)quest["Controlled"]).ToString(CultureInfo.CurrentCulture));
                table3.Rows.Add(row);
            }
            int numberChildren = 0;
            if (ds.Tables[3].Rows.Count > 0)
            {
                numberChildren = (int) ds.Tables[3].Rows[0]["NumberChildren"];
            }
            table3.Rows.Add(new List<string> { "Вихідні відповіді", numberChildren.ToString(), "" });
            sReport.Tables.Add(table3);

            StatisticsReport.TableForm table1 = new StatisticsReport.TableForm();
            table1.Columns.Add(6000);
            table1.Columns.Add(7600);
            DataTable qTable = ds.Tables[2];
            table1.Rows.Add(new List<string> { "Питання", "Кількість" });
            foreach (DataRow quest in qTable.Rows)
            {
                List<string> row = new List<string>();
                row.Add((string)quest["Name"]);
                row.Add(((int)quest["Number"]).ToString(CultureInfo.CurrentCulture));
                table1.Rows.Add(row);
            }
            sReport.Tables.Add(table1);


            int sumCountDocs = 0,
                sumControlledDocs = 0,
                prevOrganizationId = -1,
                prevDepartmentId = -1;
            string[] cols = { "OrganizationID", "OrganizationName", "DepartmentID", "DepartmentName" };
            DataTable orgsTable = ds.Tables[1].DefaultView.ToTable(true, cols);
            foreach (DataRow org in orgsTable.Rows)
            {
                string filter = "OrganizationID is Null";
                string orgName = "Не визначено";
                if (org["OrganizationID"] != DBNull.Value && (int)org["OrganizationID"] != 0)
                {
                    filter = String.Format("OrganizationID = {0}", org["OrganizationID"]);
                    orgName = (string)org["OrganizationName"];
                    if (prevOrganizationId == (int) org["OrganizationID"])
                    {
                        continue;
                    }
                    prevOrganizationId = (int)org["OrganizationID"];
                }
                else if (org["DepartmentID"] != DBNull.Value && (int)org["DepartmentID"] != 0)
                {
                    filter = String.Format("DepartmentID = {0}", org["DepartmentID"]);
                    orgName = (string)org["DepartmentName"];
                    if (prevDepartmentId == (int)org["DepartmentID"])
                    {
                        continue;
                    }
                    prevDepartmentId = (int)org["DepartmentID"];
                }

                ds.Tables[1].DefaultView.RowFilter = filter;

                List<string> row = new List<string> { orgName, String.Empty, String.Empty };

                foreach (int id in dsID)
                {
                    row.Add(String.Empty);
                }

                int countDocs = 0,
                    controlled = 0;
                int[] numList = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                DataTable orgTable = ds.Tables[1].DefaultView.ToTable();
                foreach (DataRow orgRow in orgTable.Rows)
                {
                    countDocs = countDocs + (int)orgRow["CountDocs"];
                    controlled = controlled + (int)orgRow["Controlled"];
                    row[dsID.IndexOf((int)orgRow["DocStatusID"]) + 3] = orgRow["CountDocs"].ToString();
                    numList[0] = numList[0] + (int)orgRow["NumberDecisionOfSession"];
                    numList[1] = numList[1] + (int)orgRow["NumberDecisionOfExecutiveCommittee"];
                    numList[2] = numList[2] + (int)orgRow["NumberOrderOfHeader"];
                    numList[3] = numList[3] + (int)orgRow["NumberActionWorkPlan"];
                    numList[4] = numList[4] + (int)orgRow["NumberSendResponse"];
                    numList[5] = numList[5] + (int)orgRow["NumberSendInterimResponse"];
                    numList[6] = numList[6] + (int)orgRow["NumberLeftToWorker"];
                    numList[7] = numList[7] + (int)orgRow["NumberAcquaintedWorker"];
                }

                sumCountDocs = sumCountDocs + countDocs;
                sumControlledDocs = sumControlledDocs + controlled;
                row[1] = countDocs.ToString(CultureInfo.CurrentCulture);
                row[2] = controlled.ToString(CultureInfo.CurrentCulture);
                row.Add(numList[0] > 0 ? numList[0].ToString(CultureInfo.CurrentCulture) : String.Empty);
                row.Add(numList[1] > 0 ? numList[1].ToString(CultureInfo.CurrentCulture) : String.Empty);
                row.Add(numList[2] > 0 ? numList[2].ToString(CultureInfo.CurrentCulture) : String.Empty);
                row.Add(numList[3] > 0 ? numList[3].ToString(CultureInfo.CurrentCulture) : String.Empty);
                row.Add(numList[4] > 0 ? numList[4].ToString(CultureInfo.CurrentCulture) : String.Empty);
                row.Add(numList[5] > 0 ? numList[5].ToString(CultureInfo.CurrentCulture) : String.Empty);
                row.Add(numList[6] > 0 ? numList[6].ToString(CultureInfo.CurrentCulture) : String.Empty);
                row.Add(numList[7] > 0 ? numList[7].ToString(CultureInfo.CurrentCulture) : String.Empty);
                table.Rows.Add(row);
            }
            sReport.Tables.Add(table);


            sReport.Content.Add("(Н) - кількість надходжень  " + sumCountDocs);
            sReport.Content.Add("(К) - кількість контрольных  " + sumControlledDocs);
            sReport.Content.Add("Результати розгляду:");


            sReport.Footer.Add(String.Format("Список станом на {0}", startDate.ToString("dd.MM.yyyy")));
            sReport.Footer.Add(String.Format("Всього документів: {0}", sumCountDocs));

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocTStatisticsRTF.xslt", _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(sReport);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dts_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }



        public void GetDocTStatisticsReportDocumentsVolume(int departmentID, DateTime startDate, DateTime endDate)
        {
            try
            {
                int[] numberAll;
                //int[] numberInput;
                int[] numberOutput;
                int[] numberInternal;

                int numberInputResponses;
                int numberOutputResponses;
                SqlConnection connection = new SqlConnection(Config.ConnectionString);
                try
                {
                    connection.Open();
                    SqlTransaction trans = null;
                    try
                    {
                        trans = connection.BeginTransaction();

                        numberAll = DocTemplate.GetNumberDocuments(trans, departmentID, startDate, endDate);
                        numberOutput = DocTemplate.GetNumberDocuments(trans, departmentID, startDate, endDate, 8);
                        numberInternal = DocTemplate.GetNumberDocuments(trans, departmentID, startDate, endDate, 12);

                        numberInputResponses = DocTemplate.GetNumberResponse(trans, departmentID, startDate, endDate, 8);
                        numberOutputResponses = DocTemplate.GetNumberResponse(trans, departmentID, startDate, endDate) - numberInputResponses;

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

                RtfDocument rtf = new RtfDocument();
                rtf.FontTable.Add(new RtfFont("Times New Roman"));

                RtfParagraphFormatting centered12 = new RtfParagraphFormatting(14, RtfTextAlign.Center);
                centered12.FontIndex = 0;

                RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
                header.Formatting = centered12;

                header.AppendText("ЗВІТ");
                header.AppendParagraph();
                header.AppendText("про обсяг документообігу");
                header.AppendParagraph();

                header.AppendText("за");

                if (startDate.Month == endDate.Month)
                    header.AppendText(endDate.ToString("MMMM", CultureInfo.CurrentCulture).ToLower());
                else
                    header.AppendText("______________________");

                if (startDate.Year == endDate.Year)
                    header.AppendText(startDate.ToString("yyyy", CultureInfo.CurrentCulture));
                else
                    header.AppendText("20___");

                header.AppendText("року");
                header.AppendParagraph();
                header.AppendParagraph();

                RtfTable tbl = new RtfTable(RtfTableAlign.Center, 4, 5);
                tbl.Width = TwipConverter.ToTwip(500, MetricUnit.Point);
                tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, centered12);


                foreach (RtfTableRow row in tbl.Rows)
                {
                    row.Height = TwipConverter.ToTwip(30, MetricUnit.Point);
                }

                tbl.MergeCellsVertically(0, 0, 2);
                tbl.MergeCellsHorizontally(1, 0, 3);

                tbl[0, 0].AppendText("Документи");
                tbl[1, 0].AppendText("Кількість документів");

                tbl[1, 1].AppendText("Оригінали");
                tbl[2, 1].AppendText("Копії");
                tbl[3, 1].AppendText("Усього");

                tbl[0, 2].AppendText("Вхідні");
                tbl[1, 2].AppendText((numberAll[0] - numberOutput[0] - numberInternal[0]).ToString());
                tbl[2, 2].AppendText((numberAll[1] - numberOutput[1] - numberInternal[1]).ToString());
                tbl[3, 2].AppendText((numberAll[0] - numberOutput[0] - numberInternal[0] + numberAll[1] - numberOutput[1] - numberInternal[1]).ToString());
                /*
                tbl[0, 3].AppendText("Вхідні відповіді");
                tbl[1, 3].AppendText(numberInputResponses.ToString());
                tbl[2, 3].AppendText("0");
                tbl[3, 3].AppendText(numberInputResponses.ToString());

                tbl[0, 4].AppendText("Вихідні");
                tbl[1, 4].AppendText(numberOutput[0].ToString());
                tbl[2, 4].AppendText(numberOutput[1].ToString());
                tbl[3, 4].AppendText((numberOutput[0] + numberOutput[1]).ToString());

                tbl[0, 5].AppendText("Вихідні відповіді");
                tbl[1, 5].AppendText(numberOutputResponses.ToString());
                tbl[2, 5].AppendText("0");
                tbl[3, 5].AppendText(numberOutputResponses.ToString());

                tbl[0, 6].AppendText("Внутрішні");
                
                tbl[1, 6].AppendText(numberInternal[0].ToString());
                tbl[2, 6].AppendText(numberInternal[1].ToString());
                tbl[3, 6].AppendText((numberInternal[0] + numberInternal[1]).ToString());

                tbl[0, 7].AppendText("Усього");
                tbl[1, 7].AppendText((numberAll[0] + numberInputResponses + numberOutputResponses).ToString());
                tbl[2, 7].AppendText(numberAll[1].ToString());
                tbl[3, 7].AppendText((numberAll[0] + numberAll[1] + numberInputResponses + numberOutputResponses).ToString());
                */

                tbl[0, 3].AppendText("Вихідні");
                tbl[1, 3].AppendText(numberOutput[0].ToString());
                tbl[2, 3].AppendText(numberOutput[1].ToString());
                tbl[3, 3].AppendText((numberOutput[0] + numberOutput[1]).ToString());

                tbl[0, 4].AppendText("Внутрішні");

                RtfFormattedParagraph footer = new RtfFormattedParagraph();
                footer.Formatting.FontIndex = 0;
                footer.Formatting.FontSize = 14;
                footer.Formatting.Align = RtfTextAlign.Left;

                footer.AppendParagraph();
                footer.AppendParagraph();
                footer.AppendParagraph();
                /*footer.AppendText("Начальник загального");
                footer.AppendParagraph();
                footer.AppendText("відділу апарату");
                footer.AppendParagraph();
                footer.AppendText("ради і виконкому");
                footer.AppendText(new RtfTabCharacter());*/
                footer.AppendText("____________  _______________________________");
                footer.AppendText(new RtfTabCharacter());
                footer.AppendParagraph(new RtfFormattedText("       (підпис)                     (ініціали (ініціал імені), прізвище)", -1, 8));
                footer.AppendParagraph();
                footer.AppendText("___ _________________ 20___ року");

                rtf.Contents.AddRange(new RtfDocumentContentBase[] { header, tbl, footer });


                RtfWriter rtfWriter = new RtfWriter();
                StringBuilder sb = new StringBuilder();
                using (TextWriter writer = new StringWriter(sb))
                {
                    rtfWriter.Write(writer, rtf);
                }

                string fileName = String.Format("dv_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

                _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                _context.Response.ContentType = "application/rtf";
                _context.Response.Write(sb.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GetDocTStatisticsReportControlledDocuments(int departmentID, int documentCodeID, DateTime currentDate)
        {
            try
            {
                List<Department> departments;
                DataTable controlled;
                DataTable controlledLastMonth;

                DataTable executed;
                DataTable executeInTime;
                DataTable continued;
                DataTable overdue;


                SqlConnection connection = new SqlConnection(Config.ConnectionString);
                try
                {
                    connection.Open();
                    SqlTransaction trans = null;
                    try
                    {
                        trans = connection.BeginTransaction();

                        departments = Department.GetList(trans, UserName);
                        controlled = DocTemplate.GetNumberControlled(trans, departmentID, documentCodeID).Tables[0];

                        //get controlled for previous month
                        DateTime startDateFrom;
                        DateTime startDateTo;
                        if (currentDate.Month != 1)
                        {
                            startDateFrom = new DateTime(currentDate.Year, currentDate.Month - 1, 1);
                            startDateTo = new DateTime(currentDate.Year, currentDate.Month - 1, DateTime.DaysInMonth(currentDate.Year, currentDate.Month - 1));
                        }
                        else
                        {
                            startDateFrom = new DateTime(currentDate.Year - 1, 12, 1);
                            startDateTo = new DateTime(currentDate.Year - 1, 12, DateTime.DaysInMonth(currentDate.Year - 1, 12));
                        }
                        controlledLastMonth = DocTemplate.GetNumberControlledType(trans, departmentID, documentCodeID, startDateFrom, startDateTo).Tables[0];

                        executed = DocTemplate.GetNumberControlledExecuted(trans, departmentID, documentCodeID, startDateFrom, startDateTo).Tables[0];
                        executeInTime = DocTemplate.GetNumberControlledExecuteInTime(trans, departmentID, documentCodeID, startDateFrom, startDateTo, currentDate).Tables[0];
                        continued = DocTemplate.GetNumberControlledContinued(trans, departmentID, documentCodeID, startDateFrom, startDateTo).Tables[0];
                        overdue = DocTemplate.GetNumberControlledOverdue(trans, departmentID, documentCodeID, startDateFrom, startDateTo, currentDate).Tables[0];

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

                RtfDocument rtf = new RtfDocument();
                rtf.FontTable.Add(new RtfFont("Times New Roman"));

                RtfParagraphFormatting centered14 = new RtfParagraphFormatting(14, RtfTextAlign.Center);
                centered14.FontIndex = 0;

                RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
                header.Formatting = centered14;

                RtfTableCellStyle verticalText = new RtfTableCellStyle(RtfBorderSetting.All, centered14, RtfTableCellVerticalAlign.Center, RtfTableCellTextFlow.BottomToTopLeftToRight);

                header.AppendText("ЗВЕДЕННЯ");
                header.AppendParagraph();
                header.AppendText("про виконання документів, що підлягають");
                header.AppendParagraph();

                header.AppendText("індивідуальному контролю, станом на " + currentDate.ToString("dd MMMM yyyy", CultureInfo.CurrentCulture).ToLower() + "р.");

                header.AppendParagraph();
                header.AppendParagraph();

                RtfTable tbl = new RtfTable(RtfTableAlign.Center, 8, departments.Count + 2);
                tbl.Width = TwipConverter.ToTwip(500, MetricUnit.Point);
                tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, centered14);


                foreach (RtfTableRow row in tbl.Rows)
                {
                    row.Height = TwipConverter.ToTwip(30, MetricUnit.Point);
                }
                tbl.Rows[1].Height = TwipConverter.ToTwip(120, MetricUnit.Point);

                tbl.MergeCellsVertically(0, 0, 2);
                tbl.MergeCellsVertically(1, 0, 2);
                tbl.MergeCellsVertically(2, 0, 2);
                tbl.MergeCellsVertically(3, 0, 2);
                tbl.MergeCellsHorizontally(4, 0, 4);

                tbl.Columns[0].Width = TwipConverter.ToTwip(30, MetricUnit.Point);
                tbl.Columns[1].Width = TwipConverter.ToTwip(220, MetricUnit.Point);

                tbl[0, 0].AppendText("№");
                tbl[1, 0].AppendText("Найменування та індекс структурних підрозділів");

                tbl[2, 0].AppendText("Документи на контролі");
                tbl[2, 0].Definition.Style = verticalText;
                tbl[3, 0].AppendText("надійшло за попередній місяць");
                tbl[3, 0].Definition.Style = verticalText;
                tbl[4, 0].AppendText("з них");

                tbl[4, 1].AppendText("виконано");
                tbl[4, 1].Definition.Style = verticalText;
                tbl[5, 1].AppendText("виконуються у строк");
                tbl[5, 1].Definition.Style = verticalText;
                tbl[6, 1].AppendText("продовжено строк виконання");
                tbl[6, 1].Definition.Style = verticalText;
                tbl[7, 1].AppendText("прострочено");
                tbl[7, 1].Definition.Style = verticalText;

                int i = 0;
                foreach (Department department in departments)
                {
                    tbl[0, i + 2].AppendText(department.ID.ToString());
                    tbl[1, i + 2].AppendText(department.Name);

                    controlled.DefaultView.RowFilter = String.Format("ExecutiveDepartmentID = {0}", department.ID);
                    int numberControlled = 0;
                    if (controlled.DefaultView.ToTable().Rows.Count > 0)
                        numberControlled = (int)controlled.DefaultView.ToTable().Rows[0]["Number"];
                    tbl[2, i + 2].AppendText(numberControlled.ToString());

                    controlledLastMonth.DefaultView.RowFilter = String.Format("ExecutiveDepartmentID = {0}", department.ID);
                    int numberControlledLastMonth = 0;
                    if (controlledLastMonth.DefaultView.ToTable().Rows.Count > 0)
                        numberControlledLastMonth = (int)controlledLastMonth.DefaultView.ToTable().Rows[0]["Number"];
                    tbl[3, i + 2].AppendText(numberControlledLastMonth.ToString());

                    executed.DefaultView.RowFilter = String.Format("ExecutiveDepartmentID = {0}", department.ID);
                    int numberExecuted = 0;
                    if (executed.DefaultView.ToTable().Rows.Count > 0)
                        numberExecuted = (int)executed.DefaultView.ToTable().Rows[0]["Number"];
                    tbl[4, i + 2].AppendText(numberExecuted.ToString());

                    executeInTime.DefaultView.RowFilter = String.Format("ExecutiveDepartmentID = {0}", department.ID);
                    int numberExecuteInTime = 0;
                    if (executeInTime.DefaultView.ToTable().Rows.Count > 0)
                        numberExecuteInTime = (int)executeInTime.DefaultView.ToTable().Rows[0]["Number"];
                    tbl[5, i + 2].AppendText(numberExecuteInTime.ToString());

                    continued.DefaultView.RowFilter = String.Format("ExecutiveDepartmentID = {0}", department.ID);
                    int numberContinuede = 0;
                    if (continued.DefaultView.ToTable().Rows.Count > 0)
                        numberContinuede = (int)continued.DefaultView.ToTable().Rows[0]["Number"];
                    tbl[6, i + 2].AppendText(numberContinuede.ToString());

                    overdue.DefaultView.RowFilter = String.Format("ExecutiveDepartmentID = {0}", department.ID);
                    int numberOverdue = 0;
                    if (overdue.DefaultView.ToTable().Rows.Count > 0)
                        numberOverdue = (int)overdue.DefaultView.ToTable().Rows[0]["Number"];
                    tbl[7, i + 2].AppendText(numberOverdue.ToString());

                    i++;
                }

                RtfFormattedParagraph footer = new RtfFormattedParagraph();
                footer.Formatting.FontIndex = 0;
                footer.Formatting.FontSize = 14;
                footer.Formatting.Align = RtfTextAlign.Left;

                footer.AppendParagraph();
                footer.AppendParagraph();
                footer.AppendParagraph();
                /*footer.AppendText("Начальник загального");
                footer.AppendParagraph();
                footer.AppendText("відділу апарату");
                footer.AppendParagraph();
                footer.AppendText("ради і виконкому");
                footer.AppendText(new RtfTabCharacter());*/
                footer.AppendText("____________  _______________________________");
                footer.AppendText(new RtfTabCharacter());
                footer.AppendParagraph(new RtfFormattedText("       (підпис)                     (ініціали (ініціал імені), прізвище)", -1, 8));
                footer.AppendParagraph();
                footer.AppendText("___ _________________ 20___ року");

                rtf.Contents.AddRange(new RtfDocumentContentBase[] { header, tbl, footer });


                RtfWriter rtfWriter = new RtfWriter();
                StringBuilder sb = new StringBuilder();
                using (TextWriter writer = new StringWriter(sb))
                {
                    rtfWriter.Write(writer, rtf);
                }

                string fileName = String.Format("dcn_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

                _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                _context.Response.ContentType = "application/rtf";
                _context.Response.Write(sb.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void GetDocTStatisticsReportControlledOverdueDocuments(int departmentID, int documentCodeID, int executiveDepartmentID, DateTime currentDate)
        {
            try
            {
                DataTable controlled;
                Department executiveDepartment;

                SqlConnection connection = new SqlConnection(Config.ConnectionString);
                try
                {
                    connection.Open();
                    SqlTransaction trans = null;
                    try
                    {
                        trans = connection.BeginTransaction();

                        controlled = DocTemplate.GetControlledOverdue(trans, departmentID, documentCodeID, executiveDepartmentID, currentDate).Tables[0];
                        executiveDepartment = new Department(trans, executiveDepartmentID, UserName);

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

                RtfDocument rtf = new RtfDocument();

                rtf.FontTable.Add(new RtfFont("Times New Roman"));

                RtfParagraphFormatting centered12 = new RtfParagraphFormatting(14, RtfTextAlign.Center);
                centered12.FontIndex = 0;

                RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
                header.Formatting = centered12;

                header.AppendText("ПЕРЕЛІК");
                header.AppendParagraph();
                header.AppendText("документів, не виконаних " + executiveDepartment.Name);
                header.AppendParagraph();

                header.AppendText("в установлений строк станом на " + currentDate.ToString("dd MMMM yyyy", CultureInfo.CurrentCulture).ToLower() + "р.");

                header.AppendParagraph();
                header.AppendParagraph();

                RtfTable tbl = new RtfTable(RtfTableAlign.Center, 8, controlled.Rows.Count + 1);
                tbl.Width = TwipConverter.ToTwip(560, MetricUnit.Point);
                tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, centered12);


                foreach (RtfTableRow row in tbl.Rows)
                {
                    row.Height = TwipConverter.ToTwip(30, MetricUnit.Point);
                }
                tbl.Columns[0].Width = TwipConverter.ToTwip(26, MetricUnit.Point);
                tbl.Columns[1].Width = TwipConverter.ToTwip(110, MetricUnit.Point);
                tbl.Columns[2].Width = TwipConverter.ToTwip(120, MetricUnit.Point);
                tbl.Columns[3].Width = TwipConverter.ToTwip(64, MetricUnit.Point);
                tbl.Columns[4].Width = TwipConverter.ToTwip(80, MetricUnit.Point);

                tbl[0, 0].AppendText("№");
                tbl[1, 0].AppendText("Найменування установи, що надіслала документ, номер і дата документа");

                tbl[2, 0].AppendText("Короткий змісті");
                tbl[3, 0].AppendText("Строк виконання");
                tbl[4, 0].AppendText("Прізвище і посада виконавця");
                tbl[5, 0].AppendText("Причини невиконання в установлений строк");
                tbl[6, 0].AppendText("Стан виконання");
                tbl[7, 0].AppendText("Очікувана дата виконання");

                int i = 0;
                foreach (DataRow row in controlled.Rows)
                {
                    tbl[0, i + 1].AppendText((i + 1).ToString());
                    tbl[1, i + 1].AppendText(String.Format("{0}, № {1}, {2}", row["OrganizationName"], row["Number"], ((DateTime)row["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)));

                    tbl[2, i + 1].AppendText((string)row["Content"]);

                    tbl[3, i + 1].AppendText(((DateTime)row["EndDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));

                    string workerName = FormatHelper.FormatToLastNameAndInitials((string)row["WorkerLastName"],
                                                                                 (string)row["WorkerFirstName"],
                                                                                 (string)row["WorkerMiddleName"]);
                    tbl[4, i + 1].AppendText(String.Format("{0}, {1}", workerName, row["PostName"]));

                    tbl[5, i + 1].AppendText(" ");
                    tbl[6, i + 1].AppendText(" ");
                    tbl[7, i + 1].AppendText(" ");

                    i++;
                }

                RtfFormattedParagraph footer = new RtfFormattedParagraph();
                footer.Formatting.FontIndex = 0;
                footer.Formatting.FontSize = 14;
                footer.Formatting.Align = RtfTextAlign.Left;

                footer.AppendParagraph();
                footer.AppendParagraph();
                footer.AppendParagraph();

                footer.AppendText("____________  _______________________________");
                footer.AppendText(new RtfTabCharacter());
                footer.AppendParagraph(new RtfFormattedText("       (підпис)                     (ініціали (ініціал імені), прізвище)", -1, 8));
                footer.AppendParagraph();
                footer.AppendText("___ _________________ 20___ року");

                rtf.Contents.AddRange(new RtfDocumentContentBase[] { header, tbl, footer });


                RtfWriter rtfWriter = new RtfWriter();
                StringBuilder sb = new StringBuilder();
                using (TextWriter writer = new StringWriter(sb))
                {
                    rtfWriter.Write(writer, rtf);
                }

                string fileName = String.Format("dco_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

                _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                _context.Response.ContentType = "application/rtf";
                _context.Response.Write(sb.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
