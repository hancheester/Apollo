using Apollo.Core.Configuration;

namespace Apollo.Core.Domain.Orders
{
    public class ShoppingCartSettings : ISettings
    {
        public int MaxPharmaceuticalProduct { get; set; }
        public int LoyaltyRate { get; set; }
    }
}
