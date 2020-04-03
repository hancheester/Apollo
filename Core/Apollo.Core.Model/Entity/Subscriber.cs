
namespace Apollo.Core.Model.Entity
{
    public class Subscriber : BaseEntity
    {
        public string Email { get; set; }     
        public bool IsActive { get; set; }     
        
        public Subscriber()
        {
            this.Email = string.Empty;
        }
    }
}
