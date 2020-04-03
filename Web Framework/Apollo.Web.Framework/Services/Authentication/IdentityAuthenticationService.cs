using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Apollo.Web.Framework.Services.Authentication
{
    public class IdentityAuthenticationService : IAuthenticationService, IIdentityExternalAuthService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IAccountService _accountService;
        private Account _cachedAccount;

        public IdentityAuthenticationService(HttpContextBase httpContext, IAccountService accountService)
        {
            _httpContext = httpContext;
            _accountService = accountService;
        }

        public Account GetAuthenticatedAccount()
        {
            if (_cachedAccount != null)
                return _cachedAccount;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is ClaimsIdentity))
            {
                return null;
            }

            var claimsIdentity = (ClaimsIdentity)_httpContext.User.Identity;
            var customer = GetAuthenticatedCustomerFromClaims(claimsIdentity);
            
            if (customer != null && customer.IsApproved && !customer.IsLockedOut)
                _cachedAccount = customer;

            return _cachedAccount;
        }

        public Account GetAuthenticatedCustomerFromClaims(ClaimsIdentity claims)
        {
            if (claims == null) throw new ArgumentNullException("claims");

            var usernameOrEmail = claims.Name;

            if (string.IsNullOrWhiteSpace(usernameOrEmail)) return null;

            var account = _accountService.GetAccountByUsername(usernameOrEmail);

            return account;
        }
        
        public CustomerLoginResults SignIn(string username, string password, bool isPersistent, bool shouldLockOut)
        {
            var identityLoginResult = _accountService.ValidateIdentityUser(username, password, shouldLockOut);

            if (identityLoginResult.CustomerLoginResults == CustomerLoginResults.Successful)
            {
                var authenticationManager = _httpContext.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identityLoginResult.ClaimsIdentity);
            }
            
            return identityLoginResult.CustomerLoginResults;
        }

        public CustomerLoginResults ExternalSignIn(string email, string loginProvider, string providerKey, bool isPersistent)
        {
            var identityLoginResult = _accountService.ValidateIdentityUser(email, loginProvider, providerKey);
            
            if (identityLoginResult.CustomerLoginResults == CustomerLoginResults.Successful)
            {
                var authenticationManager = _httpContext.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identityLoginResult.ClaimsIdentity);
            }
            
            return identityLoginResult.CustomerLoginResults;
        }

        public async Task<CustomerLoginResults> SignInAsync(string username, string password, bool isPersistent, bool shouldLockOut)
        {
            return await Task.FromResult(SignIn(username, password, isPersistent, shouldLockOut));
        }

        public async Task<CustomerLoginResults> ExternalSignInAsync(string email, string loginProvider, string providerKey, bool isPersistent)
        {
            return await Task.FromResult(ExternalSignIn(email, loginProvider, providerKey, isPersistent));
        }

        public async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            return await Task.FromResult(ChangePassword(username, oldPassword, newPassword));
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (_accountService.ChangePassword(username, oldPassword, newPassword))
            {
                var result = SignIn(username, newPassword, isPersistent: false, shouldLockOut: true);

                return result == CustomerLoginResults.Successful;
            }

            return false;
        }

        public async Task<bool> SetPasswordAsync(string username, string password)
        {
            return await Task.FromResult(SetPassword(username, password));
        }

        public bool SetPassword(string username, string password)
        {
            if (_accountService.SetPassword(username, password))
            {
                var result = SignIn(username, password, isPersistent: false, shouldLockOut: true);

                return result == CustomerLoginResults.Successful;
            }

            return false;
        }

        public IDictionary<string, string> GetLogins(string username)
        {
            return _accountService.GetLogins(username);
        }

        public async Task<IDictionary<string, string>> GetLoginsAsync(string username)
        {
            return await Task.FromResult(GetLogins(username));
        }

        public bool RemoveLogin(string username, string loginProvider, string providerKey)
        {
            var claimsIdentity = _accountService.RemoveLoginAndReturnIdentity(username, loginProvider, providerKey);

            if (claimsIdentity != null)
            {
                var authenticationManager = _httpContext.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, claimsIdentity);

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveLoginAsync(string username, string loginProvider, string providerKey)
        {
            return await Task.FromResult(RemoveLogin(username, loginProvider, providerKey));
        }

        public void SignOut()
        {
            var authenticationManager = _httpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}
