using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Text;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_currency_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public IShippingService ShippingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadItems();
        }
        
        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(CURRENCY_CODE_FILTER);            
            LoadItems();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(CURRENCY_CODE_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtFilterCurrencyCode")).Text.Trim());
            
            LoadItems();
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Currency);

            if (result)
                enbNotice.Message = "All currencies related data on store front has been refreshed successfully.";
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
        
        protected void gvItems_PreRender(object sender, EventArgs e)
        {
            if (gvItems.TopPagerRow != null)
            {
                gvItems.TopPagerRow.Visible = true;
                ((TextBox)gvItems.HeaderRow.FindControl("txtFilterCurrencyCode")).Text = GetStringState(CURRENCY_CODE_FILTER);                
            }
        }

        protected string GetShippingCountryImage(int currencyId)
        {
            const string FLAG_HTML_FORMAT = "<span class='flag-icon flag-icon-{0}' alt='{1}' title='{1}'></span> ";
            var sb = new StringBuilder();
            var list = UtilityService.GetCurrencyCountryByCurrencyId(currencyId);

            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    var country = ShippingService.GetCountryById(item.CountryId);

                    if (country != null)
                        sb.AppendFormat(FLAG_HTML_FORMAT, country.ISO3166Code.ToLower(), country.Name);
                }
            }

            return sb.ToString();
        }

        private void LoadItems()
        {
            string currencyCode = null;

            if (HasState(CURRENCY_CODE_FILTER)) currencyCode = GetStringState(CURRENCY_CODE_FILTER);

            var result = UtilityService.GetPagedCurrency(
                pageIndex: gvItems.CustomPageIndex,
                pageSize: gvItems.PageSize,
                currencyCode: currencyCode);

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