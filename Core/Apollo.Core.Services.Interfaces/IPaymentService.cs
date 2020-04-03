using Apollo.Core.Model.Entity;

namespace Apollo.Core.Services.Interfaces
{
    public interface IPaymentService
    {
        TransactionOutput ProcessPaymentAfter3DCallback(string md, string pares, bool sendEmailFlag);
        TransactionOutput ProcessPaymentFromCart(
            int profileId,
            string paymentMethod,
            string email,
            string contactNumber,
            string userAgent,
            string clientIPAddress,
            Card card,
            bool sendEmailFlag);
        TransactionOutput ProcessPaymentFromEmailInvoice(
            int emailInvoiceId,
            Address billingAddress,
            Card card,
            string userAgent,
            string clientIPAddress,
            bool sendEmailFlag);
        TransactionOutput ProcessPaymentFromBackOffice(
            int profileId,
            string paymentMethod,
            string email,
            string contactNumber,
            string userAgent,
            string clientIPAddress,
            Card card,
            Address billingAddress,
            Address shippingAddress,
            bool sendEmailFlag,
            bool exemptedFromPayment = false);
        TransactionOutput ProcessOrderCharging(OrderPayment payment);
    }
}
