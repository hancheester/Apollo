using Apollo.Core.Logging;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Blogs
{
    public class BlogService : BaseRepository, IBlogService
    {
        #region Fields

        private readonly IRepository<BlogPost> _blogPostRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public BlogService(
            IRepository<BlogPost> blogPostRepository,
            IRepository<BlogComment> blogCommentRepository,
            ILogBuilder logBuilder,
            IAccountService accountService)
        {
            _blogPostRepository = blogPostRepository;
            _blogCommentRepository = blogCommentRepository;
            _accountService = accountService;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion
        
        #region Create

        public int InsertBlogPost(BlogPost blogPost)
        {
            if (blogPost == null) throw new ArgumentNullException("blogPost");

            return _blogPostRepository.Create(blogPost);
        }

        public int InsertBlogComment(BlogComment blogComment)
        {
            if (blogComment == null) throw new ArgumentNullException("blogComment");
            return _blogCommentRepository.Create(blogComment);
        }

        #endregion

        #region Return

        public IList<BlogComment> GetAllComments(int profileId)
        {
            var query = _blogCommentRepository.Table
                .Where(x => x.ProfileId == 0 || x.ProfileId == profileId)
                .OrderBy(x => x.CreatedOnDate);

            var content = query.ToList();
            var profileName = _accountService.GetNameByProfileId(profileId);

            foreach (var item in content)
            {
                item.ProfileName = profileName;
            }

            return content;
        }

        public IList<BlogComment> GetBlogCommentsByBlogPostId(int blogPostId)
        {
            var query = _blogCommentRepository.Table
                .Where(x => x.BlogPostId == blogPostId)
                .OrderBy(x => x.CreatedOnDate);

            var content = query.ToList();

            foreach (var item in content)
            {
                item.ProfileName = _accountService.GetNameByProfileId(item.ProfileId);
            }

            return content;
        }

        public BlogComment GetBlogCommentById(int blogCommentId)
        {
            if (blogCommentId == 0) return null;
            return _blogCommentRepository.Return(blogCommentId);
        }

        public BlogPost GetBlogPostById(int blogPostId)
        {
            if (blogPostId == 0) return null;

            return _blogPostRepository.Return(blogPostId);
        }

        public BlogPost GetBlogPostByUrlKey(string urlKey)
        {
            if (string.IsNullOrEmpty(urlKey)) return null;

            return _blogPostRepository.Table.Where(x => x.UrlKey == urlKey).FirstOrDefault();
        }

        public PagedList<BlogPost> GetAllBlogPosts(
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false)
        {
            var query = _blogPostRepository.Table;

            if (dateFrom.HasValue) query = query.Where(b => dateFrom.Value <= b.CreatedOnDate);
            if (dateTo.HasValue) query = query.Where(b => dateTo.Value >= b.CreatedOnDate);

            if (!showHidden)
            {
                var now = DateTime.Now;
                query = query.Where(b => !b.StartDate.HasValue || b.StartDate <= now);
                query = query.Where(b => !b.EndDate.HasValue || b.EndDate >= now);
            }

            query = query.OrderByDescending(b => b.CreatedOnDate);

            var blogPosts = new PagedList<BlogPost>(query, pageIndex, pageSize);
            return blogPosts;
        }

        public PagedList<BlogComment> GetPagedBlogComments(
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var query = _blogCommentRepository.Table;
            
            query = query.OrderByDescending(b => b.CreatedOnDate);

            var blogComments = new PagedList<BlogComment>(query, pageIndex, pageSize);
            return blogComments;
        }

        public PagedList<BlogPost> GetAllBlogPostsByTag(
            string tag = "",
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false)
        {
            tag = tag.Trim();

            //we load all records and only then filter them by tag
            var blogPostsAll = GetAllBlogPosts(showHidden: showHidden);
            var taggedBlogPosts = new List<BlogPost>();

            foreach (var blogPost in blogPostsAll.Items)
            {
                var tags = blogPost.ParseTags();
                if (!string.IsNullOrEmpty(tags.FirstOrDefault(t => t.Equals(tag, StringComparison.InvariantCultureIgnoreCase))))
                    taggedBlogPosts.Add(blogPost);
            }

            //server-side paging
            var result = new PagedList<BlogPost>(taggedBlogPosts, pageIndex, pageSize);
            return result;
        }

        public IList<BlogPostTag> GetAllBlogPostTags(bool showHidden = false)
        {
            var blogPostTags = new List<BlogPostTag>();

            var blogPosts = GetAllBlogPosts(showHidden: showHidden);
            foreach (var blogPost in blogPosts.Items)
            {
                var tags = blogPost.ParseTags();
                foreach (string tag in tags)
                {
                    var foundBlogPostTag = blogPostTags.Find(bpt => bpt.Name.Equals(tag, StringComparison.InvariantCultureIgnoreCase));
                    if (foundBlogPostTag == null)
                    {
                        foundBlogPostTag = new BlogPostTag
                        {
                            Name = tag,
                            BlogPostCount = 1
                        };
                        blogPostTags.Add(foundBlogPostTag);
                    }
                    else
                        foundBlogPostTag.BlogPostCount++;
                }
            }

            return blogPostTags;
        }

        #endregion

        #region Update

        public void UpdateBlogPost(BlogPost blogPost)
        {
            if (blogPost == null) throw new ArgumentNullException("blogPost");
            _blogPostRepository.Update(blogPost);
        }

        #endregion

        #region Delete

        public void DeleteBlogPost(int blogPostId)
        {
            _blogPostRepository.Delete(blogPostId);
        }

        public void DeleteBlogComment(int blogCommentId)
        {
            _blogCommentRepository.Delete(blogCommentId);
        }

        #endregion
    }
}
