namespace Apollo.Core.Model.OverviewModel
{
    public class CartItemOverviewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal PriceExclTax { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal OfferPriceExclTax { get; set; }
        public decimal OfferPriceInclTax { get; set; }
        public string Option { get; set; }
        public string UrlKey { get; set; }
        public string ThumbnailFilename { get; set; }
        public int StepQuantity { get; set; }
        public int MaxAllowedQuantity { get; set; }        
        public bool IsPharmaceutical { get; set; }
        public bool Discontinued { get; set; }
        public bool BrandEnforcedStockCount { get; set; }
        public bool ProductEnforcedStockCount { get; set; }
        public int Stock { get; set; }
        public int? MaximumAllowedPurchaseQuantity { get; set; }
    }
}
