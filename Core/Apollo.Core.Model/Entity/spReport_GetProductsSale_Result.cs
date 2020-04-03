
namespace Apollo.Core.Model.Entity
{
    public class spReport_GetProductsSale_Result
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string ColourValue { get; set; }
        public int UniqueSales { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalValue { get; set; }
        public int ProductPriceId { get; set; }
        public int Stock { get; set; }
    }
}
