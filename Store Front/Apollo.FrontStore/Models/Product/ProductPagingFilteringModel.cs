using Apollo.Core.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductPagingFilteringModel : BasePageableModel
    {
        public PriceRangeFilterModel PriceRangeFilter { get; set; }
        public BrandRangeFilterModel BrandRangeFilter { get; set; }
        public CategoryFilterRangeFilterModel CategoryFilterRangeFilter { get; set; }
        public int SelectedPageSize { get; set; }        
        public IList<SelectListItem> AvailableSortOptions { get; set; }
        public IList<SelectListItem> AvailableViewModes { get; set; }
        public ProductSortingType OrderBy { get; set; }
        public string ViewMode { get; set; }
        public string Brands { get; set; }
        public string Filters { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        
        public ProductPagingFilteringModel()
        {
            PriceRangeFilter = new PriceRangeFilterModel();
            BrandRangeFilter = new BrandRangeFilterModel();
            CategoryFilterRangeFilter = new CategoryFilterRangeFilterModel();
            AvailableSortOptions = new List<SelectListItem>();
            AvailableViewModes = new List<SelectListItem>();            
            OrderBy = ProductSortingType.Position;
            ViewMode = "grid";
            PageSize = 36; //Default
            PageNumber = 1; //Default
            From = string.Empty;
            To = string.Empty;
        }

        public void PrepareSortingOptions()
        {
            foreach (ProductSortingType enumValue in Enum.GetValues(typeof(ProductSortingType)))
            {
                var sortValue = ConvertEnum(enumValue);
                if (!string.IsNullOrEmpty(sortValue))
                {
                    AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = ((int)enumValue).ToString(),
                        Selected = enumValue == OrderBy
                    });
                }
            }
        }

        public void PrepareViewModeOptions()
        {
            AvailableViewModes.Add(new SelectListItem { Text = "glyphicon glyphicon-th", Value = "grid", Selected = "grid" == ViewMode });
            AvailableViewModes.Add(new SelectListItem { Text = "glyphicon glyphicon-th-list", Value = "list", Selected = "list" == ViewMode });
        }

        public string ConvertEnum(ProductSortingType type)
        {
            switch(type)
            {
                case ProductSortingType.NameAsc:
                    return "Name: A to Z";
                case ProductSortingType.NameDesc:
                    return "Name: Z to A";
                case ProductSortingType.PriceAsc:
                    return "Price: Low to High";
                case ProductSortingType.PriceDesc:
                    return "Price: High to Low";
                case ProductSortingType.CreatedOn:
                    return "Recently Added";
                case ProductSortingType.ReviewScoreDesc:
                    return "Top Rated";
                case ProductSortingType.StockDesc:
                    return "Stock Level";
                case ProductSortingType.Position:                
                    return "Default";
                default:
                    return string.Empty;
            }
        }
    }
}