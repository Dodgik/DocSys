using System;
using System.Drawing;
using System.IO;
using System.Text;
using ESCommon;
using ESCommon.Rtf;

namespace WcfDocsService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "ReportService" в коде, SVC-файле и файле конфигурации.
    public class ReportService : IReportService
    {
        public string GetDocTControlledReport(int departmentID, DateTime currentDate, DateTime endDate)
        {
            /*
            DataSet ds;

            SqlConnection connection = new SqlConnection(ConstantCode.ConnectionString);
            try
            {
                connection.Open();
                SqlTransaction trans = null;
                try
                {
                    trans = connection.BeginTransaction();

                    ds = DocTemplate.GetControlled(trans, departmentID, currentDate, endDate);

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

            WorkerCardsReport wcReport = new WorkerCardsReport();

            wcReport.Header.Add("Список документів, які стоять на контролі за виконавцями");

            string[] cols = { "WorkerID", "WorkerFirstName", "WorkerMiddleName", "WorkerLastName" };

            DataTable workerTable = ds.Tables[0].DefaultView.ToTable(true, cols);
            foreach (DataRow worker in workerTable.Rows)
            {
                ds.Tables[0].DefaultView.RowFilter = String.Format("WorkerID = {0}", worker["WorkerID"]);

                WorkerCards wc = new WorkerCards();
                wc.Worker = FormatHelper.FormatToLastNameAndInitials((string)worker["WorkerLastName"],
                                                                     (string)worker["WorkerFirstName"],
                                                                     (string)worker["WorkerMiddleName"]);
                DataTable cardTable = ds.Tables[0].DefaultView.ToTable();
                foreach (DataRow card in cardTable.Rows)
                {
                    CardInfo cardInfo = new CardInfo();
                    cardInfo.Number = ReportHelper.EscapeStringForRtf((string)card["Number"]);
                    cardInfo.Content = ReportHelper.EscapeStringForRtf((string)card["Content"]);
                    cardInfo.CreationDate = ((DateTime)card["CreationDate"]).ToString("yyyy.MM.dd");
                    cardInfo.EndDate = ((DateTime)card["EndDate"]).ToString("yyyy.MM.dd");
                    cardInfo.Citizen = ReportHelper.EscapeStringForRtf(card["DocumentCodeID"].ToString());
                    cardInfo.Worker = FormatHelper.FormatToLastNameAndInitials((string)card["WorkerLastName"],
                                                                               (string)card["WorkerFirstName"],
                                                                               (string)card["WorkerMiddleName"]);
                    wc.Cards.Add(cardInfo);
                }

                wcReport.Statistic.Add(wc);
            }

            wcReport.Footer.Add(String.Format("Список станом на {0}", currentDate.ToString("dd.MM.yyyy")));
            wcReport.Footer.Add(String.Format("Всього документів: {0}", ds.Tables[0].Rows.Count));

            string templateUrl = String.Format("{0}\\ReportTemplates\\DocTControlledRTF.xslt", _context.Request.PhysicalApplicationPath);

            XmlDocument xmlDocument = XmlHelper.ToXmlDocument(wcReport);
            */
            //report = ReportHelper.BuildReport(xmlDocument, templateUrl);

            //report = ReportHelper.GetRtfUnicodeEscapedString(report);


            RtfDocument rtf = new RtfDocument();
            rtf.FontTable.Add(new RtfFont("Calibri"));
            rtf.FontTable.Add(new RtfFont("Constantia"));
            rtf.ColorTable.AddRange(new RtfColor[] {
                new RtfColor(Color.Red),
                new RtfColor(0, 0, 255)
            });

            RtfParagraphFormatting LeftAligned12 = new RtfParagraphFormatting(12, RtfTextAlign.Left);
            RtfParagraphFormatting Centered10 = new RtfParagraphFormatting(10, RtfTextAlign.Center);

            RtfFormattedParagraph header = new RtfFormattedParagraph(new RtfParagraphFormatting(16, RtfTextAlign.Center));
            RtfFormattedParagraph p1 = new RtfFormattedParagraph(new RtfParagraphFormatting(12, RtfTextAlign.Left));

            RtfTable t = new RtfTable(RtfTableAlign.Center, 2, 3);

            header.Formatting.SpaceAfter = TwipConverter.ToTwip(12F, MetricUnit.Point);
            header.AppendText("Calibri ");
            header.AppendText(new RtfFormattedText("Bold", RtfCharacterFormatting.Bold));

            t.Width = TwipConverter.ToTwip(5, MetricUnit.Centimeter);
            t.Columns[1].Width = TwipConverter.ToTwip(2, MetricUnit.Centimeter);

            foreach (RtfTableRow row in t.Rows)
            {
                row.Height = TwipConverter.ToTwip(2, MetricUnit.Centimeter);
            }

            t.MergeCellsVertically(1, 0, 2);

            t.DefaultCellStyle = new RtfTableCellStyle(RtfBorderSetting.None, Centered10);

            t[0, 0].Definition.Style = new RtfTableCellStyle(RtfBorderSetting.None, LeftAligned12, RtfTableCellVerticalAlign.Bottom);
            t[0, 0].AppendText("Bottom");

            t[1, 0].Definition.Style = new RtfTableCellStyle(RtfBorderSetting.Left, Centered10, RtfTableCellVerticalAlign.Center, RtfTableCellTextFlow.BottomToTopLeftToRight);
            t[1, 1].Definition.Style = t[1, 0].Definition.Style;
            t[1, 0].AppendText("Vertical");

            t[0, 1].Formatting = new RtfParagraphFormatting(10, RtfTextAlign.Center);
            t[0, 1].Formatting.TextColorIndex = 1;
            t[0, 1].AppendText(new RtfFormattedText("Black ", 0));
            t[0, 1].AppendText("Red ");
            t[0, 1].AppendText(new RtfFormattedText("Blue", 2));

            t[0, 2].AppendText("Normal");
            t[1, 2].AppendText(new RtfFormattedText("Italic", RtfCharacterFormatting.Caps | RtfCharacterFormatting.Italic));
            t[1, 2].AppendParagraph("+");
            t[1, 2].AppendParagraph(new RtfFormattedText("Caps", RtfCharacterFormatting.Caps | RtfCharacterFormatting.Italic));

            p1.Formatting.FontIndex = 1;
            p1.Formatting.IndentLeft = TwipConverter.ToTwip(6.05F, MetricUnit.Centimeter);
            p1.Formatting.SpaceBefore = TwipConverter.ToTwip(6F, MetricUnit.Point);
            p1.AppendText("Constantia ");
            p1.AppendText(new RtfFormattedText("Superscript", RtfCharacterFormatting.Superscript));
            p1.AppendParagraph(new RtfFormattedText("Inline", -1, 8));
            p1.AppendText(new RtfFormattedText(" font size ", -1, 14));
            p1.AppendText(new RtfFormattedText("change", -1, 8));


            RtfFormattedText linkText = new RtfFormattedText("View article", RtfCharacterFormatting.Underline, 2);
            linkText.BackgroundColorIndex = 1;
            p1.AppendParagraph(new RtfHyperlink("http://www.codeproject.com/KB/cs/RtfConstructor.aspx", linkText));

            RtfFormattedParagraph p2 = new RtfFormattedParagraph();
            p2.Formatting = new RtfParagraphFormatting(10);

            p2.Tabs.Add(new RtfTab(TwipConverter.ToTwip(2.5F, MetricUnit.Centimeter), RtfTabKind.Decimal));
            p2.Tabs.Add(new RtfTab(TwipConverter.ToTwip(5F, MetricUnit.Centimeter), RtfTabKind.FlushRight, RtfTabLead.Dots));
            p2.Tabs.Add(new RtfTab(TwipConverter.ToTwip(7.5F, MetricUnit.Centimeter)));
            p2.Tabs.Add(new RtfTab(TwipConverter.ToTwip(10F, MetricUnit.Centimeter), RtfTabKind.Centered));
            p2.Tabs.Add(new RtfTab(TwipConverter.ToTwip(15F, MetricUnit.Centimeter), RtfTabLead.Hyphens));

            p2.Tabs[2].Bar = true;

            p2.AppendText("One");
            p2.AppendText(new RtfTabCharacter());
            p2.AppendText("Two");
            p2.AppendText(new RtfTabCharacter());
            p2.AppendText("Three");
            p2.AppendText(new RtfTabCharacter());
            p2.AppendText("Five");
            p2.AppendText(new RtfTabCharacter());
            p2.AppendText("Six");

            rtf.Contents.AddRange(new RtfDocumentContentBase[] {
                header,
                t,
                p1,
                p2
            });

            
            RtfWriter rtfWriter = new RtfWriter();
            StringBuilder sb = new StringBuilder();
            try
            {
                using (TextWriter writer = new StringWriter(sb))
                {
                    rtfWriter.Write(writer, rtf);
                }
            }
            catch (IOException)
            {
                sb.Append("I/O Exception");
            }
            
            return sb.ToString();
        }
    }
}