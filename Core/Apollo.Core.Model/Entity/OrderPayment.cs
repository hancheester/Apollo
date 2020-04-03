using System;

namespace Apollo.Core.Model.Entity
{
    public class OrderPayment : BaseEntity
    {
        public int OrderId { get; set; }
        public string PaymentReference { get; set; }
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// Amount in GBP.
        /// </summary>
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
        public int OrderShipmentId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
