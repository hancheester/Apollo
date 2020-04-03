using System;

namespace Apollo.Core.Model.Entity
{
    public class BranchOrderItem : BaseEntity
    {
        public string BranchOrderNumber { get; set; }
        public int BranchId { get; set; }
        public string Barcode { get; set; }
        public int ProductPriceId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime UpdatedOnDate { get; set; }
    }
}
