namespace Apollo.Core.Model.Entity
{
    public class SagePayDirect : PaymentEntity
    {
        public string VPSProtocol { get; set; }
        public string TxType { get; set; }
        public string Vendor { get; set; }
        public string VendorTxCode { get; set; }
        public string CardHolder { get; set; }
        public string CardNumber { get; set; }
        public string StartMonth { get; set; }
        public string StartYear { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string IssueNumber { get; set; }
        public string CV2 { get; set; }
        public string CardType { get; set; }
        /// <summary>
        /// Amount in GBP.
        /// </summary>
        public decimal Amount { get; set; }
        public string ACSUrl { get; set; }
        public string VPSTxId { get; set; }
        public string SecurityKey { get; set; }
        public string TxAuthNo { get; set; }
        public string AVSCV2 { get; set; }
        public string AddressResult { get; set; }
        public string PostCodeResult { get; set; }
        public string CV2Result { get; set; }
        public string ThreeDSecureStatus { get; set; }
        public string CAVV { get; set; }
        public string Apply3DSecure { get; set; }
        public string ApplyAVSCV2 { get; set; }
        public string AccountType { get; set; }
        public string Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
    }
}
