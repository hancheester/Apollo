using System;

namespace Apollo.Core.Model.Entity
{
    public class OrderComment : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProfileId { get; set; }
        public string CommentText { get; set; }
        public DateTime DateStamp { get; set; }
        public string FullName { get; set; }

        public OrderComment()
        {
            DateStamp = DateTime.Now;
        }
    }
}
