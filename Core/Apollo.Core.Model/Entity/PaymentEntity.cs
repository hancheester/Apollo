namespace Apollo.Core.Model.Entity
{
    public abstract class PaymentEntity : BaseEntity
    {
        public int OrderId { get; set; }
        public int EmailInvoiceId { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string UserAgent { get; set; }
        public string ClientIPAddress { get; set; }
        public string MD { get; set; }
        public string PAReq { get; set; }
        public bool Completed { get; set; }
    }
}
