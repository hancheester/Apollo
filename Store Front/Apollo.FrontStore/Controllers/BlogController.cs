using Apollo.Core;
using Apollo.Core.Caching;
using Apollo.Core.Domain;
using Apollo.Core.Domain.Blogs;
using Apollo.Core.Domain.Captcha;
using Apollo.Core.Domain.Customers;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Models.Blog;
using Apollo.Web.Framework;
using Apollo.Web.Framework.Controllers;
using Apollo.Web.Framework.Security;
using Apollo.Web.Framework.Security.Captcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class BlogController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;        
        private readonly IBlogService _blogService;
        private readonly ICacheManager _cacheManager;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly BlogSettings _blogSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;

        public BlogController(
            IWorkContext workContext,
            IWebHelper webHelper,            
            IBlogService blogService,
            ICacheManager cacheManager,
            StoreInformationSettings storeInformationSettings,
            BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings)
        {
            _workContext = workContext;
            _blogService = blogService;
            _cacheManager = cacheManager;
            _webHelper = webHelper;
            _storeInformationSettings = storeInformationSettings;
            _blogSettings = blogSettings;
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
        }

        #region Methods

        public ActionResult List(BlogPagingFilteringModel command)
        {
            var model = PrepareBlogPostListModel(command);
            return View("List", model);
        }

        public ActionResult BlogByTag(BlogPagingFilteringModel command)
        {
            var model = PrepareBlogPostListModel(command);
            return View("List", model);
        }

        public ActionResult BlogByMonth(BlogPagingFilteringModel command)
        {
            var model = PrepareBlogPostListModel(command);
            return View("List", model);
        }

        public ActionResult ListRss()
        {
            var feed = new SyndicationFeed(
                                    string.Format("{0}: Blog", _storeInformationSettings.CompanyName),
                                    "Blog",
                                    new Uri(_webHelper.GetStoreLocation(false)),
                                    "BlogRSS",
                                    DateTime.Now);
            
            var items = new List<SyndicationItem>();
            var blogPosts = _blogService.GetAllBlogPosts();
            foreach (var blogPost in blogPosts.Items)
            {
                string blogPostUrl = Url.RouteUrl("Blog Post", new { postid = blogPost.Id }, "http");
                items.Add(new SyndicationItem(blogPost.Title, blogPost.Body, new Uri(blogPostUrl), String.Format("Blog:{0}", blogPost.Id), blogPost.CreatedOnDate));
            }
            feed.Items = items;
            return new RssActionResult { Feed = feed };
        }

        public ActionResult BlogPost(string urlkey)
        {
            var blogPost = _blogService.GetBlogPostByUrlKey(urlkey);

            if (blogPost == null ||
                (blogPost.StartDate.HasValue && blogPost.StartDate.Value >= DateTime.Now) ||
                (blogPost.EndDate.HasValue && blogPost.EndDate.Value <= DateTime.Now))
                return RedirectToRoute("Home");
            
            var model = new BlogPostModel();
            PrepareBlogPostModel(model, blogPost, true);

            return View(model);
        }

        [HttpPost, ActionName("BlogPost")]
        [FormValueRequired("add-comment")]
        [CaptchaValidator]
        public ActionResult AddBlogComment(string urlkey, BlogPostModel model, bool captchaValid)
        {
            if (!_blogSettings.Enabled) return RedirectToRoute("Home");

            var blogPost = _blogService.GetBlogPostByUrlKey(urlkey);
            if (blogPost == null || !blogPost.AllowComments)
                return RedirectToRoute("Home");

            if (_workContext.CurrentProfile.IsAnonymous && !_blogSettings.AllowNotRegisteredUsersToLeaveComments)
            {
                ModelState.AddModelError("", "Only registered users can leave comments.");
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage && !captchaValid)
            {
                ModelState.AddModelError("", "The characters didn't match the picture. Please try again.");
            }

            if (ModelState.IsValid)
            {
                var comment = new BlogComment
                {
                    BlogPostId = blogPost.Id,
                    ProfileId = _workContext.CurrentProfile.Id,
                    CommentText = model.CommentText,
                    CreatedOnDate = DateTime.Now,
                };
                _blogService.InsertBlogComment(comment);

                //notify a store owner
                //if (_blogSettings.NotifyAboutNewBlogComments)
                //    _workflowMessageService.SendBlogCommentNotificationMessage(comment, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                //_customerActivityService.InsertActivity("PublicStore.AddBlogComment", _localizationService.GetResource("ActivityLog.PublicStore.AddBlogComment"));

                //The text boxes should be cleared after a comment has been posted
                //That's why we reload the page
                ViewBag.Message = "Comment is successfully added.";
                return RedirectToRoute("Blog Post", new { urlkey = model.UrlKey });
            }

            //If we got this far, something failed, redisplay form
            PrepareBlogPostModel(model, blogPost, true);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult BlogTags()
        {
            if (!_blogSettings.Enabled) return Content("");

            var cacheKey = CacheKey.BLOG_TAGS_MODEL_KEY;
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new BlogPostTagListModel();

                //get tags
                var tags = _blogService.GetAllBlogPostTags()
                    .OrderByDescending(x => x.BlogPostCount)
                    .Take(_blogSettings.NumberOfTags)
                    .ToList();
                //sorting
                tags = tags.OrderBy(x => x.Name).ToList();

                foreach (var tag in tags)
                    model.Tags.Add(new BlogPostTagModel
                    {
                        Name = tag.Name,
                        BlogPostCount = tag.BlogPostCount
                    });
                return model;
            });

            return PartialView(cachedModel);
        }

        [ChildActionOnly]
        public ActionResult BlogMonths()
        {
            if (!_blogSettings.Enabled) return Content("");

            var cacheKey = CacheKey.BLOG_MONTHS_MODEL_KEY;
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new List<BlogPostYearModel>();

                var blogPosts = _blogService.GetAllBlogPosts();
                if (blogPosts.Items.Count > 0)
                {
                    var months = new SortedDictionary<DateTime, int>();

                    var first = blogPosts.Items[blogPosts.Items.Count - 1].CreatedOnDate;
                    while (DateTime.SpecifyKind(first, DateTimeKind.Unspecified) <= DateTime.Now.AddMonths(1))
                    {
                        var list = blogPosts.Items.GetPostsByDate(new DateTime(first.Year, first.Month, 1), new DateTime(first.Year, first.Month, 1).AddMonths(1).AddSeconds(-1));
                        if (list.Count > 0)
                        {
                            var date = new DateTime(first.Year, first.Month, 1);
                            months.Add(date, list.Count);
                        }

                        first = first.AddMonths(1);
                    }


                    int current = 0;
                    foreach (var kvp in months)
                    {
                        var date = kvp.Key;
                        var blogPostCount = kvp.Value;
                        if (current == 0)
                            current = date.Year;

                        if (date.Year > current || model.Count == 0)
                        {
                            var yearModel = new BlogPostYearModel
                            {
                                Year = date.Year
                            };
                            model.Add(yearModel);
                        }

                        model.Last().Months.Add(new BlogPostMonthModel
                        {
                            Month = date.Month,
                            BlogPostCount = blogPostCount
                        });

                        current = date.Year;
                    }
                }
                return model;
            });
            return PartialView(cachedModel);
        }

        #endregion

        #region Utilities

        [NonAction]
        protected void PrepareBlogPostModel(BlogPostModel model, BlogPost blogPost, bool prepareComments)
        {
            if (blogPost == null) throw new ArgumentNullException("blogPost");
            if (model == null) throw new ArgumentNullException("model");

            model.Id = blogPost.Id;
            model.MetaTitle = blogPost.MetaTitle;
            model.MetaDescription = blogPost.MetaDescription;
            model.MetaKeywords = blogPost.MetaKeywords;
            model.UrlKey = blogPost.UrlKey;
            model.Title = blogPost.Title;
            model.Body = blogPost.Body;
            model.BodyOverview = blogPost.BodyOverview;
            model.AllowComments = blogPost.AllowComments;
            model.CreatedOn = blogPost.CreatedOnDate;
            model.Tags = blogPost.ParseTags().ToList();            
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage;

            var blogComments = _blogService.GetBlogCommentsByBlogPostId(blogPost.Id);
            model.NumberOfComments = blogComments.Count;

            if (prepareComments)
            {                
                foreach (var bc in blogComments)
                {
                    var commentModel = new BlogCommentModel
                    {
                        Id = bc.Id,
                        ProfileId = bc.ProfileId,
                        ProfileName = bc.ProfileName,
                        CommentText = bc.CommentText,
                        CreatedOn = bc.CreatedOnDate,
                        //AllowViewingProfiles = _customerSettings.AllowViewingProfiles && !string.IsNullOrEmpty(bc.ProfileName),
                    };
                    //if (_customerSettings.AllowCustomersToUploadAvatars)
                    //{
                    //    commentModel.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                    //        bc.Customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                    //        _mediaSettings.AvatarPictureSize,
                    //        _customerSettings.DefaultAvatarEnabled,
                    //        defaultPictureType: PictureType.Avatar);
                    //}
                    model.Comments.Add(commentModel);
                }
            }
        }

        [NonAction]
        protected BlogPostListModel PrepareBlogPostListModel(BlogPagingFilteringModel command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var model = new BlogPostListModel();
            model.PagingFilteringContext.Tag = command.Tag;
            model.PagingFilteringContext.Month = command.Month;
            //model.WorkingLanguageId = _workContext.WorkingLanguage.Id;

            if (command.PageSize <= 0) command.PageSize = _blogSettings.PostsPageSize;            
            if (command.PageNumber <= 0) command.PageNumber = 1;

            DateTime? dateFrom = command.GetFromMonth();
            DateTime? dateTo = command.GetToMonth();

            PagedList<BlogPost> blogPosts;
            if (string.IsNullOrEmpty(command.Tag))
            {
                blogPosts = _blogService.GetAllBlogPosts(dateFrom, dateTo, command.PageNumber - 1, command.PageSize);
            }
            else
            {
                blogPosts = _blogService.GetAllBlogPostsByTag(command.Tag, command.PageNumber - 1, command.PageSize);
            }
            model.PagingFilteringContext.LoadPagedList(blogPosts);

            model.BlogPosts = blogPosts.Items
                .Select(x =>
                {
                    var blogPostModel = new BlogPostModel();
                    PrepareBlogPostModel(blogPostModel, x, false);
                    return blogPostModel;
                })
                .ToList();

            return model;
        }

        #endregion
    }
}