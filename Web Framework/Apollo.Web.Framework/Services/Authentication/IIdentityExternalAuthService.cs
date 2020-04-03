using Apollo.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apollo.Web.Framework.Services.Authentication
{
    public interface IIdentityExternalAuthService
    {
        CustomerLoginResults ExternalSignIn(string email, string loginProvider, string providerKey, bool isPersistent);
        Task<CustomerLoginResults> ExternalSignInAsync(string email, string loginProvider, string providerKey, bool isPersistent);
        bool ChangePassword(string username, string oldPassword, string newPassword);
        Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
        bool SetPassword(string username, string password);        
        Task<bool> SetPasswordAsync(string username, string password);
        IDictionary<string, string> GetLogins(string username);
        Task<IDictionary<string, string>> GetLoginsAsync(string username);
        bool RemoveLogin(string username, string loginProvider, string providerKey);
        Task<bool> RemoveLoginAsync(string username, string loginProvider, string providerKey);
    }
}
