using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web.Script.Serialization;
using BizObj.CustomException;
using BizObj.Data;
using BizObj.Models.Helpers;
using BizObj.Models.JqGrid;
using PermissionMembership;

namespace BizObj.Document
{
    [Serializable]
    public class ControlCard
    {
        protected struct SpNames
        {
            public const string Get = "usp_ControlCard_Get";
            public const string GetLast = "usp_ControlCard_GetLast";
            public const string GetWorkerCards = "usp_ControlCard_GetWorkerCards";
            public const string GetExecutiveCards = "usp_ControlCard_GetExecutiveCards";
            public const string GetCardsExternalToDepartment = "usp_ControlCard_GetCardsExternalToDepartment";
            public const string GetCardsExternalToWorker = "usp_ControlCard_GetCardsExternalToWorker";
            public const string Insert = "usp_ControlCard_Insert";
            public const string Update = "usp_ControlCard_Update";
            public const string Delete = "usp_ControlCard_Delete";
            public const string Find = "usp_ControlCard_Find";
            public const string List = "usp_ControlCard_List";
            public const string Children = "usp_ControlCard_Children";
            public const string DepartmentTop = "usp_ControlCard_DepartmentTop";
            public const string GetMaxCardNumber = "usp_ControlCard_GetMaxCardNumber";
            public const string SetOpenWorker = "usp_ControlCard_SetOpenWorker";
            public const string SetInnerNumber = "usp_ControlCard_SetInnerNumber";
            public const string SetActionComment = "usp_ControlCard_SetActionComment";
        }

        public const int ObjectTypeID = 7;
        private const int StateIDAll = ObjectTypeID * 1000 + 1;
        
        private enum ActionType
        {
            Insert = ObjectTypeID * 1000 + 1,
            Update = ObjectTypeID * 1000 + 2,
            Delete = ObjectTypeID * 1000 + 3,
            View = ObjectTypeID * 1000 + 4
        }
        
        #region Properties

        //[ParamAttribute("@ControlCardID", SqlDbType.Int)]
        //[FieldAttribute("ControlCardID")]
        public int ID { get; set; }

        //[ParamAttribute("@DocumentID", SqlDbType.Int)]
        //[FieldAttribute("DocumentID")]
        public int DocumentID { get; set; }

        //[ParamAttribute("@HeadID", SqlDbType.Int)]
        //[FieldAttribute("HeadID")]
        public int HeadID { get; set; }

        //[ParamAttribute("@WorkerID", SqlDbType.Int)]
        //[FieldAttribute("WorkerID")]
        public int WorkerID { get; set; }

        //[ParamAttribute("@CardNumber", SqlDbType.Int)]
        //[FieldAttribute("CardNumber")]
        public int CardNumber { get; set; }

        //[ParamAttribute("@StartDate", SqlDbType.DateTime)]
        //[FieldAttribute("StartDate")]
        public DateTime StartDate { get; set; }

        //[ParamAttribute("@EndDate", SqlDbType.DateTime)]
        //[FieldAttribute("EndDate")]
        public DateTime EndDate { get; set; }

        //[ParamAttribute("@ControlResponseDate", SqlDbType.DateTime, true)]
        //[FieldAttribute("ControlResponseDate")]
        public DateTime ControlResponseDate { get; set; }

        //[ParamAttribute("@ControlResponse", SqlDbType.NVarChar, -1)]
        //[FieldAttribute("ControlResponse")]
        public string ControlResponse { get; set; }

        //[ParamAttribute("@HeadResponseID", SqlDbType.Int, true)]
        //[FieldAttribute("HeadResponseID")]
        public int HeadResponseID { get; set; }

        //[ParamAttribute("@FixedWorkerID", SqlDbType.Int, true)]
        //[FieldAttribute("FixedWorkerID")]
        public int FixedWorkerID { get; set; }

        //[ParamAttribute("@CardStatusID", SqlDbType.Int)]
        //[FieldAttribute("CardStatusID")]
        public int CardStatusID { get; set; }

        //[ParamAttribute("@IsSpeciallyControlled", SqlDbType.Bit)]
        //[FieldAttribute("IsSpeciallyControlled")]
        public bool IsSpeciallyControlled { get; set; }

