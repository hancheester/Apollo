using System;

namespace Apollo.Core.Model.Entity
{
    public class BranchPendingLine : BaseEntity
    {
        public int BranchId { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }        
        public int BranchItemStatusId { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public BranchItemStatus BranchItemStatus { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductPrice ProductPrice { get; set; }
    }
}
