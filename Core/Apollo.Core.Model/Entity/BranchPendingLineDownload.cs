namespace Apollo.Core.Model.Entity
{
    public class BranchPendingLineDownload : BaseEntity
    {
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }
    }
}
