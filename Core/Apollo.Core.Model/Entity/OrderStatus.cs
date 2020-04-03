
namespace Apollo.Core.Model.Entity
{
    public class OrderStatus : BaseEntity
    {
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public static object AWAITING_REPLY_CODE { get; set; }

        public OrderStatus()
        {
            Status = string.Empty;
            StatusCode = string.Empty;
        }
    }
}
