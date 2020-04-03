using Apollo.Core.Configuration;

namespace Apollo.Core.Domain.Customers
{
    public class CustomerSettings : ISettings
    {
        public bool AllowViewingProfiles { get; set; }
    }
}
