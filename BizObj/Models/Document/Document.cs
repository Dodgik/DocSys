using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Document;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class Document
    {
        private struct SpNames
        {
            public const string Get = "usp_Document_Get";
            public const string Insert = "usp_Document_Insert";
            public const string Update = "usp_Document_Update";
            public const string Delete = "usp_Document_Delete";
        }

        public const int ObjectTypeID = 1;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;
        
        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }
        
        #region Properties

        //[ParamAttribute("@DocumentID", SqlDbType.Int)]
        //[FieldAttribute("DocumentID")]
        public int ID { get; set; }

        //[ParamAttribute("@DocumentCodeID", SqlDbType.Int)]
        //[FieldAttribute("DocumentCodeID")]
        public int CodeID { get; set; }

        public string CodeName { get; set; }
        public string Code { get; set; }

        //[ParamAttribute("@CreationDate", SqlDbType.DateTime)]
        //[FieldAttribute("CreationDate")]
        public DateTime CreationDate { get; set; }

        //[ParamAttribute("@OriginatorID", SqlDbType.UniqueIdentifier)]
        //[FieldAttribute("OriginatorID")]
        [ScriptIgnore]
        public Guid OriginatorID { get; set; }

        //[ParamAttribute("@RequestDate", SqlDbType.DateTime)]
        //[FieldAttribute("RequestDate")]
        [ScriptIgnore]
        public DateTime RequestDate { get; set; }

        //[ParamAttribute("@RequestorID", SqlDbType.UniqueIdentifier)]
        //[FieldAttribute("RequestorID")]
        [ScriptIgnore]
        public Guid RequestorID { get; set; }

        //[ParamAttribute("@OwnerID", SqlDbType.UniqueIdentifier)]
        //[FieldAttribute("OwnerID")]
        [ScriptIgnore]
        public Guid OwnerID { get; set; }

        //[ParamAttribute("@ObjectID", SqlDbType.UniqueIdentifier)]
        //[FieldAttribute("ObjectID")]
        public Guid ObjectID { get; set; }

        //[ParamAttribute("@Number", SqlDbType.NVarChar, 50)]
        //[FieldAttribute("Number")]
        public string Number { get; set; }

        //[ParamAttribute("@Notes", SqlDbType.NVarChar, -1)]
        //[FieldAttribute("Notes")]
        public string Notes { get; set; }

        //[ParamAttribute("@DocStatusID", SqlDbType.Int)]
        //[FieldAttribute("DocStatusID")]
        public int DocStatusID { get; set; }

        //[ParamAttribute("@DepartmentID", SqlDbType.Int)]
        //[FieldAttribute("DepartmentID")]
        public int DepartmentID { get; set; }

        public int TemplateId { get; set; }
        public int ParentDocumentID { get; set; }
        public int DocStateID { get; set; }

        public ExternalSource ExternalSource { get; set; }
        public Source Source { get; set; }
        public Destination Destination { get; set; }

        public Department Department { get; set; }

        public List<DocumentFile> Files { get; set; }
        public int[] Labels { get; set; }
        
        [ScriptIgnore]
        public string UserName { get; set; }
        #endregion
        
        #region Constructors

        public Document()
        {
            Files = new List<DocumentFile>();
        }

        public Document(string userName)
        {
            UserName = userName;
        }

        public Document(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);
            if (ExternalSource.IsExist(trans, id))
                ExternalSource = new ExternalSource(trans, id, userName);
            if (Source.IsExist(trans, id))
                Source = new Source(trans, id, userName);
            if (Destination.IsExist(trans, id))
                Destination = new Destination(trans, id, userName);
            Department = new Department(trans, DepartmentID, userName);
            DocumentCode dc = new DocumentCode(trans, CodeID, userName);
            CodeName = dc.Name;
            Code = dc.Code;
            Files = DocumentFile.GetList(trans, id);
            Labels = DocumentLabel.GetDocumentLabelIds(trans, id);
        }

        public Document(int id, string userName): this(null, id, userName)
        {
            
        }

        #endregion

        #region Private Methods

        private void Init(SqlTransaction trans, int documentId)
        {
            /*
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }
            */
            SqlParameter[] prms = new SqlParameter[15];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentId;

            prms[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@OriginatorID", SqlDbType.UniqueIdentifier);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@RequestDate", SqlDbType.DateTime);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@RequestorID", SqlDbType.UniqueIdentifier);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@OwnerID", SqlDbType.UniqueIdentifier);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[7].Direction = ParameterDirection.Output;

            prms[8] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[8].Direction = ParameterDirection.Output;

            prms[9] = new SqlParameter("@Notes", SqlDbType.NVarChar, -1);
            prms[9].Direction = ParameterDirection.Output;

            prms[10] = new SqlParameter("@DocStatusID", SqlDbType.Int);
            prms[10].Direction = ParameterDirection.Output;

            prms[11] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[11].Direction = ParameterDirection.Output;

            prms[12] = new SqlParameter("@TemplateId", SqlDbType.Int);
            prms[12].Direction = ParameterDirection.Output;

            prms[13] = new SqlParameter("@ParentDocumentID", SqlDbType.Int);
            prms[13].Direction = ParameterDirection.Output;

            prms[14] = new SqlParameter("@DocStateID", SqlDbType.Int);
            prms[14].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = documentId;
            CodeID = (int) prms[1].Value;
            CreationDate = (DateTime) prms[2].Value;
            OriginatorID = (Guid) prms[3].Value;
            RequestDate = (DateTime) prms[4].Value;
            RequestorID = (Guid) prms[5].Value;
            OwnerID = (Guid) prms[6].Value;
            ObjectID = (Guid) prms[7].Value;
            Number = (string) prms[8].Value;
            Notes = (string) prms[9].Value;
            DocStatusID = (int)prms[10].Value;
            DepartmentID = (int)prms[11].Value;
            TemplateId = (int)prms[12].Value;
            ParentDocumentID = prms[13].Value != DBNull.Value ? (int)prms[13].Value : 0;
            DocStateID = prms[14].Value != DBNull.Value ? (int)prms[14].Value : 0;

            /*
            Department department = new Department(DepartmentID, UserName);
            if (!Permission.IsUserPermission(Config.ConnectionString, UserName, department.ObjectID, (int)Department.ActionType.Insert))
            {
                throw new AccessException(UserName, "Init");
            }*/
        }

        #endregion
        
        #region Public Methods

        public int Insert(SqlTransaction trans)
        {
            /*if (!CanInsert(UserName, DepartmentID))
            {
                throw new AccessException(UserName, "Insert");
            }
            */
            AccessObject accessObject = new AccessObject(trans.Connection.ConnectionString);
            accessObject.Id = Guid.NewGuid();
            accessObject.Name = Number;
            accessObject.ObjectTypeId = ObjectTypeID;
            accessObject.ObjectStateId = StateIDAll;
            ObjectID = accessObject.Insert(trans);

            SqlParameter[] prms = new SqlParameter[15];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            prms[1].Value = CodeID;

            prms[2] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[2].Value = CreationDate;

            prms[3] = new SqlParameter("@OriginatorID", SqlDbType.UniqueIdentifier);
            prms[3].Value = OriginatorID;

            prms[4] = new SqlParameter("@RequestDate", SqlDbType.DateTime);
            prms[4].Value = RequestDate;

            prms[5] = new SqlParameter("@RequestorID", SqlDbType.UniqueIdentifier);
            prms[5].Value = RequestorID;

            prms[6] = new SqlParameter("@OwnerID", SqlDbType.UniqueIdentifier);
            prms[6].Value = OwnerID;

            prms[7] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[7].Value = ObjectID;

            prms[8] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[8].Value = Number;

            prms[9] = new SqlParameter("@Notes", SqlDbType.NVarChar, -1);
            prms[9].Value = Notes;

            prms[10] = new SqlParameter("@DocStatusID", SqlDbType.Int);
            prms[10].Value = DocStatusID;

            prms[11] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[11].Value = DepartmentID;

            prms[12] = new SqlParameter("@TemplateId", SqlDbType.Int);
            prms[12].Value = TemplateId;

            prms[13] = new SqlParameter("@ParentDocumentID", SqlDbType.Int);
            prms[13].IsNullable = true;
            if (ParentDocumentID > 0) {
                prms[13].Value = ParentDocumentID;
            } else {
                prms[13].Value = DBNull.Value;
            }

            prms[14] = new SqlParameter("@DocStateID", SqlDbType.Int);
            prms[14].Value = DocStateID;

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
            /*if (!CanUpdate(UserName, DepartmentID))
            {
                throw new AccessException(UserName, "Update");
            }
            */
            SqlParameter[] prms = new SqlParameter[15];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@DocumentCodeID", SqlDbType.Int);
            prms[1].Value = CodeID;

            prms[2] = new SqlParameter("@CreationDate", SqlDbType.DateTime);
            prms[2].Value = CreationDate;

            prms[3] = new SqlParameter("@OriginatorID", SqlDbType.UniqueIdentifier);
            prms[3].Value = OriginatorID;

            prms[4] = new SqlParameter("@RequestDate", SqlDbType.DateTime);
            prms[4].Value = RequestDate;

            prms[5] = new SqlParameter("@RequestorID", SqlDbType.UniqueIdentifier);
            prms[5].Value = RequestorID;

            prms[6] = new SqlParameter("@OwnerID", SqlDbType.UniqueIdentifier);
            prms[6].Value = OwnerID;

            prms[7] = new SqlParameter("@ObjectID", SqlDbType.UniqueIdentifier);
            prms[7].Value = ObjectID;

            prms[8] = new SqlParameter("@Number", SqlDbType.NVarChar, 50);
            prms[8].Value = Number;

            prms[9] = new SqlParameter("@Notes", SqlDbType.NVarChar, -1);
            prms[9].Value = Notes;

            prms[10] = new SqlParameter("@DocStatusID", SqlDbType.Int);
            prms[10].Value = DocStatusID;

            prms[11] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[11].Value = DepartmentID;

            prms[12] = new SqlParameter("@TemplateId", SqlDbType.Int);
            prms[12].Value = TemplateId;

            prms[13] = new SqlParameter("@ParentDocumentID", SqlDbType.Int);
            prms[13].IsNullable = true;
            if (ParentDocumentID > 0)
            {
                prms[13].Value = ParentDocumentID;
            }
            else
            {
                prms[13].Value = DBNull.Value;
            }

            prms[14] = new SqlParameter("@DocStateID", SqlDbType.Int);
            prms[14].Value = DocStateID;

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

        public static void Delete(SqlTransaction trans, int id, string userName)
        {
            Document document = new Document(trans, id, userName);
            /*if (!CanDelete(userName, document.DepartmentID))
            {
                throw new AccessException(userName, "Delete");
            }*/

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

        public static bool CanInsert(string userName, int departmentID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }

            Department department = new Department(departmentID, userName);
            if (!Permission.IsUserPermission(Config.ConnectionString, userName, department.ObjectID, (int)Department.ActionType.Insert))
            {
                return false;
            }

            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int)ActionType.Insert);
        }

        public static bool CanUpdate(string userName, int departmentID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }

            Department department = new Department(departmentID, userName);
            if (!Permission.IsUserPermission(Config.ConnectionString, userName, department.ObjectID, (int)Department.ActionType.Insert))
            {
                return false;
            }

            return Permission.IsUserPermission(Config.ConnectionString, userName, ObjectTypeID, StateIDAll, (int)ActionType.Update);
        }

        public static bool CanDelete(string userName, int departmentID)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new DocumentException("Is empty or null");
            }

            Department department = new Department(departmentID, userName);
            if (!Permission.IsUserPermission(Config.ConnectionString, userName, department.ObjectID, (int)Department.ActionType.Insert))
            {
                return false;
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
        
    }
}
