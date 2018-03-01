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

namespace BizObj.Document
{
    [Serializable]
    public class ControlCardGroup : ControlCardBlank
    {
        #region Properties
        public List<ControlCardBlank> Group { get; set; }
        public int? GroupID { get; set; }

        #endregion

        #region Constructors

        public ControlCardGroup()
        {
            
        }

        public ControlCardGroup(string userName): base(userName)
        {
            
        }

        public ControlCardGroup(int id, string userName): base(null, id, userName)
        {
            
        }

        public ControlCardGroup(SqlTransaction trans, int id, string userName): base(trans, id, userName)
        {
            Init(trans, id);
            /*
            Head = new Worker(trans, HeadID, userName);
            Worker = new Worker(trans, WorkerID, userName);
            HeadResponse = new Worker(trans, HeadResponseID, userName);
            FixedWorker = new Worker(trans, FixedWorkerID, userName);

            DocStatusID = (new Document(trans, DocumentID, userName)).DocStatusID;

            CardStatus = new CardStatus(trans, CardStatusID, userName);
            DocStatus = new DocStatus(trans, DocStatusID, userName);
            ExecutiveDepartment = new Department(trans, ExecutiveDepartmentID, userName);
            ChildrenControlCards = GetChildren(trans);
            */
        }
        public void InitSubObjects(SqlTransaction trans, string userName)
        {

            Head = new Worker(trans, HeadID, userName);
            Worker = new Worker(trans, WorkerID, userName);
            HeadResponse = new Worker(trans, HeadResponseID, userName);
            FixedWorker = new Worker(trans, FixedWorkerID, userName);

            DocStatusID = (new Document(trans, DocumentID, userName)).DocStatusID;

            CardStatus = new CardStatus(trans, CardStatusID, userName);
            DocStatus = new DocStatus(trans, DocStatusID, userName);
            ExecutiveDepartment = new Department(trans, ExecutiveDepartmentID, userName);
        }
        #endregion

        #region Private Methods
        private void Init(SqlTransaction trans, int id)
        {
            if (!CanView(UserName))
            {
                throw new AccessException(UserName, "Init");
            }

            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Value = id;

            DataTable cTable;
            if (trans == null)
                cTable = SPHelper.ExecuteDataset("usp_ControlCard_GetGroup", prms).Tables[0];
            else
                cTable = SPHelper.ExecuteDataset(trans, "usp_ControlCard_GetGroup", prms).Tables[0];

            Group = new List<ControlCardBlank>();
            foreach (DataRow row in cTable.Rows)
            {
                ControlCardGroup card = new ControlCardGroup();

                card.ID = (int)row["ControlCardID"];
                card.DocumentID = (int)row["DocumentID"];
                card.HeadID = (int)row["HeadID"];
                card.WorkerID = (int)row["WorkerID"];
                card.CardNumber = (int)row["CardNumber"];
                card.StartDate = (DateTime)row["StartDate"];
                card.EndDate = (DateTime)row["EndDate"];

                card.ControlResponseDate = row["ControlResponseDate"] != DBNull.Value ? (DateTime)row["ControlResponseDate"] : DateTime.MinValue;

                card.ControlResponse = (string)row["ControlResponse"];
                card.HeadResponseID = (int)row["HeadResponseID"];
                card.FixedWorkerID = (int)row["FixedWorkerID"];
                card.CardStatusID = (int)row["CardStatusID"];
                card.IsSpeciallyControlled = (bool)row["IsSpeciallyControlled"];
                card.Notes = (string)row["Notes"];
                card.Resolution = (string)row["Resolution"];
                card.IsDecisionOfSession = (bool)row["IsDecisionOfSession"];
                card.IsDecisionOfExecutiveCommittee = (bool)row["IsDecisionOfExecutiveCommittee"];
                card.IsOrderOfHeader = (bool)row["IsOrderOfHeader"];
                card.IsActionWorkPlan = (bool)row["IsActionWorkPlan"];
                card.IsSendResponse = (bool)row["IsSendResponse"];
                card.IsSendInterimResponse = (bool)row["IsSendInterimResponse"];
                card.IsLeftToWorker = (bool)row["IsLeftToWorker"];
                card.IsAcquaintedWorker = (bool)row["IsAcquaintedWorker"];
                card.ExecutiveDepartmentID = (int)row["ExecutiveDepartmentID"];
                card.IsContinuation = (bool)row["IsContinuation"];
                card.ParentControlCardID = row["ParentControlCardID"] != DBNull.Value ? (int)row["ParentControlCardID"] : 0;
                card.InnerNumber = row["InnerNumber"] != DBNull.Value ? (string)row["InnerNumber"] : String.Empty;
                card.DepartmentID = (int)row["DepartmentID"];
                card.GroupID = row["GroupID"] != DBNull.Value ? (int)row["GroupID"] : 0;

                card.InitSubObjects(trans, UserName);
                Group.Add(card);

                if (card.ID == id)
                {
                    ID = (int) row["ControlCardID"];
                    DocumentID = (int) row["DocumentID"];
                    HeadID = (int) row["HeadID"];
                    WorkerID = (int) row["WorkerID"];
                    CardNumber = (int) row["CardNumber"];
                    StartDate = (DateTime) row["StartDate"];
                    EndDate = (DateTime) row["EndDate"];

                    ControlResponseDate = row["ControlResponseDate"] != DBNull.Value
                                              ? (DateTime) row["ControlResponseDate"]
                                              : DateTime.MinValue;

                    ControlResponse = (string) row["ControlResponse"];
                    HeadResponseID = (int) row["HeadResponseID"];
                    FixedWorkerID = (int) row["FixedWorkerID"];
                    CardStatusID = (int) row["CardStatusID"];
                    IsSpeciallyControlled = (bool) row["IsSpeciallyControlled"];
                    Notes = (string) row["Notes"];
                    Resolution = (string) row["Resolution"];
                    IsDecisionOfSession = (bool) row["IsDecisionOfSession"];
                    IsDecisionOfExecutiveCommittee = (bool) row["IsDecisionOfExecutiveCommittee"];
                    IsOrderOfHeader = (bool) row["IsOrderOfHeader"];
                    IsActionWorkPlan = (bool) row["IsActionWorkPlan"];
                    IsSendResponse = (bool) row["IsSendResponse"];
                    IsSendInterimResponse = (bool) row["IsSendInterimResponse"];
                    IsLeftToWorker = (bool) row["IsLeftToWorker"];
                    IsAcquaintedWorker = (bool) row["IsAcquaintedWorker"];
                    ExecutiveDepartmentID = (int) row["ExecutiveDepartmentID"];
                    IsContinuation = (bool) row["IsContinuation"];
                    ParentControlCardID = row["ParentControlCardID"] != DBNull.Value
                                              ? (int) row["ParentControlCardID"]
                                              : 0;
                    InnerNumber = row["InnerNumber"] != DBNull.Value ? (string) row["InnerNumber"] : String.Empty;
                    DepartmentID = (int) row["DepartmentID"];
                    GroupID = row["GroupID"] != DBNull.Value ? (int) row["GroupID"] : 0;
                }
            }
        }

