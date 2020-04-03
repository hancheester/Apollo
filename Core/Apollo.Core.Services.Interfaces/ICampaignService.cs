using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface ICampaignService
    {
        PagedList<SearchTerm> GetSearchTermLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> searchTermsIds = null,
            string queryValue = null,
            string redirectUrl = null,
            SearchTermSortingBy orderBy = SearchTermSortingBy.IdAsc);
        void DeleteSearchTerm(int searchTermId);
        SearchTerm GetSearchTerm(int searchTermId);
        void UpdateSearchTerm(SearchTerm term);
        int InsertSearchTerm(SearchTerm term);
        ProductGroup GetProductGroupById(int productGroupId);
        PagedList<ProductGroupMapping> GetFeaturedItemLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            FeaturedItemSortingType orderBy = FeaturedItemSortingType.IdAsc);
        ProductGroupMapping GetProductGroupMappingById(int productGroupMappingId);
        void UpdateProductGroupMapping(ProductGroupMapping mapping);
        IList<ProductGroup> GetProductGroups();
        void DeleteProductGroupMapping(int productGroupMappingId);
        int InsertProductGroupMapping(ProductGroupMapping mapping);
        PagedList<LargeBanner> GetLargeBannerLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string title = null,
            bool? enabled = null,
            string fromDate = null,
            string toDate = null,
            BannerSortingType orderBy = BannerSortingType.IdAsc);
        LargeBanner GetLargeBannerById(int largeBannerId);
        void DeleteLargeBanner(int largeBannerId);
        void UpdateLargeBanner(LargeBanner largeBanner);
        int InsertLargeBanner(LargeBanner largeBanner);
        PagedList<OfferBanner> GetOfferBannerLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string title = null,
            bool? enabled = null,
            string fromDate = null,
            string toDate = null,
            BannerSortingType orderBy = BannerSortingType.IdAsc);
        OfferBanner GetOfferBannerById(int offerBannerId);
        void DeleteOfferBanner(int offerBannerId);
        void UpdateOfferBanner(OfferBanner offerBanner);
        int InsertOfferBanner(OfferBanner offerBanner);
        IList<OfferBanner> GetActiveOfferBanners();
        IList<LargeBanner> GetActiveLargeBanners(BannerDisplayType type);
    }
}
