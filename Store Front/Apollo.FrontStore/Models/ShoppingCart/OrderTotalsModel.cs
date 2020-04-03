using Apollo.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.ShoppingCart
{
    public class OrderTotalsModel
    {
        public bool IsEditable { get; set; }
        public string SubTotal { get; set; }
        public string SubTotalDiscount { get; set; }
        public bool AllowRemovingSubTotalDiscount { get; set; }
        public IList<string> CartOffers { get; set; }

        public string Shipping { get; set; }
        public bool RequiresShipping { get; set; }
        public string SelectedShippingMethod { get; set; }
        public string PaymentMethodAdditionalFee { get; set; }

        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public IList<GiftCard> GiftCards { get; set; }

        public string OrderTotalDiscount { get; set; }
        public bool AllowRemovingOrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }

        public int WillEarnRewardPoints { get; set; }

        public int AllocatedPoints { get; set; }
        public string AllocatedPointsAmount { get; set; }
        
        public string OrderTotal { get; set; }

        public OrderTotalsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            CartOffers = new List<string>();
        }

        #region Nested classes

        public partial class TaxRate
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseEntityModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
            public string Remaining { get; set; }
        }

        #endregion
    }
}