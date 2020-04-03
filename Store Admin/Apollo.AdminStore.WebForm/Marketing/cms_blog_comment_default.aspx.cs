using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_blog_comment_default : BasePage
    {
        public IBlogService BlogService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadBlogComments();
        }

        protected string GetBlogPostTitle(int blogPostId)
        {
            var post = BlogService.GetBlogPostById(blogPostId);
            if (post != null) return post.Title;
            return string.Empty;
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvBlogComments.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvBlogComments.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvBlogComments.CustomPageIndex = gotoIndex;

            LoadBlogComments();
        }

        protected void gvBlogComments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "remove":
                    var blogCommentId = Convert.ToInt32(e.CommandArgument);
                    BlogService.DeleteBlogComment(blogCommentId);
                    enbNotice.Message = "Blog comment was deleted successfully.";
                    break;
                default:
                    break;
            }

            LoadBlogComments();
        }

        protected void gvBlogComments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBlogComments.CustomPageIndex = gvBlogComments.CustomPageIndex + e.NewPageIndex;

            if (gvBlogComments.CustomPageIndex < 0)
                gvBlogComments.CustomPageIndex = 0;

            LoadBlogComments();
        }

        private void LoadBlogComments()
        {
            var result = BlogService.GetPagedBlogComments(
                pageIndex: gvBlogComments.CustomPageIndex,
                pageSize: gvBlogComments.PageSize);

            if (result != null)
            {
                gvBlogComments.DataSource = result.Items;
                gvBlogComments.RecordCount = result.TotalCount;
                gvBlogComments.CustomPageCount = result.TotalPages;
            }

            gvBlogComments.DataBind();

            if (gvBlogComments.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}