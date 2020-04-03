using Apollo.Core.Model.Entity;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Apollo.Core.Services.Accounts.Identity
{
    public interface IIdentityUserManager
    {
        bool SupportsUserSecurityStamp { get; }

        bool UserExistsById(string userId);
        Task<bool> UserExistsByIdAsync(string userId);
        string GetSecurityStamp(string userId);
        ClaimsIdentity CreateIdentityById(string userId);
        ClaimsIdentity CreateIdentityByName(string username);
        ClaimsIdentity CreateIdentity(ApplicationUser user, string authenticationType);
        bool AddUserLogin(string userId, string loginProvider, string providerKey);
        bool AddPassword(string userId, string password);
        bool HasPassword(string userName);
        ApplicationUser Find(UserLoginInfo login);
        ApplicationUser FindByName(string userName);
        ApplicationUser FindByEmail(string email);
        bool IsLockedOut(string userId);
        bool CheckPassword(ApplicationUser user, string password);
        IdentityResult ResetAccessFailedCount(string userId);
        IdentityResult AccessFailed(string userId);
        WebMembershipUser CreateUserAndUserLogin(string userName, string email, string loginProvider, string providerKey);
        Task<WebMembershipUser> CreateUserAndUserLoginAsync(string userName, string email, string loginProvider, string providerKey);
        IdentityResult AddLogin(string email, string loginProvider, string providerKey);
        IList<UserLoginInfo> GetLogins(string userName);
        IdentityResult RemoveLogin(string userName, string loginProvider, string providerKey);
        IdentityResult RemovePassword(string userName);
    }
}
