using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Accounts.Identity;
using System.Web.Security;

namespace Apollo.Core.Services.Accounts
{
    public static class MembershipExtensions
    {
        public static WebMembershipUser PrepareWebMembershipUser(this ApplicationUser applicationUser, bool isLockedOut)
        {
            return new WebMembershipUser
            {
                Id = applicationUser.Id,
                Username = applicationUser.UserName,
                Email = applicationUser.Email,
                IsApproved = applicationUser.IsApproved,
                IsLockedOut = isLockedOut,
                CreationDate = applicationUser.CreateDate,
                LastLoginDate = applicationUser.LastLoginDate,
            };
        }

        public static WebMembershipUser PrepareWebMembershipUser(this MembershipUser membership)
        {
            if (membership == null) return null;

            return new WebMembershipUser
            {
                Username = membership.UserName,
                Email = membership.Email,
                IsApproved = membership.IsApproved,
                IsLockedOut = membership.IsLockedOut,
                CreationDate = membership.CreationDate,
                LastLoginDate = membership.LastLoginDate                
            };
        }

        public static MembershipUser MergeWebMembershipUser(this MembershipUser membership, WebMembershipUser webMembership)
        {
            if (membership == null) return null;
            if (webMembership == null) return membership;

            membership.Email = webMembership.Email;
            membership.IsApproved = webMembership.IsApproved;
            
            if ((membership.IsLockedOut != webMembership.IsLockedOut) && webMembership.IsLockedOut == false)
                 membership.UnlockUser();

            return membership;
        }

        public static ApplicationUser PrepareApplicationUser(this WebMembershipUser webMembershipUser)
        {
            return new ApplicationUser
            {
                Id = webMembershipUser.Id,
                UserName = webMembershipUser.Username,
                Email = webMembershipUser.Email,
                IsApproved = webMembershipUser.IsApproved,                
                CreateDate = webMembershipUser.CreationDate,
                LastLoginDate = webMembershipUser.LastLoginDate,                
            };
        }
    }
}