        //[ParamAttribute("@Notes", SqlDbType.NVarChar, -1)]
        //[FieldAttribute("Notes")]
        public string Notes { get; set; }

        //[ParamAttribute("@Resolution", SqlDbType.NVarChar, -1)]
        //[FieldAttribute("Resolution")]
        public string Resolution { get; set; }

        //[ParamAttribute("@IsDecisionOfSession", SqlDbType.Bit)]
        //[FieldAttribute("IsDecisionOfSession")]
        public bool IsDecisionOfSession { get; set; }

        //[ParamAttribute("@IsDecisionOfExecutiveCommittee", SqlDbType.Bit)]
        //[FieldAttribute("IsDecisionOfExecutiveCommittee")]
        public bool IsDecisionOfExecutiveCommittee { get; set; }

        //[ParamAttribute("@IsOrderOfHeader", SqlDbType.Bit)]
        //[FieldAttribute("IsOrderOfHeader")]
        public bool IsOrderOfHeader { get; set; }

        //[ParamAttribute("@IsActionWorkPlan", SqlDbType.Bit)]
        //[FieldAttribute("IsActionWorkPlan")]
        public bool IsActionWorkPlan { get; set; }

        //[ParamAttribute("@IsSendResponse", SqlDbType.Bit)]
        //[FieldAttribute("IsSendResponse")]
        public bool IsSendResponse { get; set; }

        //[ParamAttribute("@IsSendInterimResponse", SqlDbType.Bit)]
        //[FieldAttribute("IsSendInterimResponse")]
        public bool IsSendInterimResponse { get; set; }

        //[ParamAttribute("@IsLeftToWorker", SqlDbType.Bit)]
        //[FieldAttribute("IsLeftToWorker")]
        public bool IsLeftToWorker { get; set; }

        //[ParamAttribute("@IsAcquaintedWorker", SqlDbType.Bit)]
        //[FieldAttribute("IsAcquaintedWorker")]
        public bool IsAcquaintedWorker { get; set; }

        //[ParamAttribute("@ExecutiveDepartmentID", SqlDbType.Int)]
        //[FieldAttribute("ExecutiveDepartmentID")]
        public int ExecutiveDepartmentID { get; set; }

        //[ParamAttribute("@IsContinuation", SqlDbType.Bit)]
        //[FieldAttribute("IsContinuation")]
        public bool IsContinuation { get; set; }

        public int ParentControlCardID { get; set; }

        public string InnerNumber { get; set; }

        public int DepartmentID { get; set; }
        
        [ScriptIgnore]
        public string UserName { get; set; }

        public int DocStatusID { get; set; }

        public string CardStatusName { get; set; }

        public Worker Head { get; set; }

        public Worker Worker { get; set; }

        public Worker HeadResponse { get; set; }

        public Worker FixedWorker { get; set; }

        #endregion
        
        #region Constructors

        public ControlCard()
        {
            
        }

        public ControlCard(string userName)
        {
            UserName = userName;
        }

        public ControlCard(int id, string userName): this(null, id, userName)
        {
            
        }

        public ControlCard(SqlTransaction trans, int id, string userName): this(userName)
        {
            Init(trans, id);

            Head = new Worker(trans, HeadID, userName);
            Worker = new Worker(trans, WorkerID, userName);
            HeadResponse = new Worker(trans, HeadResponseID, userName);
            FixedWorker = new Worker(trans, FixedWorkerID, userName);

            DocStatusID = (new Document(trans, DocumentID, userName)).DocStatusID;
        }
        #endregion

        #region Private Methods
        
