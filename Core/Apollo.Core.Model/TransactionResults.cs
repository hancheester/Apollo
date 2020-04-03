namespace Apollo.Core.Model
{
    public enum TransactionResults
    {
        Error = 0,
        Success = 1,
        Failed = 2,
        PaymentTransactionNotFound = 3,
        URLRedirectRequired = 4
    }
}
