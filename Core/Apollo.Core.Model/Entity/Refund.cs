using System;

namespace Apollo.Core.Model.Entity
{
    public class Refund : BaseEntity
    {
        public int OrderId { get; set; }
        public int PointToRefund { get; set; }
        /// <summary>
        /// Amount in GBP.
        /// </summary>
        public decimal ValueToRefund { get; set; }
        public int ProfileId { get; set; }
        public DateTime DateStamp { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCancellation { get; set; }
        public string Reason { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
