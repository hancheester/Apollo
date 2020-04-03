using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace Apollo.Core.Services.Accounts.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
