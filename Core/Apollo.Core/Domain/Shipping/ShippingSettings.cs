using Apollo.Core.Configuration;

namespace Apollo.Core.Domain.Shipping
{
    public class ShippingSettings : ISettings
    {
        public int PrimaryStoreCountryId { get; set; }
    }
}
