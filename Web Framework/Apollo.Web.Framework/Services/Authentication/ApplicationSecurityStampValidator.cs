using Apollo.Core.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Apollo.Web.Framework.Services.Authentication
{
    public static class ApplicationSecurityStampValidator
    {
        public static Func<CookieValidateIdentityContext, Task> OnValidateIdentity<TManager>(
            TimeSpan validateInterval, Func<TManager, string, Task<ClaimsIdentity>> regenerateIdentity, TManager manager)
            where TManager : IAccountService
        {
            return OnValidateIdentity(validateInterval, regenerateIdentity, id => id.GetUserId(), manager);
        }
        
        public static Func<CookieValidateIdentityContext, Task> OnValidateIdentity<TManager>(
            TimeSpan validateInterval, Func<TManager, string, Task<ClaimsIdentity>> regenerateIdentityCallback,
            Func<ClaimsIdentity, string> getUserIdCallback, TManager manager)
            where TManager: IAccountService
        {
            if (getUserIdCallback == null)
            {
                throw new ArgumentNullException("getUserIdCallback");
            }
            return async context =>
            {
                var currentUtc = DateTimeOffset.UtcNow;
                if (context.Options != null && context.Options.SystemClock != null)
                {
                    currentUtc = context.Options.SystemClock.UtcNow;
                }
                var issuedUtc = context.Properties.IssuedUtc;

                // Only validate if enough time has elapsed
                var validate = (issuedUtc == null);
                if (issuedUtc != null)
                {
                    var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                    validate = timeElapsed > validateInterval;
                }
                if (validate)
                {
                    var userId = getUserIdCallback(context.Identity);

                    if (manager != null && userId != null)
                    {
                        var userFound = manager.IdentityUserExistsById(userId);
                        var reject = true;
                        // Refresh the identity if the stamp matches, otherwise reject
                        if (userFound && manager.SupportsUserSecurityStamp())
                        {
                            var securityStamp = context.Identity.FindFirstValue(Constants.DefaultSecurityStampClaimType);

                            if (securityStamp == manager.GetSecurityStamp(userId))
                            {
                                reject = false;
                                // Regenerate fresh claims if possible and resign in
                                if (regenerateIdentityCallback != null)
                                {
                                    var identity = await regenerateIdentityCallback.Invoke(manager, userId);
                                    if (identity != null)
                                    {
                                        // Fix for regression where this value is not updated
                                        // Setting it to null so that it is refreshed by the cookie middleware
                                        context.Properties.IssuedUtc = null;
                                        context.Properties.ExpiresUtc = null;
                                        context.OwinContext.Authentication.SignIn(context.Properties, identity);
                                    }
                                }
                            }
                        }
                        if (reject)
                        {
                            context.RejectIdentity();
                            context.OwinContext.Authentication.SignOut(context.Options.AuthenticationType);
                        }
                    }
                }
            };
        }
    }
}
