using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface IProductService
    {
        TaxCategory GetTaxCategory(int taxCategoryId);
        IList<Product> GetActiveProducts(bool? isGoogleProductSearchDisabled = null, bool visibleIndividually = true);
        void UpdateProductGoogleTaxonomy(int productId, int googleTaxonomyId);
        string GenerateGoogleProductFeed(string countryCode);
        IList<ProductOverviewModel> GetActiveProductOverviewModelByCategoryFeaturedItemType(int categoryId, int featuredItemType);
        IList<ProductOverviewModel> GetActiveProductOverviewModelByBrandFeaturedItemType(int brandId, int featuredItemType);
        IList<ProductOverviewModel> GetHomepageProductByGroupId(int groupId);
        ProductOverviewModel GetProductOverviewModelByUrlRewrite(string urlKey);
        IList<ProductReview> GetApprovedProductReviewsByProductId(int productId);
        IList<ProductTag> GetProductTagsByProductId(int productId);
        IList<ProductMedia> GetProductMediaByProductId(int productId);
        decimal[] GetPriceRangeByCategory(IList<int> categoryIds);
        SearchResultPagedList<ProductOverviewModel> GetPagedProductOverviewModelsByBrandCategoryIds(
            int brandId,
            int pageIndex = 0,
            int pageSize = 2147483647, //Int32.MaxValue
            IList<int> brandCategoryIds = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            bool? enabled = null,
            bool? visibleIndividually = null,
            bool includeDiscontinuedButInStock = false,
            ProductSortingType orderBy = ProductSortingType.Position);
        SearchResultPagedList<ProductOverviewModel> GetPagedProductOverviewModelsByBrandCategory(
            int brandId,
            int pageIndex = 0,
            int pageSize = 2147483647, //Int32.MaxValue
            string topUrlKey = null,
            string secondUrlKey = null,
            string thirdUrlKey = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            bool? enabled = null,
            bool? visibleIndividually = null,
            bool includeDiscontinuedButInStock = false,
            ProductSortingType orderBy = ProductSortingType.Position);
        IList<ProductOverviewModel> GetActiveProductOverviewModelsByBrandId(int brandId);
        bool BelongToThisProductGroup(int productId, int productGroupId);
        decimal CalculateProductPopularity(int productId, int lastNumberOfDays);
        int CalculateProductPriority(int productId, string keywords);
        decimal CalculateProductDisplayRank(int productId, int priority, decimal popularity);
        void DeleteProductMedia(int productMediaId);
        void DeleteProductReviews(IList<int> productReviewIds);
        void AutoGenerateFeaturedItemsByCategory(int categoryId, int featuredItemType, int quantity);
        void AutoGenerateFeaturedItemsByBrand(int brandId, int featuredItemType, int quantity);
        void DeleteProductPrice(int productPriceId);
        Colour GetColour(int colourId);
        PagedList<Colour> GetColourLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> colourIds = null,
            string name = null,
            string brand = null,
            ColourSortingType orderBy = ColourSortingType.IdAsc);
        PagedList<ProductOverviewModel> GetPagedProductOverviewModelsByCategoryFilter(
            int categoryFilterId,
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> productIds = null,
            string name = null,
            ProductSortingType orderBy = ProductSortingType.Position);
        PagedList<ProductOverviewModel> GetPagedProductOverviewModelsByCategoryHierarchy(
            int pageIndex = 0,
            int pageSize = 2147483647,
            int? productId = null,
            string name = null,
            int? categoryId = null,
            int? featuredItemType = null,
            int? notFeaturedItemType = null,
            ProductSortingType orderBy = ProductSortingType.Position);
        PagedList<ProductOverviewModel> GetPagedProductOverviewModelsByBrand(
            int pageIndex = 0,
            int pageSize = 2147483647,
            int? productId = null,
            string name = null,
            int? brandId = null,
            int? featuredItemType = null,
            int? notFeaturedItemType = null,
            ProductSortingType orderBy = ProductSortingType.Position);
        IList<TaxCategory> GetTaxCategories();
        int[] GetPrevNextProductId(int productId);
        Product GetProductByUrlKey(string urlKey);
        int GetProductCountByCategory(int categoryId);
        bool GetProductDiscontinuedStatus(int productId);
        ProductOverviewModel GetProductOverviewModelById(int productId);
        PagedList<ProductPriceOverviewModel> GetPagedProductPriceOverviewModels(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> productIds = null,
            string name = null,
            string barcode = null,
            bool? productEnabled = null,
            ProductPriceSortingType orderBy = ProductPriceSortingType.ProductIdAsc);
        ProductPriceOverviewModel GetProductPriceOverviewModel(int productPriceId);
        ProductPriceOverviewModel GetProductPriceOverviewModelByBarcode(string barcode);
        void DeleteProductReview(int productReviewId);
        ProductReview GetProductReview(int productReviewId);
        Product GetProductByProductPriceId(int productPriceId);
        PagedList<ProductReview> GetProductReviewLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> productReviewIds = null,
            string alias = null,
            string comment = null,
            bool isPending = false,
            ProductReviewSortingType orderBy = ProductReviewSortingType.IdAsc);
        IList<RestrictedGroup> GetRestrictedGroups();
        IList<Tag> GetTagList();
        ProductMedia GetProductMedia(int productMediaId);
        int InsertColour(Colour colour);
        ProductPrice GetProductPrice(int productPriceId, DateTimeOffset? absoluteExpiration = null);
        int InsertProduct(Product product);
        int InsertProductMedia(ProductMedia media);        
        int InsertProductPrice(ProductPrice price);
        int InsertProductReview(ProductReview review);
        void DeleteProductRestrictedGroup(int productId, int restrictedGroupId);
        int InsertProductRestrictedGroup(RestrictedGroupMapping productRestrictedGroup);
        int InsertProductTag(ProductTag newProductTag);
        IList<Tuple<int, string>> ProcessBulkProductInsertion(IList<BulkProductsInfo> items);
        bool ProcessProductDeletion(int productId);
        void UpdateColour(Colour colour);
        void UpdateProduct(Product product);
        void UpdateProductPrice(ProductPrice productPrice);
        void UpdateProductPriceOverviewModels(IList<ProductPriceOverviewModel> productPrices);
        void UpdateProductPrimaryImage(int productId, int productMediaId);
        void UpdateProductReview(ProductReview review);
        void UpdateProductDiscontinuedStatusByProductIdList(IList<int> list, bool discontinued);
        void UpdateProductStatusByProductIdList(IList<int> list, bool status);
        void UpdateProductReviews(IList<int> list, bool approved);
        void DeleteProductTag(int productId, int tagId);
        void UpdateProductTag(ProductTag productTag);
        void SaveProductGoogleCustomLabelGroup(ProductGoogleCustomLabelGroupMapping googleCustomLabelGroup);
        void SaveProductGoogleCustomLabelGroups(IList<ProductGoogleCustomLabelGroupMapping> googleCustomLabelGroups);
        IList<ProductPrice> GetProductPricesByProductId(int productId, bool enabled = true);
        string GenerateAffiliateWindowProductFeed();
        Product GetProductById(int id);
        ProductOverviewModel GetActiveProductOverviewModelByName(string name);
        SearchResultPagedList<ProductOverviewModel> GetPagedProductOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> categoryIds = null,
            IList<int> categoryFilterIds = null,
            IList<int> brandIds = null,
            IList<int> productIds = null,
            IList<int> brandCategoryIds = null,
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
            int? notCategoryId = null,
            int? notBrandId = null,
            int? notBrandCategoryId = null,
            int? notCategoryFilterId = null,
            ProductSortingType orderBy = ProductSortingType.Position,
            bool displaySearchAnalysis = false);
        PagedList<ProductGoogleCustomLabelGroupMappingOverviewModel> GetProductGoogleCustomLabelGroupLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> productIds = null,
            string name = null,
            string customLabel1 = null,
            string customLabel2 = null,
            string customLabel3 = null,
            string customLabel4 = null,
            string customLabel5 = null,
            string value1 = null,
            string value2 = null,
            string value3 = null,
            string value4 = null,
            string value5 = null);
        void ToggleProductMediaStatus(int productMediaId);
        ProductGoogleCustomLabelGroupMapping GetProductGoogleCustomLabelGroup(int productId);
        int GetBrandIdByProductPriceId(int productPriceId);
    }
}
