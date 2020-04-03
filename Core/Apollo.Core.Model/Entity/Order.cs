using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Apollo.Core.Model.Entity
{
    public class Order : BaseEntity
    {     
        public int ProfileId { get; set; }
        public DateTime? OrderPlaced { get; set; }
        public bool Paid { get; set; }
        public DateTime? DateShipped { get; set; }
        public bool Shipped { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastAlertDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentRef { get; set; }
        public string Packing { get; set; }
        public string BillTo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public int CountryId { get; set; }
        
        public string PostCode { get; set; }
        public int USStateId { get; set; }
        
        public string ShipTo { get; set; }
        public string ShippingAddressLine1 { get; set; }
        public string ShippingAddressLine2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingCounty { get; set; }
        public int ShippingCountryId { get; set; }
        
        public string ShippingPostCode { get; set; }
        public int ShippingUSStateId { get; set; }
        
        public decimal ShippingCost { get; set; }
        public int ShippingOptionId { get; set; }
        
        public int AllocatedPoint { get; set; }
        public int EarnedPoint { get; set; }
        public int AwardedPoint { get; set; }
        public string PromoCode { get; set; }
        /// <summary>
        /// Value including tax.
        /// </summary>
        public decimal DiscountAmount { get; set; }
        public int InvoiceNumber { get; set; }
        public string IPAddress { get; set; }
        public string StatusCode { get; set; }
        public string OrderStatus { get; set; }
        public string IssueCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }

        public bool Archieved { get; set; }
        public Country Country { get; set; }
        public USState USState { get; set; }
        public Country ShippingCountry { get; set; }
        public USState ShippingUSState { get; set; }
        public ShippingOption ShippingOption { get; set; }        
        public ICollection<LineItem> LineItemCollection { get; set; }
        public PharmOrder PharmOrder { get; set; }
        public IList<OrderNote> OrderNotes { get; set; }
        public IList<OrderComment> OrderComments { get; set; }
        public SystemCheck SystemCheck { get; set; }
        /// <summary>
        /// The total order value in current currency.
        /// </summary>
        public decimal GrandTotal { get; set; }
        /// <summary>
        /// Tax in currency currency.
        /// </summary>
        public decimal Tax { get; set; }
        public int ItemQuantity
        {
            get
            {
                int qty = 0;

                foreach (var item in this.LineItemCollection)
                    qty = qty + item.Quantity;

                return qty;
            }
        }
        public int TotalItemWeight
        {
            get
            {
                int weight = 0;

                foreach (var item in this.LineItemCollection)
                    weight = weight + (item.Quantity * item.Weight);

                return weight;
            }
        }

        public Order()
        {
            this.ShippingOption = new ShippingOption();
            this.LineItemCollection = new Collection<LineItem>();
            this.PharmOrder = new PharmOrder();
            this.OrderNotes = new List<OrderNote>();
            this.OrderComments = new List<OrderComment>();
            this.SystemCheck = new SystemCheck();
            this.ExchangeRate = 1M;            
            this.Packing = string.Empty;
            this.IssueCode = string.Empty;
            this.PromoCode = string.Empty;
            this.IPAddress = string.Empty;
            this.StatusCode = string.Empty;
            this.IssueCode = string.Empty;
            this.CurrencyCode = string.Empty;
        }
    }
}
