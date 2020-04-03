using System;

namespace Apollo.Core.Model.Entity
{
    public class EmailMessage : BaseEntity
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
    }
}
