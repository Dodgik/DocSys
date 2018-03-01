using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Document;
using BizObj.Models.Document;
using BizObj.Models.Helpers;
using BizObj.Models.JqGrid;
using BizObj.Models.Pager;
using BizObj.Models.Report;
using ESCommon;
using ESCommon.Rtf;
using IO.VFS;
using Newtonsoft.Json;
using Rule = BizObj.Models.Pager.Rule;

namespace BizObj.Controllers
{
    public class AsyncDocument : IAsyncResult
    {

        #region [ Fields ]

        private AsyncCallback _callback;
        private HttpContext _context;

        private string UserName { get; set; }
        private Guid UserId { get; set; }
        private Worker Worker { get; set; }

        #endregion

        #region Implementation of IAsyncResult

        private bool _isCompleted;

        private WaitHandle _asyncWaitHandle = null;

        private object _asyncState;

        private bool _completedSynchronously = false;

        /// <summary>
        /// Возвращает значение, показывающее, выполнена ли асинхронная операция.
        /// </summary>
        /// <returns>
        /// Значение true, если операция завершена, в противном случае — значение false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
        }

        /// <summary>
        /// Возвращает дескриптор <see cref="T:System.Threading.WaitHandle"/>, используемый для режима ожидания завершения асинхронной операции.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.Threading.WaitHandle"/>, используемый для режима ожидания завершения асинхронной операции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return _asyncWaitHandle;
            }
        }

        /// <summary>
        /// Возвращает определенный пользователем объект, который определяет или содержит в себе сведения об асинхронной операции.
        /// </summary>
        /// <returns>
        /// Определенный пользователем объект, который определяет или содержит в себе сведения об асинхронной операции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object AsyncState
        {
            get
            {
                return _asyncState;
            }
        }

        /// <summary>
        /// Возвращает значение, показывающее, синхронно ли закончилась асинхронная операция.
        /// </summary>
        /// <returns>
        /// Значение true, если асинхронная операция завершилась синхронно, в противном случае — значение false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool CompletedSynchronously
        {
            get
            {
                return _completedSynchronously;
            }
        }

        #endregion

        
        #region [ Constructors ]

        public AsyncDocument(HttpContext context, AsyncCallback cb, object extraData, string userName, Guid userId)
        {
            _callback = cb;
            _context = context;
            _asyncState = extraData;
            UserName = userName;
            UserId = userId;
            _isCompleted = false;
            Worker = new Worker(null, UserId);
        }

        #endregion

        #region [ Methods ]

        public void RunProcessRequestAsync()
        {
            ThreadPool.QueueUserWorkItem(ProcessRequestAsync, null);
        }

        public void ProcessRequestAsync(Object workItemState)
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            try
            {
                if (Worker.DepartmentID == 0) {
                    throw new AccessException("User department is not valid", "user");
                }
                
                string obj = request["obj"];
                string template = request["t"];

                if (!String.IsNullOrWhiteSpace(obj))
                {
                    switch (obj)
                    {
                        case "dep":
                            ProcessDepartmentAsync();
                            break;
                        case "documentcode":
                            ProcessDocumentCodeAsync();
                            break;
                        case "docstatus":
                            ProcessDocStatusAsync();
                            break;
                        case "cardstatus":
                            ProcessCardStatusAsync();
                            break;
                        case "socialstatus":
                            ProcessSocialStatusAsync();
                            break;
                        case "socialcategory":
                            ProcessSocialCategoryAsync();
                            break;
                        case "org":
                        case "organization":
                            ProcessOrganizationAsync();
                            break;
                        case "questiontype":
                            ProcessQuestionTypeAsync();
                            break;
                        case "doctype":
                            ProcessDocTypeAsync();
                            break;
                        case "branchtype":
                            ProcessBranchTypeAsync();
                            break;
                        case "deliverytype":
                            ProcessDeliveryTypeAsync();
                            break;
                        case "inputdoctype":
                            ProcessInputDocTypeAsync();
                            break;
                        case "inputmethod":
                            ProcessInputMethodAsync();
                            break;
                        case "inputsign":
                            ProcessInputSignAsync();
                            break;
                        case "inputsubjecttype":
                            ProcessInputSubjectTypeAsync();
                            break;
                        case "post":
                            ProcessPostAsync();
                            break;
                        case "worker":
                            ProcessWorkerAsync();
                            break;
                        case "cityobjecttype":
                            ProcessCityObjectTypeAsync();
                            break;
                        case "cityobject":
                            ProcessCityObjectAsync();
                            break;

                        case "docstatement":
                            ProcessDocStatementAsync();
                            break;

                        case "card":
                            ProcessControlCardAsync();
                            break;
                        case "cardb":
                            ProcessControlCardBlankAsync();
                            break;
                        case "cardg":
                            ProcessControlCardGroup();
                            break;

                        case "docstrep":
                            ProcessDocStReportAsync();
                            break;

                        case "file":
                            ProcessFileAsync();
                            break;
                        case "wdocs":
                            new WorkerScope(_context, UserId, Worker, UserName).ParceRequest();
                            break;
                        case "adocs":
                            new AdminScope(_context, UserId, UserName).ParceRequest();
                            break;
                        case "addlbl":
                            int docId = Convert.ToInt32(request["documentId"]);
                            string jdata = request["jdata"];
                            List<Label> labels = JsonConvert.DeserializeObject<List<Label>>(jdata);

                            ProcessingResult pr = AddLabelsToDocument(docId, labels);

                            string rResult = new JavaScriptSerializer().Serialize(pr);
                            _context.Response.Write(rResult);
                            break;
                        case "coms":
                            ProcessDocumentComment();
                            break;
                    }
                }
                else if (!String.IsNullOrWhiteSpace(template))
                {
                    int templateId = Convert.ToInt32(template);
                    switch (templateId)
                    {
                        case 1:
                            ProcessDocStatementAsync();
                            break;
                        case 2:
                            ProcessDocStatementAsync();
                            break;
                        case 3:
                            new DocTemplateController(_context, UserId, Worker, UserName).ParceRequest();
                            break;
                        case 4:
                            ProcessDocAdminServiceAsync();
                            break;
                        case 5:
                            new Decision(_context, Worker, UserName).ParceRequest();
                            break;
                    }
                }
            }
            catch (AccessException e)
            {
                response.Write(e.Message);
                response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            catch (NotFoundException e)
            {
                response.Write(e.Message);
                response.StatusCode = (int)HttpStatusCode.Gone;
            }
            catch (Exception e)
            {
                response.Write(e.Message);
                response.StatusCode = (int) HttpStatusCode.UnsupportedMediaType;
            }
            finally
            {
                response.Flush();
                
                _isCompleted = true;
                _callback(this);
            }
        }

        private ProcessingResult AddLabelsToDocument(int documentId, List<Label> labels)
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

                    Document.Document doc = new Document.Document(trans, documentId, UserName);
   
                    DocumentLabel[] documentLabels = DocumentLabel.GetDocumentLabels(trans, doc.ID, Worker.DepartmentID);
                    foreach (Label lbl in labels)
                    {
                        bool isNewLabel = true;
                        foreach (DocumentLabel documentLabel in documentLabels)
                        {
                            if (documentLabel.LabelID == lbl.ID)
                            {
                                isNewLabel = false;
                            }
                        }
                        if (isNewLabel)
                        {
                            DocumentLabel label = new DocumentLabel(UserName);
                            label.LabelID = lbl.ID;
                            label.DocumentID = doc.ID;
                            label.DepartmentID = Worker.DepartmentID;
                            label.WorkerID = Worker.ID;
                            label.Insert(trans);
                        }
                    }
                    foreach (DocumentLabel documentLabel in documentLabels)
                    {
                        bool isExist = false;
                        foreach (Label lbl in labels)
                        {
                            if (documentLabel.LabelID == lbl.ID)
                            {
                                isExist = true;
                            }
                        }
                        if (!isExist)
                        {
                            DocumentLabel.Delete(trans, doc.ID, Worker.DepartmentID, documentLabel.ID);
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


        #region [ Department ]
        public void ProcessDepartmentAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];
            int departmentId = Convert.ToInt32(request["dep"]);
            string rResult = String.Empty;
            
            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(data))
                        {
                            ProcessingResult pr = InsertDepartment(data, departmentId);
                            rResult = new JavaScriptSerializer().Serialize(pr);
                        }
                        break;
                    case "page":
                        PageSettings pageSettings = JqGridSettings.GetPageSettings(_context);
                        Rule rule = new Rule {Field = "ParrentDepartmentID", Data = departmentId.ToString()};
                        pageSettings.Where.AddRule(rule);
                        JqGridResults gr = GetDepartmentPage(pageSettings);
                        rResult = new JavaScriptSerializer().Serialize(gr);
                        break;
                    case "search":
                        SearchDepartment(departmentId);
                        break;
                }
            }
            if (!String.IsNullOrWhiteSpace(rResult))
            {
                _context.Response.Write(rResult);
            }
        }

        private ProcessingResult InsertDepartment(string name, int departmentId)
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

                    Department department = new Department(UserName);
                    department.Name = name;
                    department.ShortName = name;
                    department.ObjectID = Guid.NewGuid();
                    department.ParrentDepartmentID = departmentId;
                    pr.Data = department.Insert(trans).ToString(CultureInfo.InvariantCulture);

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


        private JqGridResults GetDepartmentPage(PageSettings pageSettings)
        {

            DataSet dtPage = Department.GetPage(pageSettings);

            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();

            foreach (DataRow dr in dtPage.Tables[0].Rows)
            {
                JqGridRow row = new JqGridRow();
                row.id = (int)dr["DepartmentID"];
                row.cell = new string[3];

                row.cell[0] = dr["DepartmentID"].ToString();
                row.cell[1] = dr["Name"].ToString();
                row.cell[2] = dr["ParrentDepartmentID"].ToString();

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

        private void SearchDepartment(int departmentId)
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            using (DataTable dt = Department.Search(term, departmentId).Tables[0])
            {
                List<string[]> cList = new List<string[]>();

                foreach (DataRow dr in dt.Rows)
                {
                    string[] code = new string[4];
                    code[0] = dr["DepartmentID"].ToString();
                    code[1] = dr["Name"].ToString();
                    cList.Add(code);
                }

                string output = new JavaScriptSerializer().Serialize(cList);
                response.Write(output);
            }
        }
        #endregion
        
        #region [ DocumentCode ]
        public void ProcessDocumentCodeAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetDocumentCodeDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertDocumentCodeAsync(sd);
                        break;
                    case "upd":
                        UpdateDocumentCodeAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteDocumentCodeAsync(sd.ID);
                        break;
                    case "search":
                        SearchDocumentCode();
                        break;
                    case "searchcode":
                        SearchDocumentCodeByDep();
                        break;
                }
            }
        }

        private void GetDocumentCodeDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<DocumentCode> documentCodes = GetDocumentCodees();
            string output = BuildDocumentCodeResults(documentCodes);
            response.Write(output);
        }

        private static string BuildDocumentCodeResults(IEnumerable<DocumentCode> documentCodes)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (DocumentCode documentCode in documentCodes)
            {
                JqGridRow row = new JqGridRow();
                row.id = documentCode.ID;
                row.cell = new string[2];
                row.cell[0] = documentCode.ID.ToString();
                row.cell[1] = documentCode.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<DocumentCode> GetDocumentCodees()
        {
            Collection<DocumentCode> documentCodes = new Collection<DocumentCode>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = DocumentCode.GetReader(connection))
                {
                    DocumentCode documentCode;
                    while (dataReader.Read())
                    {
                        documentCode = new DocumentCode(UserName);
                        documentCode.ID = (int)dataReader["DocumentCodeID"];
                        documentCode.Name = Convert.ToString(dataReader["Name"]);
                        documentCodes.Add(documentCode);
                    }
                }
                return documentCodes;
            }
        }

        private void InsertDocumentCodeAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DocumentCode documentCode = new DocumentCode(UserName);
                    documentCode.ID = sd.ID;
                    documentCode.Name = sd.Name;
                    documentCode.Insert(trans);

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

        private void UpdateDocumentCodeAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DocumentCode documentCode = new DocumentCode(trans, sd.ID, UserName);
                    documentCode.Name = sd.Name;
                    documentCode.Update(trans);

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

        private static void DeleteDocumentCodeAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DocumentCode.Delete(trans, id);

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

        private void SearchDocumentCode()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            using (DataTable dt = DocumentCode.Search(term).Tables[0])
            {
                List<string[]> cList = new List<string[]>();

                foreach (DataRow dr in dt.Rows)
                {
                    string[] code = new string[4];
                    code[0] = dr["DocumentCodeID"].ToString();
                    code[1] = dr["Name"].ToString();
                    cList.Add(code);
                }

                string output = new JavaScriptSerializer().Serialize(cList);
                response.Write(output);
            }
        }

        private void SearchDocumentCodeByDep()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            int templateId = Convert.ToInt32(request["t"]);
            int departmentId = Convert.ToInt32(request["dep"]);
            using (DataTable dt = DocumentCode.SearchCode(term, departmentId, templateId).Tables[0])
            {
                List<string[]> cList = new List<string[]>();

                foreach (DataRow dr in dt.Rows)
                {
                    string[] code = new string[4];
                    code[0] = dr["DocumentCodeID"].ToString();
                    code[1] = dr["Name"].ToString();
                    code[2] = dr["Code"].ToString();
                    cList.Add(code);
                }

                string output = new JavaScriptSerializer().Serialize(cList);
                response.Write(output);
            }
        }
        #endregion
        
        #region [ DocStatus ]
        public void ProcessDocStatusAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if(!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetDocStatusDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertDocStatusAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateDocStatusAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteDocStatusAsync(sd.ID);
                        break;
                    case "search":
                        SearchDocStatus();
                        break;
                }
            }
        }

        private void GetDocStatusDataAsync()
        {
            //string _search = request["_search"];
            //string numberOfRows = request["rows"];
            //string pageIndex = request["page"];
            //string sortColumnName = request["sidx"];
            //string sortOrderBy = request["sord"];
            
            //int totalRecords;
            Collection<DocStatus> dsData = GetDocStatuses();
            string output = BuildDocStatusResults(dsData/*, Convert.ToInt32(numberOfRows), Convert.ToInt32(pageIndex), Convert.ToInt32(totalRecords)*/);
            _context.Response.Write(output);
        }

        private static string BuildDocStatusResults(Collection<DocStatus> data/*, int numberOfRows, int pageIndex, int totalRecords*/)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (DocStatus ds in data)
            {
                JqGridRow row = new JqGridRow();
                row.id = ds.ID;
                row.cell = new string[2];
                row.cell[0] = ds.ID.ToString();
                row.cell[1] = ds.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            /*
            result.page = pageIndex;
            result.total = totalRecords / numberOfRows;
            result.records = totalRecords;
            */
            return new JavaScriptSerializer().Serialize(result);
        }

        private Collection<DocStatus> GetDocStatuses()
        {
            Collection<DocStatus> docStatusCollection = new Collection<DocStatus>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = DocStatus.GetReader(connection))
                {
                    DocStatus docStatus;
                    while (dataReader.Read())
                    {
                        docStatus = new DocStatus(UserName);
                        docStatus.ID = (int)dataReader["DocStatusID"];
                        docStatus.Name = Convert.ToString(dataReader["Name"]);
                        docStatusCollection.Add(docStatus);
                    }
                }
                return docStatusCollection;
            }

        }

        private void InsertDocStatusAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    DocStatus docStatus = new DocStatus(UserName);
                    docStatus.Name = name;
                    docStatus.Insert(trans);

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

        private void UpdateDocStatusAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    DocStatus docStatus = new DocStatus(trans, sd.ID, UserName);
                    docStatus.Name = sd.Name;
                    docStatus.Update(trans);

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

        private static void DeleteDocStatusAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DocStatus.Delete(trans, id);

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

        private void SearchDocStatus()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            using (DataTable statuses = DocStatus.Search(term).Tables[0])
            {
                List<string[]> cList = new List<string[]>();

                foreach (DataRow dr in statuses.Rows)
                {
                    string[] code = new string[4];
                    code[0] = dr["DocStatusID"].ToString();
                    code[1] = dr["Name"].ToString();
                    cList.Add(code);
                }

                string output = new JavaScriptSerializer().Serialize(cList);
                response.Write(output);
            }
        }
        #endregion

        #region [ CardStatus ]
        public void ProcessCardStatusAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetCardStatusDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertCardStatusAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateCardStatusAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteCardStatusAsync(sd.ID);
                        break;
                    case "search":
                        SearchCardStatus();
                        break;
                }
            }
        }

        private void GetCardStatusDataAsync()
        {
            //string _search = request["_search"];
            //string numberOfRows = request["rows"];
            //string pageIndex = request["page"];
            //string sortColumnName = request["sidx"];
            //string sortOrderBy = request["sord"];

            //int totalRecords;
            Collection<CardStatus> dsData = GetCardStatuses();
            string output = BuildCardStatusResults(dsData/*, Convert.ToInt32(numberOfRows), Convert.ToInt32(pageIndex), Convert.ToInt32(totalRecords)*/);
            _context.Response.Write(output);
        }

        private static string BuildCardStatusResults(Collection<CardStatus> data/*, int numberOfRows, int pageIndex, int totalRecords*/)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (CardStatus ds in data)
            {
                JqGridRow row = new JqGridRow();
                row.id = ds.ID;
                row.cell = new string[2];
                row.cell[0] = ds.ID.ToString();
                row.cell[1] = ds.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            /*
            result.page = pageIndex;
            result.total = totalRecords / numberOfRows;
            result.records = totalRecords;
            */
            return new JavaScriptSerializer().Serialize(result);
        }

        private Collection<CardStatus> GetCardStatuses()
        {
            Collection<CardStatus> CardStatusCollection = new Collection<CardStatus>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = CardStatus.GetReader(connection))
                {
                    CardStatus cardStatus;
                    while (dataReader.Read())
                    {
                        cardStatus = new CardStatus(UserName);
                        cardStatus.ID = (int)dataReader["CardStatusID"];
                        cardStatus.Name = Convert.ToString(dataReader["Name"]);
                        CardStatusCollection.Add(cardStatus);
                    }
                }
                return CardStatusCollection;
            }

        }

        private void InsertCardStatusAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    CardStatus cardStatus = new CardStatus(UserName);
                    cardStatus.Name = name;
                    cardStatus.Insert(trans);

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

        private void UpdateCardStatusAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    CardStatus cardStatus = new CardStatus(trans, sd.ID, UserName);
                    cardStatus.Name = sd.Name;
                    cardStatus.Update(trans);

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

        private static void DeleteCardStatusAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    CardStatus.Delete(trans, id);

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

        private void SearchCardStatus()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            using (DataTable statuses = CardStatus.Search(term).Tables[0])
            {
                List<string[]> cList = new List<string[]>();

                foreach (DataRow dr in statuses.Rows)
                {
                    string[] code = new string[4];
                    code[0] = dr["CardStatusID"].ToString();
                    code[1] = dr["Name"].ToString();
                    cList.Add(code);
                }

                string output = new JavaScriptSerializer().Serialize(cList);
                response.Write(output);
            }
        }
        #endregion
        
        #region [ SocialStatus ]
        public void ProcessSocialStatusAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetSocialStatusDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertSocialStatusAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateSocialStatusAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteSocialStatusAsync(sd.ID);
                        break;
                }
            }
        }

        private void GetSocialStatusDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<SocialStatus> socialStatuses = GetSocialStatuses();
            string output = BuildSocialStatusResults(socialStatuses);
            response.Write(output);
        }

        private static string BuildSocialStatusResults(IEnumerable<SocialStatus> socialStatuses)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (SocialStatus socialStatus in socialStatuses)
            {
                JqGridRow row = new JqGridRow();
                row.id = socialStatus.ID;
                row.cell = new string[2];
                row.cell[0] = socialStatus.ID.ToString();
                row.cell[1] = socialStatus.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<SocialStatus> GetSocialStatuses()
        {
            Collection<SocialStatus> socialStatuses = new Collection<SocialStatus>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = SocialStatus.GetReader(connection))
                {
                    SocialStatus socialStatus;
                    while (dataReader.Read())
                    {
                        socialStatus = new SocialStatus(UserName);
                        socialStatus.ID = (int)dataReader["SocialStatusID"];
                        socialStatus.Name = Convert.ToString(dataReader["Name"]);
                        socialStatuses.Add(socialStatus);
                    }
                }
                return socialStatuses;
            }
        }

        private void InsertSocialStatusAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    SocialStatus socialStatus = new SocialStatus(UserName);
                    socialStatus.Name = name;
                    socialStatus.Insert(trans);

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

        private void UpdateSocialStatusAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    SocialStatus socialStatus = new SocialStatus(trans, sd.ID, UserName);
                    socialStatus.Name = sd.Name;
                    socialStatus.Update(trans);

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

        private static void DeleteSocialStatusAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    SocialStatus.Delete(trans, id);

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

        #region [ SocialCategory ]
        public void ProcessSocialCategoryAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetSocialCategoryDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertSocialCategoryAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateSocialCategoryAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteSocialCategoryAsync(sd.ID);
                        break;
                }
            }
        }

        private void GetSocialCategoryDataAsync()
        {
            HttpResponse response = _context.Response;

            Collection<SocialCategory> dsData = GetSocialCategoryes();
            string output = BuildSocialCategoryResults(dsData);
            response.Write(output);
        }

        private static string BuildSocialCategoryResults(Collection<SocialCategory> data)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (SocialCategory sc in data)
            {
                JqGridRow row = new JqGridRow();
                row.id = sc.ID;
                row.cell = new string[2];
                row.cell[0] = sc.ID.ToString();
                row.cell[1] = sc.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private Collection<SocialCategory> GetSocialCategoryes()
        {
            Collection<SocialCategory> socCategoryCollection = new Collection<SocialCategory>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = SocialCategory.GetReader(connection))
                {
                    SocialCategory socCategory;
                    while (dataReader.Read())
                    {
                        socCategory = new SocialCategory(UserName);
                        socCategory.ID = (int)dataReader["SocialCategoryID"];
                        socCategory.Name = Convert.ToString(dataReader["Name"]);
                        socCategoryCollection.Add(socCategory);
                    }
                }
                return socCategoryCollection;
            }

        }

        private void InsertSocialCategoryAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    SocialCategory sc = new SocialCategory(UserName);
                    sc.Name = name;
                    sc.Insert(trans);

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

        private void UpdateSocialCategoryAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    SocialCategory sc = new SocialCategory(trans, sd.ID, UserName);
                    sc.Name = sd.Name;
                    sc.Update(trans);

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

        private static void DeleteSocialCategoryAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    SocialCategory.Delete(trans, id);

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

        #region [ Organization ]
        public void ProcessOrganizationAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];
            string rResult = String.Empty;

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        if (!String.IsNullOrWhiteSpace(data))
                        {
                            int id;
                            if (int.TryParse(data, out id))
                            {
                                Organization org = new Organization(id, UserName);
                                rResult = new JavaScriptSerializer().Serialize(org);
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "data");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("data", "Inuput id is not valid");
                        }
                        break;
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(data))
                        {
                            Organization org = new JavaScriptSerializer().Deserialize<Organization>(data);
                            ProcessingResult pr = InsertOrganization(org);
                            rResult = new JavaScriptSerializer().Serialize(pr);
                        }
                        break;
                    case "upd":
                        if (!String.IsNullOrWhiteSpace(data))
                        {
                            Organization org = new JavaScriptSerializer().Deserialize<Organization>(data);
                            UpdateOrganizationAsync(org);
                            rResult = new JavaScriptSerializer().Serialize(org);
                        }
                        break;
                    case "del":
                        if (!String.IsNullOrWhiteSpace(data))
                        {
                            int id;
                            if (int.TryParse(data, out id))
                            {
                                Organization org = new Organization(id, UserName);
                                ProcessingResult pr = DeleteOrganizationAsync(org.ID);
                                rResult = new JavaScriptSerializer().Serialize(pr);
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "data");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("data", "Inuput id is not valid");
                        }
                        /*
                        if (sd != null)
                            DeleteOrganizationAsync(sd.ID);
                        */
                        break;
                    case "page":
                        PageSettings pageSettings = JqGridSettings.GetPageSettings(_context);
                        JqGridResults gr = GetOrganizationPage(pageSettings);
                        rResult = new JavaScriptSerializer().Serialize(gr);
                        break;
                    case "search":
                        SearchOrganization();
                        break;
                    case "rep":
                        if (!String.IsNullOrWhiteSpace(data))
                        {
                            int id, toId;
                            string toIdStr = request["toId"];
                            if (int.TryParse(data, out id) && int.TryParse(toIdStr, out toId))
                            {
                                Organization org = new Organization(id, UserName);
                                Organization toOrg = new Organization(toId, UserName);
                                ProcessingResult pr = ReplaceOrganization(id, toId);
                                rResult = new JavaScriptSerializer().Serialize(pr);
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "data");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("data", "Inuput id is not valid");
                        }
                        break;
                }

                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }

        private ProcessingResult InsertOrganization(Organization org)
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

                    org.UserName = UserName;
                    org.DepartmentID = Worker.DepartmentID;
                    org.WorkerID = Worker.ID;
                    pr.Data = org.Insert(trans).ToString();

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

        private void UpdateOrganizationAsync(Organization org)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    
                    org.UserName = UserName;
                    org.Update(trans);

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

        private static ProcessingResult DeleteOrganizationAsync(int id)
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

                    Organization.Delete(trans, id);

                    trans.Commit();

                    pr.Success = true;
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.Rollback();

                    pr.Message = e.Message;
                }
            }
            finally
            {
                connection.Close();
            }
            return pr;
        }

        private static ProcessingResult ReplaceOrganization(int organizationId, int toOrganizationId)
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

                    Organization.Replace(trans, organizationId, toOrganizationId);

                    trans.Commit();

                    pr.Success = true;
                }
                catch (Exception e)
                {
                    if (trans != null)
                        trans.Rollback();

                    pr.Message = e.Message;
                }
            }
            finally
            {
                connection.Close();
            }
            return pr;
        }

        private JqGridResults GetOrganizationPage(PageSettings pageSettings)
        {

            DataSet dtPage = Organization.GetPage(pageSettings);
            
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            
            foreach (DataRow dr in dtPage.Tables[0].Rows)
            {
                JqGridRow row = new JqGridRow();
                row.id = (int)dr["OrganizationID"];
                row.cell = new string[3];

                row.cell[0] = dr["OrganizationID"].ToString();
                row.cell[1] = dr["Name"].ToString();
                row.cell[2] = dr["OrganizationTypeID"].ToString();

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

        private void SearchOrganization()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            string orgTypeID = request["orgtype"];
            string orgDepIdStr = request["orgdep"];
            int organizationTypeID = -1;
            int organizationDepartmentId = -1;
            if (!String.IsNullOrWhiteSpace(orgTypeID))
                organizationTypeID = Convert.ToInt32(orgTypeID);
            if (!String.IsNullOrWhiteSpace(orgDepIdStr))
                organizationDepartmentId = Convert.ToInt32(orgDepIdStr);
            using (DataTable dt = Organization.Search(term, organizationTypeID, organizationDepartmentId).Tables[0]) {
                List<string[]> oList = new List<string[]>();

                foreach (DataRow dr in dt.Rows)
                {
                    string[] worker = new string[4];
                    worker[0] = dr["OrganizationID"].ToString();
                    worker[1] = dr["Name"].ToString();
                    oList.Add(worker);
                }

                string output = new JavaScriptSerializer().Serialize(oList);
                response.Write(output);
            }
        }
        #endregion

        #region [ QuestionType ]
        public void ProcessQuestionTypeAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetQuestionTypeDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertQuestionTypeAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateQuestionTypeAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteQuestionTypeAsync(sd.ID);
                        break;
                    case "search":
                        SearchQuestionType();
                        break;
                }
            }
        }

        private void GetQuestionTypeDataAsync()
        {
            HttpResponse response = _context.Response;

            Collection<QuestionType> dsData = GetQuestionTypes();
            string output = BuildQuestionTypeResults(dsData);
            response.Write(output);
        }

        private static string BuildQuestionTypeResults(Collection<QuestionType> questionTypes)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (QuestionType questionType in questionTypes)
            {
                JqGridRow row = new JqGridRow();
                row.id = questionType.ID;
                row.cell = new string[2];
                row.cell[0] = questionType.ID.ToString();
                row.cell[1] = questionType.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private Collection<QuestionType> GetQuestionTypes()
        {
            Collection<QuestionType> questionTypes = new Collection<QuestionType>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = QuestionType.GetReader(connection))
                {
                    QuestionType questionType;
                    while (dataReader.Read())
                    {
                        questionType = new QuestionType(UserName);
                        questionType.ID = (int)dataReader["QuestionTypeID"];
                        questionType.Name = Convert.ToString(dataReader["Name"]);
                        questionTypes.Add(questionType);
                    }
                }
                return questionTypes;
            }
        }

        private void InsertQuestionTypeAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    QuestionType questionType = new QuestionType(UserName);
                    questionType.Name = name;
                    questionType.Insert(trans);

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

        private void UpdateQuestionTypeAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    QuestionType questionType = new QuestionType(trans, sd.ID, UserName);
                    questionType.Name = sd.Name;
                    questionType.Update(trans);

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

        private static void DeleteQuestionTypeAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    QuestionType.Delete(trans, id);

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


        private void SearchQuestionType()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            using (DataTable dt = QuestionType.Search(term).Tables[0])
            {
                List<string[]> qList = new List<string[]>();

                foreach (DataRow dr in dt.Rows)
                {
                    string[] qt = new string[4];
                    qt[0] = dr["QuestionTypeID"].ToString();
                    qt[1] = dr["Name"].ToString();
                    qList.Add(qt);
                }

                string output = new JavaScriptSerializer().Serialize(qList);
                response.Write(output);
            }
        }
        #endregion

        #region [ DocType ]
        public void ProcessDocTypeAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetDocTypeDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertDocTypeAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateDocTypeAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteDocTypeAsync(sd.ID);
                        break;
                    case "search":
                        SearchDocType();
                        break;
                }
            }
        }

        private void GetDocTypeDataAsync()
        {
            HttpResponse response = _context.Response;

            Collection<DocType> dsData = GetDocTypes();
            string output = BuildDocTypeResults(dsData);
            response.Write(output);
        }

        private static string BuildDocTypeResults(Collection<DocType> docTypes)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (DocType docType in docTypes)
            {
                JqGridRow row = new JqGridRow();
                row.id = docType.ID;
                row.cell = new string[2];
                row.cell[0] = docType.ID.ToString();
                row.cell[1] = docType.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private Collection<DocType> GetDocTypes()
        {
            Collection<DocType> docTypes = new Collection<DocType>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = DocType.GetReader(connection))
                {
                    DocType docType;
                    while (dataReader.Read())
                    {
                        docType = new DocType(UserName);
                        docType.ID = (int)dataReader["DocTypeID"];
                        docType.Name = Convert.ToString(dataReader["Name"]);
                        docTypes.Add(docType);
                    }
                }
                return docTypes;
            }
        }

        private void InsertDocTypeAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    DocType docType = new DocType(UserName);
                    docType.Name = name;
                    docType.Insert(trans);

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

        private void UpdateDocTypeAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    DocType docType = new DocType(trans, sd.ID, UserName);
                    docType.Name = sd.Name;
                    docType.Update(trans);

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

        private static void DeleteDocTypeAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DocType.Delete(trans, id);

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


        private void SearchDocType()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            int documentCodeID = Convert.ToInt32(request["code"]);
            using (DataTable dt = DocType.Search(term, documentCodeID).Tables[0])
            {
                List<string[]> qList = new List<string[]>();

                foreach (DataRow dr in dt.Rows)
                {
                    string[] qt = new string[4];
                    qt[0] = dr["DocTypeID"].ToString();
                    qt[1] = dr["Name"].ToString();
                    qList.Add(qt);
                }

                string output = new JavaScriptSerializer().Serialize(qList);
                response.Write(output);
            }
        }
        #endregion

        #region [ BranchType ]
        public void ProcessBranchTypeAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetBranchTypeDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertBranchTypeAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateBranchTypeAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteBranchTypeAsync(sd.ID);
                        break;
                }
            }
        }

        private void GetBranchTypeDataAsync()
        {
            HttpResponse response = _context.Response;

            Collection<BranchType> dsData = GetBranchTypes();
            string output = BuildBranchTypeResults(dsData);
            response.Write(output);
        }

        private static string BuildBranchTypeResults(Collection<BranchType> branchTypes)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (BranchType branchType in branchTypes)
            {
                JqGridRow row = new JqGridRow();
                row.id = branchType.ID;
                row.cell = new string[2];
                row.cell[0] = branchType.ID.ToString();
                row.cell[1] = branchType.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private Collection<BranchType> GetBranchTypes()
        {
            Collection<BranchType> branchTypes = new Collection<BranchType>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = BranchType.GetReader(connection))
                {
                    BranchType branchType;
                    while (dataReader.Read())
                    {
                        branchType = new BranchType(UserName);
                        branchType.ID = (int)dataReader["BranchTypeID"];
                        branchType.Name = Convert.ToString(dataReader["Name"]);
                        branchTypes.Add(branchType);
                    }
                }
                return branchTypes;
            }
        }

        private void InsertBranchTypeAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    BranchType branchType = new BranchType(UserName);
                    branchType.Name = name;
                    branchType.Insert(trans);

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

        private void UpdateBranchTypeAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    BranchType branchType = new BranchType(trans, sd.ID, UserName);
                    branchType.Name = sd.Name;
                    branchType.Update(trans);

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

        private static void DeleteBranchTypeAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    BranchType.Delete(trans, id);

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


        #region [ DeliveryType ]
        public void ProcessDeliveryTypeAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetDeliveryTypeDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertDeliveryTypeAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateDeliveryTypeAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteDeliveryTypeAsync(sd.ID);
                        break;
                }
            }
        }

        private void GetDeliveryTypeDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<DeliveryType> deliveryTypes = GetDeliveryTypees();
            string output = BuildDeliveryTypeResults(deliveryTypes);
            response.Write(output);
        }

        private static string BuildDeliveryTypeResults(IEnumerable<DeliveryType> deliveryTypes)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (DeliveryType deliveryType in deliveryTypes)
            {
                JqGridRow row = new JqGridRow();
                row.id = deliveryType.ID;
                row.cell = new string[2];
                row.cell[0] = deliveryType.ID.ToString();
                row.cell[1] = deliveryType.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<DeliveryType> GetDeliveryTypees()
        {
            Collection<DeliveryType> deliveryTypes = new Collection<DeliveryType>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = DeliveryType.GetReader(connection))
                {
                    DeliveryType deliveryType;
                    while (dataReader.Read())
                    {
                        deliveryType = new DeliveryType(UserName);
                        deliveryType.ID = (int)dataReader["DeliveryTypeID"];
                        deliveryType.Name = Convert.ToString(dataReader["Name"]);
                        deliveryTypes.Add(deliveryType);
                    }
                }
                return deliveryTypes;
            }
        }

        private void InsertDeliveryTypeAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DeliveryType deliveryType = new DeliveryType(UserName);
                    deliveryType.Name = name;
                    deliveryType.Insert(trans);

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

        private void UpdateDeliveryTypeAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DeliveryType deliveryType = new DeliveryType(trans, sd.ID, UserName);
                    deliveryType.Name = sd.Name;
                    deliveryType.Update(trans);

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

        private static void DeleteDeliveryTypeAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    DeliveryType.Delete(trans, id);

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
        
        #region [ InputDocType ]
        public void ProcessInputDocTypeAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetInputDocTypeDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertInputDocTypeAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateInputDocTypeAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteInputDocTypeAsync(sd.ID);
                        break;
                }
            }
        }

        private void GetInputDocTypeDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<InputDocType> inputDocTypes = GetInputDocTypees();
            string output = BuildInputDocTypeResults(inputDocTypes);
            response.Write(output);
        }

        private static string BuildInputDocTypeResults(IEnumerable<InputDocType> inputDocTypes)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (InputDocType inputDocType in inputDocTypes)
            {
                JqGridRow row = new JqGridRow();
                row.id = inputDocType.ID;
                row.cell = new string[2];
                row.cell[0] = inputDocType.ID.ToString();
                row.cell[1] = inputDocType.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<InputDocType> GetInputDocTypees()
        {
            Collection<InputDocType> inputDocTypes = new Collection<InputDocType>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = InputDocType.GetReader(connection))
                {
                    InputDocType inputDocType;
                    while (dataReader.Read())
                    {
                        inputDocType = new InputDocType(UserName);
                        inputDocType.ID = (int)dataReader["InputDocTypeID"];
                        inputDocType.Name = Convert.ToString(dataReader["Name"]);
                        inputDocTypes.Add(inputDocType);
                    }
                }
                return inputDocTypes;
            }
        }

        private void InsertInputDocTypeAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputDocType inputDocType = new InputDocType(UserName);
                    inputDocType.Name = name;
                    inputDocType.Insert(trans);

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

        private void UpdateInputDocTypeAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputDocType inputDocType = new InputDocType(trans, sd.ID, UserName);
                    inputDocType.Name = sd.Name;
                    inputDocType.Update(trans);

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

        private static void DeleteInputDocTypeAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputDocType.Delete(trans, id);

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
        
        #region [ InputMethod ]
        public void ProcessInputMethodAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetInputMethodDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertInputMethodAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateInputMethodAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteInputMethodAsync(sd.ID);
                        break;
                }
            }
        }

        private void GetInputMethodDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<InputMethod> inputMethods = GetInputMethodes();
            string output = BuildInputMethodResults(inputMethods);
            response.Write(output);
        }

        private static string BuildInputMethodResults(IEnumerable<InputMethod> inputMethods)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (InputMethod inputMethod in inputMethods)
            {
                JqGridRow row = new JqGridRow();
                row.id = inputMethod.ID;
                row.cell = new string[2];
                row.cell[0] = inputMethod.ID.ToString();
                row.cell[1] = inputMethod.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<InputMethod> GetInputMethodes()
        {
            Collection<InputMethod> inputMethods = new Collection<InputMethod>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = InputMethod.GetReader(connection))
                {
                    InputMethod inputMethod;
                    while (dataReader.Read())
                    {
                        inputMethod = new InputMethod(UserName);
                        inputMethod.ID = (int)dataReader["InputMethodID"];
                        inputMethod.Name = Convert.ToString(dataReader["Name"]);
                        inputMethods.Add(inputMethod);
                    }
                }
                return inputMethods;
            }
        }

        private void InsertInputMethodAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputMethod inputMethod = new InputMethod(UserName);
                    inputMethod.Name = name;
                    inputMethod.Insert(trans);

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

        private void UpdateInputMethodAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputMethod inputMethod = new InputMethod(trans, sd.ID, UserName);
                    inputMethod.Name = sd.Name;
                    inputMethod.Update(trans);

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

        private static void DeleteInputMethodAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputMethod.Delete(trans, id);

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
        
        #region [ InputSign ]
        public void ProcessInputSignAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetInputSignDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertInputSignAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateInputSignAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteInputSignAsync(sd.ID);
                        break;
                }
            }
        }

        private void GetInputSignDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<InputSign> inputSigns = GetInputSignes();
            string output = BuildInputSignResults(inputSigns);
            response.Write(output);
        }

        private static string BuildInputSignResults(IEnumerable<InputSign> inputSigns)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (InputSign inputSign in inputSigns)
            {
                JqGridRow row = new JqGridRow();
                row.id = inputSign.ID;
                row.cell = new string[2];
                row.cell[0] = inputSign.ID.ToString();
                row.cell[1] = inputSign.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<InputSign> GetInputSignes()
        {
            Collection<InputSign> inputSigns = new Collection<InputSign>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = InputSign.GetReader(connection))
                {
                    InputSign inputSign;
                    while (dataReader.Read())
                    {
                        inputSign = new InputSign(UserName);
                        inputSign.ID = (int)dataReader["InputSignID"];
                        inputSign.Name = Convert.ToString(dataReader["Name"]);
                        inputSigns.Add(inputSign);
                    }
                }
                return inputSigns;
            }
        }

        private void InsertInputSignAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputSign inputSign = new InputSign(UserName);
                    inputSign.Name = name;
                    inputSign.Insert(trans);

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

        private void UpdateInputSignAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputSign inputSign = new InputSign(trans, sd.ID, UserName);
                    inputSign.Name = sd.Name;
                    inputSign.Update(trans);

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

        private static void DeleteInputSignAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputSign.Delete(trans, id);

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

        #region [ InputSubjectType ]
        public void ProcessInputSubjectTypeAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            SimpleDictionary sd = null;
            if (!String.IsNullOrWhiteSpace(data))
                sd = new JavaScriptSerializer().Deserialize<SimpleDictionary>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetInputSubjectTypeDataAsync();
                        break;
                    case "ins":
                        if (sd != null)
                            InsertInputSubjectTypeAsync(sd.Name);
                        break;
                    case "upd":
                        UpdateInputSubjectTypeAsync(sd);
                        break;
                    case "del":
                        if (sd != null)
                            DeleteInputSubjectTypeAsync(sd.ID);
                        break;
                }
            }
        }

        private void GetInputSubjectTypeDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<InputSubjectType> inputSubjectTypes = GetInputSubjectTypees();
            string output = BuildInputSubjectTypeResults(inputSubjectTypes);
            response.Write(output);
        }

        private static string BuildInputSubjectTypeResults(IEnumerable<InputSubjectType> inputSubjectTypes)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (InputSubjectType inputSubjectType in inputSubjectTypes)
            {
                JqGridRow row = new JqGridRow();
                row.id = inputSubjectType.ID;
                row.cell = new string[2];
                row.cell[0] = inputSubjectType.ID.ToString();
                row.cell[1] = inputSubjectType.Name;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<InputSubjectType> GetInputSubjectTypees()
        {
            Collection<InputSubjectType> inputSubjectTypes = new Collection<InputSubjectType>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = InputSubjectType.GetReader(connection))
                {
                    InputSubjectType inputSubjectType;
                    while (dataReader.Read())
                    {
                        inputSubjectType = new InputSubjectType(UserName);
                        inputSubjectType.ID = (int)dataReader["InputSubjectTypeID"];
                        inputSubjectType.Name = Convert.ToString(dataReader["Name"]);
                        inputSubjectTypes.Add(inputSubjectType);
                    }
                }
                return inputSubjectTypes;
            }
        }

        private void InsertInputSubjectTypeAsync(string name)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputSubjectType inputSubjectType = new InputSubjectType(UserName);
                    inputSubjectType.Name = name;
                    inputSubjectType.Insert(trans);

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

        private void UpdateInputSubjectTypeAsync(SimpleDictionary sd)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputSubjectType inputSubjectType = new InputSubjectType(trans, sd.ID, UserName);
                    inputSubjectType.Name = sd.Name;
                    inputSubjectType.Update(trans);

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

        private static void DeleteInputSubjectTypeAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    InputSubjectType.Delete(trans, id);

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
        

        #region [ Post ]
        public void ProcessPostAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];
            int departmentId = Convert.ToInt32(request["dep"]);
            string pid = request["pid"];
            string rResult = String.Empty;

            Post post = null;
            if (!String.IsNullOrWhiteSpace(data))
                post = new JavaScriptSerializer().Deserialize<Post>(data);

            if (departmentId > 0 && !String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetPostDataAsync(departmentId);
                        break;
                    case "ins":
                        if (post != null)
                        {
                            ProcessingResult pr = InsertPostAsync(post, departmentId);
                            rResult = new JavaScriptSerializer().Serialize(pr);
                        }
                        break;
                    case "upd":
                        if (post != null && post.ID > 0)
                        {
                            ProcessingResult pr = UpdatePostAsync(post, departmentId);
                            rResult = new JavaScriptSerializer().Serialize(pr);
                        }
                        break;
                    case "del":
                        if (!String.IsNullOrWhiteSpace(pid))
                        {
                            int id;
                            if (int.TryParse(pid, out id) && id > 0)
                            {
                                ProcessingResult pr = DeletePostAsync(id, UserName);
                                rResult = new JavaScriptSerializer().Serialize(pr);
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "pid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("pid", "Inuput id is not valid");
                        }
                        break;
                }
                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }

        private void GetPostDataAsync(int departmentID)
        {
            HttpResponse response = _context.Response;
            PageSettings pageSettings = JqGridSettings.GetPageSettings(_context);

            Post postFilter = new Post();
            postFilter.DepartmentID = departmentID;
            if (pageSettings.Where.HasRule("Name"))
                postFilter.Name = pageSettings.Where.GetRule("Name").Data;
            else
                postFilter.Name = String.Empty;

            IEnumerable<Post> dsData = Post.GetPosts(postFilter);
            string output = BuildPostResults(dsData);
            response.Write(output);
        }

        private static string BuildPostResults(IEnumerable<Post> posts)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (Post post in posts)
            {
                JqGridRow row = new JqGridRow();
                row.id = post.ID;
                row.cell = new string[5];
                row.cell[0] = post.ID.ToString();
                row.cell[1] = post.Name;
                row.cell[2] = post.DepartmentID.ToString();
                row.cell[3] = post.IsVacant.ToString();
                row.cell[4] = post.PostTypeID.ToString();
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private ProcessingResult InsertPostAsync(Post post, int departmentID)
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
                    
                    post.UserName = UserName;
                    pr.Data = post.Insert(trans).ToString(CultureInfo.InvariantCulture);

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

        private ProcessingResult UpdatePostAsync(Post post, int departmentID)
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
                    
                    post.UserName = UserName;
                    post.Update(trans);

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

        private static ProcessingResult DeletePostAsync(int id, string userName)
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

                    Post.Delete(trans, id, userName);

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
        #endregion
        
        #region [ Worker ]
        public void ProcessWorkerAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];
            int departmentId = Convert.ToInt32(request["dep"]);
            string wid = request["wid"];
            string rResult = String.Empty;

            Worker worker = null;
            if (!String.IsNullOrWhiteSpace(data))
                worker = new JavaScriptSerializer().Deserialize<Worker>(data);

            if (!String.IsNullOrWhiteSpace(type)) {
                switch (type)
                {
                    case "get":
                        //GetWorkerDataAsync(departmentID);
                        if (!String.IsNullOrWhiteSpace(wid))
                        {
                            int id;
                            if (int.TryParse(wid, out id) && id > 0)
                            {
                                Worker w = new Worker(id, UserName);
                                rResult = new JavaScriptSerializer().Serialize(w);
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "wid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("wid", "Inuput id is not valid");
                        }
                        break;
                    case "getlist":
                        GetWorkerDataAsync(departmentId);
                        break;
                    case "ins":
                        if (worker != null)
                        {
                            ProcessingResult pr = InsertWorkerAsync(worker);
                            rResult = new JavaScriptSerializer().Serialize(pr);
                        }
                        break;
                    case "upd":
                        if (worker != null && worker.ID > 0)
                        {
                            ProcessingResult pr = UpdateWorkerAsync(worker);
                            rResult = new JavaScriptSerializer().Serialize(pr);
                        }
                        break;
                    case "del":
                        if (!String.IsNullOrWhiteSpace(wid))
                        {
                            int id;
                            if (int.TryParse(wid, out id) && id > 0)
                            {
                                ProcessingResult pr = DeleteWorkerAsync(id);
                                rResult = new JavaScriptSerializer().Serialize(pr);
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "wid");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("wid", "Inuput id is not valid");
                        }
                        break;
                    case "search":
                        SearchWorker(departmentId == 0 ? (int?)null : departmentId);
                        break;
                }
                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }

        private void GetWorkerDataAsync(int departmentID)
        {
            HttpResponse response = _context.Response;

            PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

            DataTable dtPage = Worker.GetPage(gridSettings, UserName, departmentID);
            JqGridResults jqGridResults = Worker.BuildJqGridResults(dtPage, gridSettings);

            string output = new JavaScriptSerializer().Serialize(jqGridResults);
            response.Write(output);
        }

        private ProcessingResult InsertWorkerAsync(Worker worker)
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

                    worker.UserName = UserName;
                    pr.Data = worker.Insert(trans).ToString(CultureInfo.InvariantCulture);

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

        private ProcessingResult UpdateWorkerAsync(Worker worker)
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
                    
                    worker.UserName = UserName;
                    worker.Update(trans);

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

        private ProcessingResult DeleteWorkerAsync(int id)
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

                    Worker.Delete(trans, id, UserName);

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

        private void SearchWorker(int? departmentId)
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            using (DataTable dt = Worker.Search(departmentId, term).Tables[0])
            {
                List<string[]> wList = new List<string[]>();

                foreach (DataRow dr in dt.Rows)
                {
                    string[] worker = new string[4];
                    worker[0] = dr["WorkerID"].ToString();
                    worker[1] = dr["LastName"].ToString();
                    worker[2] = dr["FirstName"].ToString();
                    worker[3] = dr["MiddleName"].ToString();
                    wList.Add(worker);
                }

                string output = new JavaScriptSerializer().Serialize(wList);
                response.Write(output);
            }
        }
        #endregion


        #region [ CityObjectType ]
        public void ProcessCityObjectTypeAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            CityObjectTypeBase cityObjectTypeBase = null;
            if (!String.IsNullOrWhiteSpace(data))
                cityObjectTypeBase = new JavaScriptSerializer().Deserialize<CityObjectTypeBase>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetCityObjectTypeDataAsync();
                        break;
                    case "ins":
                        if (cityObjectTypeBase != null)
                            InsertCityObjectTypeAsync(cityObjectTypeBase);
                        break;
                    case "upd":
                        UpdateCityObjectTypeAsync(cityObjectTypeBase);
                        break;
                    case "del":
                        if (cityObjectTypeBase != null)
                            DeleteCityObjectTypeAsync(cityObjectTypeBase.ID);
                        break;
                }
            }
        }

        private void GetCityObjectTypeDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<CityObjectType> sityObjectTypes = GetCityObjectTypes();
            string output = BuildCityObjectTypeResults(sityObjectTypes);
            response.Write(output);
        }

        private static string BuildCityObjectTypeResults(IEnumerable<CityObjectType> sityObjectTypes)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (CityObjectType sityObjectType in sityObjectTypes)
            {
                JqGridRow row = new JqGridRow();
                row.id = sityObjectType.ID;
                row.cell = new string[3];
                row.cell[0] = sityObjectType.ID.ToString();
                row.cell[1] = sityObjectType.Name;
                row.cell[2] = sityObjectType.ShortName;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<CityObjectType> GetCityObjectTypes()
        {
            Collection<CityObjectType> sityObjectTypes = new Collection<CityObjectType>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = CityObjectType.GetReader(connection))
                {
                    CityObjectType sityObjectType;
                    while (dataReader.Read())
                    {
                        sityObjectType = new CityObjectType(UserName);
                        sityObjectType.ID = (short) dataReader["CityObjectTypeID"];
                        sityObjectType.Name = Convert.ToString(dataReader["CityObjectTypeName"]);
                        sityObjectType.ShortName = Convert.ToString(dataReader["CityObjectTypeShortName"]);
                        sityObjectTypes.Add(sityObjectType);
                    }
                }
                return sityObjectTypes;
            }
        }

        private void InsertCityObjectTypeAsync(CityObjectTypeBase cityObjectTypeBase)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    CityObjectType sityObjectType = new CityObjectType(UserName);
                    sityObjectType.Name = cityObjectTypeBase.Name;
                    sityObjectType.ShortName = cityObjectTypeBase.ShortName;
                    sityObjectType.Insert(trans);

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

        private void UpdateCityObjectTypeAsync(CityObjectTypeBase cityObjectTypeBase)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    CityObjectType sityObjectType = new CityObjectType(trans, cityObjectTypeBase.ID, UserName);
                    sityObjectType.Name = cityObjectTypeBase.Name;
                    sityObjectType.ShortName = cityObjectTypeBase.ShortName;
                    sityObjectType.Update(trans);

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

        private static void DeleteCityObjectTypeAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    CityObjectType.Delete(trans, id);

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

        #region [ CityObject ]
        public void ProcessCityObjectAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];

            CityObjectBase cityObjectBase = null;
            if (!String.IsNullOrWhiteSpace(data))
                cityObjectBase = new JavaScriptSerializer().Deserialize<CityObjectBase>(data);

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        GetCityObjectDataAsync();
                        break;
                    case "ins":
                        if (cityObjectBase != null)
                            InsertCityObjectAsync(cityObjectBase);
                        break;
                    case "upd":
                        UpdateCityObjectAsync(cityObjectBase);
                        break;
                    case "del":
                        if (cityObjectBase != null)
                            DeleteCityObjectAsync(cityObjectBase.ID);
                        break;
                    case "search":
                        SearchCityObject();
                        break;
                }
            }
        }

        private void GetCityObjectDataAsync()
        {
            HttpResponse response = _context.Response;

            IEnumerable<CityObject> cityObjects = GetCityObjects();
            string output = BuildCityObjectResults(cityObjects);
            response.Write(output);
        }

        private static string BuildCityObjectResults(IEnumerable<CityObject> cityObjects)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (CityObject cityObject in cityObjects)
            {
                JqGridRow row = new JqGridRow();
                row.id = cityObject.ID;
                row.cell = new string[7];
                row.cell[0] = cityObject.ID.ToString();
                row.cell[1] = cityObject.Name;
                row.cell[2] = cityObject.OldName;
                row.cell[3] = cityObject.SearchName;
                row.cell[4] = cityObject.IsReal.ToString();
                row.cell[5] = cityObject.TypeID.ToString();
                row.cell[6] = cityObject.TypeName;
                rows.Add(row);
            }
            result.rows = rows.ToArray();
            return new JavaScriptSerializer().Serialize(result);
        }

        private IEnumerable<CityObject> GetCityObjects()
        {
            Collection<CityObject> cityObjects = new Collection<CityObject>();
            using (SqlConnection connection = new SqlConnection(Config.ConnectionString))
            {
                connection.Open();

                using (SqlDataReader dataReader = CityObject.GetReader(connection))
                {
                    CityObject cityObject;
                    while (dataReader.Read())
                    {
                        cityObject = new CityObject(UserName);
                        cityObject.ID = (int) dataReader["CityObjectID"];
                        cityObject.Name = Convert.ToString(dataReader["CityObjectName"]);
                        cityObject.OldName = Convert.ToString(dataReader["CityObjectOldName"]);
                        cityObject.SearchName = Convert.ToString(dataReader["CityObjectSearchName"]);
                        cityObject.IsReal = (bool) dataReader["IsReal"];
                        cityObject.TypeID = (short) dataReader["CityObjectTypeID"];
                        cityObject.TypeName = Convert.ToString(dataReader["CityObjectTypeName"]);
                        cityObjects.Add(cityObject);
                    }
                }
                return cityObjects;
            }
        }

        private void SearchCityObject()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string term = request["term"];
            using (DataTable dt = CityObject.Search(term).Tables[0])
            {
                List<string[]> sList = new List<string[]>();

                foreach (DataRow dr in dt.Rows)
                {
                    string[] streets = new string[5];
                    streets[0] = dr["CityObjectID"].ToString();
                    streets[1] = dr["CityObjectName"].ToString();
                    streets[2] = dr["CityObjectTypeShortName"].ToString();
                    streets[3] = dr["CityObjectTypeName"].ToString();
                    streets[4] = dr["CityObjectOldName"].ToString();
                    sList.Add(streets);
                }

                string output = new JavaScriptSerializer().Serialize(sList);
                response.Write(output);
            }
        }

        private void ViewToResponse(DataView view)
        {
           _context.Response.Write(JSONHelper.ToJsonString(view));            
        }

        private void InsertCityObjectAsync(CityObjectBase cityObjectBase)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    CityObject cityObject = new CityObject(UserName);
                    cityObject.Name = cityObjectBase.Name;
                    cityObject.OldName = cityObjectBase.OldName;
                    cityObject.SearchName = cityObjectBase.SearchName;
                    cityObject.IsReal = cityObjectBase.IsReal;
                    cityObject.TypeID = cityObjectBase.TypeID;
                    cityObject.Insert(trans);

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

        private void UpdateCityObjectAsync(CityObjectBase cityObjectBase)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    CityObject cityObject = new CityObject(trans, cityObjectBase.ID, UserName);
                    cityObject.Name = cityObjectBase.Name;
                    cityObject.OldName = cityObjectBase.OldName;
                    cityObject.SearchName = cityObjectBase.SearchName;
                    cityObject.IsReal = cityObjectBase.IsReal;
                    cityObject.TypeID = cityObjectBase.TypeID;
                    cityObject.Update(trans);

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

        private static void DeleteCityObjectAsync(int id)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    CityObject.Delete(trans, id);

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



        #region [ DocStatement ]
        public void ProcessDocStatementAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string jdata = request["jdata"];
            string rResult = String.Empty;

            if (!String.IsNullOrWhiteSpace(type))
            {
                DocStatement docStatement;
                switch (type)
                {
                    case "get":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetDocStatementObjAsync(id);
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
                                GetDocStatementAdminBlank(id);
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
                        
                        bool isReception = Convert.ToBoolean(request["isReception"]);
                        int departmentID = Convert.ToInt32(request["departmentID"]);
                        GetDocStatementDataAsync(isReception, departmentID);
                        break;
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            try
                            {
                                docStatement = new JavaScriptSerializer().Deserialize<DocStatement>(jdata);
                            }
                            catch(Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            ProcessingResult pr = InsertDocStatementAsync(docStatement);
                            rResult = new JavaScriptSerializer().Serialize(pr);
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
                                docStatement = new JavaScriptSerializer().Deserialize<DocStatement>(jdata);
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            ProcessingResult pr = UpdateDocStatementAsync(docStatement);
                            rResult = new JavaScriptSerializer().Serialize(pr);
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
                                ProcessingResult pr = DeleteDocStatementAsync(id);
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
                    case "getpagedoc":
                        int depId = Convert.ToInt32(request["dep"]);
                        bool isRec = Convert.ToBoolean(request["isReception"]);
                        GetDocStPageRtf(depId, isRec);
                        break;
                }
            }

            if (!String.IsNullOrWhiteSpace(rResult))
            {
                _context.Response.Write(rResult);
            }
        }

        private void GetDocStatementObjAsync(int id)
        {
            HttpResponse response = _context.Response;
            DocStatement docStatement;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    docStatement = new DocStatement(trans, id, UserName);

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

            string output = new JavaScriptSerializer().Serialize(docStatement);

            response.Write(output);
        }

        private void GetDocStatementAdminBlank(int id)
        {
            HttpResponse response = _context.Response;
            DocStatementAdminBlank docStatementB;

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

                    docStatementB = new DocStatementAdminBlank(trans, id, departmentId, UserName);

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

            string output = new JavaScriptSerializer().Serialize(docStatementB);

            response.Write(output);
        }

        private void GetDocStatementDataAsync(bool isReception, int departmentID)
        {
            HttpResponse response = _context.Response;

            PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

            DataTable dtPage = DocStatement.GetPage(gridSettings, isReception, UserName, departmentID);
            JqGridResults jqGridResults = DocStatement.BuildJqGridResults(dtPage, gridSettings);

            string output = new JavaScriptSerializer().Serialize(jqGridResults);
            response.Write(output);
        }

        private ProcessingResult InsertDocStatementAsync(DocStatement docStatement)
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
                    docStatement.Document.UserName = UserName;
                    if (mu != null)
                    {
                        if (mu.ProviderUserKey != null)
                        {
                            docStatement.Document.OriginatorID = (Guid)mu.ProviderUserKey;
                            docStatement.Document.OwnerID = (Guid)mu.ProviderUserKey;
                            docStatement.Document.RequestorID = (Guid)mu.ProviderUserKey;
                        }
                    }
                    docStatement.Document.RequestDate = DateTime.Now;
                    docStatement.Document.DocStatusID = 0;
                    docStatement.Document.Insert(trans);

                    foreach (int labelId in docStatement.Document.Labels)
                    {
                        DocumentLabel label = new DocumentLabel(UserName);
                        label.LabelID = labelId;
                        label.DocumentID = docStatement.Document.ID;
                        label.DepartmentID = docStatement.Document.DepartmentID;
                        label.WorkerID = Worker.ID;
                        label.Insert(trans);
                    }

                    docStatement.Citizen.UserName = UserName;
                    docStatement.Citizen.Insert(trans);

                    if (!docStatement.IsReception && docStatement.Document.Source.OrganizationID > 0)
                    {
                        docStatement.HeadID = 0;
                    }

                    DateTime sDate = docStatement.Document.Source.CreationDate;
                    if (sDate < (DateTime)SqlDateTime.MinValue || sDate > (DateTime)SqlDateTime.MaxValue)
                    {
                        docStatement.Document.Source.CreationDate = (DateTime)SqlDateTime.MinValue;
                    }

                    docStatement.Document.Source.UserName = UserName;
                    docStatement.Document.Source.ID = docStatement.Document.ID;
                    docStatement.Document.Source.CitizenID = docStatement.Citizen.ID;
                    docStatement.Document.Source.Insert(trans);


                    DateTime desDate = docStatement.Document.Destination.CreationDate;
                    if (desDate < (DateTime)SqlDateTime.MinValue || desDate > (DateTime)SqlDateTime.MaxValue)
                    {
                        docStatement.Document.Destination.CreationDate = (DateTime)SqlDateTime.MinValue;
                    }
                    docStatement.Document.Destination.UserName = UserName;
                    docStatement.Document.Destination.ID = docStatement.Document.ID;
                    docStatement.Document.Destination.Insert(trans);

                    foreach (int socialCategoryID in docStatement.Citizen.SocialCategories)
                    {
                        SocialCategoryList socialCategoryList = new SocialCategoryList(UserName);
                        socialCategoryList.CitizenID = docStatement.Citizen.ID;
                        socialCategoryList.SocialCategoryID = socialCategoryID;
                        socialCategoryList.Insert(trans);
                    }

                    docStatement.UserName = UserName;
                    docStatement.CitizenID = docStatement.Citizen.ID;
                    docStatement.DocumentID = docStatement.Document.ID;
                    pr.Data = docStatement.Insert(trans).ToString(CultureInfo.InvariantCulture);

                    foreach (int branchTypeID in docStatement.Branches)
                    {
                        BranchList branchList = new BranchList(UserName);
                        branchList.BranchTypeID = branchTypeID;
                        branchList.DocStatementID = docStatement.ID;
                        branchList.Insert(trans);
                    }

                    foreach (DocumentFile df in docStatement.Document.Files)
                    {
                        if (df.FileID > 0)
                        {
                            df.DocumentID = docStatement.DocumentID;
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

        private ProcessingResult UpdateDocStatementAsync(DocStatement docStatement)
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
                    docStatement.Document.UserName = UserName;
                    if (mu != null) {
                        if (mu.ProviderUserKey != null) {
                            docStatement.Document.OriginatorID = (Guid)mu.ProviderUserKey;
                            docStatement.Document.OwnerID = (Guid)mu.ProviderUserKey;
                            docStatement.Document.RequestorID = (Guid)mu.ProviderUserKey;
                        }
                    }

                    docStatement.Document.ID = docStatement.DocumentID;
                    Document.Document doc = new Document.Document(trans, docStatement.DocumentID, UserName);
                    docStatement.Document.ObjectID = doc.ObjectID;

                    docStatement.Document.RequestDate = DateTime.Now;
                    docStatement.Document.Update(trans);

                    DocumentLabel[] documentLabels = DocumentLabel.GetDocumentLabels(trans, docStatement.Document.ID,
                                                                           docStatement.Document.DepartmentID);
                    foreach (int labelId in docStatement.Document.Labels) {
                        bool isNewLabel = true;
                        foreach (DocumentLabel documentLabel in documentLabels) {
                            if (documentLabel.LabelID == labelId) {
                                isNewLabel = false;
                            }
                        }
                        if (isNewLabel) {
                            DocumentLabel label = new DocumentLabel(UserName);
                            label.LabelID = labelId;
                            label.DocumentID = docStatement.Document.ID;
                            label.DepartmentID = docStatement.Document.DepartmentID;
                            label.WorkerID = Worker.ID;
                            label.Insert(trans);
                        }
                    }
                    foreach (DocumentLabel documentLabel in documentLabels) {
                        bool isExist = false;
                        foreach (int labelId in docStatement.Document.Labels)  {
                            if (documentLabel.LabelID == labelId) {
                                isExist = true;
                            }
                        }
                        if (!isExist) {
                            DocumentLabel.Delete(trans, docStatement.Document.ID, docStatement.Document.DepartmentID, documentLabel.ID);
                        }
                    }

                    docStatement.Citizen.UserName = UserName;
                    docStatement.Citizen.Update(trans);

                    if (!docStatement.IsReception)
                    {
                        docStatement.HeadID = 0;
                    }
                    docStatement.Document.Source.UserName = UserName;
                    DateTime sDate = docStatement.Document.Source.CreationDate;
                    if (sDate < (DateTime)SqlDateTime.MinValue || sDate > (DateTime)SqlDateTime.MaxValue)
                    {
                        docStatement.Document.Source.CreationDate = (DateTime)SqlDateTime.MinValue;
                    }
                    docStatement.Document.Source.CitizenID = docStatement.Citizen.ID;
                    docStatement.Document.Source.Update(trans);

                    docStatement.Document.Destination.UserName = UserName;
                    DateTime desDate = docStatement.Document.Destination.CreationDate;
                    if (desDate < (DateTime)SqlDateTime.MinValue || desDate > (DateTime)SqlDateTime.MaxValue)
                    {
                        docStatement.Document.Destination.CreationDate = (DateTime)SqlDateTime.MinValue;
                    }
                    docStatement.Document.Destination.Update(trans);

                    SocialCategoryList.DeleteList(trans, docStatement.Citizen.ID, UserName);
                    foreach (int socialCategoryID in docStatement.Citizen.SocialCategories)
                    {
                        SocialCategoryList socialCategoryList = new SocialCategoryList(UserName);
                        socialCategoryList.CitizenID = docStatement.Citizen.ID;
                        socialCategoryList.SocialCategoryID = socialCategoryID;
                        socialCategoryList.Insert(trans);
                    }

                    docStatement.UserName = UserName;
                    docStatement.Update(trans);

                    BranchList.DeleteList(trans, docStatement.ID, UserName);
                    foreach (int branchTypeID in docStatement.Branches)
                    {
                        BranchList branchList = new BranchList(UserName);
                        branchList.BranchTypeID = branchTypeID;
                        branchList.DocStatementID = docStatement.ID;
                        branchList.Insert(trans);
                    }


                    foreach (DocumentFile df in docStatement.Document.Files)
                    {
                        if (df.FileID > 0)
                        {
                            df.DocumentID = docStatement.DocumentID;
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

        private ProcessingResult DeleteDocStatementAsync(int id)
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

                    DocStatement docStatement = new DocStatement(trans, id, UserName);

                    BranchList.DeleteList(trans, docStatement.ID, UserName);
                    DocStatement.Delete(trans, docStatement.ID, UserName);

                    SocialCategoryList.DeleteList(trans, docStatement.CitizenID, UserName);

                    Source.Delete(trans, docStatement.DocumentID, UserName);
                    Destination.Delete(trans, docStatement.DocumentID, UserName);

                    Citizen.Delete(trans, docStatement.CitizenID, UserName);

                    DocumentLabel.DeleteList(trans, docStatement.DocumentID);
                    DocumentFile.DeleteFiles(trans, docStatement.DocumentID, Worker);

                    Document.Document.Delete(trans, docStatement.DocumentID, UserName);

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
        #endregion

        #region [ ControlCard ]
        public void ProcessControlCardAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string jdata = request["jdata"];
            string depStr = request["dep"];
            string rResult = String.Empty;

            if (!String.IsNullOrWhiteSpace(type))
            {
                ControlCard controlCard;
                int departmentId = 0;
                if (!int.TryParse(depStr, out departmentId))
                {
                    departmentId = 0;
                }
                switch (type)
                {
                    case "get":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetControlCardObjAsync(id);
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
                    case "getlast":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetLastControlCardObj(id, departmentId);
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
                        int documentID = int.Parse(request["id"]);
                        GetControlCardDataAsync(documentID, departmentId);
                        break;
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            try
                            {
                                controlCard = JsonConvert.DeserializeObject<ControlCard>(jdata, new DateTimeConvertorCustome());
                                //controlCard = new JavaScriptSerializer().Deserialize<ControlCard>(jdata);
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            ProcessingResult pr = InsertControlCardAsync(controlCard);
                            rResult = new JavaScriptSerializer().Serialize(pr);
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
                                controlCard = JsonConvert.DeserializeObject<ControlCard>(jdata, new DateTimeConvertorCustome());
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            ProcessingResult pr = UpdateControlCardAsync(controlCard);
                            rResult = new JavaScriptSerializer().Serialize(pr);
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
                                ProcessingResult pr = DeleteControlCardAsync(id);
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
                        string docStatementIDStr = request["docStatementID"];
                        if (!String.IsNullOrWhiteSpace(jdata) & !String.IsNullOrWhiteSpace(docStatementIDStr))
                        {
                            int id;
                            int docStatementID;
                            if (int.TryParse(jdata, out id) & int.TryParse(docStatementIDStr, out docStatementID))
                                GetControlCardReport(id, docStatementID);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "jdat" + "a");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("jda" + "ta", "Inuput id is not valid");
                        }
                        break;
                    case "innernumber":
                        string cardId = request["cardid"];
                        int controlCardId;
                        if (int.TryParse(cardId, out controlCardId))
                        {
                            ProcessingResult pr = new ProcessingResult();
                            try
                            {
                                string number = String.IsNullOrWhiteSpace(jdata) ? String.Empty : jdata;
                                controlCard = new ControlCard(controlCardId, UserName);
                                controlCard.InnerNumber = number;
                                ControlCard.SetInnerNumber(controlCardId, number);
                                if (DocumentRegistration.IsExist(controlCard.DocumentID, controlCard.ExecutiveDepartmentID)) {
                                    DocumentRegistration documentRegistration = new DocumentRegistration(controlCard.DocumentID, controlCard.ExecutiveDepartmentID);
                                    documentRegistration.Number = number;
                                    documentRegistration.Update();
                                } else {
                                    DocumentRegistration documentRegistration = new DocumentRegistration();
                                    documentRegistration.DocumentID = controlCard.DocumentID;
                                    documentRegistration.DepartmentID = controlCard.ExecutiveDepartmentID;
                                    documentRegistration.WorkerID = Worker.ID;
                                    documentRegistration.Number = number;
                                    documentRegistration.Insert();
                                }
                                pr.Success = true;
                            }
                            catch (Exception e)
                            {
                                pr.Message = e.Message;
                            }
                            rResult = new JavaScriptSerializer().Serialize(pr);
                        }
                        else
                        {
                            throw new ArgumentNullException("jdata", "Inuput id is not valid");
                        }
                        break;
                }

                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }

        private void GetControlCardObjAsync(int id)
        {
            HttpResponse response = _context.Response;
            ControlCard controlCard;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();
                    
                    controlCard = new ControlCard(trans, id, UserName);

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

            string output = new JavaScriptSerializer().Serialize(controlCard);

            response.Write(output);
        }

        private void GetLastControlCardObj(int id, int departmentId)
        {
            HttpResponse response = _context.Response;
            ControlCard controlCard;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    controlCard = ControlCard.GetLastCard(trans, id, departmentId, UserName);

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

            string output = new JavaScriptSerializer().Serialize(controlCard);

            response.Write(output);
        }

        private void GetControlCardDataAsync(int documentID, int departmentId)
        {
            HttpResponse response = _context.Response;

            DataTable dtPage = ControlCard.GetList(documentID, departmentId);
            JqGridResults jqGridResults = ControlCard.BuildJqGridResults(dtPage);

            string output = new JavaScriptSerializer().Serialize(jqGridResults);

            response.Write(output);
        }

        private ProcessingResult InsertControlCardAsync(ControlCard controlCard)
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

                    if (controlCard.CardNumber <= 0)
                        controlCard.CardNumber = ControlCard.GetMaxCardNumber(trans, controlCard.DocumentID, UserName);
                    controlCard.UserName = UserName;
                    controlCard.Insert(trans);

                    Document.Document document = new Document.Document(trans, controlCard.DocumentID, UserName);
                    document.DocStatusID = controlCard.DocStatusID;
                    document.Update(trans);

                    ChangeDocumentControlled(trans, controlCard.DocumentID);

                    trans.Commit();

                    pr.Success = true;
                    pr.Data = controlCard;
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

        private ProcessingResult UpdateControlCardAsync(ControlCard controlCard)
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

                    controlCard.UserName = UserName;
                    controlCard.Update(trans);

                    Document.Document document = new Document.Document(trans, controlCard.DocumentID, UserName);
                    document.DocStatusID = controlCard.DocStatusID;
                    document.Update(trans);

                    ChangeDocumentControlled(trans, controlCard.DocumentID);

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

        private ProcessingResult DeleteControlCardAsync(int id)
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

                    ControlCard controlCard = new ControlCard(trans, id, UserName);

                    ControlCard.Delete(trans, controlCard.ID, UserName);

                    ChangeDocumentControlled(trans, controlCard.DocumentID);

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

        private void ChangeDocumentControlled(SqlTransaction trans, int documentId)
        {
            DocTemplate dt = DocTemplate.GetByDocumentID(trans, documentId, UserName);
            if (dt != null)
            {
                bool isControlled = ControlCard.ExistCard(trans, documentId, 1);
                bool isSpeciallyControlled = ControlCard.ExistCard(trans, documentId, true);

                if (dt.IsControlled != isControlled || dt.IsSpeciallyControlled != isSpeciallyControlled)
                {
                    dt.IsControlled = isControlled;
                    dt.IsSpeciallyControlled = isSpeciallyControlled;
                    dt.Update(trans);
                }
            }
        }
        public void GetControlCardReport(int controlCatdId, int docStatementID)
        {
            _context.Response.Clear();

            ControlCard controlCard;
            DocStatement docStatement;
            ControlCardReport controlCardReport = new ControlCardReport();

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    docStatement = new DocStatement(trans, docStatementID, UserName);
                    controlCard = new ControlCard(trans, controlCatdId, UserName);

                    foreach (int branchID in docStatement.Branches)
                    {
                        BranchType branchType = new BranchType(trans, branchID, UserName);
                        if (String.IsNullOrEmpty(controlCardReport.Branches))
                            controlCardReport.Branches = (branchType.ID >= 10 ? "" : "0") + (branchType.ID * 10).ToString() + " " + branchType.Name;
                        else
                            controlCardReport.Branches = String.Format("{0}, {1}", controlCardReport.Branches, (branchType.ID >= 10 ? "" : "0") + (branchType.ID*10).ToString() + " " + branchType.Name);
                    }

                    controlCardReport.DeliveryType = new DeliveryType(trans, docStatement.DeliveryTypeID, UserName).Name;
                    controlCardReport.InputDocType = new InputDocType(trans, docStatement.InputDocTypeID, UserName).Name;
                    controlCardReport.InputMethod = new InputMethod(trans, docStatement.InputMethodID, UserName).Name;
                    controlCardReport.InputSign = new InputSign(trans, docStatement.InputSignID, UserName).Name;
                    controlCardReport.InputSubjectType = new InputSubjectType(trans, docStatement.InputSubjectTypeID, UserName).Name;

                    foreach (int socialCategoryID in docStatement.Citizen.SocialCategories)
                    {
                        SocialCategory socialCategory = new SocialCategory(trans, socialCategoryID, UserName);
                        if (String.IsNullOrWhiteSpace(controlCardReport.SocialCategories))
                            controlCardReport.SocialCategories = socialCategory.Name;
                        else
                            controlCardReport.SocialCategories = String.Format("{0}, {1}", controlCardReport.SocialCategories, socialCategory.Name);
                    }
                    if (docStatement.Citizen.SocialStatusID > 0)
                        controlCardReport.SocialStatus = new SocialStatus(trans, docStatement.Citizen.SocialStatusID, UserName).Name;

                    Post headPost = new Post(trans, controlCard.Head.PostID, UserName);
                    controlCard.Head.PostName = headPost.Name;
                    
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

            string phoneNamber = String.Empty;
            if(!String.IsNullOrWhiteSpace(docStatement.Citizen.PhoneNumber))
                phoneNamber = " тел. " + docStatement.Citizen.PhoneNumber;
            controlCardReport.Address = ReportHelper.EscapeStringForRtf(FormatHelper.FormatAddress(docStatement.Citizen.Address,
                                                                   docStatement.Citizen.CityObjectTypeShortName,
                                                                   docStatement.Citizen.CityObjectName,
                                                                   docStatement.Citizen.HouseNumber,
                                                                   docStatement.Citizen.Corps,
                                                                   docStatement.Citizen.ApartmentNumber) + phoneNamber);

            controlCardReport.DepartmentName = docStatement.Document.Department.Name;
            controlCardReport.Content = docStatement.Content;
            controlCardReport.Control = controlCard.IsSpeciallyControlled ? "Контроль" : String.Empty;
            controlCardReport.Correspondent = String.Format("{0} {1} {2}", docStatement.Citizen.LastName, docStatement.Citizen.FirstName, docStatement.Citizen.MiddleName);
            controlCardReport.EndDate = controlCard.EndDate.ToString("yyyy-MM-dd");
            if (docStatement.Document.Source != null)
            {
                controlCardReport.ExternalCreationDate = docStatement.Document.Source.CreationDate.ToString("yyyy-MM-dd");
                controlCardReport.ExternalNumber = ReportHelper.EscapeStringForRtf(docStatement.Document.Source.Number);
                controlCardReport.OrganizationName = docStatement.Document.Source.OrganizationName;
            }


            controlCardReport.Head = controlCard.Head.PostName + " " + FormatHelper.FormatToLastNameAndInitials(controlCard.Head.LastName,
                                                                              controlCard.Head.FirstName,
                                                                              controlCard.Head.MiddleName);
            controlCardReport.InternalCreationDate = docStatement.Document.CreationDate.ToString("yyyy-MM-dd");
            controlCardReport.InternalNumber = ReportHelper.EscapeStringForRtf(docStatement.Document.Number);
            controlCardReport.Resolution = controlCard.Resolution;

            string templateUrl = String.Format("{0}\\ReportTemplates\\ControlCardRTF.xslt", _context.Request.PhysicalApplicationPath);

            XmlDocument xmlControlCardReport = XmlHelper.ToXmlDocument(controlCardReport);
            
            string report = ReportHelper.BuildReport(xmlControlCardReport, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("cc_({0})({1}).rtf", controlCard.ID, DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }
        #endregion

        #region [ ControlCardBlank ]
        public void ProcessControlCardBlankAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string jdata = request["jdata"];
            string depStr = request["dep"];
            string idStr = request["id"];
            string rResult = String.Empty;

            if (!String.IsNullOrWhiteSpace(type))
            {
                ControlCardBlank controlCardBlank;
                int departmentId = 0;
                if (!int.TryParse(depStr, out departmentId))
                {
                    departmentId = 0;
                }

                switch (type)
                {
                    case "get":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetControlCardBlankObjAsync(id);
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
                    case "getlast":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetLastControlCardBlankObj(id, departmentId);
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
                        int documentID = int.Parse(request["id"]);
                        GetControlCardBlankDataAsync(documentID, departmentId);
                        break;
                    case "getsubcards":
                        int controlCardID = int.Parse(request["id"]);
                        GetChildControlCards(controlCardID);
                        break;
                    case "getdeptop":
                        GetDepartmentTopControlCards(int.Parse(idStr), departmentId);
                        break;
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            try
                            {
                                controlCardBlank = JsonConvert.DeserializeObject<ControlCardBlank>(jdata, new DateTimeConvertorCustome());
                                //controlCardBlank = new JavaScriptSerializer().Deserialize<ControlCardBlank>(jdata);
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            ProcessingResult pr = InsertControlCardBlankAsync(controlCardBlank);
                            rResult = new JavaScriptSerializer().Serialize(pr);
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
                                controlCardBlank = JsonConvert.DeserializeObject<ControlCardBlank>(jdata, new DateTimeConvertorCustome());
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            ProcessingResult pr = UpdateControlCardBlankAsync(controlCardBlank);
                            rResult = new JavaScriptSerializer().Serialize(pr);
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
                                ProcessingResult pr = DeleteControlCardBlankAsync(id);
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
                        string docStatementIDStr = request["docStatementID"];
                        if (!String.IsNullOrWhiteSpace(jdata) & !String.IsNullOrWhiteSpace(docStatementIDStr))
                        {
                            int id;
                            int docStatementID;
                            if (int.TryParse(jdata, out id) & int.TryParse(docStatementIDStr, out docStatementID))
                                GetControlCardBlankReport(id, docStatementID);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "jdat" + "a");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("jda" + "ta", "Inuput id is not valid");
                        }
                        break;
                }

                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }

        private void GetControlCardBlankObjAsync(int id)
        {
            HttpResponse response = _context.Response;
            ControlCardBlank controlCardBlank;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    controlCardBlank = new ControlCardBlank(trans, id, UserName);

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

            string output = new JavaScriptSerializer().Serialize(controlCardBlank);

            response.Write(output);
        }

        private void GetLastControlCardBlankObj(int id, int departmentId)
        {
            HttpResponse response = _context.Response;
            ControlCardBlank controlCardBlank;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    controlCardBlank = ControlCardBlank.GetLastCard(trans, id, departmentId, UserName);

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

            string output = new JavaScriptSerializer().Serialize(controlCardBlank);

            response.Write(output);
        }

        private void GetControlCardBlankDataAsync(int documentID, int departmentId)
        {
            HttpResponse response = _context.Response;

            DataTable dtPage = ControlCardBlank.GetList(documentID, departmentId);
            JqGridResults jqGridResults = ControlCardBlank.BuildJqGridResults(dtPage);

            string output = new JavaScriptSerializer().Serialize(jqGridResults);

            response.Write(output);
        }

        private void GetChildControlCards(int controlCardID)
        {
            HttpResponse response = _context.Response;

            DataTable dtPage = ControlCardBlank.GetChildren(controlCardID);
            JqGridResults jqGridResults = ControlCardBlank.BuildJqGridResults(dtPage);

            string output = new JavaScriptSerializer().Serialize(jqGridResults);

            response.Write(output);
        }

        private void GetDepartmentTopControlCards(int documentID, int departmentId)
        {
            HttpResponse response = _context.Response;

            DataTable dtPage = ControlCardBlank.GetDepartmentTop(documentID, departmentId);
            JqGridResults jqGridResults = ControlCardBlank.BuildJqGridResults(dtPage);

            string output = new JavaScriptSerializer().Serialize(jqGridResults);

            response.Write(output);
        }

        private ProcessingResult InsertControlCardBlankAsync(ControlCardBlank controlCardBlank)
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

                    if (controlCardBlank.CardNumber <= 0)
                        controlCardBlank.CardNumber = ControlCard.GetMaxCardNumber(trans, controlCardBlank.DocumentID, UserName);
                    controlCardBlank.UserName = UserName;
                    controlCardBlank.Insert(trans);

                    Document.Document document = new Document.Document(trans, controlCardBlank.DocumentID, UserName);
                    document.DocStatusID = controlCardBlank.DocStatusID;
                    document.Update(trans);

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

        private ProcessingResult UpdateControlCardBlankAsync(ControlCardBlank controlCardBlank)
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

                    controlCardBlank.UserName = UserName;
                    controlCardBlank.Update(trans);

                    Document.Document document = new Document.Document(trans, controlCardBlank.DocumentID, UserName);
                    document.DocStatusID = controlCardBlank.DocStatusID;
                    document.Update(trans);

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

        private ProcessingResult DeleteControlCardBlankAsync(int id)
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

                    ControlCardBlank controlCardBlank = new ControlCardBlank(trans, id, UserName);

                    ControlCard.Delete(trans, controlCardBlank.ID, UserName);

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

        public void GetControlCardBlankReport(int controlCatdId, int docStatementID)
        {
            _context.Response.Clear();

            ControlCardBlank controlCardBlank;
            DocStatement docStatement;
            ControlCardReport ccReport = new ControlCardReport();

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    docStatement = new DocStatement(trans, docStatementID, UserName);
                    controlCardBlank = new ControlCardBlank(trans, controlCatdId, UserName);

                    foreach (int branchID in docStatement.Branches)
                    {
                        BranchType branchType = new BranchType(trans, branchID, UserName);
                        if (String.IsNullOrEmpty(ccReport.Branches))
                            ccReport.Branches = (branchType.ID >= 10 ? "" : "0") + (branchType.ID * 10).ToString() + " " + branchType.Name;
                        else
                            ccReport.Branches = String.Format("{0}, {1}", ccReport.Branches, (branchType.ID >= 10 ? "" : "0") + (branchType.ID * 10).ToString() + " " + branchType.Name);
                    }

                    ccReport.DeliveryType = new DeliveryType(trans, docStatement.DeliveryTypeID, UserName).Name;
                    ccReport.InputDocType = new InputDocType(trans, docStatement.InputDocTypeID, UserName).Name;
                    ccReport.InputMethod = new InputMethod(trans, docStatement.InputMethodID, UserName).Name;
                    ccReport.InputSign = new InputSign(trans, docStatement.InputSignID, UserName).Name;
                    ccReport.InputSubjectType = new InputSubjectType(trans, docStatement.InputSubjectTypeID, UserName).Name;

                    foreach (int socialCategoryID in docStatement.Citizen.SocialCategories)
                    {
                        SocialCategory socialCategory = new SocialCategory(trans, socialCategoryID, UserName);
                        if (String.IsNullOrWhiteSpace(ccReport.SocialCategories))
                            ccReport.SocialCategories = socialCategory.Name;
                        else
                            ccReport.SocialCategories = String.Format("{0}, {1}", ccReport.SocialCategories, socialCategory.Name);
                    }
                    if (docStatement.Citizen.SocialStatusID > 0)
                        ccReport.SocialStatus = new SocialStatus(trans, docStatement.Citizen.SocialStatusID, UserName).Name;

                    Post headPost = new Post(trans, controlCardBlank.Head.PostID, UserName);
                    controlCardBlank.Head.PostName = headPost.Name;

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

            string phoneNamber = String.Empty;
            if (!String.IsNullOrWhiteSpace(docStatement.Citizen.PhoneNumber))
                phoneNamber = " тел. " + docStatement.Citizen.PhoneNumber;
            ccReport.Address = ReportHelper.EscapeStringForRtf(FormatHelper.FormatAddress(docStatement.Citizen.Address,
                                                                   docStatement.Citizen.CityObjectTypeShortName,
                                                                   docStatement.Citizen.CityObjectName,
                                                                   docStatement.Citizen.HouseNumber,
                                                                   docStatement.Citizen.Corps,
                                                                   docStatement.Citizen.ApartmentNumber) + phoneNamber);

            ccReport.DepartmentName = docStatement.Document.Department.Name;
            ccReport.Content = docStatement.Content;
            ccReport.Control = controlCardBlank.IsSpeciallyControlled ? "Контроль" : String.Empty;
            ccReport.Correspondent = String.Format("{0} {1} {2}", docStatement.Citizen.LastName, docStatement.Citizen.FirstName, docStatement.Citizen.MiddleName);
            ccReport.EndDate = controlCardBlank.EndDate.ToString("yyyy-MM-dd");
            if (docStatement.Document.Source != null)
            {
                ccReport.ExternalCreationDate = docStatement.Document.Source.CreationDate.ToString("yyyy-MM-dd");
                ccReport.ExternalNumber = ReportHelper.EscapeStringForRtf(docStatement.Document.Source.Number);
                ccReport.OrganizationName = docStatement.Document.Source.OrganizationName;
            }


            ccReport.Head = controlCardBlank.Head.PostName + " " + FormatHelper.FormatToLastNameAndInitials(controlCardBlank.Head.LastName,
                                                                              controlCardBlank.Head.FirstName,
                                                                              controlCardBlank.Head.MiddleName);
            ccReport.InternalCreationDate = docStatement.Document.CreationDate.ToString("yyyy-MM-dd");
            ccReport.InternalNumber = ReportHelper.EscapeStringForRtf(docStatement.Document.Number);
            ccReport.Resolution = controlCardBlank.Resolution;

            string templateUrl = String.Format("{0}\\ReportTemplates\\ControlCardBlankRTF.xslt", _context.Request.PhysicalApplicationPath);

            XmlDocument xmlControlCardBlankReport = XmlHelper.ToXmlDocument(ccReport);

            string report = ReportHelper.BuildReport(xmlControlCardBlankReport, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("cc_({0})({1}).rtf", controlCardBlank.ID, DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }
        #endregion

        #region [ ControlCardGroup ]
        public void ProcessControlCardGroup()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string jdata = request["jdata"];
            string depStr = request["dep"];
            string idStr = request["id"];
            string rResult = String.Empty;

            if (!String.IsNullOrWhiteSpace(type))
            {
                ControlCardGroup controlCardGroup;
                int departmentId = 0;
                if (!int.TryParse(depStr, out departmentId))
                {
                    departmentId = 0;
                }

                switch (type)
                {
                    case "get":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetControlCardGroup(id);
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
                    case "getlast":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetLastControlCardBlankObj(id, departmentId);
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
                        int documentID = int.Parse(request["id"]);
                        GetControlCardBlankDataAsync(documentID, departmentId);
                        break;
                    case "getsubcards":
                        int controlCardID = int.Parse(request["id"]);
                        GetChildControlCards(controlCardID);
                        break;
                    case "getdeptop":
                        GetDepartmentTopControlCards(int.Parse(idStr), departmentId);
                        break;
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            try
                            {
                                controlCardGroup = JsonConvert.DeserializeObject<ControlCardGroup>(jdata, new DateTimeConvertorCustome());
                            }
                            catch (Exception e)
                            {
                                throw new CustomException.CustomException("Not valid json object. " + e.Message);
                            }
                            ProcessingResult pr = InsertControlCardGroup(controlCardGroup);
                            rResult = new JavaScriptSerializer().Serialize(pr);
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
                                
                                controlCardGroup = JsonConvert.DeserializeObject<ControlCardGroup>(jdata, new DateTimeConvertorCustome());
                            }
                            catch (Exception e)
                            {
                                throw new CustomException.CustomException("Not valid json object. " + e.Message);
                            }
                            ProcessingResult pr = UpdateControlCardGroup(controlCardGroup);
                            rResult = new JavaScriptSerializer().Serialize(pr);
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
                                ProcessingResult pr = DeleteControlCardBlankAsync(id);
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
                        string docStatementIDStr = request["docStatementID"];
                        if (!String.IsNullOrWhiteSpace(jdata) & !String.IsNullOrWhiteSpace(docStatementIDStr))
                        {
                            int id;
                            int docStatementID;
                            if (int.TryParse(jdata, out id) & int.TryParse(docStatementIDStr, out docStatementID))
                                GetControlCardBlankReport(id, docStatementID);
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "jdat" + "a");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("jda" + "ta", "Inuput id is not valid");
                        }
                        break;
                }

                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }

        private void GetControlCardGroup(int id)
        {
            HttpResponse response = _context.Response;
            ControlCardGroup controlCardGroup;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    controlCardGroup = new ControlCardGroup(trans, id, UserName);

                    trans.Commit();
                }
                catch (Exception e)
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

            string output = new JavaScriptSerializer().Serialize(controlCardGroup);

            response.Write(output);
        }


        private ProcessingResult InsertControlCardGroup(ControlCardGroup controlCardGroup)
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

                    controlCardGroup.UserName = UserName;
                    controlCardGroup.Insert(trans);

                    Document.Document document = new Document.Document(trans, controlCardGroup.DocumentID, UserName);
                    document.DocStatusID = controlCardGroup.DocStatusID;
                    document.Update(trans);

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


        private ProcessingResult UpdateControlCardGroup(ControlCardGroup controlCardGroup)
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

                    controlCardGroup.UserName = UserName;
                    controlCardGroup.Update(trans);

                    Document.Document document = new Document.Document(trans, controlCardGroup.DocumentID, UserName);
                    document.DocStatusID = controlCardGroup.DocStatusID;
                    document.Update(trans);

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


        #endregion

        #region [ DocStatement Reports ]
        public void ProcessDocStReportAsync()
        {
            HttpRequest request = _context.Request;
            int dep = Convert.ToInt32(request["dep"]);
            string type = request["type"];
            string cd = request["cd"];
            string ed = request["ed"];

            if (dep > 0 && !String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "c":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime creationDate;
                            DateTime endDate;
                            if (DateTime.TryParse(cd, out creationDate) && DateTime.TryParse(ed, out endDate))
                                GetDocStControlledReport(dep, creationDate, endDate);
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
                    case "fs":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate))
                                GetDocStFullStatisticReport(dep, startDate, endDate);
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
                    case "ssc":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            int socialCategoryID = Convert.ToInt32(request["sc"]);
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate) &&
                                socialCategoryID > 0)
                                GetDocStStatisticReportBySocialCategory(dep, startDate, endDate, socialCategoryID);
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
                    case "sbt":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            int branchTypeID = Convert.ToInt32(request["bt"]);
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate) &&
                                branchTypeID > 0)
                                GetDocStStatisticReportByBranchType(dep, startDate, endDate, branchTypeID);
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
                    case "sis":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate))
                                GetDocStStatisticReportByInputSign(dep, startDate, endDate, 2);
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
                    case "sist":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate))
                                GetDocStStatisticReportByInputSubjectType(dep, startDate, endDate, 2);
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
                    case "srh":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            int headID = Convert.ToInt32(request["hd"]);
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate) && headID > 0)
                                GetDocStStatisticReportForReceptionByHead(dep, startDate, endDate, headID);
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
                    case "sorg":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            int organizationID = Convert.ToInt32(request["org"]);
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate) &&
                                organizationID > 0)
                                GetDocStStatisticReportByOrganization(dep, startDate, endDate, organizationID);
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
                    case "sc":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate))
                                GetDocStStatisticReportSpeciallyControlled(dep, startDate, endDate);
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
                    case "hd":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            int headerID = Convert.ToInt32(request["hd"]);
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate) &&
                                headerID > 0)
                                GetDocStStatisticReportForReceptionByHead(dep, startDate, endDate, headerID);
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
                    case "wk":
                        if (!String.IsNullOrWhiteSpace(cd) || !String.IsNullOrWhiteSpace(ed))
                        {
                            DateTime startDate;
                            DateTime endDate;
                            int workerID = Convert.ToInt32(request["wk"]);
                            if (DateTime.TryParse(cd, out startDate) && DateTime.TryParse(ed, out endDate) &&
                                workerID > 0)
                                GetDocStStatisticReportForStatementsByWorker(dep, startDate, endDate, workerID);
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

        public void GetDocStControlledReport(int departmentID, DateTime creationDate, DateTime endDate)
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

                    ds = DocStatement.GetControlled(trans, departmentID, creationDate, endDate);

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

            List<WorkerCards> workerCards = new List<WorkerCards>();

            string[] cols = {"WorkerID", "WorkerFirstName", "WorkerMiddleName", "WorkerLastName"};

            DataTable workerTable = ds.Tables[0].DefaultView.ToTable(true, cols);
            foreach (DataRow worker in workerTable.Rows)
            {
                ds.Tables[0].DefaultView.RowFilter = String.Format("WorkerID = {0}", worker["WorkerID"]);

                WorkerCards wc = new WorkerCards();
                wc.Worker = FormatHelper.FormatToLastNameAndInitials((string) worker["WorkerLastName"],
                                                                     (string) worker["WorkerFirstName"],
                                                                     (string) worker["WorkerMiddleName"]);
                DataTable cardTable = ds.Tables[0].DefaultView.ToTable();
                foreach (DataRow card in cardTable.Rows)
                {
                    CardInfo cardInfo = new CardInfo();
                    cardInfo.Number = ReportHelper.EscapeStringForRtf((string) card["Number"]);
                    cardInfo.Content = ReportHelper.EscapeStringForRtf((string) card["Content"]);
                    cardInfo.CreationDate = ((DateTime) card["CreationDate"]).ToString("yyyy.MM.dd");
                    cardInfo.EndDate = ((DateTime) card["EndDate"]).ToString("yyyy.MM.dd");
                    cardInfo.Citizen = FormatHelper.FormatToLastNameAndInitials((string) card["CitizenLastName"],
                                                                                (string) card["CitizenFirstName"],
                                                                                (string) card["CitizenMiddleName"]);
                    cardInfo.Worker = FormatHelper.FormatToLastNameAndInitials((string) card["WorkerLastName"],
                                                                               (string) card["WorkerFirstName"],
                                                                               (string) card["WorkerMiddleName"]);
                    wc.Cards.Add(cardInfo);
                }

                workerCards.Add(wc);
            }


            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementControlledRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(workerCards);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("ds_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStFullStatisticReport(int departmentID, DateTime startDate, DateTime endDate)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.StartDate = startDate.ToString("yyyy.MM.dd");
            dsFullStatistics.EndDate = endDate.ToString("yyyy.MM.dd");
            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetFullStatistics(trans, departmentID, startDate, endDate);
                    dsFullStatistics.CountStatements = DocStatement.GetCountStatements(trans, departmentID, startDate, endDate);

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

            string[] listParts = new string[12];
            listParts[0] = "I. Характеристика звернень";
            listParts[1] = "1. За формою надходження";
            listParts[2] = "2. За ознакою надходження";
            listParts[3] = "3. За видами";
            listParts[4] = "4. За статтю авторів звернень";
            listParts[5] = "5. За суб’єктом";
            listParts[6] = "6. За типорм";
            listParts[7] = "7. За категорією авторів звернень";
            listParts[8] = "8. За соціальним станом авторів звернень";
            listParts[9] = "9. За результатами розгляду";
            listParts[10] = "ІІ. Основні питання, що порушуються у зверненнях";
            listParts[11] = "Звернення, що надійшли через інші організації";


            dsFullStatistics.Statistic.Add(new StatisticRow
                                               {
                                                   Name = "Всього звернень:",
                                                   Count = dsFullStatistics.CountStatements.ToString()
                                               });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i + 1] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                                srFull.Number = String.Empty;
                            else if (i == ds.Tables.Count - 2)
                                srFull.Number = String.Format("{0}0", (int) dr["ID"]);
                            else
                                srFull.Number = String.Format("{0}.{1}.", i + 1, (int) dr["ID"]);

                            srFull.Name = ReportHelper.EscapeStringForRtf((string) dr["Name"]);
                            srFull.Count = ((int) dr["CountStatements"]).ToString();

                            if (i != 3 || j != 0)
                                dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementFullStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dsfs_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportBySocialCategory(int departmentID, DateTime startDate, DateTime endDate, int socialCategoryID)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());
            dsFullStatistics.Header.Add("Про звернення громадян певної категорії за період".ToUpper());

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsBySocialCategory(trans, departmentID, startDate, endDate, socialCategoryID);
                    dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());
                    dsFullStatistics.Header.Add(String.Format("Категорія: {0}", (new SocialCategory(trans, socialCategoryID, UserName)).Name).ToUpper());

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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow {Name = listParts[i - 2]});

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + (string)dr["Number"] + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int) dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt", _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dsssc_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportByBranchType(int departmentID, DateTime startDate, DateTime endDate, int branchTypeID)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());
            dsFullStatistics.Header.Add("Про звернення громадян по виду питання за період".ToUpper());

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsByBranchType(trans, departmentID, startDate, endDate, branchTypeID);
                    dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());
                    dsFullStatistics.Header.Add(String.Format("Вид питання: {0}", (new BranchType(trans, branchTypeID, UserName)).Name).ToUpper());

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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + (string)dr["Number"] + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int)dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dssbt_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportByInputSign(int departmentID, DateTime startDate, DateTime endDate, int inputSign)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsByInputSign(trans, departmentID, startDate, endDate, inputSign);
                    string inputSignName = (new InputSign(trans, inputSign, UserName)).Name;
                    dsFullStatistics.Header.Add(String.Format("Про {0}і звернення громадян за період", inputSignName.Substring(0, inputSignName.Length-2)).ToUpper());
                    dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());

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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + (string)dr["Number"] + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int)dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dssis_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportByInputSubjectType(int departmentID, DateTime startDate, DateTime endDate, int inputSubjectType)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsByInputSubjectType(trans, departmentID, startDate, endDate, inputSubjectType);
                    string inputSignName = (new InputSubjectType(trans, inputSubjectType, UserName)).Name;
                    dsFullStatistics.Header.Add(String.Format("Про {0}і звернення громадян за період", inputSignName.Substring(0, inputSignName.Length - 2)).ToUpper());
                    dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());

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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + (string)dr["Number"] + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int)dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dssist_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportForReceptionByHead(int departmentID, DateTime startDate, DateTime endDate, int headID)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());
            dsFullStatistics.Header.Add("Про запис громадян на особистий прийос за період");

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsForReceptionByHead(trans, departmentID, startDate, endDate, headID);
                    Worker header = new Worker(trans, headID, UserName);
                    dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());
                    dsFullStatistics.Header.Add(String.Format("Керівник прийому: {0}",  FormatHelper.FormatToLastNameAndInitials(header.LastName, header.FirstName, header.MiddleName)).ToUpper());
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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + (string)dr["Number"] + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int)dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("drsrh_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportByOrganization_(int departmentID, DateTime startDate, DateTime endDate, int organizationID)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());
            dsFullStatistics.Header.Add("Про звернення громадян за період".ToUpper());

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsByOrganization(trans, departmentID, startDate, endDate, organizationID);
                    dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());
                    dsFullStatistics.Header.Add(String.Format("Організація: {0}", (new Organization(trans, organizationID, UserName)).Name).ToUpper());

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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + (string)dr["Number"] + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int)dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dsso_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportByOrganization(int departmentID, DateTime startDate, DateTime endDate, int organizationID)
        {
            _context.Response.Clear();

            RtfDocument rtf = new RtfDocument();
            rtf.FontTable.Add(new RtfFont("Times New Roman"));

            RtfParagraphFormatting centered14 = new RtfParagraphFormatting(14, RtfTextAlign.Center);
            centered14.FontIndex = 0;
            RtfParagraphFormatting left12 = new RtfParagraphFormatting(12, RtfTextAlign.Left);
            left12.FontIndex = 0;

            RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
            header.Formatting = centered14;

            header.AppendText("Відомості".ToUpper());
            header.AppendParagraph();
            header.AppendText("Про звернення громадян за період".ToUpper());
            header.AppendParagraph();

            header.AppendText(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());
            header.AppendParagraph();
            
            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsByOrganization(trans, departmentID, startDate, endDate, organizationID);
                   
                    header.AppendText(String.Format("Організація: {0}", (new Organization(trans, organizationID, UserName)).Name).ToUpper());

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

            header.AppendParagraph();
            header.AppendParagraph();
            rtf.Contents.AddRange(new RtfDocumentContentBase[] { header });

            RtfFormattedParagraph rowPar = new RtfFormattedParagraph(new RtfParagraphFormatting(12, RtfTextAlign.Left));
            rowPar.Formatting = left12;
            rowPar.AppendText(String.Format("Всього звернень: {0}", ds.Tables[ds.Tables.Count - 1].Rows.Count));
            rtf.Contents.AddRange(new RtfDocumentContentBase[] { rowPar });

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";
            
            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    RtfFormattedParagraph tblHeader = new RtfFormattedParagraph(new RtfParagraphFormatting(12, RtfTextAlign.Left));
                    tblHeader.Formatting = left12;
                    if (i > 1)
                    {
                        tblHeader.AppendParagraph();
                        tblHeader.AppendText(listParts[i - 2]);
                    }
                    RtfTable tbl;
                    DataTable workerTable = new DataTable();
                    if (i == ds.Tables.Count - 1)
                    {
                        string[] cols = { "WorkerID", "WorkerFirstName", "WorkerMiddleName", "WorkerLastName" };
                        workerTable = dt.DefaultView.ToTable(true, cols);

                        tbl = new RtfTable(RtfTableAlign.Left, 6, dt.Rows.Count + workerTable.Rows.Count + 1);
                        tbl.Width = TwipConverter.ToTwip(500, MetricUnit.Point);
                    }
                    else
                    {
                        tbl = new RtfTable(RtfTableAlign.Left, 2, dt.Rows.Count);
                        tbl.Width = TwipConverter.ToTwip(500, MetricUnit.Point);
                        tbl.Columns[0].Width = TwipConverter.ToTwip(370, MetricUnit.Point);
                        tbl.Columns[1].Width = TwipConverter.ToTwip(100, MetricUnit.Point);
                    }
                    tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, left12);

                    foreach (RtfTableRow row in tbl.Rows)
                    {
                        row.Height = TwipConverter.ToTwip(22, MetricUnit.Point);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        if (i == ds.Tables.Count - 1)
                        {

                            tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Center));
                            tbl.Columns[0].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
                            tbl.Columns[1].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
                            tbl.Columns[2].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
                            tbl.Columns[3].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
                            tbl.Columns[4].Width = TwipConverter.ToTwip(150, MetricUnit.Point);
                            tbl.Columns[4].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));
                            tbl.Columns[5].Width = TwipConverter.ToTwip(72, MetricUnit.Point);
                            tbl[0, 0].AppendText("Дата звернення");
                            tbl[1, 0].AppendText("Прізвище заявника");
                            tbl[2, 0].AppendText("Номер документа");
                            tbl[3, 0].AppendText("Дата відповіді");
                            tbl[4, 0].AppendText("Зміст звернення");
                            tbl[5, 0].AppendText("Результат розгляду");

                            int numRow = 1;

                            foreach (DataRow worker in workerTable.Rows)
                            {
                                dt.DefaultView.RowFilter = String.Format("WorkerID = {0}", worker["WorkerID"]);

                                tbl.MergeCellsHorizontally(0, numRow, 5);
                                tbl[0, numRow].AppendText("Виконавець: " +
                                                          FormatHelper.FormatToLastNameAndInitials(
                                                              (string) worker["WorkerLastName"],
                                                              (string) worker["WorkerFirstName"],
                                                              (string) worker["WorkerMiddleName"]));
                                tbl.Rows[numRow].Cells[0].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Left);
                                numRow++;


                                DataTable cardTable = dt.DefaultView.ToTable();
                                foreach (DataRow dr in cardTable.Rows)
                                {
                                    string citizen =
                                        ReportHelper.EscapeStringForRtf(
                                            FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                     (string)dr["CitizenFirstName"],
                                                                                     (string)dr["CitizenMiddleName"]));

                                    int docStatusID = (int)dr["DocStatusID"];
                                    string statusName = String.Empty;
                                    switch (docStatusID)
                                    {
                                        case 0:
                                            statusName = "На виконанні";
                                            break;
                                        case 1:
                                            statusName = "Задовільнено";
                                            break;
                                        case 2:
                                            statusName = "Відмовлено";
                                            break;
                                        case 3:
                                            statusName = "Пояснено";
                                            break;
                                        case 4:
                                            statusName = "Повернуто";
                                            break;
                                        case 5:
                                            statusName = "Переслано";
                                            break;
                                        case 6:
                                            statusName = "Не підлягає розгляду";
                                            break;
                                    }

                                    tbl[0, numRow].AppendText(((DateTime)dr["CreationDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture));
                                    tbl[1, numRow].AppendText(citizen);
                                    tbl[2, numRow].AppendText((string)dr["Number"]);
                                    tbl[3, numRow].AppendText((DBNull.Value != dr["ControlResponseDate"]
                                                                   ? ((DateTime) dr["ControlResponseDate"]).ToString(
                                                                       "dd.MM.yyyy", CultureInfo.CurrentCulture)
                                                                   : String.Empty));
                                    tbl[4, numRow].AppendText(ReportHelper.EscapeStringForRtf((string)dr["Content"]));
                                    tbl[5, numRow].AppendText(statusName);
                                    
                                    numRow++;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < dt.Rows.Count; j++)
                            {
                                DataRow dr = dt.Rows[j];

                                tbl[0, j].AppendText((string)dr["Name"]);
                                tbl[1, j].AppendText(((int)dr["CountStatements"]).ToString());
                            }
                        }
                        rtf.Contents.AddRange(new RtfDocumentContentBase[] {tblHeader, tbl});
                    }
                }


            RtfWriter rtfWriter = new RtfWriter();
            StringBuilder sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                rtfWriter.Write(writer, rtf);
            }

            string fileName = String.Format("dsso_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(sb.ToString());
        }
        public void GetDocStStatisticReportSpeciallyControlled(int departmentID, DateTime startDate, DateTime endDate)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());
            dsFullStatistics.Header.Add("Про поставлені на контроль заяви, звернення громадян за період".ToUpper());
            dsFullStatistics.Header.Add("які надійшли за період".ToUpper());
            dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsSpeciallyControlled(trans, departmentID, startDate, endDate);

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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + (string)dr["Number"] + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int)dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dssc_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportByHead(int departmentID, DateTime startDate, DateTime endDate, int headID)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());
            dsFullStatistics.Header.Add("Про звернення громадян за період".ToUpper());

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    int[] count = new int[8];
                    ds = DocStatement.GetStatisticsByHead(trans, departmentID, startDate, endDate, headID, out count);
                    Worker header = new Worker(trans, headID, UserName);
                    dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());
                    dsFullStatistics.Header.Add(String.Format("Керівник: {0}", FormatHelper.FormatToLastNameAndInitials(header.LastName, header.FirstName, header.MiddleName)).ToUpper());

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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + /*(string)dr["Number"]*/"" + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int)dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("dsh_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportForStatementsByWorker(int departmentID, DateTime startDate, DateTime endDate, int workerID)
        {
            _context.Response.Clear();

            DsFullStatistics dsFullStatistics = new DsFullStatistics();
            dsFullStatistics.Header.Add("Відомості".ToUpper());
            dsFullStatistics.Header.Add("Про розгляд виконавцями звернень громадян за період");

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsForStatementsByWorker(trans, departmentID, startDate, endDate, workerID);
                    Worker header = new Worker(trans, workerID, UserName);
                    dsFullStatistics.Header.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());
                    dsFullStatistics.Header.Add(String.Format("Виконавець: {0}", FormatHelper.FormatToLastNameAndInitials(header.LastName, header.FirstName, header.MiddleName)).ToUpper());
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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            dsFullStatistics.CountStatements = ds.Tables[ds.Tables.Count - 1].Rows.Count;
            dsFullStatistics.Statistic.Add(new StatisticRow
            {
                Name = "Всього звернень:",
                Count = dsFullStatistics.CountStatements.ToString()
            });
            dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        dsFullStatistics.Statistic.Add(new StatisticRow { Name = listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            StatisticRow srFull = new StatisticRow();
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull.Name = ReportHelper.EscapeStringForRtf("(№ " + (string)dr["Number"] + " ) " + (string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull.Count = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull.Number = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull.Count = ((int)dr["CountStatements"]).ToString();
                            }
                            dsFullStatistics.Statistic.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(dsFullStatistics);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("drswk_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStStatisticReportForStatementsByWorker2(int departmentID, DateTime startDate, DateTime endDate, int workerID)
        {
            _context.Response.Clear();

            StatisticsBlank statisticsBlank = new StatisticsBlank();
            statisticsBlank.Headers.Add("Відомості".ToUpper());
            statisticsBlank.Headers.Add("Про розгляд виконавцями звернень громадян за період");

            DataSet ds;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocStatement.GetStatisticsForStatementsByWorker(trans, departmentID, startDate, endDate, workerID);
                    Worker header = new Worker(trans, workerID, UserName);
                    statisticsBlank.Headers.Add(String.Format("з {0} по {1}", startDate.ToString("yyyy.MM.dd"), endDate.ToString("yyyy.MM.dd")).ToUpper());
                    statisticsBlank.Headers.Add(String.Format("Виконавець: {0}", FormatHelper.FormatToLastNameAndInitials(header.LastName, header.FirstName, header.MiddleName)).ToUpper());
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

            string[] listParts = new string[5];
            listParts[0] = "Результати розгляду";
            listParts[1] = "Звернення, що надійшли через інші організації";
            listParts[2] = "Категорії авторів звернень";
            listParts[3] = "Питання, які порушені у зверненнях";
            listParts[4] = "Список громадян, які звернулися";

            statisticsBlank.ColumnsWidth = new[] { "", "", "" };
            statisticsBlank.Statistics.Add(new[] { "Всього звернень:", ds.Tables[ds.Tables.Count - 1].Rows.Count.ToString() });
            statisticsBlank.Statistics.Add(new[] { listParts[0] });

            if (ds.Tables.Count > 0)
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    DataTable dt = ds.Tables[i];

                    if (i > 1)
                        statisticsBlank.Statistics.Add(new [] { listParts[i - 2] });

                    if (dt.Rows.Count > 0)
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            DataRow dr = dt.Rows[j];

                            string[] srFull = new string[4];
                            if (i == ds.Tables.Count - 1)
                            {
                                srFull[0] = ReportHelper.EscapeStringForRtf(FormatHelper.FormatToLastNameAndInitials((string)dr["CitizenLastName"],
                                                                                         (string)dr["CitizenFirstName"],
                                                                                         (string)dr["CitizenMiddleName"]));

                                srFull[2] = ReportHelper.EscapeStringForRtf((string)dr["Content"]);
                                int docStatusID = (int)dr["DocStatusID"];
                                string statusName = String.Empty;
                                switch (docStatusID)
                                {
                                    case 0:
                                        statusName = "На виконанні";
                                        break;
                                    case 1:
                                        statusName = "Задовільнено";
                                        break;
                                    case 2:
                                        statusName = "Відмовлено";
                                        break;
                                    case 3:
                                        statusName = "Пояснено";
                                        break;
                                    case 4:
                                        statusName = "Повернуто";
                                        break;
                                    case 5:
                                        statusName = "Переслано";
                                        break;
                                    case 6:
                                        statusName = "Не підлягає розгляду";
                                        break;
                                }
                                srFull[3] = ReportHelper.EscapeStringForRtf(statusName);
                            }
                            else
                            {
                                srFull[1] = ReportHelper.EscapeStringForRtf((string)dr["Name"]);
                                srFull[3] = ((int)dr["CountStatements"]).ToString();
                            }
                            statisticsBlank.Statistics.Add(srFull);
                        }
                }

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocStatementStatisticsRTF.xslt",
                                               _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(statisticsBlank);

            string report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            report = ReportHelper.GetRtfUnicodeEscapedString(report);

            string fileName = String.Format("drswk_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(report);
        }

        public void GetDocStPageRtf(int departmentId, bool isReception)
        {
            HttpRequest request = _context.Request;
            Filter filters = new Filter();
            filters.Rules = new List<Rule>();
            string[] fields =
                {
                    "CreationDate",
                    "CreationDateStart",
                    "CreationDateEnd",
                    "Number",
                    "ExternalNumber",
                    "DocumentCodeID",
                    "Content",
                    "OrganizationName",
                    "InnerNumber",
                    "Controlled",
                    "EndDateFrom",
                    "EndDateTo",
                    "IsInput",
                    "IsDepartmentOwner",
                    "ControlledInner",
                    "InnerEndDateFrom",
                    "InnerEndDateTo",
                    "LableID",
                    "DocStatusID"
                };

            foreach (var f in fields)
            {
                string data = request[f];
                if (!String.IsNullOrWhiteSpace(data))
                {
                    filters.AddRule(new Rule { Field = f, Data = data, Op = "cn" });
                }
            }
            PageSettings gridSettings = new PageSettings
            {
                IsSearch = bool.Parse(request["_search"] ?? "false"),
                PageIndex = int.Parse(request["page"] ?? "1"),
                PageSize = int.Parse(request["rows"] ?? "50"),
                SortColumn = request["sidx"] ?? "",
                SortOrder = request["sord"] ?? "asc",
                Where = filters
            };

            DataTable dtPage = DocStatement.GetPage(gridSettings, isReception, UserName, departmentId);
            JqGridResults jqGridResults = DocStatement.BuildJqGridResults(dtPage, gridSettings);

            _context.Response.Clear();


            RtfDocument rtf = new RtfDocument();
            rtf.FontTable.Add(new RtfFont("Times New Roman"));

            RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Center));
            header.AppendText("");
            header.AppendParagraph();

            RtfTable tbl = new RtfTable(RtfTableAlign.Center, 8, dtPage.Rows.Count + 1);
            tbl.Width = TwipConverter.ToTwip(489, MetricUnit.Point);
            tbl.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Center));
            tbl.Columns[0].Width = TwipConverter.ToTwip(14, MetricUnit.Point);
            tbl.Columns[1].Width = TwipConverter.ToTwip(60, MetricUnit.Point);
            tbl.Columns[2].Width = TwipConverter.ToTwip(50, MetricUnit.Point);
            tbl.Columns[3].Width = TwipConverter.ToTwip(50, MetricUnit.Point);
            tbl.Columns[4].Width = TwipConverter.ToTwip(30, MetricUnit.Point);
            tbl.Columns[5].Width = TwipConverter.ToTwip(50, MetricUnit.Point);
            tbl.Columns[5].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));
            tbl.Columns[6].Width = TwipConverter.ToTwip(95, MetricUnit.Point);
            tbl.Columns[6].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));
            tbl.Columns[7].Width = TwipConverter.ToTwip(140, MetricUnit.Point);
            tbl.Columns[7].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(10, RtfTextAlign.Left));

            foreach (RtfTableRow row in tbl.Rows)
            {
                row.Height = TwipConverter.ToTwip(20, MetricUnit.Point);
            }
            tbl.Rows[0].Height = TwipConverter.ToTwip(30, MetricUnit.Point);
            //tbl.Rows[0].Cells[5].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Center);

            tbl[0, 0].AppendText("");
            tbl[1, 0].AppendText("Дата створення");
            tbl[2, 0].AppendText("Прізвище");
            tbl[3, 0].AppendText("№");
            tbl[4, 0].AppendText("№ внутрішній");
            tbl[5, 0].AppendText("№ в організації");
            tbl[6, 0].AppendText("Адреса");
            tbl[7, 0].AppendText("Зміст");
            int numRow = 1;

            foreach (JqGridRow dr in jqGridResults.rows)
            {
                tbl[0, numRow].AppendText(numRow.ToString());
                tbl[1, numRow].AppendText(dr.cell[2]);
                tbl[2, numRow].AppendText(dr.cell[4]);
                tbl[3, numRow].AppendText(dr.cell[7]);
                tbl[4, numRow].AppendText(dr.cell[16]);
                tbl[5, numRow].AppendText(dr.cell[8]);
                tbl[6, numRow].AppendText(dr.cell[5]);
                tbl[7, numRow].AppendText(dr.cell[9]);

                numRow++;

            }

            RtfFormattedParagraph footer = new RtfFormattedParagraph(new RtfParagraphFormatting(14, RtfTextAlign.Left));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Список станом на "));
            footer.AppendParagraph();
            footer.AppendText(String.Format("Всього документів: {0}", dtPage.Rows.Count));
            footer.AppendParagraph();
            footer.AppendText("Виконав ______________________");

            rtf.Contents.AddRange(new RtfDocumentContentBase[] { header, tbl, footer });


            RtfWriter rtfWriter = new RtfWriter();
            StringBuilder sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                rtfWriter.Write(writer, rtf);
            }

            string fileName = String.Format("docStatements_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(sb.ToString());
        }
        #endregion


        #region [ File ]
        public void ProcessFileAsync()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string fileIDStr = request["fileID"];
            string documentIdStr = request["documentid"];

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "getlist":
                        if (!String.IsNullOrWhiteSpace(documentIdStr))
                        {
                            int documentID;
                            if (int.TryParse(documentIdStr, out documentID))
                            {
                                List<DocumentFile> fileList = new List<DocumentFile>();
                                SqlConnection connection = new SqlConnection(Config.ConnectionString);
                                try
                                {
                                    connection.Open();
                                    SqlTransaction trans = null;
                                    try
                                    {
                                        trans = connection.BeginTransaction();

                                        fileList = DocumentFile.GetList(trans, documentID);

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
                                    HttpResponse response = _context.Response;
                                    string output = new JavaScriptSerializer().Serialize(fileList);
                                    response.Write(output);
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "documentID");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("documentID", "Inuput id is not valid");
                        }
                        break;
                    case "del":
                        if (!String.IsNullOrWhiteSpace(fileIDStr))
                        {
                            int fileID;
                            if (int.TryParse(fileIDStr, out fileID))
                            {
                                SqlConnection connection = new SqlConnection(Config.ConnectionString);
                                try
                                {
                                    connection.Open();
                                    SqlTransaction trans = null;
                                    try
                                    {
                                        trans = connection.BeginTransaction();

                                        if (!String.IsNullOrWhiteSpace(documentIdStr))
                                        {
                                            int documentID;
                                            if (int.TryParse(documentIdStr, out documentID))
                                                DocumentFile.Delete(trans, fileID, documentID, Worker);
                                        }
                                        if (!DocumentFile.IsAttached(trans, fileID))
                                        {
                                            FileVFS.Delete(trans, fileID);
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
                            }
                            else
                            {
                                throw new ArgumentException("Inuput id is not valid", "fileId");
                            }
                        }
                        else
                        {
                            throw new ArgumentNullException("fileId", "Inuput id is not valid");
                        }
                        break;
                }
            }
        }
        #endregion


        #region [ DocAdminService ]
        public void ProcessDocAdminServiceAsync()
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            string type = request["type"];
            string jdata = request["jdata"];
            string rResult = String.Empty;

            DocAdminService docAdminService = null;

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "get":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            int id;
                            if (int.TryParse(jdata, out id))
                                GetDocAdminServiceObjAsync(id);
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
                                GetDocAdminServiceBlank(id);
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

                        bool isReception = Convert.ToBoolean(request["isReception"]);
                        int departmentID = Convert.ToInt32(request["departmentID"]);
                        GetDocAdminServiceDataAsync(isReception, departmentID);
                        break;
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(jdata))
                        {
                            try
                            {
                                //JavaScriptSerializer serializer2 = new JavaScriptSerializer();
                                //JavaScriptSerializer serializer2 = ExtendedJavaScriptConverter<DocAdminService>.GetSerializer();
                                //docAdminService = serializer2.Deserialize<DocAdminService>(jdata);
                                docAdminService = JsonConvert.DeserializeObject<DocAdminService>(jdata, new DateTimeConvertorCustome());
                                //serializer2.RegisterConverters(new[] {new DateTimeJavaScriptConverter()});
                                //docAdminService = new JavaScriptSerializer().Deserialize<DocAdminService>(jdata);
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            string errorMsg;
                            if (IsValidDocAdminService(docAdminService, out errorMsg))
                            {
                                int docAdminServiceID = InsertDocAdminServiceAsync(docAdminService);
                                response.Write(docAdminServiceID);
                                response.Flush();
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
                                //docAdminService = new JavaScriptSerializer().Deserialize<DocAdminService>(jdata);
                                docAdminService = JsonConvert.DeserializeObject<DocAdminService>(jdata, new DateTimeConvertorCustome());
                            }
                            catch (Exception)
                            {
                                throw new CustomException.CustomException("Not valid json object");
                            }
                            string errorMsg;
                            if (IsValidDocAdminService(docAdminService, out errorMsg))
                            {
                                ProcessingResult pr = UpdateDocAdminServiceAsync(docAdminService);
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
                                ProcessingResult pr = DeleteDocAdminServiceAsync(id);
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
                        new DocTemplateController(_context, UserId, Worker, UserName).ProcessDocTReportAsync();
                        break;
                    case "getnextnumber":
                        int depID = Convert.ToInt32(request["dep"]);
                        int documentCodeID = Convert.ToInt32(request["code"]);
                        int number = DocAdminService.GetNextNumber(UserName, depID, documentCodeID);
                        rResult = number.ToString();
                        break;
                }
                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    _context.Response.Write(rResult);
                }
            }
        }

        private bool IsValidDocAdminService(DocAdminService docAdminService, out string msg)
        {
            msg = String.Empty;
            if (docAdminService == null)
                return false;

            if (docAdminService.Document == null)
                return false;

            if (String.IsNullOrWhiteSpace(docAdminService.Document.Number))
            {
                msg = "Не вказаний номер документа";
                return false;
            }
            DateTime dcd = docAdminService.Document.CreationDate;
            if (dcd < (DateTime)SqlDateTime.MinValue || dcd > (DateTime)SqlDateTime.MaxValue)
            {
                msg = "Не вказана коректна дата документа";
                return false;
            }
            if (docAdminService.Document.Source != null && String.IsNullOrWhiteSpace(docAdminService.Document.Source.Number))
            {
                msg = "Не вказаний номер документа в організації";
                return false;
            }
            
            return true;
        }

        private void GetDocAdminServiceObjAsync(int id)
        {
            HttpResponse response = _context.Response;
            DocAdminService docAdminService;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    docAdminService = new DocAdminService(trans, id, UserName);

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

            string output = new JavaScriptSerializer().Serialize(docAdminService);

            response.Write(output);
        }

        private void GetDocAdminServiceBlank(int id)
        {
            HttpResponse response = _context.Response;
            DocAdminServiceBlank docAdminServiceB;

            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    docAdminServiceB = new DocAdminServiceBlank(trans, id, UserName);

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

            string output = new JavaScriptSerializer().Serialize(docAdminServiceB);

            response.Write(output);
        }

        private void GetDocAdminServiceDataAsync(bool isReception, int departmentID)
        {
            HttpResponse response = _context.Response;

            PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

            DataTable dtPage = DocAdminService.GetPage(gridSettings, UserName, departmentID);
            JqGridResults jqGridResults = DocAdminService.BuildJqGridResults(dtPage, gridSettings);

            string output = new JavaScriptSerializer().Serialize(jqGridResults);
            response.Write(output);
        }

        private int InsertDocAdminServiceAsync(DocAdminService docAdminService)
        {
            int docAdminServiceID = 0;
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    MembershipUser mu = Membership.GetUser();
                    docAdminService.Document.UserName = UserName;
                    if (mu != null)
                    {
                        if (mu.ProviderUserKey != null)
                        {
                            docAdminService.Document.OriginatorID = (Guid)mu.ProviderUserKey;
                            docAdminService.Document.OwnerID = (Guid)mu.ProviderUserKey;
                            docAdminService.Document.RequestorID = (Guid)mu.ProviderUserKey;
                        }
                    }
                    docAdminService.Document.RequestDate = DateTime.Now;
                    docAdminService.Document.DocStatusID = 0;
                    docAdminService.Document.Insert(trans);
                    
                    docAdminService.UserName = UserName;
                    docAdminService.DocumentID = docAdminService.Document.ID;
                    docAdminServiceID = docAdminService.Insert(trans);

                    foreach (DocumentFile df in docAdminService.Document.Files)
                    {
                        if (df.FileID > 0)
                        {
                            df.DocumentID = docAdminService.DocumentID;
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
            return docAdminServiceID;
        }

        private ProcessingResult UpdateDocAdminServiceAsync(DocAdminService docAdminService)
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
                    docAdminService.Document.UserName = UserName;
                    if (mu != null)
                    {
                        if (mu.ProviderUserKey != null)
                        {
                            docAdminService.Document.OriginatorID = (Guid)mu.ProviderUserKey;
                            docAdminService.Document.OwnerID = (Guid)mu.ProviderUserKey;
                            docAdminService.Document.RequestorID = (Guid)mu.ProviderUserKey;
                        }
                    }

                    docAdminService.Document.ID = docAdminService.DocumentID;
                    Document.Document doc = new Document.Document(trans, docAdminService.DocumentID, UserName);
                    docAdminService.Document.ObjectID = doc.ObjectID;

                    docAdminService.Document.RequestDate = DateTime.Now;
                    docAdminService.Document.Update(trans);
                    
                    docAdminService.UserName = UserName;
                    docAdminService.Update(trans);

                    foreach (DocumentFile df in docAdminService.Document.Files)
                    {
                        if (df.FileID > 0)
                        {
                            df.DocumentID = docAdminService.DocumentID;
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

        private ProcessingResult DeleteDocAdminServiceAsync(int id)
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

                    DocAdminService docAdminService = new DocAdminService(trans, id, UserName);

                    DocAdminService.Delete(trans, docAdminService.ID, UserName);

                    DocumentFile.DeleteFiles(trans, docAdminService.DocumentID, Worker);

                    Document.Document.Delete(trans, docAdminService.DocumentID, UserName);

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
        #endregion


        #region [ DocumentComment ]
        public void ProcessDocumentComment()
        {
            HttpRequest request = _context.Request;
            string type = request["type"];
            string data = request["data"];
            int documentId = Convert.ToInt32(request["documentId"]);
            int? controlCardId = null;
            int ccId;
            if (!String.IsNullOrWhiteSpace(request["controlCardId"]) && int.TryParse(request["controlCardId"], out ccId)) {
                controlCardId = ccId;
            }
            string rResult = String.Empty;

            if (!String.IsNullOrWhiteSpace(type)) {
                switch (type) {
                    case "list":
                        rResult = new JavaScriptSerializer().Serialize(DocumentCommentList(documentId));
                        break;
                    case "ins":
                        if (!String.IsNullOrWhiteSpace(data)) {
                            string attachToCard = request["attachToCard"];
                            ProcessingResult pr = InsertDocumentComment(data, documentId, controlCardId, !String.IsNullOrEmpty(attachToCard));
                            rResult = new JavaScriptSerializer().Serialize(pr);
                        }
                        break;
                }

                if (!String.IsNullOrWhiteSpace(rResult)) {
                    _context.Response.Write(rResult);
                }
            }
        }

        private ProcessingResult InsertDocumentComment(string content, int documentId, int? controlCardId, bool attachToCard)
        {
            ProcessingResult pr = new ProcessingResult();
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try {
                connection.Open();
                SqlTransaction trans = null;
                try {
                    trans = connection.BeginTransaction();

                    DocumentComment dc = new DocumentComment();
                    dc.Content = content;
                    dc.DocumentID = documentId;
                    //dc.DepartmentID = Worker.DepartmentID;
                    dc.WorkerID = Worker.ID;
                    dc.BehalfWorkerID = Worker.ID;
                    dc.DocumentCommentTypeID = 1;
                    dc.ControlCardID = controlCardId;
                    dc.ParentDocumentCommentID = null;
                    pr.Data = dc.Insert(trans).ToString();

                    if (attachToCard && controlCardId != null) {
                        ControlCard.SetActionComment(trans, (int) controlCardId, dc.ID);
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

        private ProcessingResult DocumentCommentList(int documentId)
        {
            ProcessingResult pr = new ProcessingResult();
            try {

                List<DocumentComment> comments = DocumentComment.GetList(documentId);
                pr.Data = comments;
                pr.Success = true;
            } catch (Exception e) {
                pr.Message = e.Message;
                throw;
            }
            return pr;
        }

        #endregion

        #endregion
    }

    [Serializable]
    public class ProcessingResult
    {
        public bool Success { get; set; }

        private string _message = String.Empty;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        private object _data = String.Empty;
        public object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public ProcessingResult()
        {
            
        }

        public ProcessingResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}