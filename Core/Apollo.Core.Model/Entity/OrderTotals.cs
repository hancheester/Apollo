using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class OrderTotals
    {
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }

        public int ItemCount { get; set; }

        /// <summary>
        /// Value excluding VAT.
        /// </summary>
        public decimal Subtotal { get; set; }
        /// <summary>
        /// Value including VAT.
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// [Offer Rule Alias, Discount], discount (including tax) in GBP.
        /// </summary>
        public Dictionary<string, decimal> Discounts { get; set; }

        public bool DisplayTax { get; set; }
        public decimal Tax { get; set; }

        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }

        public int AllocatedPoints { get; set; }
        public int EarnedPoints { get; set; }
        public int AwardedPoints { get; set; }

        public decimal Total { get; set; }

        public OrderTotals()
        {
            Discounts = new Dictionary<string, decimal>();
        }
    }
}
