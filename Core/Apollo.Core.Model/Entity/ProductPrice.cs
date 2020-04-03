using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class ProductPrice : BaseEntity
    {
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public decimal PriceExclTax { get; set; }
        [DataMember]
        public decimal PriceInclTax { get; set; }
        [DataMember]
        public decimal OfferPriceExclTax { get; set; }
        [DataMember]
        public decimal OfferPriceInclTax { get; set; }
        [DataMember]
        public int Stock { get; set; }
        [DataMember]
        public int OfferRuleId { get; set; }
        [DataMember]
        public string Barcode { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public int Weight { get; set; }
        [DataMember]
        public decimal CostPrice { get; set; }
        [DataMember]
        public bool ShowOfferTag { get; set; }
        [DataMember]
        public bool ShowRRP { get; set; }
        [DataMember]
        public string PriceCode { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public decimal AdditionalShippingCost { get; set; }
        [DataMember]
        public int? ColourId { get; set; }
        [DataMember]
        public string Size { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public int? MaximumAllowedPurchaseQuantity { get; set; }
        [DataMember]
        public int? ProductMediaId { get; set; }
        [DataMember]
        public DateTime CreatedOnDate { get; set; }
        [DataMember]
        public DateTime UpdatedOnDate { get; set; }        
        [DataMember]
        public string Option { get; set; }
        [DataMember]
        public bool DisableStockSync { get; set; }
        
        public ProductPrice()
        {
            this.CreatedOnDate = DateTime.Now;
            this.UpdatedOnDate = DateTime.Now;
            this.Size = string.Empty;
            this.PriceCode = string.Empty;
        }
    }
}
