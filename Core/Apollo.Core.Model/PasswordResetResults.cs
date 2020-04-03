namespace Apollo.Core.Model
{
    public enum PasswordResetResults
    {
        Error = 0,
        Successful = 1,
        MemberNotExist = 2,
        ProfileNotExist = 3,
        AccountNotExist = 4,        
        IsLockedOut = 5,
        NotApproved = 6,
        NotRegistered = 7,
    }
}
