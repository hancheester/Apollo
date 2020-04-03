using Microsoft.AspNet.Identity.EntityFramework;

namespace Apollo.Core.Services.Accounts.Identity
{
    public class ApplicationUserStore : UserStore<ApplicationUser>
    {
        public ApplicationUserStore(ApplicationIdentityContext identityDbContext)
            : base(identityDbContext)
        {

        }
    }
}
