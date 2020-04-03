using System;

namespace Apollo.Core.Model.Entity
{
    public class StockNotification : BaseEntity
    {
        public string Email { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime UpdatedOnDate { get; set; }
        public bool Notified { get; set; }
    }
}
