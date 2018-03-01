namespace AdLib.Models.Pager
{
    public class PageSettings
    {
        public bool IsSearch { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

        public int TotalRecords { get; set; }

        public Filter Where { get; set; }
    }
}