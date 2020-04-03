using System;

namespace Apollo.Core.Model.Entity
{
    public class BlogPost : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string BodyOverview { get; set; }
        public bool AllowComments { get; set; }
        public int CommentCount { get; set; }
        public string Tags { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public bool LimitedToStores { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string UrlKey { get; set; }
        public int LanguageId { get; set; }

        public BlogPost()
        {
            LanguageId = 1; // Default language ID which is English
        }
    }
}
