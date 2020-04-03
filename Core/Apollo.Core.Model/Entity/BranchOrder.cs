using System;

namespace Apollo.Core.Model.Entity
{
    public class BranchOrder : BaseEntity
    {
        public string BranchOrderNumber { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime ProcessedOnDate { get; set; }
    }
}
