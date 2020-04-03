
namespace Apollo.Core.Model.Entity
{
    public class USState : BaseEntity
    {
        public string State { get; set; }
        public string Code { get; set; }

        public USState()
        {
            State = string.Empty;
            Code = string.Empty;
        }
    }
}