        private void Init(SqlTransaction trans, int id)
        {
            /*if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }*/
            
            SqlParameter[] prms = new SqlParameter[28];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Value = id;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            prms[2] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            prms[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[3].Direction = ParameterDirection.Output;

            prms[4] = new SqlParameter("@CardNumber", SqlDbType.Int);
            prms[4].Direction = ParameterDirection.Output;

            prms[5] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            prms[5].Direction = ParameterDirection.Output;

            prms[6] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            prms[6].Direction = ParameterDirection.Output;

            prms[7] = new SqlParameter("@ControlResponseDate", SqlDbType.DateTime);
            prms[7].Direction = ParameterDirection.Output;
            prms[7].IsNullable = true;

            prms[8] = new SqlParameter("@ControlResponse", SqlDbType.NVarChar, -1);
            prms[8].Direction = ParameterDirection.Output;

            prms[9] = new SqlParameter("@HeadResponseID", SqlDbType.Int);
            prms[9].Direction = ParameterDirection.Output;

            prms[10] = new SqlParameter("@FixedWorkerID", SqlDbType.Int);
            prms[10].Direction = ParameterDirection.Output;

            prms[11] = new SqlParameter("@CardStatusID", SqlDbType.Int);
            prms[11].Direction = ParameterDirection.Output;

            prms[12] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Bit);
            prms[12].Direction = ParameterDirection.Output;

            prms[13] = new SqlParameter("@Notes", SqlDbType.NVarChar, -1);
            prms[13].Direction = ParameterDirection.Output;

            prms[14] = new SqlParameter("@Resolution", SqlDbType.NVarChar, -1);
            prms[14].Direction = ParameterDirection.Output;

            prms[15] = new SqlParameter("@IsDecisionOfSession", SqlDbType.Bit);
            prms[15].Direction = ParameterDirection.Output;

            prms[16] = new SqlParameter("@IsDecisionOfExecutiveCommittee", SqlDbType.Bit);
            prms[16].Direction = ParameterDirection.Output;

            prms[17] = new SqlParameter("@IsOrderOfHeader", SqlDbType.Bit);
            prms[17].Direction = ParameterDirection.Output;

            prms[18] = new SqlParameter("@IsActionWorkPlan", SqlDbType.Bit);
            prms[18].Direction = ParameterDirection.Output;

            prms[19] = new SqlParameter("@IsSendResponse", SqlDbType.Bit);
            prms[19].Direction = ParameterDirection.Output;

            prms[20] = new SqlParameter("@IsSendInterimResponse", SqlDbType.Bit);
            prms[20].Direction = ParameterDirection.Output;

            prms[21] = new SqlParameter("@IsLeftToWorker", SqlDbType.Bit);
            prms[21].Direction = ParameterDirection.Output;

            prms[22] = new SqlParameter("@IsAcquaintedWorker", SqlDbType.Bit);
            prms[22].Direction = ParameterDirection.Output;

            prms[23] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            prms[23].Direction = ParameterDirection.Output;

            prms[24] = new SqlParameter("@IsContinuation", SqlDbType.Bit);
            prms[24].Direction = ParameterDirection.Output;

            prms[25] = new SqlParameter("@ParentControlCardID", SqlDbType.Int);
            prms[25].Direction = ParameterDirection.Output;

            prms[26] = new SqlParameter("@InnerNumber", SqlDbType.NVarChar, 50);
            prms[26].Direction = ParameterDirection.Output;

            prms[27] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[27].Direction = ParameterDirection.Output;

            if (trans == null)
                SPHelper.ExecuteNonQuery(SpNames.Get, prms);
            else
                SPHelper.ExecuteNonQuery(trans, SpNames.Get, prms);

            ID = id;
            DocumentID = (int)prms[1].Value;
            HeadID = (int)prms[2].Value;
            WorkerID = (int)prms[3].Value;
            CardNumber = (int)prms[4].Value;
            StartDate = (DateTime)prms[5].Value;
            EndDate = (DateTime)prms[6].Value;

            if (prms[7].Value != DBNull.Value)
                ControlResponseDate = (DateTime)prms[7].Value;
            else
                ControlResponseDate = DateTime.MinValue;

            ControlResponse = (string)prms[8].Value;
            HeadResponseID = (int)prms[9].Value;
            FixedWorkerID = (int)prms[10].Value;
            CardStatusID = (int)prms[11].Value;
            IsSpeciallyControlled = (bool)prms[12].Value;
            Notes = (string)prms[13].Value;
            Resolution = (string)prms[14].Value;
            IsDecisionOfSession = (bool)prms[15].Value;
            IsDecisionOfExecutiveCommittee = (bool)prms[16].Value;
            IsOrderOfHeader = (bool)prms[17].Value;
            IsActionWorkPlan = (bool)prms[18].Value;
            IsSendResponse = (bool)prms[19].Value;
            IsSendInterimResponse = (bool)prms[20].Value;
            IsLeftToWorker = (bool)prms[21].Value;
            IsAcquaintedWorker = (bool)prms[22].Value;
            ExecutiveDepartmentID = (int)prms[23].Value;
            IsContinuation = (bool)prms[24].Value;
            ParentControlCardID = prms[25].Value != DBNull.Value ? (int)prms[25].Value : 0;
            InnerNumber = prms[26].Value != DBNull.Value ? (string) prms[26].Value : String.Empty;
            DepartmentID = (int)prms[27].Value;
        }
        
