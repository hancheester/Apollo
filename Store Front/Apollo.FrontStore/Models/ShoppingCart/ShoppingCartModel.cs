using Apollo.FrontStore.Models.Common;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.ShoppingCart
{
    public class ShoppingCartModel
    {
        public bool IsEditable { get; set; }
        public bool IsAllowedToProceed { get; set; }
        public IList<CartItemModel> Items { get; set; }
        public DiscountBoxModel DiscountBox { get; set; }
        public LoyaltyBoxModel LoyaltyBox { get; set; }
        public decimal OrderTotal { get; set; }
        public OrderReviewDataModel OrderReviewData { get; set; }
        public EstimateShippingModel EstimateShipping { get; set; }

        public ShoppingCartModel()
        {
            Items = new List<CartItemModel>();
            DiscountBox = new DiscountBoxModel();
            LoyaltyBox = new LoyaltyBoxModel();
            OrderReviewData = new OrderReviewDataModel();
            EstimateShipping = new EstimateShippingModel();
        }

        #region Nested Classes
        
        public class DiscountBoxModel
        {
            public bool Display { get; set; }
            public string Message { get; set; }
            public string CurrentCode { get; set; }
            public bool IsApplied { get; set; }
        }

        public class LoyaltyBoxModel
        {
            public bool IsLoggedIn { get; set; }
            public int MyPoints { get; set; }
            public int AllocatedPoints { get; set; }
        }

        public class OrderReviewDataModel
        {
            public bool Display { get; set; }
            public bool DisplayBillingAddress { get; set; }
            public AddressModel BillingAddress { get; set; }
            public AddressModel ShippingAddress { get; set; }
            public string ShippingMethod { get; set; }
            public bool DisplayContactNumberInDespatch { get; set; }
            public string ContactNumber { get; set; }

            public OrderReviewDataModel()
            {
                BillingAddress = new AddressModel();
                ShippingAddress = new AddressModel();            
            }
        }
        
        #endregion
    }
}