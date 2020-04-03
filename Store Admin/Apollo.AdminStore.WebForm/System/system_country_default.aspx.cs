using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_country_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadItems();
        }
        
        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(COUNTRY_NAME_FILTER);
            DisposeState(ISO3166_CODE_FILTER);

            LoadItems();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(COUNTRY_NAME_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtCountryName")).Text.Trim());
            SetState(ISO3166_CODE_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtlnkISO3166Code")).Text.Trim());
            LoadItems();
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Country);

            if (result)
                enbNotice.Message = "All countries related data on store front has been refreshed successfully.";
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

        protected void gvItems_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = CountrySortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "ISO3166Code":
                    orderBy = CountrySortingType.CountryCodeDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = CountrySortingType.CountryCodeAsc;
                    break;
                case "Name":
                default:
                    orderBy = CountrySortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = CountrySortingType.NameAsc;
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadItems();
        }

        protected void gvItems_PreRender(object sender, EventArgs e)
        {
            if (gvItems.TopPagerRow != null)
            {
                gvItems.TopPagerRow.Visible = true;
                ((TextBox)gvItems.HeaderRow.FindControl("txtCountryName")).Text = GetStringState(COUNTRY_NAME_FILTER);
                ((TextBox)gvItems.HeaderRow.FindControl("txtlnkISO3166Code")).Text = GetStringState(ISO3166_CODE_FILTER);                
            }
        }

        private void LoadItems()
        {
            string name = null;
            string countryCode = null;
            var orderBy = CountrySortingType.IdAsc;

            if (HasState(ISO3166_CODE_FILTER)) countryCode = GetStringState(ISO3166_CODE_FILTER);
            if (HasState(COUNTRY_NAME_FILTER)) name = GetStringState(COUNTRY_NAME_FILTER);
            if (HasState("OrderBy")) orderBy = (CountrySortingType)GetIntState("OrderBy");

            var result = ShippingService.GetPagedCountry(
                pageIndex: gvItems.CustomPageIndex,
                pageSize: gvItems.PageSize,
                name: name,
                countryCode: countryCode,
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