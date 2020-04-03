using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_featureditem_default : BasePage
    {
        public ICampaignService CampaignService { get; set; }
        public IProductService ProductService { get; set; }
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadItems();            
        }
        
        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            SetState(STATUS_CODE_FILTER, string.Empty);
            LoadItems();
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Product);

            if (result)
                enbNotice.Message = "All product related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(STATUS_CODE_FILTER, ((DropDownList)gvItems.HeaderRow.FindControl("ddlStatus")).SelectedValue);
            LoadItems();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvItems.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvItems.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvItems.CustomPageIndex = gotoIndex;

            LoadItems();
        }

        protected void gvItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvItems.CustomPageIndex = gvItems.CustomPageIndex + e.NewPageIndex;

            if (gvItems.CustomPageIndex < 0)
                gvItems.CustomPageIndex = 0;

            LoadItems();
        }

        protected void gvItems_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = FeaturedItemSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                default:
                    orderBy = FeaturedItemSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = FeaturedItemSortingType.IdAsc;
                    break;
                case "Priority":
                    orderBy = FeaturedItemSortingType.PriorityDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = FeaturedItemSortingType.PriorityAsc;
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadItems();
        }
        
        protected void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.DataItem != null)
            {
                var item = e.Row.DataItem as ProductGroupMapping;

                Literal ltlProductName = e.Row.FindControl("ltlProductName") as Literal;
                var product = ProductService.GetProductOverviewModelById(item.ProductId);
                if (product != null) ltlProductName.Text = product.Name;

                Literal ltlProductGroup = e.Row.FindControl("ltlProductGroup") as Literal;
                var group = CampaignService.GetProductGroupById(item.ProductGroupId);
                if (group != null) ltlProductGroup.Text = group.Name;
            }
        }

        private void LoadItems()
        {
            bool? enabled = null;
            FeaturedItemSortingType orderBy = FeaturedItemSortingType.IdAsc;

            if (HasState(STATUS_CODE_FILTER)) enabled = Convert.ToBoolean(GetStringState(STATUS_CODE_FILTER));
            if (HasState("OrderBy")) orderBy = (FeaturedItemSortingType)GetIntState("OrderBy");

            var result = CampaignService.GetFeaturedItemLoadPaged(
                pageIndex: gvItems.CustomPageIndex,
                pageSize: gvItems.PageSize,
                orderBy: orderBy);

            if (result != null)
            {
                gvItems.DataSource = result.Items;
                gvItems.RecordCount = result.TotalCount;
                gvItems.CustomPageCount = result.TotalPages;
            }

            gvItems.DataBind();

            if (gvItems.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}