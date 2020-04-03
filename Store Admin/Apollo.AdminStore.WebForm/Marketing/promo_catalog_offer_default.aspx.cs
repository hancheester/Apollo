using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class promo_catalog_offer_default : BasePage
    {
        public IOfferService OfferService { get; set; }
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)            
                LoadRules();
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(OFFER_RULE_ID_FILTER);
            DisposeState(NAME_FILTER);
            DisposeState(FROM_DATE_FILTER);
            DisposeState(TO_DATE_FILTER);
            DisposeState(IS_ACTIVE_FILTER);

            gvRules.CustomPageIndex = 0;

            LoadRules();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(OFFER_RULE_ID_FILTER, ((TextBox)gvRules.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvRules.HeaderRow.FindControl("txtRuleName")).Text.Trim());
            SetState(FROM_DATE_FILTER, ((TextBox)gvRules.HeaderRow.FindControl("txtFromDate")).Text.Trim());
            SetState(TO_DATE_FILTER, ((TextBox)gvRules.HeaderRow.FindControl("txtToDate")).Text.Trim());
            DropDownList ddl = (DropDownList)gvRules.HeaderRow.FindControl("ddlStatus");
            SetState(IS_ACTIVE_FILTER, ddl.SelectedIndex != 0 ? ddl.SelectedValue.ToString() : string.Empty);

            gvRules.CustomPageIndex = 0;

            LoadRules();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvRules.TopPagerRow.FindControl("txtPageIndex")).Text) - 1;

            if ((gvRules.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvRules.CustomPageIndex = gotoIndex;

            LoadRules();
        }

        protected void lbCleanBasket_Click(object sender, EventArgs e)
        {
            OfferService.RemoveOfferedItemsFromBaskets();
            enbNotice.Message = "All items which are related to offers were successfully to be removed from basket.";
        }

        protected void gvRules_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRules.CustomPageIndex = gvRules.CustomPageIndex + e.NewPageIndex;

            if (gvRules.CustomPageIndex < 0)
                gvRules.CustomPageIndex = 0;

            LoadRules();
        }

        protected void gvRules_Sorting(object sender, GridViewSortEventArgs e)
        {
            OfferRuleSortingType orderBy = OfferRuleSortingType.IdAsc;

            switch (e.SortExpression)
            {
                default:
                case "Id":
                    orderBy = OfferRuleSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OfferRuleSortingType.IdAsc;
                    break;
                case "Name":
                    orderBy = OfferRuleSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OfferRuleSortingType.NameAsc;
                    break;
                case "Priority":
                    orderBy = OfferRuleSortingType.PriorityDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OfferRuleSortingType.PriorityAsc;
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadRules();
        }

        protected void gvRules_PreRender(object sender, EventArgs e)
        {
            if (gvRules.TopPagerRow != null)
            {
                gvRules.TopPagerRow.Visible = true;                
            }

            ((TextBox)gvRules.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(OFFER_RULE_ID_FILTER);
            ((TextBox)gvRules.HeaderRow.FindControl("txtRuleName")).Text = GetStringState(NAME_FILTER);
            ((TextBox)gvRules.HeaderRow.FindControl("txtFromDate")).Text = GetStringState(FROM_DATE_FILTER);
            ((TextBox)gvRules.HeaderRow.FindControl("txtToDate")).Text = GetStringState(TO_DATE_FILTER);

            DropDownList ddl = (DropDownList)gvRules.HeaderRow.FindControl("ddlStatus");
            if (GetStringState(IS_ACTIVE_FILTER) != string.Empty) ddl.Items.FindByValue(GetStringState(IS_ACTIVE_FILTER)).Selected = true;
        }

        protected string GetStatus(object objOfferRuleId)
        {
            int offerRuleId = Convert.ToInt32(objOfferRuleId);

            if (offerRuleId != 0)
            {
                var offer = OfferService.GetOfferRuleById(offerRuleId);

                if (offer.Condition == null)
                    return "No condition setup.";

                return "Valid.";
            }

            return string.Empty;
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Offer | CacheEntityKey.Category | CacheEntityKey.Brand | CacheEntityKey.Product);

            if (result)
                enbNotice.Message = "All offers and products related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        private void LoadRules()
        {
            int[] offerRuleIds = null;
            string name = null;
            string promocode = null;
            string fromDate = null;
            string toDate = null;
            bool? isActive = null;
            OfferRuleSortingType orderBy = OfferRuleSortingType.IdAsc;

            if (HasState(OFFER_RULE_ID_FILTER))
            {
                string value = GetStringState(OFFER_RULE_ID_FILTER);
                int temp;
                offerRuleIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }

            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);
            if (HasState(PROMO_CODE_FILTER)) promocode = GetStringState(PROMO_CODE_FILTER);
            if (HasState(FROM_DATE_FILTER)) fromDate = GetStringState(FROM_DATE_FILTER);
            if (HasState(TO_DATE_FILTER)) toDate = GetStringState(TO_DATE_FILTER);
            if (HasState(IS_ACTIVE_FILTER)) isActive = Convert.ToBoolean(GetStringState(IS_ACTIVE_FILTER));
            if (HasState("OrderBy")) orderBy = (OfferRuleSortingType)GetIntState("OrderBy");

            var result = OfferService.GetOfferRuleLoadPaged(
                pageIndex: gvRules.CustomPageIndex,
                pageSize: gvRules.PageSize,
                offerRuleIds: offerRuleIds,
                name: name,
                promocode: promocode,
                fromDate: fromDate,
                toDate: toDate,
                isActive: isActive,
                isCart: false,
                orderBy: orderBy);

            if (result != null)
            {
                gvRules.DataSource = result.Items;
                gvRules.RecordCount = result.TotalCount;
                gvRules.CustomPageCount = result.TotalPages;
            }

            gvRules.DataBind();

            if (gvRules.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}