using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_blog_post_info : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public IBlogService BlogService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadBlogPost();
        }

        protected void lbBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/marketing/cms_blog_post_default.aspx");
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var blogPost = BlogService.GetBlogPostById(QueryId);

            if (blogPost == null)
                Response.Redirect("/marketing/cms_blog_post_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.BlogPostNotFound);

            blogPost.Title = txtTitle.Text.Trim();
            blogPost.Body = txtBody.Text.Trim();
            blogPost.BodyOverview = txtBodyOverview.Text.Trim();
            blogPost.Tags = txtTags.Text.Trim();
            blogPost.AllowComments = cbAllowComments.Checked;
            blogPost.MetaKeywords = txtMetaKeywords.Text.Trim();
            blogPost.MetaDescription = txtMetaDescription.Text.Trim();
            blogPost.MetaTitle = txtMetaTitle.Text.Trim();
            blogPost.UrlKey = txtUrlKey.Text.Trim();
            
            if (!string.IsNullOrEmpty(txtDateFrom.Text.Trim()))
                blogPost.StartDate = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(txtDateTo.Text.Trim()))
                blogPost.EndDate = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);

            bool proceed = true;

            if (string.IsNullOrEmpty(blogPost.UrlKey))
            {
                blogPost.UrlKey = AdminStoreUtility.GetFriendlyUrlKey(blogPost.Title);
                txtUrlKey.Text = blogPost.UrlKey;

                var existingBlogPost = BlogService.GetBlogPostByUrlKey(blogPost.UrlKey);
                if (existingBlogPost != null)
                {
                    proceed = false;
                    enbNotice.Message = "Blog post was failed to save. Search engine friendly page name was not unique. Please try again with an unique name.";
                }
            }

            if (proceed)
            {
                BlogService.UpdateBlogPost(blogPost);
                enbNotice.Message = "Blog post was updated successfully.";
                LoadBlogPost();
            }
        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            BlogService.DeleteBlogPost(QueryId);
            Response.Redirect("/marketing/cms_blog_post_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.BlogPostDeleted);
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Blog);

            if (result)
                enbNotice.Message = "All blog related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";

        }
        private void LoadBlogPost()
        {
            var post = BlogService.GetBlogPostById(QueryId);

            if (post != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1})", post.Title, post.Id);
                txtTitle.Text = post.Title;
                txtBody.Text = post.Body;
                txtBodyOverview.Text = post.BodyOverview;
                txtTags.Text = post.Tags;
                cbAllowComments.Checked = post.AllowComments;

                if (post.StartDate.HasValue)
                    txtDateFrom.Text = post.StartDate.Value.ToString(AppConstant.DATE_FORM1);

                if (post.EndDate.HasValue)
                    txtDateTo.Text = post.EndDate.Value.ToString(AppConstant.DATE_FORM1);
                
                txtMetaKeywords.Text = post.MetaKeywords;
                txtMetaDescription.Text = post.MetaDescription;
                txtMetaTitle.Text = post.MetaTitle;
                txtUrlKey.Text = post.UrlKey;
            }
        }
    }
}