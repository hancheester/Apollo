using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class custom_dict_default : BasePage
    {
        public ISearchService SearchService { get; set; }
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadItems();
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.CustomDictionary);

            if (result)
                enbNotice.Message = "All search related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
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

        private void LoadItems()
        {
            var result = SearchService.GetPagedCustomDictionary(
                pageIndex: gvItems.CustomPageIndex,
                pageSize: gvItems.PageSize);

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