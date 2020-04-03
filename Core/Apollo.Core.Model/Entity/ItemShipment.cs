
namespace Apollo.Core.Model.Entity
{
    public class ItemShipment : BaseEntity
    {
        public int OrderShipmentId { get; set; }
        public int LineItemId { get; set; }
        public int Quantity { get; set; }        
        public LineItem LineItem { get; set; }
    }
}
