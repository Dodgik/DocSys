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
using BizObj.Data;
using BizObj.Document;
using BizObj.Models.Document;
using BizObj.Models.Helpers;
using BizObj.Models.JqGrid;
using BizObj.Models.Pager;
using ESCommon;
using ESCommon.Rtf;
using Newtonsoft.Json;
using Rule = BizObj.Models.Pager.Rule;

namespace BizObj.Controllers
{
    public class AdminScope
    {
        private readonly HttpContext _context;
        private string UserName { get; set; }//TODO: нужно заменить использование UserName на UserID
        private Guid UserId { get; set; }

        public AdminScope(HttpContext context, Guid userId, string userName)
        {
            _context = context;
            UserId = userId;
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
                    case "getpage":
                        int departmentId = Convert.ToInt32(r["departmentID"]);
                        
                        PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

                        DataTable dtPage = AdminDocumets.GetPage(gridSettings, UserId, departmentId);
                        JqGridResults jqGridResults = AdminDocumets.BuildJqGridResults(dtPage, gridSettings);

                        rResult = new JavaScriptSerializer().Serialize(jqGridResults);
                        break;
                    case "getreplaypage":
                        rResult = GetReplayPage(r);
                        break;
                    case "getpagedoc":
                        int depId = Convert.ToInt32(r["dep"]);
                        GetPageRtf(depId);
                        break;
                }
                if (!String.IsNullOrWhiteSpace(rResult))
                {
                    res.Write(rResult);
                }
            }
        }

        private string GetReplayPage(HttpRequest r)
        {
            int documentId = Convert.ToInt32(r["documentId"]);

            PageSettings gridSettings = JqGridSettings.GetPageSettings(_context);

            DataTable dtPage = AdminDocumets.GetReplayPage(gridSettings, UserId, documentId);

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

        public void GetPageRtf(int departmentId)
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

            foreach (var f in fields) {
                string data = request[f];
                if (!String.IsNullOrWhiteSpace(data)) {
                    filters.AddRule(new Rule { Field = f, Data = data, Op = "cn" });
                }
            }
            PageSettings gridSettings = new PageSettings {
                IsSearch = bool.Parse(request["_search"] ?? "false"),
                PageIndex = int.Parse(request["page"] ?? "1"),
                PageSize = int.Parse(request["rows"] ?? "50"),
                SortColumn = request["sidx"] ?? "",
                SortOrder = request["sord"] ?? "asc",
                Where = filters
            };

            DataTable dtPage = AdminDocumets.GetPage(gridSettings, UserId, departmentId);
            JqGridResults jqGridResults = AdminDocumets.BuildJqGridResults(dtPage, gridSettings);
            
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
            tbl.Columns[2].Width = TwipConverter.ToTwip(30, MetricUnit.Point);
            tbl.Columns[3].Width = TwipConverter.ToTwip(30, MetricUnit.Point);
            tbl.Columns[4].Width = TwipConverter.ToTwip(30, MetricUnit.Point);
            tbl.Columns[5].Width = TwipConverter.ToTwip(70, MetricUnit.Point);
            tbl.Columns[5].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));
            tbl.Columns[6].Width = TwipConverter.ToTwip(85, MetricUnit.Point);
            tbl.Columns[6].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(12, RtfTextAlign.Left));
            tbl.Columns[7].Width = TwipConverter.ToTwip(170, MetricUnit.Point);
            tbl.Columns[7].DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.All, new RtfParagraphFormatting(10, RtfTextAlign.Left));

            foreach (RtfTableRow row in tbl.Rows)
            {
                row.Height = TwipConverter.ToTwip(20, MetricUnit.Point);
            }
            tbl.Rows[0].Height = TwipConverter.ToTwip(30, MetricUnit.Point);
            //tbl.Rows[0].Cells[5].Formatting = new RtfParagraphFormatting(12, RtfTextAlign.Center);

            tbl[0, 0].AppendText("");
            tbl[1, 0].AppendText("Дата створення");
            tbl[2, 0].AppendText("Шифр");
            tbl[3, 0].AppendText("№");
            tbl[4, 0].AppendText("№ внутрішній");
            tbl[5, 0].AppendText("№ в організації");
            tbl[6, 0].AppendText("Організація");
            tbl[7, 0].AppendText("Зміст");
            int numRow = 1;

            foreach (JqGridRow dr in jqGridResults.rows)
            {
                tbl[0, numRow].AppendText(numRow.ToString());
                tbl[1, numRow].AppendText(dr.cell[2]);
                tbl[2, numRow].AppendText(dr.cell[16]);
                tbl[3, numRow].AppendText(dr.cell[3]);
                tbl[4, numRow].AppendText(dr.cell[23]);
                tbl[5, numRow].AppendText(dr.cell[4]);
                tbl[6, numRow].AppendText(dr.cell[19]);
                tbl[7, numRow].AppendText(dr.cell[5]);

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

            string fileName = String.Format("docTempates_({0}).rtf", DateTime.Now.ToString("yyyy.MM.dd"));

            _context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            _context.Response.ContentType = "application/rtf";
            _context.Response.Write(sb.ToString());
        }
    }
}
