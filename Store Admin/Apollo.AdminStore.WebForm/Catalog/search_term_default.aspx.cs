using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class search_term_default : BasePage
    {
        public ICampaignService CampaignService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadItems();
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(SEARCH_TERM_ID_FILTER);
            DisposeState(QUERY_FILTER);
            DisposeState(REDIRECT_URL_FILTER);

            LoadItems();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(SEARCH_TERM_ID_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtFilterSearchTermId")).Text.Trim());
            SetState(QUERY_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtFilterQuery")).Text.Trim());
            SetState(REDIRECT_URL_FILTER, ((TextBox)gvItems.HeaderRow.FindControl("txtFilterRedirectUrl")).Text.Trim());

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
            var orderBy = SearchTermSortingBy.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                    orderBy = SearchTermSortingBy.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = SearchTermSortingBy.IdAsc;
                    break;
                case "Query":
                    orderBy = SearchTermSortingBy.QueryDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = SearchTermSortingBy.QueryAsc;
                    break;
                case "RedirectUrl":
                    orderBy = SearchTermSortingBy.RedirectUrlDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = SearchTermSortingBy.RedirectUrlAsc;
                    break;
                default:
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
                ((TextBox)gvItems.HeaderRow.FindControl("txtFilterSearchTermId")).Text = GetStringState(SEARCH_TERM_ID_FILTER);
                ((TextBox)gvItems.HeaderRow.FindControl("txtFilterQuery")).Text = GetStringState(QUERY_FILTER);
                ((TextBox)gvItems.HeaderRow.FindControl("txtFilterRedirectUrl")).Text = GetStringState(REDIRECT_URL_FILTER);
            }
        }

        private void LoadItems()
        {
            int[] searchTermIds = null;
            string query = null;
            string redirectUrl = null;
            SearchTermSortingBy orderBy = SearchTermSortingBy.IdAsc;

            if (HasState(USER_ID_FILTER))
            {
                string value = GetStringState(SEARCH_TERM_ID_FILTER);
                int temp;
                searchTermIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(QUERY_FILTER)) query = GetStringState(QUERY_FILTER);
            if (HasState(REDIRECT_URL_FILTER)) redirectUrl = GetStringState(REDIRECT_URL_FILTER);
            if (HasState("OrderBy")) orderBy = (SearchTermSortingBy)GetIntState("OrderBy");

            var result = CampaignService.GetSearchTermLoadPaged(
                pageIndex: gvItems.CustomPageIndex,
                pageSize: gvItems.PageSize,
                queryValue: query,
                redirectUrl: redirectUrl,
                searchTermsIds: searchTermIds,
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