        #endregion
        
        #region Public Methods
        
        public int Insert(SqlTransaction trans)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }
            
            SqlParameter[] prms = new SqlParameter[28];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[2].Value = HeadID;

            prms[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[3].Value = WorkerID;

            prms[4] = new SqlParameter("@CardNumber", SqlDbType.Int);
            prms[4].Value = CardNumber;

            prms[5] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            prms[5].Value = StartDate;

            prms[6] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            prms[6].Value = EndDate;

            prms[7] = new SqlParameter("@ControlResponseDate", SqlDbType.DateTime);
            if (ControlResponseDate > (DateTime)SqlDateTime.MinValue)
                prms[7].Value = ControlResponseDate;
            else
                prms[7].Value = DBNull.Value;
            prms[7].IsNullable = true;

            prms[8] = new SqlParameter("@ControlResponse", SqlDbType.NVarChar, -1);
            prms[8].Value = ControlResponse;

            prms[9] = new SqlParameter("@HeadResponseID", SqlDbType.Int);
            prms[9].Value = HeadResponseID;

            prms[10] = new SqlParameter("@FixedWorkerID", SqlDbType.Int);
            prms[10].Value = FixedWorkerID;

            prms[11] = new SqlParameter("@CardStatusID", SqlDbType.Int);
            prms[11].Value = CardStatusID;

            prms[12] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Bit);
            prms[12].Value = IsSpeciallyControlled;

            prms[13] = new SqlParameter("@Notes", SqlDbType.NVarChar, -1);
            prms[13].Value = Notes;

            prms[14] = new SqlParameter("@Resolution", SqlDbType.NVarChar, -1);
            prms[14].Value = Resolution;

            prms[15] = new SqlParameter("@IsDecisionOfSession", SqlDbType.Bit);
            prms[15].Value = IsDecisionOfSession;

            prms[16] = new SqlParameter("@IsDecisionOfExecutiveCommittee", SqlDbType.Bit);
            prms[16].Value = IsDecisionOfExecutiveCommittee;

            prms[17] = new SqlParameter("@IsOrderOfHeader", SqlDbType.Bit);
            prms[17].Value = IsOrderOfHeader;

            prms[18] = new SqlParameter("@IsActionWorkPlan", SqlDbType.Bit);
            prms[18].Value = IsActionWorkPlan;

            prms[19] = new SqlParameter("@IsSendResponse", SqlDbType.Bit);
            prms[19].Value = IsSendResponse;

            prms[20] = new SqlParameter("@IsSendInterimResponse", SqlDbType.Bit);
            prms[20].Value = IsSendInterimResponse;

            prms[21] = new SqlParameter("@IsLeftToWorker", SqlDbType.Bit);
            prms[21].Value = IsLeftToWorker;

            prms[22] = new SqlParameter("@IsAcquaintedWorker", SqlDbType.Bit);
            prms[22].Value = IsAcquaintedWorker;

            prms[23] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            prms[23].Value = ExecutiveDepartmentID;

            prms[24] = new SqlParameter("@IsContinuation", SqlDbType.Bit);
            prms[24].Value = IsContinuation;

            prms[25] = new SqlParameter("@ParentControlCardID", SqlDbType.Int);
            prms[25].IsNullable = true;
            if (ParentControlCardID > 0) {
                prms[25].Value = ParentControlCardID;
            } else {
                prms[25].Value = DBNull.Value;
            }
            
