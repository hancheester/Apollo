using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Apollo.Web.Framework.Services.Authentication
{
    public class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IAccountService _accountService;
        private readonly TimeSpan _expirationTimeSpan;

        private Account _cachedAccount;

        public FormsAuthenticationService(HttpContextBase httpContext, IAccountService accountService)
        {
            _httpContext = httpContext;
            _accountService = accountService;
            _expirationTimeSpan = FormsAuthentication.Timeout;
        }

        public CustomerLoginResults SignIn(string username, string password, bool isPersistent, bool shouldLockOut)
        {
            var loginResults = _accountService.ValidateUser(username, password);

            if (loginResults != CustomerLoginResults.Successful) return loginResults;

            var account = _accountService.GetAccountByUsername(username);

            if (account == null) return CustomerLoginResults.AccountNotExists;

            var now = DateTime.Now.ToLocalTime();
            
            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                account.Email,
                now,
                now.Add(_expirationTimeSpan),
                isPersistent,
                account.Email,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true
            };

            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            _httpContext.Response.Cookies.Add(cookie);
            _cachedAccount = account;

            return CustomerLoginResults.Successful;
        }

        public async Task<CustomerLoginResults> SignInAsync(string username, string password, bool isPersistent, bool shouldLockOut)
        {
            return await Task.FromResult(SignIn(username, password, isPersistent, shouldLockOut));
        }

        public void SignOut()
        {
            _cachedAccount = null;
            FormsAuthentication.SignOut();
        }

        public Account GetAuthenticatedAccount()
        {
            if (_cachedAccount != null)
                return _cachedAccount;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var customer = GetAuthenticatedCustomerFromTicket(formsIdentity.Ticket);

            if (customer != null && customer.IsApproved && !customer.IsLockedOut)
                _cachedAccount = customer;
            return _cachedAccount;
        }

        public Account GetAuthenticatedCustomerFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null) throw new ArgumentNullException("ticket");

            var usernameOrEmail = ticket.UserData;

            if (string.IsNullOrWhiteSpace(usernameOrEmail)) return null;

            var account = _accountService.GetAccountByUsername(usernameOrEmail);

            return account;
        }
    }
}
