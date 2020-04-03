using System.Collections.Generic;

namespace Apollo.Core.Model.OverviewModel
{
    public class BrandCategoryOverviewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }        
        public int BrandId { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string UrlKey { get; set; }
        public List<BrandCategoryOverviewModel> Children { get; set; }

        public BrandCategoryOverviewModel()
        {
            this.Children = new List<BrandCategoryOverviewModel>();
        }
    }
}
