namespace Apollo.AdminStore.WebForm.Classes
{
    public class AccountLite
    {
        public int ProfileId { get; set; }
        public string Name { get; set; }        
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string DateOfBirth { get; set; }
        public bool CreateAccount { get; set; }
    }
}