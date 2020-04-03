using System;

namespace Apollo.Core.Model.Entity
{
    public class spGetBestSellers_Result
    {
        public int ProductId { get; set; }
        public DateTime LastSoldOn { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
        public string Name { get; set; }
    }
}
