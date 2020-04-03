using System;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    public class BranchProductStock : BaseEntity
    {
        public int ProductPriceId { get; set; }
        public int BranchId { get; set; }
        public int Stock { get; set; }
    }
}