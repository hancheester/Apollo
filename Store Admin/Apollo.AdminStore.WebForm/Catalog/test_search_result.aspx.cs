using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class test_search_result : BasePage
    {
        public ISearchService SearchService { get; set; }
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                
                LoadProducts();
            }
        }

        protected void gvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResults.CustomPageIndex = gvResults.CustomPageIndex + e.NewPageIndex;

            if (gvResults.CustomPageIndex < 0)
                gvResults.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void gvResults_PreRender(object sender, EventArgs e)
        {
            if (gvResults.TopPagerRow != null)
            {
                gvResults.TopPagerRow.Visible = true;                
            }
            
            if (gvResults.Rows.Count == 1 && (int)gvResults.DataKeys[0].Value == 0 && gvResults.TopPagerRow != null)
            {
                gvResults.TopPagerRow.FindControl(PH_RECORD_FOUND).Visible = false;
                gvResults.TopPagerRow.FindControl(PH_RECORD_NOT_FOUND).Visible = true;
            }
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvResults.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvResults.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvResults.CustomPageIndex = gotoIndex;

            LoadProducts();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Product | CacheEntityKey.Brand);

            if (result)
                enbNotice.Message = "All product related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }

        private void LoadProducts()
        {
            var productId = 0;
            string keywords = null;
            string query = txtQuery.Text.Trim();
            string originalKeywords = query;
            if (int.TryParse(query, out productId) == false)
                keywords = query;

            if (!string.IsNullOrEmpty(keywords)) keywords = keywords.Trim();

            var result = SearchService.SearchProduct(
                pageIndex: gvResults.CustomPageIndex,
                pageSize: gvResults.PageSize,
                enabled: true,
                visibleIndividually: true,
                includeDiscontinuedButInStock: true,
                keywords: keywords,
                searchDescriptions: false,
                useFullTextSearch: true,
                fullTextMode: FulltextSearchMode.Or,
                applySearchAnalysis: true,
                applyKeywordSuggestion: true,
                orderBy: ProductSortingType.Position,
                displaySearchAnalysis: true);

            if (result != null)
            {
                gvResults.DataSource = result.Items;
                gvResults.RecordCount = result.TotalCount;
                gvResults.CustomPageCount = result.TotalPages;

                lbResult.Text = string.Empty;

                if (!string.IsNullOrEmpty(result.SuggestedKeywords))
                {
                    lbResult.Text = string.Format("Suggested keywords '{0}'", result.SuggestedKeywords.ToLower() != keywords.ToLower() ? result.SuggestedKeywords : string.Empty);
                }
                
                if (result.HasSearchTerm)
                {
                    lbResult.Text = string.Format("It is a predefined search term, redirecting visitor to {0}", result.SearchTerm.RedirectUrl);
                }                
            }

            gvResults.DataBind();

            if (gvResults.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}