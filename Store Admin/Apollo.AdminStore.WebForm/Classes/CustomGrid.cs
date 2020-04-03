using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class CustomGrid : GridView
    {
        private const string RECORD_COUNT = "recordcount";
        private const string CUSTOM_PAGE_COUNT = "custompagecount";
        private const string CUSTOM_PAGE_INDEX = "custompageindex";
        
        public int RecordCount
        {
            get { return Convert.ToInt32(ViewState[RECORD_COUNT] ?? 0); }
            set { ViewState[RECORD_COUNT] = value; }
        }

        public int CustomPageCount
        {
            get { return Convert.ToInt32(ViewState[CUSTOM_PAGE_COUNT] ?? 0); }
            set { ViewState[CUSTOM_PAGE_COUNT] = value; }
        }

        public int CustomPageIndex
        {
            get { return Convert.ToInt32(ViewState[CUSTOM_PAGE_INDEX] ?? 0); }
            set { ViewState[CUSTOM_PAGE_INDEX] = value; }
        }

        protected override void OnSorting(GridViewSortEventArgs e)
        {
            if (ViewState[e.SortExpression] == null || (ViewState["SortExpression"].ToString() != e.SortExpression))
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState[e.SortExpression] = SortDirection.Descending;
            }
            else
            {
                ViewState[e.SortExpression] = (SortDirection)(1 - (int)ViewState[e.SortExpression]);
            }

            e.SortDirection = (SortDirection)ViewState[e.SortExpression];

            base.OnSorting(e);
        }

        protected override void InitializePager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
        {
            this.GridLines = GridLines.None;
            pagedDataSource.AllowPaging = this.AllowPaging;
            pagedDataSource.AllowCustomPaging = this.AllowPaging;            
            pagedDataSource.VirtualCount = this.RecordCount;
            pagedDataSource.CurrentPageIndex = this.CustomPageIndex;
            
            base.InitializePager(row, columnSpan, pagedDataSource);
        }       
    }
}