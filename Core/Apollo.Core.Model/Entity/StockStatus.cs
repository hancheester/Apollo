using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class StockStatus
    {
        public bool IsOutOfStock { get; set; }
        public bool HasOutOfStockItem { get; set; }
        public List<int> CartItemIdList { get; set; }
        public StockStatus()
        {
            this.CartItemIdList = new List<int>();
        }
    }
}
