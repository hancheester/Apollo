using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.OverviewModel
{
    [Serializable]
    [DataContract]
    public class ProductPriceOverviewModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public bool ProductEnabled { get; set; }
        [DataMember]
        public string PriceCode { get; set; }
        [DataMember]
        public int Weight { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public int Stock { get; set; }
        [DataMember]
        public string Barcode { get; set; }
        [DataMember]
        public string Size { get; set; }
        [DataMember]
        public int? ColourId { get; set; }
        [DataMember]
        public string Option { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public int? MaximumAllowedPurchaseQuantity { get; set; }
    }
}
