using System;

namespace Apollo.Core.Model.Entity
{
    public class BlogComment : BaseEntity
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string CommentText { get; set; }
        public int BlogPostId { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
}
