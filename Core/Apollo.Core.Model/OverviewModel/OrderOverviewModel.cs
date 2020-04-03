using System;

namespace Apollo.Core.Model.OverviewModel
{
    public class OrderOverviewModel
    {
        public int Id { get; set; }
        public string StatusCode { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? OrderPlaced { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastAlertDate { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// Amount in current currency.
        /// </summary>
        public decimal GrandTotal { get; set; }
        public string CurrencyCode { get; set; }
        public int ShippingOptionId { get; set; }
        public string ShippingOptionName { get; set; }
        public int ShippingCountryId { get; set; }
        public string IssueCode { get; set; }
        public string OrderIssue { get; set; }
        public string LastUpdatedBy { get; set; }
        public int InvoiceNumber { get; set; }
        public string BillTo { get; set; }
        public string ShipTo { get; set; }
        public int ProfileId { get; set; }
        public int AccountId { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal DiscountValue { get; set; }
        public int AllocatedPoint { get; set; }
        public int AwardedPoint { get; set; }
        public int EarnedPoint { get; set;  }
        public string PromoCode { get; set; }
        public int PointValue { get; set; }
        public bool Paid { get; set; }        
        public decimal ShippingCost { get; set; }
        public string Packing { get; set; }        
    }
}
