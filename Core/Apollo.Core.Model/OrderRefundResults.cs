namespace Apollo.Core.Model
{
    public enum OrderRefundResults
    {
        Error = 0,
        Success = 1,
        SuccessButNoLoyaltyFound = 2,
        RefundNotFound = 3
    }
}
