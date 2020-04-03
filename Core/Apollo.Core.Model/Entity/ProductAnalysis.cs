using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class ProductAnalysis
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Sold { get; set; }
        public int Cancelled { get; set; }
        public int CurrentStock { get; set; }
        public Dictionary<string, int> TopCountries { get; set; }

        public ProductAnalysis()
        {
            TopCountries = new Dictionary<string, int>();
        }
    }
}
