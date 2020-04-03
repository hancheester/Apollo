using Apollo.FrontStore.Models.Common;
using Apollo.FrontStore.Models.Media;
using Apollo.FrontStore.Models.Product;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Brand
{
    public class BrandModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UrlKey { get; set; }
        public bool HasMicrosite { get; set; }
        
        public string TopUrlKey { get; set; }
        public string TopCategoryName { get; set; }
        public string SecondUrlKey { get; set; }
        public string SecondCategoryName { get; set; }
        public string ThirdUrlKey { get; set; }
        public string ThirdCategoryName { get; set; }
        
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }

        public IList<BrandCategoryModel> Categories { get; set; }
        public ProductPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ProductBoxModel> Products { get; set; }
        public IList<BannerModel> Banners { get; set; }

        public IList<int> FeatureItemTypes { get; set; }
        public PictureModel Logo { get; set; }

        public BrandModel()
        {
            Categories = new List<BrandCategoryModel>();
            PagingFilteringContext = new ProductPagingFilteringModel();
            Products = new List<ProductBoxModel>();
            Banners = new List<BannerModel>();
        }
    }
}