using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class AdmissionLineItem
    {
        public int ProductPriceId { get; set; }
        public IList<int> LineItemIds { get; set; }
    }
}
