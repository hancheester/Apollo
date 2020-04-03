using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface ISearchService
    {
        SearchResultPagedList<ProductOverviewModel> SearchProduct(
           int pageIndex = 0,
           int pageSize = 2147483647,  //Int32.MaxValue
           IList<int> categoryIds = null,
           IList<int> brandIds = null,
           IList<int> productIds = null,
           bool? enabled = null,
           bool? discontinued = null,
           bool? visibleIndividually = null,
           bool includeDiscontinuedButInStock = false,
           decimal? priceMin = null,
           decimal? priceMax = null,
           string keywords = null,
           bool searchDescriptions = false,
           bool useFullTextSearch = false,
           FulltextSearchMode fullTextMode = FulltextSearchMode.Or,
           bool applySearchAnalysis = false,
           ProductSortingType orderBy = ProductSortingType.Position,
           bool applyKeywordSuggestion = false,
           bool displaySearchAnalysis = false);
        int InsertCustomDictionary(CustomDictionary word);
        void UpdateCustomDictionary(CustomDictionary word);
        CustomDictionary GetCustomDictionary(int id);
        void DeleteCustomDictionary(int id);
        PagedList<CustomDictionary> GetPagedCustomDictionary(
            int pageIndex = 0,
            int pageSize = 2147483647);
    }
}
