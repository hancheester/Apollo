using Microsoft.AspNet.Identity.EntityFramework;

namespace Apollo.Core.Services.Accounts.Identity
{
    public class ApplicationIdentityContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationIdentityContext()
            : base ("ApolloContext", throwIfV1Schema: false)
        {

        }
    }
}
