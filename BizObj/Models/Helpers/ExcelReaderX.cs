using System;
using System.Data;

namespace BizObj.Models.Helpers
{
    public class ExcelReaderX : ExcelReader
    {


        public override void Read(string FileName)
        {
            DataTable SourceTabs = null;
            DataRow R = null;
            System.Data.OleDb.OleDbDataAdapter daSource = null;
            string TableName = null;
            DataTable T = null;

            _ExcelFile = FileName;

            // Opening connection
            var _with1 = _ExcelCon;
            //.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Password="""";User ID=Admin;Data Source=" & FileName & ";Mode=Share Deny None;Extended Properties=""Excel 14.0;HDR=YES;IMEX=1;"";Jet OLEDB:System database="""";Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=35;Jet OLEDB:Database Locking Mode=0;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False"
            if (_with1.State != ConnectionState.Closed)
                _with1.Close();
            _with1.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Password=\"\";User ID=Admin;Data Source=" + FileName + ";Mode=Share Deny None;Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;\";Jet OLEDB:System database=\"\";Jet OLEDB:Registry Path=\"\";Jet OLEDB:Database Password=\"\";Jet OLEDB:Engine Type=35;Jet OLEDB:Database Locking Mode=0;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password=\"\";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
            _with1.Open();
            if (_with1.State == ConnectionState.Closed)
            {
                throw new Exception(Strings.Replace(ERR_FILE_NOT_FOUND, "%File%", _ExcelFile));
                return;
            }

            try
            {
                // Retrieving list of worksheets from workbook in Table
                SourceTabs = _ExcelCon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] {
					null,
					null,
					null,
					"TABLE"
				});

                // Reading worksheets
                _WorkSheets.Clear();
                foreach (DataRow R_loopVariable in SourceTabs.Rows)
                {
                    R = R_loopVariable;
                    // Populate Collection
                    TableName = R["TABLE_NAME"];
                    if (Strings.Right(TableName, 1) == "$" || Strings.Right(TableName, 2) == "$'")
                    {
                        T = new DataTable(Strings.Replace(Strings.Replace(Strings.Replace(Strings.Replace(TableName, "'", ""), " ", ""), "#", ""), "$", ""));

                        // Read Table
                        daSource = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM [" + TableName + "]", _ExcelCon);
                        daSource.Fill(T);
                        _WorkSheets.Add(T);
                        daSource = null;
                    }
                }
            }
            catch (System.Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                if (_ExcelCon.State != ConnectionState.Closed)
                    _ExcelCon.Close();
            }
        }
    }
}
