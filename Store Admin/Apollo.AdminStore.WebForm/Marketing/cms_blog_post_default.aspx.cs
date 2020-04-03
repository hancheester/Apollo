using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_blog_post_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public IBlogService BlogService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadBlogs();
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(FROM_DATE_FILTER);
            DisposeState(TO_DATE_FILTER);

            LoadBlogs();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(FROM_DATE_FILTER, ((TextBox)gvBlog.HeaderRow.FindControl("txtFromDate")).Text.Trim());
            SetState(TO_DATE_FILTER, ((TextBox)gvBlog.HeaderRow.FindControl("txtToDate")).Text.Trim());

            LoadBlogs();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvBlog.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvBlog.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvBlog.CustomPageIndex = gotoIndex;

            LoadBlogs();
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Blog);

            if (result)
                enbNotice.Message = "All blog related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        protected void gvBlog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBlog.CustomPageIndex = gvBlog.CustomPageIndex + e.NewPageIndex;

            if (gvBlog.CustomPageIndex < 0)
                gvBlog.CustomPageIndex = 0;

            LoadBlogs();
        }

        protected void gvBlog_PreRender(object sender, EventArgs e)
        {
            if (gvBlog.TopPagerRow != null)
            {
                gvBlog.TopPagerRow.Visible = true;
                ((TextBox)gvBlog.HeaderRow.FindControl("txtFromDate")).Text = GetStringState(FROM_DATE_FILTER);
                ((TextBox)gvBlog.HeaderRow.FindControl("txtToDate")).Text = GetStringState(TO_DATE_FILTER);
            }
        }

        private void LoadBlogs()
        {
            DateTime? dateFrom = null;
            DateTime? dateTo = null;

            string fromDate = null;
            string toDate = null;
            
            if (HasState(FROM_DATE_FILTER)) fromDate = GetStringState(FROM_DATE_FILTER);
            if (HasState(TO_DATE_FILTER)) toDate = GetStringState(TO_DATE_FILTER);

            DateTime startDate;
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime.TryParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                dateFrom = startDate;
            }
            
            DateTime endDate;
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime.TryParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                dateTo = endDate;
            }

            var result = BlogService.GetAllBlogPosts(
                pageIndex: gvBlog.CustomPageIndex,
                pageSize: gvBlog.PageSize,
                dateFrom: dateFrom,
                dateTo: dateTo,
                showHidden: true);

            if (result != null)
            {
                gvBlog.DataSource = result.Items;
                gvBlog.RecordCount = result.TotalCount;
                gvBlog.CustomPageCount = result.TotalPages;
            }

            gvBlog.DataBind();

            if (gvBlog.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}