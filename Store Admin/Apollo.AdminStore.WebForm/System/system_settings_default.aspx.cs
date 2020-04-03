using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadItems();
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState("KEY_FILTER");
            DisposeState("APPID_FILTER");
            LoadItems();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            //SetState("KEY_FILTER", ((TextBox)gvItems.HeaderRow.FindControl("txtFilterKey")).Text.Trim());
            //SetState("APPID_FILTER", ((TextBox)gvItems.HeaderRow.FindControl("txtFilterAppId")).Text.Trim());

            LoadItems();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            //int gotoIndex = Convert.ToInt32(((TextBox)gvItems.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            //if ((gvItems.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
            //    gvItems.CustomPageIndex = gotoIndex;

            LoadItems();
        }

        protected void gvItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //gvItems.CustomPageIndex = gvItems.CustomPageIndex + e.NewPageIndex;

            //if (gvItems.CustomPageIndex < 0)
            //    gvItems.CustomPageIndex = 0;

            LoadItems();
        }

        protected void gvItems_PreRender(object sender, EventArgs e)
        {
            //if (gvItems.TopPagerRow != null)
            //{
            //    gvItems.TopPagerRow.Visible = true;
            //    ((TextBox)gvItems.HeaderRow.FindControl("txtFilterKey")).Text = GetStringState("KEY_FILTER");
            //    ((TextBox)gvItems.HeaderRow.FindControl("txtFilterAppId")).Text = GetStringState("APPID_FILTER");
            //}
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            //AdminStoreUtility.LoadAppConfiguration();

            var result = UtilityService.RefreshCache(CacheEntityKey.Setting);

            if (result)
                enbNotice.Message = "All settings related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        private void LoadItems()
        {
            string key = null;
            int? appId = null;

            if (HasState("KEY_FILTER")) key = GetStringState("KEY_FILTER");
            if (HasState("APPID_FILTER")) appId = GetIntState("APPID_FILTER");

            //var result = UtilityAgent.GetPagedSettings(
            //    pageIndex: gvItems.CustomPageIndex,
            //    pageSize: gvItems.PageSize,
            //    key: key,
            //    appId: appId);

            //if (result != null)
            //{
            //    gvItems.DataSource = result.Items;
            //    gvItems.RecordCount = result.TotalCount;
            //    gvItems.CustomPageCount = result.TotalPages;
            //}

            //gvItems.DataBind();

            //if (gvItems.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}