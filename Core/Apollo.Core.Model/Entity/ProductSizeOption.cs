using System;

namespace Apollo.Core.Model
{
    [Serializable]
    public class ProductSizeOption : ProductPrice
    {
        public string SizeOption { get; set; }
        public int ProductPriceId { get; set; }

        public ProductSizeOption()
        {
            this.UPC = string.Empty;
            this.EAN = string.Empty;         
            this.SizeOption = string.Empty;
        }
    }
}
