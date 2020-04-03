using Apollo.Core.Model.Entity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Apollo.Core.Services.Accounts.Identity
{
    public class IdentityMembership : IWebMembership, IIdentityUserManager
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;

        public bool SupportsUserSecurityStamp { get
            {
                return _userManager.SupportsUserSecurityStamp;
            }
        }

        public IdentityMembership(
            ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void AddUserToRole(string username, string roleName)
        {
            var user = _userManager.FindByName(username);
            _userManager.AddToRole(user.Id, roleName);
        }

        public async void AddUserToRoleAsync(string username, string roleName)
        {
            await Task.Run(() => AddUserToRole(username, roleName));
        }

        public void ChangePassword(string username, string newPassword)
        {
            var user = _userManager.FindByName(username);
            var result = _userManager.RemovePassword(user.Id);

            if (result.Succeeded)
            {
                _userManager.AddPassword(user.Id, newPassword);
            }
        }

        public async void ChangePasswordAsync(string username, string newPassword)
        {
            await Task.Run(() => ChangePassword(username, newPassword));
        }

        public WebMembershipUser CreateUser(string username, string password, string email)
        {
            return Task.Run(() => CreateUserAsync(username, password, email)).Result;
        }

        public async Task<WebMembershipUser> CreateUserAsync(string username, string password, string email)
        {
            var result = await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = username,
                Email = email,
                IsApproved = true,
                CreateDate = DateTime.Now,
                LastLoginDate = DateTime.Now
            }, password);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user.PrepareWebMembershipUser(false);
            }

            return null;
        }

        public WebMembershipUser CreateUser(string username, string email)
        {
            return Task.Run(() => CreateUserAsync(username, email)).Result;
        }

        public async Task<WebMembershipUser> CreateUserAsync(string username, string email)
        {
            var result = await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = username,
                Email = email,
                IsApproved = true,
                CreateDate = DateTime.UtcNow,
            });

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user.PrepareWebMembershipUser(false);
            }

            return null;
        }

        public string[] GetAllRoles()
        {
            return _roleManager.Roles.Select(x => x.Name).ToArray();
        }

        public string[] GetRolesForUser(string username)
        {
            var user = _userManager.FindByName(username);
            var roles = _userManager.GetRoles(user.Id);
            return roles.ToArray();
        }

        public async Task<string[]> GetRolesForUserAsync(string username)
        {
            return await Task.FromResult(GetRolesForUser(username));
        }

        public WebMembershipUser GetUser(string username)
        {
            var user = _userManager.FindByName(username);
            var isLockedOut = _userManager.IsLockedOut(user.Id);

            return user.PrepareWebMembershipUser(isLockedOut);
        }

        public async Task<WebMembershipUser> GetUserAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var isLockedOut = await _userManager.IsLockedOutAsync(user.Id);

            return user.PrepareWebMembershipUser(isLockedOut);
        }

        public bool UserExists(string username)
        {
            return Task.Run(() => UserExistsAsync(username)).Result;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user != null;
        }

        public bool UserExistsById(string userId)
        {
            return Task.Run(() => UserExistsByIdAsync(userId)).Result;
        }

        public async Task<bool> UserExistsByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null;
        }

        public string GetSecurityStamp(string userId)
        {
            return _userManager.GetSecurityStamp(userId);
        }

        public ClaimsIdentity CreateIdentityById(string userId)
        {
            var user = _userManager.FindById(userId);
            return _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public ClaimsIdentity CreateIdentityByName(string username)
        {
            var user = _userManager.FindByName(username);
            return _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public ClaimsIdentity CreateIdentity(ApplicationUser user, string authenticationType)
        {
            return _userManager.CreateIdentity(user, authenticationType);
        }

        public bool AddUserLogin(string userId, string loginProvider, string providerKey)
        {
            var login = new UserLoginInfo(loginProvider, providerKey);
            var result = _userManager.AddLogin(userId, login);
            return result.Succeeded;
        }

        public bool AddPassword(string userId, string password)
        {
            var result = _userManager.AddPassword(userId, password);
            return result.Succeeded;
        }

        public bool HasPassword(string userName)
        {
            var user = _userManager.FindByName(userName);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public ApplicationUser Find(UserLoginInfo login)
        {
            return _userManager.Find(login);
        }

        public ApplicationUser FindByName(string userName)
        {
            return _userManager.FindByName(userName);
        }

        public ApplicationUser FindByEmail(string email)
        {
            return _userManager.FindByEmail(email);
        }

        public bool CheckPassword(ApplicationUser user, string password)
        {
            return _userManager.CheckPassword(user, password);
        }

        public IdentityResult ResetAccessFailedCount(string userId)
        {
            return _userManager.ResetAccessFailedCount(userId);
        }

        public IdentityResult AccessFailed(string userId)
        {
            return _userManager.AccessFailed(userId);
        }

        public bool IsLockedOut(string userId)
        {
            return _userManager.IsLockedOut(userId);
        }

        public void RemoveUserFromRoles(string username, string[] roles)
        {
            var user = _userManager.FindByName(username);
            _userManager.RemoveFromRoles(user.Id, roles);
        }

        public async void RemoveUserFromRolesAsync(string username, string[] roles)
        {
            var user = await _userManager.FindByNameAsync(username);
            await _userManager.RemoveFromRolesAsync(user.Id, roles);
        }

        public string ResetPassword(string username)
        {
            var user = _userManager.FindByName(username);
            var result = _userManager.RemovePassword(user.Id);
            string newPassword = string.Empty;

            if (result.Succeeded)
            {
                string randomGuid = Guid.NewGuid().ToString("N");
                newPassword = randomGuid.Substring(0, 8);

                _userManager.AddPassword(user.Id, newPassword);
            }

            return newPassword;
        }

        public async Task<string> ResetPasswordAsync(string username)
        {
            return await Task.FromResult(ResetPassword(username));
        }

        public IdentityResult RemovePassword(string userName)
        {
            var user = _userManager.FindByName(userName);
            return _userManager.RemovePassword(user.Id);
        }

        public void UpdateUsername(string username, string newUsername)
        {
            var appUser = _userManager.FindByName(username);
            appUser.UserName = newUsername;
            appUser.Email = newUsername;
            
            _userManager.Update(appUser);
        }

        public bool ValidateUser(string username, string password)
        {
            return Task.Run(() => ValidateUserAsync(username, password)).Result;
        }

        public async Task<bool> ValidateUserAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                return await _userManager.CheckPasswordAsync(user, password);
            }

            return false;
        }

        public WebMembershipUser CreateUserAndUserLogin(string userName, string email, string loginProvider, string providerKey)
        {
            return Task.Run(() => CreateUserAndUserLoginAsync(userName, email, loginProvider, providerKey)).Result;
        }

        public async Task<WebMembershipUser> CreateUserAndUserLoginAsync(string userName, string email, string loginProvider, string providerKey)
        {
            var result = await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = userName,
                Email = email,
                IsApproved = true,
                CreateDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow
            });

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                var login = new UserLoginInfo(loginProvider, providerKey);
                result = _userManager.AddLogin(user.Id, login);
                if (result.Succeeded) return user.PrepareWebMembershipUser(false);
            }

            return null;
        }

        public IdentityResult AddLogin(string email, string loginProvider, string providerKey)
        {
            var user = _userManager.FindByEmail(email);
            var login = new UserLoginInfo(loginProvider, providerKey);
            return _userManager.AddLogin(user.Id, login);
        }

        public IList<UserLoginInfo> GetLogins(string userName)
        {
            var user = _userManager.FindByName(userName);
            return _userManager.GetLogins(user.Id);
        }

        public IdentityResult RemoveLogin(string userName, string loginProvider, string providerKey)
        {
            var user = _userManager.FindByName(userName);
            return _userManager.RemoveLogin(user.Id, new UserLoginInfo(loginProvider, providerKey));
        }

        public void UnlockUser(string username)
        {
            var user = _userManager.FindByName(username);
            _userManager.SetLockoutEndDate(user.Id, new DateTimeOffset(DateTime.UtcNow.AddDays(-1D)));
        }

        public void DisapproveUser(string username)
        {
            var user = _userManager.FindByName(username);
            user.IsApproved = false;

            _userManager.Update(user);
        }

        public void ApproveUser(string username)
        {
            var user = _userManager.FindByName(username);
            user.IsApproved = true;

            _userManager.Update(user);
        }
    }
}
