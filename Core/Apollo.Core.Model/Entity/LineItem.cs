using System;

namespace Apollo.Core.Model.Entity
{
    public class LineItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public decimal InitialPriceInclTax { get; set; }
        public decimal InitialPriceExclTax { get; set; }
        public int Quantity { get; set; }
        public int PendingQuantity { get; set; }
        public int InvoicedQuantity { get; set; }
        public int ShippedQuantity { get; set; }
        public int InTransitQuantity { get; set; }
        public int AllocatedQuantity { get; set; }
        public bool Wrapped { get; set; }
        public string Note { get; set; }
        public bool IsPharmaceutical { get; set; }
        public string StatusCode { get; set; }
        public int Weight { get; set; }
        public decimal CostPrice { get; set; }
        public decimal ExchangeRate { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductPrice ProductPrice { get; set; }

        public LineItem()
        {
            StatusCode = string.Empty;
            CreatedOnUtc = DateTime.Now;
            UpdatedOnUtc = DateTime.Now;
        }
    }
}
