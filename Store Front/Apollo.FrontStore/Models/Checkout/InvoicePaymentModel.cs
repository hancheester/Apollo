using Apollo.FrontStore.Models.Common;

namespace Apollo.FrontStore.Models.Checkout
{
    public class InvoicePaymentModel
    {
        public int EmailInvoiceId { get; set; }
        public AddressModel BillingAddress { get; set; }
        public CheckoutPaymentModel Payment { get; set; }

        public InvoicePaymentModel()
        {
            BillingAddress = new AddressModel();
            Payment = new CheckoutPaymentModel();
        }
    }
}