using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.Catalog
{
    public partial class review_info : BasePage
    {
        public IProductService ProductService { get; set; }
        public IAccountService AccountService { get; set; }
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadReviewInfo();
        }

        private void LoadReviewInfo()
        {
            var review = ProductService.GetProductReview(QueryProductReviewId);

            if (review == null)
                Response.Redirect("/catalog/review_all.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ProductReviewNotFound);

            ltlTitle.Text = string.Format("Edit Review '{0}' (ID: {1})", Server.HtmlDecode(review.Title), QueryProductReviewId);
            ltlProduct.Text = string.Format("{1} <a href='/catalog/product_info.aspx?productid={0}' target='_blank'><i class='fa fa-external-link' aria-hidden='true'></i></a>", review.ProductId.ToString(), review.ProductName);

            var account = AccountService.GetAccountByProfileId(review.ProfileId);
            if (account != null)
                ltlPostedBy.Text = account.Name;
            else
                ltlPostedBy.Text = string.Empty;
            
            ddlScore.SelectedIndex = -1;
            ddlScore.Items.FindByValue(review.Score.ToString()).Selected = true;

            cbStatus.Checked = review.Approved;
            txtAlias.Text = review.Alias;
            txtTitle.Text = review.Title;
            txtComment.Text = Server.HtmlDecode(review.Comment);            
        }

        protected void lbReset_Click(object sender, EventArgs e)
        {
            LoadReviewInfo();
        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            ProductService.DeleteProductReview(QueryProductReviewId);
            Response.Redirect("/catalog/reviews_all.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ProductReviewDeleted);
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var review = ProductService.GetProductReview(QueryProductReviewId);

            if (review == null)
                Response.Redirect("/catalog/reviews_all.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ProductReviewNotFound);

            review.Alias = Server.HtmlEncode(txtAlias.Text);
            review.Title = Server.HtmlEncode(txtTitle.Text);
            review.Comment = Server.HtmlEncode(txtComment.Text);
            review.Score = Convert.ToInt32(ddlScore.SelectedValue);
            review.Approved = cbStatus.Checked;

            ProductService.UpdateProductReview(review);

            UtilityService.RefreshCache(CacheEntityKey.Product | CacheEntityKey.Brand | CacheEntityKey.Category);

            enbInfo.Message = "Review was updated successfully.";
        }
    }
}