            prms[26] = new SqlParameter("@InnerNumber", SqlDbType.NVarChar, 50);
            prms[26].Value = InnerNumber;

            prms[27] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[27].Value = DepartmentID;


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

            SqlParameter[] prms = new SqlParameter[28];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Value = ID;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = DocumentID;

            prms[2] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[2].Value = HeadID;

            prms[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[3].Value = WorkerID;

            prms[4] = new SqlParameter("@CardNumber", SqlDbType.Int);
            prms[4].Value = CardNumber;

            prms[5] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            prms[5].Value = StartDate;

            prms[6] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            prms[6].Value = EndDate;

            prms[7] = new SqlParameter("@ControlResponseDate", SqlDbType.DateTime);
            if (ControlResponseDate > (DateTime) SqlDateTime.MinValue)
                prms[7].Value = ControlResponseDate;
            else
                prms[7].Value = DBNull.Value;
            prms[7].IsNullable = true;

            prms[8] = new SqlParameter("@ControlResponse", SqlDbType.NVarChar, -1);
            prms[8].Value = ControlResponse;

            prms[9] = new SqlParameter("@HeadResponseID", SqlDbType.Int);
            prms[9].Value = HeadResponseID;

            prms[10] = new SqlParameter("@FixedWorkerID", SqlDbType.Int);
            prms[10].Value = FixedWorkerID;

            prms[11] = new SqlParameter("@CardStatusID", SqlDbType.Int);
            prms[11].Value = CardStatusID;

            prms[12] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Bit);
            prms[12].Value = IsSpeciallyControlled;

            prms[13] = new SqlParameter("@Notes", SqlDbType.NVarChar, -1);
            prms[13].Value = Notes;

            prms[14] = new SqlParameter("@Resolution", SqlDbType.NVarChar, -1);
            prms[14].Value = Resolution;

            prms[15] = new SqlParameter("@IsDecisionOfSession", SqlDbType.Bit);
            prms[15].Value = IsDecisionOfSession;

            prms[16] = new SqlParameter("@IsDecisionOfExecutiveCommittee", SqlDbType.Bit);
            prms[16].Value = IsDecisionOfExecutiveCommittee;

            prms[17] = new SqlParameter("@IsOrderOfHeader", SqlDbType.Bit);
            prms[17].Value = IsOrderOfHeader;

            prms[18] = new SqlParameter("@IsActionWorkPlan", SqlDbType.Bit);
            prms[18].Value = IsActionWorkPlan;

            prms[19] = new SqlParameter("@IsSendResponse", SqlDbType.Bit);
            prms[19].Value = IsSendResponse;

            prms[20] = new SqlParameter("@IsSendInterimResponse", SqlDbType.Bit);
            prms[20].Value = IsSendInterimResponse;

            prms[21] = new SqlParameter("@IsLeftToWorker", SqlDbType.Bit);
            prms[21].Value = IsLeftToWorker;

            prms[22] = new SqlParameter("@IsAcquaintedWorker", SqlDbType.Bit);
            prms[22].Value = IsAcquaintedWorker;

            prms[23] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            prms[23].Value = ExecutiveDepartmentID;

            prms[24] = new SqlParameter("@IsContinuation", SqlDbType.Bit);
            prms[24].Value = IsContinuation;

            prms[25] = new SqlParameter("@ParentControlCardID", SqlDbType.Int);
            prms[25].IsNullable = true;
            if (ParentControlCardID > 0) {
                prms[25].Value = ParentControlCardID;
            } else {
                prms[25].Value = DBNull.Value;
            }

            prms[26] = new SqlParameter("@InnerNumber", SqlDbType.NVarChar, 50);
            prms[26].Value = InnerNumber;

            prms[27] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[27].Value = DepartmentID;

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

        public static ControlCard GetLastCard(SqlTransaction trans, int documentID, int departmentId, string userName)
        {
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[1].Value = departmentId;

            prms[2] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[2].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, SpNames.GetLast, prms);

            int controlCardID = 0;
            if (prms[2].Value != DBNull.Value)
                controlCardID = (int) prms[2].Value;

            if (controlCardID <= 0)
                return null;

