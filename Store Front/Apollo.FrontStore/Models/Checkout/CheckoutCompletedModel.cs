namespace Apollo.FrontStore.Models.Checkout
{
    public class CheckoutCompletedModel
    {
        public int? OrderId { get; set; }
        public int? EmailInvoiceId { get; set; }
        public string ThankYouTitle { get; set; }
        public string Message { get; set; }

        //public bool EnableReview { get; set; }
        //public string Email { get; set; }
        //public string Amount { get; set; }
        //public string CurrencyCode { get; set; }
        //public string PaymentType { get; set; }
        //public string EstimatedDeliveryDate { get; set; }
    }
}