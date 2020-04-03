using Apollo.Core.Model.OverviewModel;
using Apollo.FrontStore.Models.Common;
using Apollo.FrontStore.Models.Product;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Category
{
    public class CategoryModel
    {
        public CategoryOverviewModel Category { get; set; }
        public CategoryProductPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ProductBoxModel> Products { get; set; }
        public IList<int> FeatureItemTypes { get; set; }

        public string SelectedSecondCategoryName { get; set; }
        public string SelectedSecondCategoryUrl { get; set; }
        public string SelectedThirdCategoryName { get; set; }
        public string SelectedThirdCategoryUrl { get; set; }
        public string SelectedCategoryDescription { get; set; }

        public string CategoryTemplateViewPath { get; set; }
        
        public IList<BannerModel> Banners { get; set; }

        public CategoryModel()
        {
            PagingFilteringContext = new CategoryProductPagingFilteringModel();
            Products = new List<ProductBoxModel>();
            Banners = new List<BannerModel>();
        }
    }
}