using Apollo.Core.Model;
using Apollo.FrontStore.Models.Product;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Category
{
    public class CategoryProductPagingFilteringModel : ProductPagingFilteringModel
    {
        public CategoryProductPagingFilteringModel()
        {
            PriceRangeFilter = new PriceRangeFilterModel();
            BrandRangeFilter = new BrandRangeFilterModel();
            CategoryFilterRangeFilter = new CategoryFilterRangeFilterModel();
            AvailableSortOptions = new List<SelectListItem>();
            AvailableViewModes = new List<SelectListItem>();
            OrderBy = ProductSortingType.StockDesc;
            ViewMode = "grid";
            PageSize = 36; //Default
            PageNumber = 1; //Default
            From = string.Empty;
            To = string.Empty;
        }
    }
}