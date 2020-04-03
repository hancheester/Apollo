using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Apollo.Core.Services.Cms
{
    public class CampaignService : BaseRepository, ICampaignService
    {
        #region Fields
        
        private readonly IRepository<LargeBanner> _largeBannerRepository;
        private readonly IRepository<OfferBanner> _offerBannerRepository;
        private readonly IRepository<ProductGroupMapping> _productGroupMappingRepository;
        private readonly IRepository<ProductGroup> _productGroupRepository;
        private readonly IRepository<SearchTerm> _searchTermRepository;        
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public CampaignService(IRepository<LargeBanner> largeBannerRepository,
                               IRepository<OfferBanner> offerBannerRepository,
                               IRepository<ProductGroupMapping> productGroupMappingRepository,
                               IRepository<ProductGroup> productGroupRepository,
                               IRepository<SearchTerm> searchTermRepository,                               
                               ILogBuilder logBuilder,
                               ICacheManager cacheManager) 
        {
            _largeBannerRepository = largeBannerRepository;
            _offerBannerRepository = offerBannerRepository;
            _productGroupMappingRepository = productGroupMappingRepository;
            _productGroupRepository = productGroupRepository;
            _searchTermRepository = searchTermRepository;            
            _cacheManager = cacheManager;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion
        
        #region Create

        public int InsertSearchTerm(SearchTerm term)
        {
            return _searchTermRepository.Create(term);
        }

        public int InsertProductGroupMapping(ProductGroupMapping mapping)
        {
            return _productGroupMappingRepository.Create(mapping);
        }

        public int InsertLargeBanner(LargeBanner largeBanner)
        {
            return _largeBannerRepository.Create(largeBanner);
        }

        public int InsertOfferBanner(OfferBanner offerBanner)
        {
            return _offerBannerRepository.Create(offerBanner);
        }

        #endregion

        #region Update

        public void UpdateSearchTerm(SearchTerm term)
        {
            _searchTermRepository.Update(term);
        }

        public void UpdateLargeBanner(LargeBanner banner)
        {
            _largeBannerRepository.Update(banner);

            _cacheManager.RemoveByPattern(CacheKey.LARGE_BANNER_PATTERN_KEY);

        }

        public void UpdateProductGroupMapping(ProductGroupMapping mapping)
        {
            _productGroupMappingRepository.Update(mapping);
        }

        public void UpdateOfferBanner(OfferBanner banner)
        {
            _offerBannerRepository.Update(banner);
        }

        #endregion

        #region Return

        public IList<ProductGroup> GetProductGroups()
        {
            return _productGroupRepository.Table.OrderBy(x => x.Name).ToList();
        }

        public ProductGroupMapping GetProductGroupMappingById(int productGroupMappingId)
        {
            return _productGroupMappingRepository.Return(productGroupMappingId);
        }

        public ProductGroup GetProductGroupById(int productGroupId)
        {
            return _productGroupRepository.Return(productGroupId);
        }

        public SearchTerm GetSearchTerm(int searchTermId)
        {
            return _searchTermRepository.Return(searchTermId);
        }

        public PagedList<SearchTerm> GetSearchTermLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> searchTermsIds = null,
            string queryValue = null,
            string redirectUrl = null,
            SearchTermSortingBy orderBy = SearchTermSortingBy.IdAsc)
        {
            var query = _searchTermRepository.Table;

            if (searchTermsIds != null && searchTermsIds.Count > 0)
                query = query.Where(x => searchTermsIds.Contains(x.Id));

            if (!string.IsNullOrWhiteSpace(queryValue))
                query = query.Where(x => x.Query.Contains(queryValue));

            if (!string.IsNullOrWhiteSpace(redirectUrl))
                query = query.Where(x => x.RedirectUrl.Contains(redirectUrl));

            int totalRecords = query.Count();

            switch (orderBy)
            {
                case SearchTermSortingBy.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case SearchTermSortingBy.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case SearchTermSortingBy.QueryAsc:
                    query = query.OrderBy(x => x.Query);
                    break;
                case SearchTermSortingBy.QueryDesc:
                    query = query.OrderByDescending(x => x.Query);
                    break;
                case SearchTermSortingBy.RedirectUrlAsc:
                    query = query.OrderBy(x => x.RedirectUrl);
                    break;
                case SearchTermSortingBy.RedirectUrlDesc:
                    query = query.OrderByDescending(x => x.RedirectUrl);
                    break;
                default:
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList();

            return new PagedList<SearchTerm>(list, pageIndex, pageSize, totalRecords);
        }

        public PagedList<ProductGroupMapping> GetFeaturedItemLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            FeaturedItemSortingType orderBy = FeaturedItemSortingType.IdAsc)
        {
            var query = _productGroupMappingRepository.Table;
            
            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case FeaturedItemSortingType.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case FeaturedItemSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case FeaturedItemSortingType.PriorityAsc:
                    query = query.OrderBy(x => x.Priority);
                    break;
                case FeaturedItemSortingType.PriorityDesc:
                    query = query.OrderByDescending(x => x.Priority);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList();

            return new PagedList<ProductGroupMapping>(list, pageIndex, pageSize, totalRecords);
        }

        public PagedList<OfferBanner> GetOfferBannerLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string title = null,
            bool? enabled = null,
            string fromDate = null,
            string toDate = null,
            BannerSortingType orderBy = BannerSortingType.IdAsc)
        {
            var query = _offerBannerRepository.Table;

            if (!string.IsNullOrEmpty(title))
                query = query.Where(x => x.Title.Contains(title));

            if (enabled.HasValue)
                query = query.Where(x => x.Enabled == enabled.Value);

            DateTime startDate;
            if (!string.IsNullOrEmpty(fromDate)
                && DateTime.TryParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                query = query.Where(x => startDate <= x.StartDate);
            }

            DateTime endDate;
            if (!string.IsNullOrEmpty(toDate)
                && DateTime.TryParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                query = query.Where(x => endDate >= x.EndDate);
            }

            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case BannerSortingType.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case BannerSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case BannerSortingType.TitleAsc:
                    query = query.OrderBy(x => x.Title);
                    break;
                case BannerSortingType.TitleDesc:
                    query = query.OrderByDescending(x => x.Title);
                    break;
                case BannerSortingType.PriorityAsc:
                    query = query.OrderBy(x => x.Priority);
                    break;
                case BannerSortingType.PriorityDesc:
                    query = query.OrderByDescending(x => x.Priority);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList();

            return new PagedList<OfferBanner>(list, pageIndex, pageSize, totalRecords);
        }

        public PagedList<LargeBanner> GetLargeBannerLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string title = null,
            bool? enabled = null,
            string fromDate = null,
            string toDate = null,
            BannerSortingType orderBy = BannerSortingType.IdAsc)
        {
            var query = _largeBannerRepository.Table;

            if (!string.IsNullOrEmpty(title))
                query = query.Where(x => x.Title.Contains(title));

            if (enabled.HasValue)
                query = query.Where(x => x.Enabled == enabled.Value);

            DateTime startDate;
            if (!string.IsNullOrEmpty(fromDate)
                && DateTime.TryParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                query = query.Where(x => startDate >= x.StartDate);
            }

            DateTime endDate;
            if (!string.IsNullOrEmpty(toDate)
                && DateTime.TryParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                query = query.Where(x => endDate <= x.EndDate);
            }

            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case BannerSortingType.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case BannerSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case BannerSortingType.TitleAsc:
                    query = query.OrderBy(x => x.Title);
                    break;
                case BannerSortingType.TitleDesc:
                    query = query.OrderByDescending(x => x.Title);
                    break;
                case BannerSortingType.PriorityAsc:
                    query = query.OrderBy(x => x.Priority);
                    break;
                case BannerSortingType.PriorityDesc:
                    query = query.OrderByDescending(x => x.Priority);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList();

            return new PagedList<LargeBanner>(list, pageIndex, pageSize, totalRecords);
        }
        
        public IList<OfferBanner> GetActiveOfferBanners()
        {
            var list = _offerBannerRepository.Table.Where(o => o.Enabled == true)
                .Where(o => o.Enabled == true)
                .Where(o =>
                    (o.StartDate == null || o.StartDate.Value.CompareTo(DateTime.Now) <= 0)
                    &&
                    (o.EndDate == null || o.EndDate == null || o.EndDate.Value.CompareTo(DateTime.Now) >= 0))
                .OrderBy(o => o.Priority)
                .ToList();

            return list;
        }

        public IList<LargeBanner> GetActiveLargeBanners(BannerDisplayType type)
        {
            string key = string.Format(CacheKey.LARGE_BANNER_ALL_ACTIVE_BY_DISPLAY_TYPE_KEY, type);

            return _cacheManager.GetWithExpiry(key, delegate ()
            {
                var query = _largeBannerRepository.Table
                    .Where(o => o.Enabled == true);

                switch (type)
                {
                    case BannerDisplayType.OffersPage:
                        query = query.Where(o => o.DisplayOnOffersPage == true);
                        break;
                    default:
                    case BannerDisplayType.HomePage:
                        query = query.Where(o => o.DisplayOnHomePage == true);
                        break;
                }
                var list = query.Where(o =>
                    (o.StartDate == null || o.StartDate.Value.CompareTo(DateTime.Now) <= 0)
                    &&
                    (o.EndDate == null || o.EndDate.Value.CompareTo(DateTime.Now) >= 0))
                    .OrderBy(o => o.Priority)
                    .ToList();

                return list;
            },
            expiredEndOfDay: true);
        }

        public OfferBanner GetOfferBannerById(int id)
        {
            var banner = _offerBannerRepository.TableNoTracking.Where(x => x.Id == id).FirstOrDefault();
            return banner;
        }

        public LargeBanner GetLargeBannerById(int largeBannerId)
        {
            return _largeBannerRepository.Return(largeBannerId);
        }

        #endregion

        #region Delete

        public void DeleteSearchTerm(int searchTermId)
        {
            _searchTermRepository.Delete(searchTermId);
        }

        public void DeleteLargeBanner(int largeBannerId)
        {
            _largeBannerRepository.Delete(largeBannerId);

            _cacheManager.RemoveByPattern(CacheKey.LARGE_BANNER_PATTERN_KEY);
        }

        public void DeleteProductGroupMapping(int productGroupMappingId)
        {
            _productGroupMappingRepository.Delete(productGroupMappingId);
        }

        public void DeleteOfferBanner(int offerBannerId)
        {
            _offerBannerRepository.Delete(offerBannerId);
        }

        #endregion
    }
}
