using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_shipping_default : BasePage
    {
        public IShippingService ShippingService { get; set; }
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadItems();
        }
        
        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(DESCRIPTION_FILTER);
            DisposeState(SHIPPING_COUNTRY_FILTER);
            DisposeState(SHIPPING_NAME_FILTER);
            LoadItems();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(SHIPPING_NAME_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtShippingName")).Text.Trim());
            SetState(SHIPPING_COUNTRY_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtShippingCountry")).Text.Trim());
            SetState(DESCRIPTION_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtDescription")).Text.Trim());
            LoadItems();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvItems.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvItems.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvItems.CustomPageIndex = gotoIndex;

            LoadItems();
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.ShippingOption);

            if (result)
                enbNotice.Message = "All shipping options related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
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
            ShippingOptionSortingType orderBy = ShippingOptionSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Description":
                    orderBy = ShippingOptionSortingType.DescriptionDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ShippingOptionSortingType.DescriptionAsc;
                    break;

                case "Priority":
                    orderBy = ShippingOptionSortingType.PriorityDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ShippingOptionSortingType.PriorityAsc;
                    break;

                case "Name":
                    orderBy = ShippingOptionSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ShippingOptionSortingType.NameAsc;
                    break;

                default:
                    orderBy = ShippingOptionSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ShippingOptionSortingType.IdAsc;
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
                ((TextBox)gvItems.HeaderRow.FindControl("txtShippingName")).Text = GetStringState(SHIPPING_NAME_FILTER);
                ((TextBox)gvItems.HeaderRow.FindControl("txtShippingCountry")).Text = GetStringState(SHIPPING_COUNTRY_FILTER);
                ((TextBox)gvItems.HeaderRow.FindControl("txtDescription")).Text = GetStringState(DESCRIPTION_FILTER);
            }
        }

        protected string GetCountryImage(int countryId)
        {
            const string FLAG_HTML_FORMAT = "<span class='flag-icon flag-icon-{0}' alt='{1}' title='{1}'></span> {1}";
            var country = ShippingService.GetCountryById(countryId);

            if (country != null)            
                return string.Format(FLAG_HTML_FORMAT, country.ISO3166Code.ToLower(), country.Name);
            
            return string.Empty;
        }

        private void LoadItems()
        {
            string description = null;
            string name = null;
            string countryName = null;
            ShippingOptionSortingType orderBy = ShippingOptionSortingType.IdAsc;

            if (HasState(DESCRIPTION_FILTER)) description = GetStringState(DESCRIPTION_FILTER);
            if (HasState(SHIPPING_COUNTRY_FILTER)) countryName = GetStringState(SHIPPING_COUNTRY_FILTER);
            if (HasState(SHIPPING_NAME_FILTER)) name = GetStringState(SHIPPING_NAME_FILTER);
            if (HasState("OrderBy")) orderBy = (ShippingOptionSortingType)GetIntState("OrderBy");

            var result = ShippingService.GetPagedShippingOptionOverviewModel(
                pageIndex: gvItems.CustomPageIndex,
                pageSize: gvItems.PageSize,
                description: description,
                name: name,
                countryName: countryName,
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