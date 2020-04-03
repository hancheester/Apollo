using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Common
{
    public interface IPdfService
    {
        byte[] PrintOrderToInvoicePdf(Order order);
        byte[] PrintBranchPendingLinePdf(IList<BranchPendingLineDownload> list);
        byte[] PrintInventoryPendingLinePdf(IList<InventoryPendingLine> list);
        byte[] PrintPickInProgressLinePdf(IList<PickingLineItem> list);
    }
}
