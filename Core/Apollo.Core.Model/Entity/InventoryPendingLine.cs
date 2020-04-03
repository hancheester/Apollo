using System;
using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class InventoryPendingLine
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }        
        public int Quantity { get; set; }
        public int Stock { get; set; }
        public DateTime OrderPlaced { get; set; }
        public int OrderCount { get; set; }
        public IList<int> RelatedOrderIds { get; set; }
    }
}
