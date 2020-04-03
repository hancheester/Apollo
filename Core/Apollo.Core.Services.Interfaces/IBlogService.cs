using Apollo.Core.Model.Entity;
using System;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface IBlogService
    {
        void DeleteBlogPost(int blogPostId);
        BlogPost GetBlogPostById(int blogPostId);
        BlogPost GetBlogPostByUrlKey(string urlKey);
        PagedList<BlogPost> GetAllBlogPosts(
            DateTime? dateFrom = null, 
            DateTime? dateTo = null,
            int pageIndex = 0, 
            int pageSize = int.MaxValue, 
            bool showHidden = false);
        PagedList<BlogPost> GetAllBlogPostsByTag(
            string tag = "",
            int pageIndex = 0, 
            int pageSize = int.MaxValue, 
            bool showHidden = false);
        IList<BlogPostTag> GetAllBlogPostTags(bool showHidden = false);
        int InsertBlogPost(BlogPost blogPost);
        void UpdateBlogPost(BlogPost blogPost);
        IList<BlogComment> GetAllComments(int profileId);
        IList<BlogComment> GetBlogCommentsByBlogPostId(int blogPostId);
        PagedList<BlogComment> GetPagedBlogComments(
            int pageIndex = 0,
            int pageSize = int.MaxValue);
        BlogComment GetBlogCommentById(int blogCommentId);
        void DeleteBlogComment(int blogCommentId);
        int InsertBlogComment(BlogComment blogComment);
    }
}
