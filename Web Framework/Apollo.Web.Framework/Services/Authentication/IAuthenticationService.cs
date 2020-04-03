using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using System.Threading.Tasks;

namespace Apollo.Web.Framework.Services.Authentication
{
    public interface IAuthenticationService
    {
        CustomerLoginResults SignIn(string username, string password, bool isPersistent, bool shouldLockOut);
        Task<CustomerLoginResults> SignInAsync(string username, string password, bool isPersistent, bool shouldLockOut);
        void SignOut();
        Account GetAuthenticatedAccount();
    }
}
