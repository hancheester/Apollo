using System;

namespace Apollo.Core.Model.Entity
{
    public class Account : BaseEntity
    {
        public string Name { get; set; }        
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string DOB { get; set; }
        public string Note { get; set; }
        public string[] Roles { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastActvitityDate { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsApproved { get; set; }
        public bool IsSubscribed { get; set; }
        public string Username { get; set; }
        public int ProfileId { get; set; }
        public bool DisplayContactNumberInDespatch { get; set; }
        
        public Account()
        {
            Username = string.Empty;
        }
    }
}
