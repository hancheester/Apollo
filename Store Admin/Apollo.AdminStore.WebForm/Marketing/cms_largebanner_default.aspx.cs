using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_largebanner_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ICampaignService CampaignService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadBanners();
        }
        
        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(TITLE_FILTER);
            DisposeState(FROM_DATE_FILTER);
            DisposeState(TO_DATE_FILTER);
            SetState(STATUS_CODE_FILTER, string.Empty);

            LoadBanners();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(TITLE_FILTER, ((TextBox)gvBanners.HeaderRow.FindControl("txtFilterTitle")).Text.Trim());
            SetState(STATUS_CODE_FILTER, ((DropDownList)gvBanners.HeaderRow.FindControl("ddlStatus")).SelectedValue);
            SetState(FROM_DATE_FILTER, ((TextBox)gvBanners.HeaderRow.FindControl("txtFromDate")).Text.Trim());
            SetState(TO_DATE_FILTER, ((TextBox)gvBanners.HeaderRow.FindControl("txtToDate")).Text.Trim());

            LoadBanners();
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.LargeBanner);

            if (result)
                enbNotice.Message = "All banners related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvBanners.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvBanners.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvBanners.CustomPageIndex = gotoIndex;

            LoadBanners();
        }

        protected void gvBanners_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBanners.CustomPageIndex = gvBanners.CustomPageIndex + e.NewPageIndex;

            if (gvBanners.CustomPageIndex < 0)
                gvBanners.CustomPageIndex = 0;

            LoadBanners();
        }

        protected void gvBanners_Sorting(object sender, GridViewSortEventArgs e)
        {
            BannerSortingType orderBy = BannerSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Priority":
                    orderBy = BannerSortingType.PriorityDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = BannerSortingType.PriorityAsc;
                    break;
                case "Title":
                    orderBy = BannerSortingType.TitleDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = BannerSortingType.TitleAsc;
                    break;
                case "Id":
                default:
                    orderBy = BannerSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = BannerSortingType.IdAsc;
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadBanners();
        }

        protected void gvBanners_PreRender(object sender, EventArgs e)
        {
            if (gvBanners.TopPagerRow != null)
            {
                gvBanners.TopPagerRow.Visible = true;
                ((TextBox)gvBanners.HeaderRow.FindControl("txtFilterTitle")).Text = GetStringState(TITLE_FILTER);
                ((TextBox)gvBanners.HeaderRow.FindControl("txtFromDate")).Text = GetStringState(FROM_DATE_FILTER);
                ((TextBox)gvBanners.HeaderRow.FindControl("txtToDate")).Text = GetStringState(TO_DATE_FILTER);
                ((DropDownList)gvBanners.HeaderRow.FindControl("ddlStatus")).Items.FindByValue(GetStringState(STATUS_CODE_FILTER)).Selected = true;
            }
        }

        private void LoadBanners()
        {
            string title = null;
            bool? enabled = null;
            string fromDate = null;
            string toDate = null;
            BannerSortingType orderBy = BannerSortingType.IdAsc;
            
            if (HasState(TITLE_FILTER)) title = GetStringState(TITLE_FILTER);
            if (HasState(STATUS_CODE_FILTER)) enabled = Convert.ToBoolean(GetStringState(STATUS_CODE_FILTER));
            if (HasState(FROM_DATE_FILTER)) fromDate = GetStringState(FROM_DATE_FILTER);
            if (HasState(TO_DATE_FILTER)) toDate = GetStringState(TO_DATE_FILTER);
            if (HasState("OrderBy")) orderBy = (BannerSortingType)GetIntState("OrderBy");

            var result = CampaignService.GetLargeBannerLoadPaged(
                pageIndex: gvBanners.CustomPageIndex,
                pageSize: gvBanners.PageSize,
                title: title,
                enabled: enabled,
                fromDate: fromDate,
                toDate: toDate,
                orderBy: orderBy);

            if (result != null)
            {
                gvBanners.DataSource = result.Items;
                gvBanners.RecordCount = result.TotalCount;
                gvBanners.CustomPageCount = result.TotalPages;
            }

            gvBanners.DataBind();

            if (gvBanners.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}