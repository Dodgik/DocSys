using System;
using System.Collections.Generic;

namespace BizObj.Models.Report
{
    [Serializable]
    public class StatisticsReport
    {
        public class TableForm
        {
            public List<int> Columns { get; set; }
            public List<List<string>> Rows { get; set; }

            public TableForm()
            {
                Columns = new List<int>();
                Rows = new List<List<string>>();
            }

            public TableForm(List<int> columns)
            {
                Columns = columns;
            }

            public TableForm(List<List<string>> rows)
            {
                Rows = rows;
            }

            public TableForm(List<int> columns, List<List<string>> rows)
            {
                Columns = columns;
                Rows = rows;
            }
        }

        public List<string> Header { get; set; }
        public List<string> Content { get; set; }
        public List<string> Footer { get; set; }

        public List<TableForm> Tables { get; set; }

        public StatisticsReport()
        {
            Header = new List<string>();
            Content = new List<string>();
            Footer = new List<string>();

            Tables = new List<TableForm>();
        }
    }
}