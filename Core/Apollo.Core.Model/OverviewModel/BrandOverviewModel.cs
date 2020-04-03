using System.Collections.Generic;

namespace Apollo.Core.Model.OverviewModel
{
    public class BrandOverviewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlKey { get; set; }
        public bool Enabled { get; set; }
        public IList<int> AssignedCategoryIdForFeaturedBrand { get; set; }
    }
}
