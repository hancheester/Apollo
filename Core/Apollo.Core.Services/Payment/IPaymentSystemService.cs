using Apollo.Core.Model.Entity;

namespace Apollo.Core.Services.Payment
{
    public interface IPaymentSystemService
    {
        PaymentEntity InsertPaymentEntity(PaymentEntity entity);
        PaymentEntity UpdateAndGetPaymentEntityForOutOfStock(int id);
        PaymentEntity GetPaymentEntityByMD(string md);
        void UpdatePaymentEntityForPAReq(int id, string paReq);
        PaymentEntity BuildPaymentEntity(string vendorTxCode,
                                         decimal amount,
                                         decimal exchangeRate,
                                         string currencyCode,
                                         Card card,
                                         string email,
                                         string contactNumber,
                                         string userAgent,
                                         string clientIPAddress,
                                         int orderId,
                                         int? emailInvoiceId = null);
        PaymentEntity BuildPaymentEntityForBackOffice(string vendorTxCode,
                                                     decimal amount,
                                                     decimal exchangeRate,
                                                     string currencyCode,
                                                     Card card,
                                                     string email,
                                                     string contactNumber,
                                                     string userAgent,
                                                     string clientIPAddress,
                                                     int orderId,
                                                     int? emailInvoiceId = null);
        TransactionOutput ProcessCardPayment(Order order, PaymentEntity entity);
        TransactionOutput ProcessCardPayment(EmailInvoice emailInvoice, PaymentEntity entity);
        TransactionOutput ProcessPaymentAfter3DCallback(PaymentEntity entity);
        TransactionOutput ProcessRefund(Refund refund, PaymentEntity paymentEntity = null);
        TransactionOutput ProcessCancel(Refund refund);
        TransactionOutput ProcessOrderCharging(OrderPayment orderPayment);
        string GetThirdManResult(int orderId);
        string GetPaymentDetails(int orderId);
        string GetIPLocation(int orderId);
        string GetLatestStatusFromLog(int orderId);
        string GetTransactionCardDetails(int orderId);
    }
}
