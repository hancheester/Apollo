using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_blog_post_new : BasePage
    {
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public IBlogService BlogService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lbBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("/marketing/cms_blog_post_default.aspx");
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var blogPost = new BlogPost
            {
                Title = txtTitle.Text.Trim(),
                Body = txtBody.Text.Trim(),
                BodyOverview = txtBodyOverview.Text.Trim(),
                Tags = txtTags.Text.Trim(),
                AllowComments = cbAllowComments.Checked,
                MetaKeywords = txtMetaKeywords.Text.Trim(),
                MetaDescription = txtMetaDescription.Text.Trim(),
                MetaTitle = txtMetaTitle.Text.Trim(),
                UrlKey = txtUrlKey.Text.Trim(),
                CreatedOnDate = DateTime.Now
            };

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
                    enbNotice.Message = "Blog post was failed to create. Search engine friendly page name was not unique. Please try again with an unique name.";
                }
            }

            if (proceed)
            {
                var id = BlogService.InsertBlogPost(blogPost);
                Response.Redirect("/marketing/cms_blog_post_info.aspx?id=" + id + "&" + QueryKey.MSG_TYPE + "=" + (int)MessageType.BlogPostCreated);
            }
        }

        protected void lbReset_Click(object sender, EventArgs e)
        {
            Response.Redirect("/marketing/cms_blog_post_new.aspx");
        }
    }
}