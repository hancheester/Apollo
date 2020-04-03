
namespace Apollo.Core.Model.Entity
{
    public class TransactionOutput
    {
        public bool Status { get; set; }
        public TransactionResults TransactionResult { get; set; }
        public string MD { get; set; }
        public string RedirectUrl { get; set; }
        public string PaReq { get; set; }
        public bool Has3DSecure { get; set; }
        public string EchoData { get; set; }
        public string Message { get; set; }
        public bool AVSCheck { get; set; }
        public bool ThreeDCheck { get; set; }
        public StockStatus StockStatus { get; set; }
        public int OrderId { get; set; }
        public int EmailInvoiceId { get; set; }
        public bool Completed { get; set; }
        public bool PaymentReleased { get; set; }
        public string EmailInvoiceEncodedKey { get; set; }
        public string Email { get; set; }
        public bool HasNHSPrescription { get; set; }
    }
}
