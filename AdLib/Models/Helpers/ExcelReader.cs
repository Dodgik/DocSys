using System;
using System.Collections;
using System.Data;
using System.ComponentModel;

namespace BizObj.Models.Helpers
{
    public class DataTableCollection : CollectionBase, IBindingList
	{

		public bool AllowEdit {
			get { return true; }
		}

		public bool AllowNew {
			get { return true; }
		}

		public bool AllowRemove {
			get { return true; }
		}

		public bool SupportsChangeNotification {
			get { return true; }
		}

		public bool SupportsSearching {
			get { return false; }
		}

		public bool SupportsSorting {
			get { return false; }
		}

		// Events.

		public event System.ComponentModel.ListChangedEventHandler IBindingList.ListChanged;
		public delegate void ListChangedEventHandler(object sender, System.ComponentModel.ListChangedEventArgs e);

		// Methods.

		// Unsupported properties.
		public bool IsSorted {
			get {
				throw new NotSupportedException();
			}
		}

		public ListSortDirection SortDirection {
			get {
				throw new NotSupportedException();
			}
		}

		public PropertyDescriptor SortProperty {
			get {
				throw new NotSupportedException();
			}
		}

		// Unsupported Methods.
		public object AddNew()
		{
			throw new NotSupportedException();
		}
		//IBindingList.AddNew

		public void AddIndex(PropertyDescriptor prop)
		{
			throw new NotSupportedException();
		}
		//IBindingList.AddIndex

		public void ApplySort(PropertyDescriptor prop, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}
		//IBindingList.ApplySort

		public int Find(PropertyDescriptor prop, object key)
		{
			throw new NotSupportedException();
		}
		//IBindingList.Find

		public void RemoveIndex(PropertyDescriptor prop)
		{
			throw new NotSupportedException();
		}
		//IBindingList.RemoveIndex

		public void RemoveSort()
		{
			throw new NotSupportedException();
		}
		//IBindingList.RemoveSort

		public void Add(DataTable Table)
		{
			// Invokes Add method of the List object to add a widget.
			List.Add(Table);
			if (ListChanged != null) {
				ListChanged(this, new System.ComponentModel.ListChangedEventArgs(System.ComponentModel.ListChangedType.ItemAdded, List.Count));
			}
		}

		public new void Clear()
		{
			base.Clear();
			if (ListChanged != null) {
				ListChanged(this, new System.ComponentModel.ListChangedEventArgs(System.ComponentModel.ListChangedType.Reset, 0));
			}
		}

		public DataTable this[int index] {
			get { return (DataTable)List[index]; }
		}

		public DataTable this[string Name] {
			get {
				DataTable o = null;
				foreach (DataTable o_loopVariable in List) {
					o = o_loopVariable;
					if (o.TableName == Name) {
						return List[Name];
					}
				}
				throw new Exception("Name not found");
			}
		}
	}


	public class ExcelReader
	{
		protected System.Data.OleDb.OleDbConnection _ExcelCon;

		protected DataTableCollection _WorkSheets;

		protected string _ExcelFile;
		protected const string ERR_FILE_NOT_FOUND = "File %Filename% is not found or it is not a valid Excel file.";
		protected const string ERR_COLUMNS = "Worksheet does not contain following required column(s): ";

		protected const string ABORT_CHECK = "Check stopped";
		public System.Data.OleDb.OleDbConnection Connection {
			get { return _ExcelCon; }
		}

		public DataTableCollection Worksheets {
			get { return _WorkSheets; }
		}

		public string File {
			get { return _ExcelFile; }
		}

		public virtual void Read(string FileName)
		{
			DataTable SourceTabs = null;
			DataRow R = null;
			System.Data.OleDb.OleDbDataAdapter daSource = null;
			string TableName = null;
			DataTable T = null;

			_ExcelFile = FileName;

			// Opening connection
			var _with1 = _ExcelCon;
			if (_with1.State != ConnectionState.Closed)
				_with1.Close();
			_with1.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Password=\"\";User ID=Admin;Data Source=" + FileName + ";Mode=Share Deny None;Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;\";Jet OLEDB:System database=\"\";Jet OLEDB:Registry Path=\"\";Jet OLEDB:Database Password=\"\";Jet OLEDB:Engine Type=35;Jet OLEDB:Database Locking Mode=0;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password=\"\";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
			_with1.Open();
			if (_with1.State == ConnectionState.Closed) {
				throw new Exception(Strings.Replace(ERR_FILE_NOT_FOUND, "%File%", _ExcelFile));
				return;
			}

			try {
				// Retrieving list of worksheets from workbook in Table
				SourceTabs = _ExcelCon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] {
					null,
					null,
					null,
					"TABLE"
				});

				// Reading worksheets
				_WorkSheets.Clear();
				foreach (DataRow R_loopVariable in SourceTabs.Rows) {
					R = R_loopVariable;
					// Populate Collection
					TableName = R["TABLE_NAME"];
					if (Strings.Right(TableName, 1) == "$" || Strings.Right(TableName, 2) == "$'") {
						T = new DataTable(Strings.Replace(Strings.Replace(Strings.Replace(Strings.Replace(TableName, "'", ""), " ", ""), "#", ""), "$", ""));

						// Read Table
						daSource = new System.Data.OleDb.OleDbDataAdapter("SELECT * FROM [" + TableName + "]", _ExcelCon);
						daSource.Fill(T);
						_WorkSheets.Add(T);
						daSource = null;
					}
				}
			} catch (System.Exception Ex) {
				throw Ex;
			} finally {
				if (_ExcelCon.State != ConnectionState.Closed)
					_ExcelCon.Close();
			}
		}

		public ExcelReader()
		{
			_ExcelCon = new System.Data.OleDb.OleDbConnection();
			_WorkSheets = new DataTableCollection();
		}

		protected override void Finalize()
		{
			_ExcelCon = null;
			_WorkSheets = null;
		}
	}
}
