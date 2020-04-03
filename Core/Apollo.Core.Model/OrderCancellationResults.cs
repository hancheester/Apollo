namespace Apollo.Core.Model
{
    public enum OrderCancellationResults
    {
        Error = 0,
        Success = 1,        
        SuccessButNoLoyaltyFound = 2,
        CancellationNotFound = 3
    }
}
