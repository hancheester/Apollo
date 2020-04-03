namespace Apollo.Core.Services.Common
{
    public interface IEmailManager
    {
        void SendEmailAsync(string email, string subject, string body);
        void SendAccountRegistrationEmail(string email, string firstName);
        void SendAccountRegistrationWithPasswordEmail(string email, string firstName, string password);
        void SendPasswordRetrievalEmail(string email, string firstName, string password);
        void SendNewUsernameEmail(string email, string firstName);
        void SendPaymentInvoiceConfirmationEmail(string email, string name, string amount);
        void SendDespatchConfirmationEmail(string email, string name, string orderId, string items);
        void SendOrderConfirmationEmail(
            string email,
            string name,
            string orderId,
            string billingAddress,
            string shippingAddress,
            string shippingOption,
            string items,
            string itemTotal,
            string discountTitle,
            string discount,
            string loyaltyPointTitle,
            string loyaltyPoint,            
            string vat,
            string shippingCost,
            string total,
            string nhsNotice);
        void SendPaymentInvoiceEmail(string email, string name, string message, string amount, string payLink);
        void SendBackInStockEmail(string email, string productName, string productUrl, string option, string productImage);
    }
}
