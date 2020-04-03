namespace Apollo.Core.Model
{
    public enum CustomerLoginResults
    {
        Error = 0,
        Successful = 1,
        MemberNotExists = 2,
        ProfileNotExists = 3,
        AccountNotExists = 4,
        WrongPassword = 5,
        IsLockedOut = 6,
        NotApproved = 7,
        NotPermitted = 8,
    }
}
