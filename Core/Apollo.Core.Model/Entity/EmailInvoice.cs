using System;

namespace Apollo.Core.Model.Entity
{
    public class EmailInvoice : BaseEntity
    {
        public int OrderId { get; set; }
        public string FirstName{ get; set; }
        public string Email{ get; set; }
        public string ContactNumber { get; set; }
        public string PaymentRef { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// Amount in GBP.
        /// </summary>
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
        public int ProfileId { get; set; }
        public string EncodedKey { get; set; }
        public DateTime EndDate { get; set; }
        public bool Paid { get; set; }
        public DateTime? DatePaid { get; set; }
        public int OrderPaymentId { get; set; }
        public string BillTo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public string PostCode { get; set; }
        public int USStateId { get; set; }
        public USState USState { get; set; }
        public string IPAddress { get; set; }

        public EmailInvoice()
        {
            this.FirstName = string.Empty;
            this.Email = string.Empty;
            this.ContactNumber = string.Empty;
            this.PaymentRef = string.Empty;
            this.Message = string.Empty;
            this.CurrencyCode = string.Empty;
            this.BillTo = string.Empty;
            this.AddressLine1 = string.Empty;
            this.AddressLine2 = string.Empty;
            this.City = string.Empty;
            this.County = string.Empty;
            this.PostCode = string.Empty;
            this.IPAddress = string.Empty;
            this.PaymentRef = string.Empty;
            this.EncodedKey = string.Empty;
        }
    }
}
