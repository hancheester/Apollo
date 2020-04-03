using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class CartOffer
    {
        public bool IsValid { get; set; }
        /// <summary>
        /// Value (including tax) in GBP.
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// [Offer Rule ID, Discount], discount (including tax) in GBP.
        /// </summary>
        public Dictionary<int, decimal> Discounts { get; set; }

        public bool FreeDelivery { get; set; }
        public int RewardPoint { get; set; }
        public string Description { get; set; }

        public CartOffer()
        {
            Discounts = new Dictionary<int, decimal>();
        }
    }
}
