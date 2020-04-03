using Apollo.Web.Framework.Mvc;
using System;

namespace Apollo.FrontStore.Models.Blog
{
    public class BlogCommentModel : BaseEntityModel
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileAvatarUrl { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool AllowViewingProfiles { get; set; }
    }
}