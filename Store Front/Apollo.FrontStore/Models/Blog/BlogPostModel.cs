using Apollo.FrontStore.Validators.Blog;
using Apollo.Web.Framework.Mvc;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Blog
{
    [Validator(typeof(BlogPostValidator))]
    public class BlogPostModel : BaseEntityModel
    {
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string UrlKey { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public string BodyOverview { get; set; }
        public bool AllowComments { get; set; }
        public int NumberOfComments { get; set; }
        public DateTime CreatedOn { get; set; }

        public IList<string> Tags { get; set; }

        public IList<BlogCommentModel> Comments { get; set; }

        [AllowHtml]
        [Display(Name = "Leave your comment")]
        public string CommentText { get; set; }
        public bool DisplayCaptcha { get; set; }
        
        public BlogPostModel()
        {
            Tags = new List<string>();
            Comments = new List<BlogCommentModel>();
        }        
    }
}