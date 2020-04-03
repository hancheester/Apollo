using Apollo.FrontStore.Models.Common;

namespace Apollo.FrontStore.Models.Checkout
{
    public class CheckoutAddressModel
    {
        public bool NeedBillingAddress { get; set; }
        public bool HasBillingAddress { get; set; }
        public AddressModel BillingAddress { get; set; }

        public bool HasShippingAddress { get; set; }
        public AddressModel ShippingAddress { get; set; }

        public bool HasSavedAddress { get; set; }
        public bool DisableProceed { get; set; }

        public CheckoutAddressModel()
        {
            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
        }
    }
}