using System;

namespace Apollo.Core.Model.Entity
{
    public class CartItem : BaseEntity
    {
        public int ProfileId { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public int Quantity { get; set; }
        public int CartItemMode { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime UpdatedOnDate { get; set; }
        
        public virtual Product Product { get; set; }
        public virtual ProductPrice ProductPrice { get; set; }

        public CartItem()
        {
            CreatedOnDate = DateTime.Now;
            UpdatedOnDate = DateTime.Now;
        }
    }
}
