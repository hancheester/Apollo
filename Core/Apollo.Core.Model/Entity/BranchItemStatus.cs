using System;

namespace Apollo.Core.Model.Entity
{
    public class BranchItemStatus : BaseEntity
    {
        public int BranchId { get; set; }
        public int ProductPriceId { get; set; }
        public string StatusCode { get; set; }
        public DateTime? ExpectedDelivery { get; set; }        
    }
}
