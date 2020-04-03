namespace Apollo.Core.Model
{
    public enum BranchOrderProcessResults
    {
        Error = 0,
        Success = 1,
        VectorBranchOrderNotReady = 2,
        VectorBranchOrderNotFound = 3,
        BranchOrderAlreadyProcessed = 4
    }
}
