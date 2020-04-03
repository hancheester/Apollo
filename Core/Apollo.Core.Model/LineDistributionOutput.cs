using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Model
{
    public class LineDistributionOutput
    {
        public LineDistributionResults LineDistributionResult { get; set; }
        public IList<AllocatedItem> AllocatedItems { get; set; }
    }
}
