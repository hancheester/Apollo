using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Common;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Catalog
{
    public class SearchService : BaseRepository, ISearchService
    {
        #region Fields

        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IRepository<CustomDictionary> _customDictionaryRepository;
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly ISpellCheckerService _spellCheckerService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public SearchService(IBrandService brandService,
                             IProductService productService,
                             IRepository<SearchTerm> searchTermRepository,
                             IRepository<CustomDictionary> customDictionaryRepository,
                             ISpellCheckerService spellCheckerService,
                             ILogBuilder logBuilder)
        {            
            _brandService = brandService;
            _searchTermRepository = searchTermRepository;
            _customDictionaryRepository = customDictionaryRepository;
            _productService = productService;
            _spellCheckerService = spellCheckerService;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        #region Command

        public SearchResultPagedList<ProductOverviewModel> SearchProduct(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            IList<int> brandIds = null,
            IList<int> productIds = null,
            bool? enabled = default(bool?),
            bool? discontinued = default(bool?),
            bool? visibleIndividually = default(bool?),
            bool includeDiscontinuedButInStock = false,
            decimal? priceMin = default(decimal?),
            decimal? priceMax = default(decimal?),
            string keywords = null,
            bool searchDescriptions = false,
            bool useFullTextSearch = false,
            FulltextSearchMode fullTextMode = FulltextSearchMode.Or,
            bool applySearchAnalysis = false,
            ProductSortingType orderBy = ProductSortingType.Position,
            bool applyKeywordSuggestion = false,
            bool displaySearchAnalysis = false)
        {
            string suggestedKeywords = null;

            if (!string.IsNullOrEmpty(keywords))
            {
                // First, we check for search term
                var searchTerm = GetSearchTermByQuery(keywords);
                if (searchTerm != null) return new SearchResultPagedList<ProductOverviewModel>(searchTerm);

                // Then, we check for product name
                var product = _productService.GetActiveProductOverviewModelByName(keywords);
                if (product != null) return new SearchResultPagedList<ProductOverviewModel>(new List<ProductOverviewModel> { product });

                suggestedKeywords = keywords;

                if (applyKeywordSuggestion == true && !string.IsNullOrWhiteSpace(keywords))
                {
                    // We check if it's a brand name
                    var suggestionBrands = _spellCheckerService.Suggest(keywords);
                    Brand brand = null;

                    if (suggestionBrands.Count > 0)
                    {
                        brand = _brandService.GetBrandByName(suggestionBrands[0]);
                        if (brand != null)
                        {
                            suggestedKeywords = suggestionBrands[0];
                            if (brandIds == null)
                                brandIds = new List<int> { brand.Id };
                            else
                                brandIds.Add(brand.Id);
                        }
                    }

                    if (brand == null)
                    {
                        var words = keywords.Split(' ');

                        for (int i = 0; i < words.Length; i++)
                        {
                            bool correct = _spellCheckerService.Spell(words[i]);

                            if (!correct)
                            {
                                var suggestions = _spellCheckerService.Suggest(words[i]);
                                if (suggestions.Count > 0) words[i] = suggestions[0]; //Choose the first one.
                            }
                        }
                        suggestedKeywords = string.Join(" ", words);
                    }
                }
            }

            var list = _productService.GetPagedProductOverviewModel(
                pageIndex: pageIndex,
                pageSize: pageSize,
                categoryIds: categoryIds,
                brandIds: brandIds,
                productIds: productIds,
                enabled: enabled,
                discontinued: discontinued,
                visibleIndividually: visibleIndividually,
                includeDiscontinuedButInStock: includeDiscontinuedButInStock,
                priceMin: priceMin,
                priceMax: priceMax,
                //keywords: applyKeywordSuggestion ? suggestedKeywords : keywords,
                keywords: keywords,
                searchDescriptions: searchDescriptions,
                useFullTextSearch: useFullTextSearch,
                fullTextMode: fullTextMode,
                applySearchAnalysis: applySearchAnalysis,
                orderBy: orderBy,
                displaySearchAnalysis: displaySearchAnalysis);

            if (string.IsNullOrEmpty(suggestedKeywords) == false && suggestedKeywords.ToLower().CompareTo(keywords.ToLower()) == 0)
                suggestedKeywords = null;

            var result = new SearchResultPagedList<ProductOverviewModel>(
                list.Items, list.PageIndex, list.PageSize, list.TotalCount, suggestedKeywords,
                list.MaxPriceFilterByKeyword, list.MinPriceFilterByKeyword);

            return result;
        }

        #endregion

        #region Return

        public SearchTerm GetSearchTermByQuery(string query)
        {
            var item = _searchTermRepository.Table
                    .Where(s => s.Query == query)
                    .FirstOrDefault();

            return item;
        }

        public PagedList<CustomDictionary> GetPagedCustomDictionary(
            int pageIndex = 0,
            int pageSize = 2147483647)
        {
            var query = _customDictionaryRepository.Table;
            
            int totalRecords = query.Count();
            
            query = query.OrderBy(x => x.Id);
            
            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var items = query.ToList();

            return new PagedList<CustomDictionary>(items, pageIndex, pageSize, totalRecords);
        }

        public CustomDictionary GetCustomDictionary(int id)
        {
            return _customDictionaryRepository.Return(id);
        }

        #endregion

        #region Update

        public void UpdateCustomDictionary(CustomDictionary word)
        {
            _customDictionaryRepository.Update(word);
        }

        #endregion

        #region Delete

        public void DeleteCustomDictionary(int id)
        {
            _customDictionaryRepository.Delete(id);
        }

        #endregion

        #region Create

        public int InsertCustomDictionary(CustomDictionary word)
        {
            return _customDictionaryRepository.Create(word);
        }

        #endregion
    }
}
