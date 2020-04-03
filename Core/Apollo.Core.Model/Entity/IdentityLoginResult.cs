using System.Security.Claims;

namespace Apollo.Core.Model.Entity
{
    public class IdentityLoginResult
    {
        public CustomerLoginResults CustomerLoginResults { get; set; }
        public ClaimsIdentity ClaimsIdentity { get; set; }
    }
}
