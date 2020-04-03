using System;

namespace Apollo.Core.Model.Entity
{
    public class Profile : BaseEntity
    {
        public string Username { get; set; }
        public bool IsAnonymous { get; set; }
        public string SystemName { get; set; }
        public bool IsSystemProfile { get; set; }
        public DateTime LastActivityDate { get; set; }
    }
}
