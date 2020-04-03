using System;

namespace Apollo.Core.Model
{
    [Serializable]
    public class ProductColourOption : ProductPrice
    {
        public int ProductPriceId { get; set; }
        public int ColourId { get; set; }
        public Colour Colour { get; set; }
    }
}
