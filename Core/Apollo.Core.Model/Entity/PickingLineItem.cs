using System;
using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    public class PickingLineItem
    {
        public int ProductPriceId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }
        public int PendingQuantity { get; set; }
        public string UrlRewrite { get; set; }
        public IList<int> LineItemIds { get; set; }
        public IList<int> OrderIds { get; set; }
        public string SelectedBranch { get; set; }
        public string Barcode { get; set; }
        public bool Chosen { get; set; }
        public BranchProductStock WarehouseStock { get; set; }
    }
}
