
namespace Apollo.Core.Model.Entity
{
    public class WarehouseAllocation : BaseEntity
    {
        public int LineItemId { get; set; }
        public int ProductPriceId { get; set; }
        public int Quantity { get; set; }
        public string Barcode { get; set; }
    }
}
