using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Product
{
    public class SearchModel
    {
        public ProductPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ProductBoxModel> Products { get; set; }
        public string q { get; set; }
        public string OriginalKeywords { get; set; }
        public string SuggestedKeywords { get; set; }

        public SearchModel()
        {
            PagingFilteringContext = new ProductPagingFilteringModel();
            Products = new List<ProductBoxModel>();
        }
    }
}