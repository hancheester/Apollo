using Apollo.FrontStore.Models.Common;

namespace Apollo.FrontStore.Models.Customer
{
    public class AccountPrimaryAddressModel
    {
        public AddressModel Billing { get; set; }
        public AddressModel Shipping { get; set; }
    }
}