using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class reviews_all : BasePage
    {
        public IProductService ProductService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadReviews();
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_REVIEW_ID_FILTER);
            DisposeState(ALIAS_FILTER);
            DisposeState(COMMENT_FILTER);

            LoadReviews();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_REVIEW_ID_FILTER, ((TextBox)gvReviews.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(ALIAS_FILTER, ((TextBox)gvReviews.HeaderRow.FindControl("txtFilterAlias")).Text.Trim());
            SetState(COMMENT_FILTER, ((TextBox)gvReviews.HeaderRow.FindControl("txtFilterComment")).Text.Trim());

            LoadReviews();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvReviews.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvReviews.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvReviews.CustomPageIndex = gotoIndex;

            LoadReviews();
        }

        protected void gvReviews_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReviews.CustomPageIndex = gvReviews.CustomPageIndex + e.NewPageIndex;

            if (gvReviews.CustomPageIndex < 0)
                gvReviews.CustomPageIndex = 0;

            LoadReviews();
        }

        protected void gvReviews_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = ProductReviewSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                    orderBy = ProductReviewSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductReviewSortingType.IdAsc;
                    break;
                case "TimeStamp":
                    orderBy = ProductReviewSortingType.TimeStampDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductReviewSortingType.TimeStampAsc;
                    break;
                case "Alias":
                    orderBy = ProductReviewSortingType.AliasDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductReviewSortingType.AliasAsc;
                    break;
                case "Product":
                default:
                    orderBy = ProductReviewSortingType.ProductNameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = ProductReviewSortingType.ProductNameAsc;
                    break;
            }
            SetState("OrderBy", (int)orderBy);
            LoadReviews();
        }

        protected void gvReviews_PreRender(object sender, EventArgs e)
        {
            if (gvReviews.TopPagerRow != null)
            {
                gvReviews.TopPagerRow.Visible = true;
                ((TextBox)gvReviews.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(PRODUCT_REVIEW_ID_FILTER);
                ((TextBox)gvReviews.HeaderRow.FindControl("txtFilterAlias")).Text = GetStringState(ALIAS_FILTER);
                ((TextBox)gvReviews.HeaderRow.FindControl("txtFilterComment")).Text = GetStringState(COMMENT_FILTER);                
            }            
        }

        private void LoadReviews()
        {
            int[] productReviewIds = null;
            string alias = null;
            string comment = null;
            ProductReviewSortingType orderBy = ProductReviewSortingType.IdDesc;

            if (HasState("OrderBy")) orderBy = (ProductReviewSortingType)GetIntState("OrderBy");
            if (HasState(PRODUCT_REVIEW_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_REVIEW_ID_FILTER);
                int temp;
                productReviewIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }
            if (HasState(ALIAS_FILTER)) alias = GetStringState(ALIAS_FILTER);
            if (HasState(COMMENT_FILTER)) comment = GetStringState(COMMENT_FILTER);

            var result = ProductService.GetProductReviewLoadPaged(
                pageIndex: gvReviews.CustomPageIndex,
                pageSize: gvReviews.PageSize,
                productReviewIds: productReviewIds,
                alias: alias,
                comment: comment,
                orderBy: orderBy);

            if (result != null)
            {
                gvReviews.DataSource = result.Items;
                gvReviews.RecordCount = result.TotalCount;
                gvReviews.CustomPageCount = result.TotalPages;
            }

            gvReviews.DataBind();

            if (gvReviews.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}