        #endregion

        #region Public Methods

        private int InsertGroup(SqlTransaction trans, ControlCardBlank controlCardBlank)
        {
            if (!CanInsert(UserName))
            {
                throw new AccessException(UserName, "Insert");
            }

            SqlParameter[] prms = new SqlParameter[29];
            prms[0] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            prms[0].Direction = ParameterDirection.Output;

            prms[1] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[1].Value = controlCardBlank.DocumentID;

            prms[2] = new SqlParameter("@HeadID", SqlDbType.Int);
            prms[2].Value = controlCardBlank.HeadID;

            prms[3] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[3].Value = controlCardBlank.WorkerID;

            prms[4] = new SqlParameter("@CardNumber", SqlDbType.Int);
            prms[4].Value = controlCardBlank.CardNumber;

            prms[5] = new SqlParameter("@StartDate", SqlDbType.DateTime);
            prms[5].Value = controlCardBlank.StartDate;

            prms[6] = new SqlParameter("@EndDate", SqlDbType.DateTime);
            prms[6].Value = controlCardBlank.EndDate;

            prms[7] = new SqlParameter("@ControlResponseDate", SqlDbType.DateTime);
            if (controlCardBlank.ControlResponseDate > (DateTime)SqlDateTime.MinValue)
                prms[7].Value = controlCardBlank.ControlResponseDate;
            else
                prms[7].Value = DBNull.Value;
            prms[7].IsNullable = true;

            prms[8] = new SqlParameter("@ControlResponse", SqlDbType.NVarChar, -1);
            prms[8].Value = controlCardBlank.ControlResponse;

            prms[9] = new SqlParameter("@HeadResponseID", SqlDbType.Int);
            prms[9].Value = controlCardBlank.HeadResponseID;

            prms[10] = new SqlParameter("@FixedWorkerID", SqlDbType.Int);
            prms[10].Value = controlCardBlank.FixedWorkerID;

            prms[11] = new SqlParameter("@CardStatusID", SqlDbType.Int);
            prms[11].Value = controlCardBlank.CardStatusID;

            prms[12] = new SqlParameter("@IsSpeciallyControlled", SqlDbType.Bit);
            prms[12].Value = controlCardBlank.IsSpeciallyControlled;

            prms[13] = new SqlParameter("@Notes", SqlDbType.NVarChar, -1);
            prms[13].Value = controlCardBlank.Notes;

            prms[14] = new SqlParameter("@Resolution", SqlDbType.NVarChar, -1);
            prms[14].Value = controlCardBlank.Resolution;

            prms[15] = new SqlParameter("@IsDecisionOfSession", SqlDbType.Bit);
            prms[15].Value = controlCardBlank.IsDecisionOfSession;

            prms[16] = new SqlParameter("@IsDecisionOfExecutiveCommittee", SqlDbType.Bit);
            prms[16].Value = controlCardBlank.IsDecisionOfExecutiveCommittee;

            prms[17] = new SqlParameter("@IsOrderOfHeader", SqlDbType.Bit);
            prms[17].Value = controlCardBlank.IsOrderOfHeader;

            prms[18] = new SqlParameter("@IsActionWorkPlan", SqlDbType.Bit);
            prms[18].Value = controlCardBlank.IsActionWorkPlan;

            prms[19] = new SqlParameter("@IsSendResponse", SqlDbType.Bit);
            prms[19].Value = controlCardBlank.IsSendResponse;

            prms[20] = new SqlParameter("@IsSendInterimResponse", SqlDbType.Bit);
            prms[20].Value = controlCardBlank.IsSendInterimResponse;

            prms[21] = new SqlParameter("@IsLeftToWorker", SqlDbType.Bit);
            prms[21].Value = controlCardBlank.IsLeftToWorker;

            prms[22] = new SqlParameter("@IsAcquaintedWorker", SqlDbType.Bit);
            prms[22].Value = controlCardBlank.IsAcquaintedWorker;

            prms[23] = new SqlParameter("@ExecutiveDepartmentID", SqlDbType.Int);
            prms[23].Value = controlCardBlank.ExecutiveDepartmentID;

            prms[24] = new SqlParameter("@IsContinuation", SqlDbType.Bit);
            prms[24].Value = controlCardBlank.IsContinuation;

            prms[25] = new SqlParameter("@ParentControlCardID", SqlDbType.Int);
            prms[25].IsNullable = true;
            if (controlCardBlank.ParentControlCardID > 0)
            {
                prms[25].Value = controlCardBlank.ParentControlCardID;
            }
            else
            {
                prms[25].Value = DBNull.Value;
            }

            prms[26] = new SqlParameter("@InnerNumber", SqlDbType.NVarChar, 50);
            prms[26].Value = controlCardBlank.InnerNumber;

            prms[27] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[27].Value = controlCardBlank.DepartmentID;

            prms[28] = new SqlParameter("@GroupID", SqlDbType.Int);
            prms[28].Direction = ParameterDirection.InputOutput;
            prms[28].IsNullable = true;
            if (GroupID > 0) {
                prms[28].Value = GroupID;
            } else {
                prms[28].Value = DBNull.Value;
            }


            if (trans == null)
                SPHelper.ExecuteNonQuery("usp_ControlCard_InsertGroup", prms);
            else
                SPHelper.ExecuteNonQuery(trans, "usp_ControlCard_InsertGroup", prms);

            ID = (int)prms[0].Value;
            if (prms[28].Value != DBNull.Value)
            {
                GroupID = (int?)prms[28].Value;
            }

            return ID;
        }

        public new int Insert(SqlTransaction trans)
        {
            foreach (ControlCardBlank controlCardBlank in Group)
            {
                InsertGroup(trans, controlCardBlank);
            }
            ChangeDocumentControlled(trans, DocumentID);
            return ID;
        }

        public new int Insert()
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

        public new int Update(SqlTransaction trans)
        {
            ControlCardGroup cardGroup = new ControlCardGroup(trans, ID, UserName);
            foreach (ControlCardBlank card in cardGroup.Group)
            {
                bool exist = false;
                foreach (ControlCardBlank newCard in Group)
                {
                    if (card.ID == newCard.ID)
                    {
                        exist = true;
                    }
                }
                if (exist == false)
                {
                    ControlCard.Delete(trans, card.ID, UserName);
                }
            }

            foreach (ControlCardBlank controlCardBlank in Group)
            {
                controlCardBlank.UserName = UserName;
                if (controlCardBlank.ID == 0)
                {
                    InsertGroup(trans, controlCardBlank);
                }
                else
                {
                    controlCardBlank.Update(trans);
                }
            }
            ChangeDocumentControlled(trans, DocumentID);
            return ID;
        }
        #endregion

        #region Static Public Methods

        #endregion

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
    }
}
