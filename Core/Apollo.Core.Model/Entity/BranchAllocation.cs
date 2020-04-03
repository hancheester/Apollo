using System;

namespace Apollo.Core.Model.Entity
{
    public class BranchAllocation : BaseEntity
    {
        public int BranchId { get; set; }
        public int LineItemId { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public bool Enabled { get; set; }
        public int BranchItemStatusId { get; set; }
        public bool Printed { get; set; }
        public DateTime? PrintedDateTime { get; set; }
    }
}
