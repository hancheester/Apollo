
namespace Apollo.Core.Model.Entity
{
    public class spReport_GetBrandsSale_Result
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string Name { get; set; }
        public int UniqueSales { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalValue { get; set; }
        public byte[] SortKey { get; set; }
    }
}