            return new ControlCard(trans, controlCardID, userName);
        }

        public static DataTable GetList(SqlTransaction trans, int documentID, int departmentId)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[1].Value = departmentId;

            return SPHelper.ExecuteDataset(trans, SpNames.List, prms).Tables[0];
        }

        public static DataTable GetList(int documentID, int departmentId)
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

                    dt = GetList(trans, documentID, departmentId);

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

        public static DataTable GetChildren(SqlTransaction trans, int controlCardID)
        {
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Value = controlCardID;

            return SPHelper.ExecuteDataset(trans, SpNames.Children, prms).Tables[0];
        }

        public static DataTable GetDepartmentTop(SqlTransaction trans, int documentID, int departmentId)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[1].Value = departmentId;

            return SPHelper.ExecuteDataset(trans, SpNames.DepartmentTop, prms).Tables[0];
        }

        public static DataTable GetDepartmentTop(int documentID, int departmentId)
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

                    dt = GetDepartmentTop(trans, documentID, departmentId);

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

        public static DataTable GetChildren(int controlCardID)
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

                    dt = GetChildren(trans, controlCardID);

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


        public static List<ControlCard> GetExecutiveCards(SqlTransaction trans, int documentID, int departmentId, string userName)
        {
            List<ControlCard> cards = new List<ControlCard>();

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[1].Value = departmentId;

            DataTable cTable = SPHelper.ExecuteDataset(trans, SpNames.GetExecutiveCards, prms).Tables[0];

            foreach (DataRow row in cTable.Rows)
            {
                int controlCardID = (int) row["ControlCardID"];
                ControlCard card = new ControlCard(trans, controlCardID, userName);
                cards.Add(card);
            }

            return cards;
        }

        public static List<ControlCardBlank> GetCardsExternalToDepartment(SqlTransaction trans, int documentID, int departmentId, string userName)
        {
            List<ControlCardBlank> cards = new List<ControlCardBlank>();

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[1].Value = departmentId;

            DataTable cTable = SPHelper.ExecuteDataset(trans, SpNames.GetCardsExternalToDepartment, prms).Tables[0];

            foreach (DataRow row in cTable.Rows)
            {
                int controlCardID = (int)row["ControlCardID"];
                ControlCardBlank card = new ControlCardBlank(trans, controlCardID, userName);
                cards.Add(card);
            }

            return cards;
        }

        public static List<ControlCardBlank> GetCardsExternalToWorker(SqlTransaction trans, int documentID, int workerId, string userName)
        {
            List<ControlCardBlank> cards = new List<ControlCardBlank>();

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[1].Value = workerId;

            DataTable cTable = SPHelper.ExecuteDataset(trans, SpNames.GetCardsExternalToWorker, prms).Tables[0];

            foreach (DataRow row in cTable.Rows)
            {
                int controlCardID = (int)row["ControlCardID"];
                ControlCardBlank card = new ControlCardBlank(trans, controlCardID, userName);
                cards.Add(card);
            }

            return cards;
        }

        public static List<ControlCard> GetWorkerCards(SqlTransaction trans, int documentID, int workerId, string userName)
        {
            List<ControlCard> cards = new List<ControlCard>();

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[1].Value = workerId;

            DataTable cTable = SPHelper.ExecuteDataset(trans, SpNames.GetWorkerCards, prms).Tables[0];

            foreach (DataRow row in cTable.Rows)
            {
                int controlCardID = (int)row["ControlCardID"];
                ControlCard card = new ControlCard(trans, controlCardID, userName);
                cards.Add(card);
            }

            return cards;
        }

        public static JqGridResults BuildJqGridResults(DataTable dataTable)
        {
            JqGridResults result = new JqGridResults();
            List<JqGridRow> rows = new List<JqGridRow>();
            foreach (DataRow dr in dataTable.Rows)
            {
                JqGridRow row = new JqGridRow();
                row.id = (int) dr["ControlCardID"];
                row.cell = new string[13];

                row.cell[0] = dr["ControlCardID"].ToString();
                row.cell[1] = dr["DocumentID"].ToString();
                row.cell[2] = dr["CardNumber"].ToString();

                row.cell[3] = ((DateTime) dr["StartDate"]).ToString("yyyy-MM-dd");
                row.cell[4] = ((DateTime) dr["EndDate"]).ToString("yyyy-MM-dd");

                row.cell[5] = (string) dr["CardStatusName"];
                row.cell[6] = FormatHelper.FormatToLastNameAndInitials((string)dr["WorkerLastName"], (string)dr["WorkerFirstName"], (string)dr["WorkerMiddleName"]);

                row.cell[7] = (string) dr["ControlResponse"];
                row.cell[8] = (string) dr["Resolution"];
                row.cell[9] = "завантажити";
                if (dr["GroupID"] == DBNull.Value)
                {
                    row.cell[10] = "0";
                }
                else
                {
                    row.cell[10] = dr["GroupID"].ToString();
                }
                row.cell[11] = dr["InnerNumber"] == DBNull.Value ? String.Empty : (string)dr["InnerNumber"];
                row.cell[12] = dr["ActionCommentID"].ToString();

                rows.Add(row);
            }
            result.rows = rows.ToArray();
            result.page = 1;
            result.total = dataTable.Rows.Count;
            result.records = dataTable.Rows.Count;

            return result;
        }


        public static DataTable FindCard(SqlTransaction trans, int documentID, int? cardStatusID, bool? isSpeciallyControlled)
        {
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;
            prms[1] = new SqlParameter("@CardStatusID", SqlDbType.Int);
            if (cardStatusID != null)
                prms[1].Value = cardStatusID;
            else
                prms[1].Value = DBNull.Value;
            prms[2] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Int);
            if (isSpeciallyControlled != null)
                prms[2].Value = isSpeciallyControlled;
            else
                prms[2].Value = DBNull.Value;

            return SPHelper.ExecuteDataset(trans, SpNames.Find, prms).Tables[0];
        }

        public static bool ExistCard(SqlTransaction trans, int documentID, int cardStatusID, bool isSpeciallyControlled)
        {
            return FindCard(trans, documentID, cardStatusID, isSpeciallyControlled).Rows.Count > 0;
        }
        public static bool ExistCard(SqlTransaction trans, int documentID, int cardStatusID)
        {
            return FindCard(trans, documentID, cardStatusID, null).Rows.Count > 0;
        }
        public static bool ExistCard(SqlTransaction trans, int documentID, bool isSpeciallyControlled)
        {
            return FindCard(trans, documentID, null, isSpeciallyControlled).Rows.Count > 0;
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

        public static int GetMaxCardNumber(SqlTransaction trans, int documentID, string userName)
        {
            if (!CanDelete(userName))
            {
                throw new AccessException(userName, "Delete");
            }

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@CardNumber", SqlDbType.Int);
            prms[1].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, SpNames.GetMaxCardNumber, documentID);

            return prms[0].Value != DBNull.Value ? (int) prms[0].Value : 0;
        }

        public static void SetOpenWorker(SqlTransaction trans, int controlCardId, int workerId, string userName)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Value = controlCardId;

            prms[1] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[1].Value = workerId;

            SPHelper.ExecuteNonQuery(trans, SpNames.SetOpenWorker, prms);
        }

        public static void SetOpenWorker(int controlCardId, int workerId, string userName)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    SetOpenWorker(trans, controlCardId, workerId, userName);

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

        public static void SetActionComment(SqlTransaction trans, int controlCardId, int actionCommentId)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Value = controlCardId;

            prms[1] = new SqlParameter("@ActionCommentID", SqlDbType.Int);
            prms[1].Value = actionCommentId;

            SPHelper.ExecuteNonQuery(trans, SpNames.SetActionComment, prms);
        }

        public static void SetInnerNumber(SqlTransaction trans, int controlCardId, string innerNumber)
        {
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Value = controlCardId;

            prms[1] = new SqlParameter("@InnerNumber", SqlDbType.NVarChar, 50);
            prms[1].Value = innerNumber;

            SPHelper.ExecuteNonQuery(trans, SpNames.SetInnerNumber, prms);
        }

        public static void SetInnerNumber(int controlCardId, string innerNumber)
        {
            SqlConnection connection = new SqlConnection(Config.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    SetInnerNumber(trans, controlCardId, innerNumber);

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
        
    }
}
