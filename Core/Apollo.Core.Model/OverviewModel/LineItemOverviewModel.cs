namespace Apollo.Core.Model.OverviewModel
{
    public class LineItemOverviewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public string OrderStatus { get; set; }
        public string StatusCode { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }        
        public string Option { get; set; }
        public int Quantity { get; set; }
        public int ShippedQuantity { get; set; }        
        public int AllocatedQuantity { get; set; }
        public int PendingQuantity { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal PriceExclTax { get; set; }
        public decimal InitialPriceInclTax { get; set; }
        public decimal InitialPriceExclTax { get; set; }
        public int InvoicedQuantity { get; set; }
        public int InTransitQuantity { get; set; }
        public string UrlRewrite { get; set; }        
        public string Note { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
        public bool FreeWrapped { get; set; }
        public int Weight { get; set; }
        public string RestrictedGroup { get; set; }
        public string CodeinMessage { get; set; }
    }
}
