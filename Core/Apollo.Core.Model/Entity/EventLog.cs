using System;

namespace Apollo.Core.Model.Entity
{
    public class EventLog : BaseEntity
    {
        public string Type { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateStamp { get; set; }
    }
}
