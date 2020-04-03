using Microsoft.AspNet.Identity.EntityFramework;

namespace Apollo.Core.Services.Accounts.Identity
{
    public class ApplicationRoleStore : RoleStore<IdentityRole>
    {
        public ApplicationRoleStore(ApplicationIdentityContext identityDbContext)
            : base(identityDbContext)
        {

        }
    }
}
