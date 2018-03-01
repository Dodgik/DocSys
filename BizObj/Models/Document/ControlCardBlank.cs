using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using BizObj.Data;
using BizObj.Models.Helpers;
using BizObj.Models.JqGrid;

namespace BizObj.Document
{
    [Serializable]
    public class ControlCardBlank : ControlCard
    {
        #region Properties

        public CardStatus CardStatus { get; set; }
        public DocStatus DocStatus { get; set; }
        public Department ExecutiveDepartment { get; set; }
        public List<ControlCardBlank> ChildrenControlCards { get; set; }

        #endregion

        #region Constructors

        public ControlCardBlank()
        {
            
        }

        public ControlCardBlank(string userName): base(userName)
        {
            
        }

        public ControlCardBlank(int id, string userName): base(null, id, userName)
        {
            
        }

        public ControlCardBlank(SqlTransaction trans, int id, string userName): base(trans, id, userName)
        {
            CardStatus = new CardStatus(trans, CardStatusID, userName);
            DocStatus = new DocStatus(trans, DocStatusID, userName);
            ExecutiveDepartment = new Department(trans, ExecutiveDepartmentID, userName);
            ChildrenControlCards = GetChildren(trans);
        }
        #endregion

        #region Private Methods

        private List<ControlCardBlank> GetChildren(SqlTransaction trans)
        {
            List<ControlCardBlank> cards = new List<ControlCardBlank>();

            DataTable cTable = GetChildren(trans, ID);
            
            foreach (DataRow row in cTable.Rows)
            {
                int controlCardID = (int)row["ControlCardID"];
                ControlCardBlank card = new ControlCardBlank(trans, controlCardID, UserName);
                cards.Add(card);
            }

            return cards;
        }

        #endregion

        #region Static Public Methods
        public new static List<ControlCardBlank> GetExecutiveCards(SqlTransaction trans, int documentID, int departmentId, string userName)
        {
            List<ControlCardBlank> cards = new List<ControlCardBlank>();

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            prms[1].Value = departmentId;

            DataTable cTable = SPHelper.ExecuteDataset(trans, SpNames.GetExecutiveCards, prms).Tables[0];

            foreach (DataRow row in cTable.Rows)
            {
                int controlCardID = (int)row["ControlCardID"];
                ControlCardBlank card = new ControlCardBlank(trans, controlCardID, userName);
                cards.Add(card);
            }

            return cards;
        }

        public new static List<ControlCardBlank> GetWorkerCards(SqlTransaction trans, int documentID, int workerId, string userName)
        {
            List<ControlCardBlank> cards = new List<ControlCardBlank>();

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            prms[0].Value = documentID;

            prms[1] = new SqlParameter("@WorkerID", SqlDbType.Int);
            prms[1].Value = workerId;

            DataTable cTable = SPHelper.ExecuteDataset(trans, SpNames.GetWorkerCards, prms).Tables[0];

            foreach (DataRow row in cTable.Rows)
            {
                int controlCardID = (int)row["ControlCardID"];
                ControlCardBlank card = new ControlCardBlank(trans, controlCardID, userName);
                cards.Add(card);
            }

            return cards;
        }

        public new static ControlCardBlank GetLastCard(SqlTransaction trans, int documentID, int departmentId, string userName)
        {
            SqlParameter[] arParams = new SqlParameter[3];
            arParams[0] = new SqlParameter("@DocumentID", SqlDbType.Int);
            arParams[0].Value = documentID;

            arParams[1] = new SqlParameter("@DepartmentID", SqlDbType.Int);
            arParams[1].Value = departmentId;

            arParams[2] = new SqlParameter("@ControlCardID", SqlDbType.Int);
            arParams[2].Direction = ParameterDirection.Output;

            SPHelper.ExecuteNonQuery(trans, "usp_ControlCard_GetLast", arParams);

            int controlCardID = 0;
            if (arParams[2].Value != DBNull.Value)
                controlCardID = (int)arParams[2].Value;

            if (controlCardID <= 0)
                return null;

            return new ControlCardBlank(trans, controlCardID, userName);
        }

        public new static JqGridResults BuildJqGridResults(DataTable dataTable)
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

                row.cell[3] = ((DateTime) dr["StartDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);
                row.cell[4] = ((DateTime) dr["EndDate"]).ToString("dd.MM.yyyy", CultureInfo.CurrentCulture);

                row.cell[5] = (string) dr["CardStatusName"];
                row.cell[6] = FormatHelper.FormatToLastNameAndInitials((string) dr["WorkerLastName"],
                                                                       (string) dr["WorkerFirstName"],
                                                                       (string) dr["WorkerMiddleName"]);

                row.cell[7] = (string) dr["ControlResponse"];
                row.cell[8] = (string) dr["Resolution"];
                row.cell[9] = "завантажити";

                if (dr["GroupID"] == DBNull.Value) {
                    row.cell[10] = "0";
                } else {
                    row.cell[10] = dr["GroupID"].ToString();
                }
                row.cell[11] = dr["InnerNumber"] == DBNull.Value ? String.Empty : (string) dr["InnerNumber"];
                row.cell[12] = dr["ActionCommentID"].ToString();

                rows.Add(row);
            }
            result.rows = rows.ToArray();
            result.page = 1;
            result.total = dataTable.Rows.Count;
            result.records = dataTable.Rows.Count;

            return result;
        }

        #endregion
    }
}
