using Apollo.Core.Caching;
using Apollo.Core.Domain.Media;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Common;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Apollo.Core.Services.Catalog
{
    public class ProductService : BaseRepository, IProductService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IRepository<CategoryFilter> _categoryFilterRepository;
        private readonly IRepository<CategoryFeaturedItem> _categoryFeaturedItemRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<BrandFeaturedItem> _brandFeaturedItemRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IRepository<ProductMedia> _productMediaRepository;
        private readonly IRepository<Colour> _colourRepository;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<LineItem> _lineItemRepository;
        private readonly IRepository<CartPharmItem> _cartPharmItemRepository;
        private readonly IRepository<Delivery> _deliveryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<ProductCategoryFilter> _productCategoryFilterRepository;        
        private readonly IRepository<OfferRule> _offerRuleRepository;
        private readonly IRepository<OfferRelatedItem> _offerRelatedItemRepository;
        private readonly IRepository<RestrictedGroup> _restrictedGroupRepository;
        private readonly IRepository<RestrictedGroupMapping> _restrictedGroupMappingRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<ProductGroup> _productGroupRepository;
        private readonly IRepository<ProductGroupMapping> _productGroupMappingRepository;
        private readonly IRepository<ProductGoogleCustomLabelGroupMapping> _productGoogleCustomLabelGroupMappingRepository;
        private readonly IRepository<TaxCategoryMapping> _taxCategoryMappingRepository;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly IBrandService _brandService;
        private readonly IOrderService _orderService;
        private readonly IOfferService _offerService;
        private readonly ICategoryService _categoryService;
        private readonly ICacheManager _cacheManager;
        private readonly IEnumerable<Lazy<IFeedGenerator, IFeedGeneratorMetadata>> _generators;        
        private readonly IProductBuilder _productBuilder;
        private readonly ILogger _logger;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public ProductService(
            IDbContext dbContext,
            IRepository<CategoryFilter> categoryFilterRepository,
            IRepository<CategoryFeaturedItem> categoryFeaturedItemRepository,
            IRepository<Brand> brandRepository,
            IRepository<BrandFeaturedItem> brandFeaturedItemRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Product> productRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductPrice> productPriceRepository,
            IRepository<ProductMedia> productMediaRepository,
            IRepository<Colour> colourRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<CartItem> cartItemRepository,
            IRepository<LineItem> lineItemRepository,
            IRepository<CartPharmItem> cartPharmItemRepository,
            IRepository<Delivery> deliveryRepository,
            IRepository<ProductCategory> productCategoryRepository,            
            IRepository<SearchTerm> searchTermRepository,
            IRepository<Tag> tagRepository,
            IRepository<ProductCategoryFilter> productCategoryFilterRepository,
            IRepository<OfferRule> offerRuleRepository,
            IRepository<OfferRelatedItem> offerRelatedItemRepository,
            IRepository<RestrictedGroup> restrictedGroupRepository,
            IRepository<RestrictedGroupMapping> restrictedGroupMappingRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<ProductGroup> productGroupRepository,
            IRepository<ProductGroupMapping> productGroupMappingRepository,
            IRepository<ProductGoogleCustomLabelGroupMapping> productGoogleCustomLabelGroupMappingRepository,
            IRepository<TaxCategoryMapping> taxCategoryMappingRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IBrandService brandService,
            IOrderService orderService,
            IOfferService offerService,
            ICategoryService categoryService,
            ICacheManager cacheManager,
            ILogBuilder logBuilder,
            MediaSettings mediaSettings,
            IEnumerable<Lazy<IFeedGenerator, IFeedGeneratorMetadata>> generators,
            IProductBuilder productBuilder)
        {
            _dbContext = dbContext;
            _categoryFilterRepository = categoryFilterRepository;
            _categoryFeaturedItemRepository = categoryFeaturedItemRepository;
            _brandRepository = brandRepository;
            _brandFeaturedItemRepository = brandFeaturedItemRepository;
            _currencyRepository = currencyRepository;
            _productRepository = productRepository;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeMappingRepository = productAttributeMappingRepository;
            _productPriceRepository = productPriceRepository;
            _productMediaRepository = productMediaRepository;
            _colourRepository = colourRepository;
            _productReviewRepository = productReviewRepository;
            _deliveryRepository = deliveryRepository;
            _productCategoryRepository = productCategoryRepository;
            _searchTermRepository = searchTermRepository;
            _tagRepository = tagRepository;
            _productCategoryFilterRepository = productCategoryFilterRepository;
            _offerRuleRepository = offerRuleRepository;
            _offerRelatedItemRepository = offerRelatedItemRepository;
            _restrictedGroupRepository = restrictedGroupRepository;
            _restrictedGroupMappingRepository = restrictedGroupMappingRepository;
            _productTagRepository = productTagRepository;
            _productGroupRepository = productGroupRepository;
            _productGroupMappingRepository = productGroupMappingRepository;
            _productGoogleCustomLabelGroupMappingRepository = productGoogleCustomLabelGroupMappingRepository;
            _taxCategoryMappingRepository = taxCategoryMappingRepository;
            _taxCategoryRepository = taxCategoryRepository;
            _cartItemRepository = cartItemRepository;
            _lineItemRepository = lineItemRepository;
            _cartPharmItemRepository = cartPharmItemRepository;
            _brandService = brandService;
            _orderService = orderService;
            _offerService = offerService;
            _categoryService = categoryService;
            _cacheManager = cacheManager;
            _mediaSettings = mediaSettings;
            _generators = generators;
            _productBuilder = productBuilder;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        #region Return

        public TaxCategory GetTaxCategory(int taxCategoryId)
        {
            return _taxCategoryRepository.Return(taxCategoryId);
        }

        public IList<TaxCategory> GetTaxCategories()
        {
            return _taxCategoryRepository.Table.ToList();
        }

        public int[] GetPrevNextProductId(int productId)
        {
            return new int[] { GetPrevProductId(productId), GetNextProductId(productId) };
        }

        public IList<RestrictedGroup> GetRestrictedGroups()
        {
            return _restrictedGroupRepository.Table.ToList();
        }

        public RestrictedGroup GetRestrictedGroupById(int id)
        {
            return _restrictedGroupRepository.Return(id);
        }

        public Colour GetColour(int colourId)
        {
            return _colourRepository.Return(colourId);
        }

        public ProductMedia GetProductMedia(int productMediaId)
        {
            return _productMediaRepository.Return(productMediaId);
        }

        public IList<Tag> GetTagList()
        {
            return _tagRepository.Table.ToList();
        }

        public ProductOverviewModel GetActiveProductOverviewModelByName(string name)
        {
            name = name.ToLower().Trim();

            var id = _productRepository.TableNoTracking
                .Where(x => x.Name.ToLower().CompareTo(name) == 0)
                .Where(x => x.Enabled == true)
                .Select(x => x.Id)
                .FirstOrDefault();

            if (id != 0)
            {
                var product = GetProductById(id);                
                var model = BuildProductOverviewModel(product);
                return model;
            }

            return null;
        }

        public IList<ProductOverviewModel> GetActiveProductOverviewModelByCategoryFeaturedItemType(int categoryId, int featuredItemType)
        {
            var items = _categoryFeaturedItemRepository.Table
                .Join(_productRepository.Table, cf => cf.ProductId, p => p.Id, (cf, p) => new { cf, p })
                .Where(x => x.cf.CategoryId == categoryId)
                .Where(x => x.cf.FeaturedItemType == featuredItemType)
                .Where(x => x.p.Enabled == true)
                .OrderBy(x => x.cf.Priority)
                .Select(x => x.p.Id)
                .ToList();
            
            var models = new List<ProductOverviewModel>();

            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var product = GetProductById(items[i]);
                    models.Add(BuildProductOverviewModel(product));
                }
            }

            return models;
        }

        public IList<ProductOverviewModel> GetActiveProductOverviewModelByBrandFeaturedItemType(int brandId, int featuredItemType)
        {
            var items = _brandFeaturedItemRepository.Table
                .Join(_productRepository.Table, bf => bf.ProductId, p => p.Id, (bf, p) => new { bf, p })
                .Where(x => x.bf.BrandId == brandId)
                .Where(x => x.bf.FeaturedItemType == featuredItemType)
                .Where(x => x.p.Enabled == true)
                .OrderBy(x => x.bf.Priority)
                .Select(x => x.p.Id)
                .ToList();

            var models = new List<ProductOverviewModel>();

            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var product = GetProductById(items[i]);
                    models.Add(BuildProductOverviewModel(product));
                }
            }

            return models;
        }

        public ProductReview GetProductReview(int productReviewId)
        {
            var review = _productReviewRepository.Return(productReviewId);

            if (review != null)
            {
                var product = _productRepository.Return(review.ProductId);
                if (product != null) review.ProductName = product.Name;
            }

            return review;
        }

        public PagedList<ProductReview> GetProductReviewLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> productReviewIds = null,
            string alias = null,
            string comment = null,
            bool isPending = false,
            ProductReviewSortingType orderBy = ProductReviewSortingType.IdAsc)
        {
            var query = _productReviewRepository.TableNoTracking
                 .Join(_productRepository.TableNoTracking, pr => pr.ProductId, p => p.Id, (pr, p) => new { pr, p });

            if (productReviewIds != null && productReviewIds.Count > 0)
                query = query.Where(x => productReviewIds.Contains(x.pr.Id));

            if (!string.IsNullOrEmpty(alias))
                query = query.Where(x => x.pr.Alias.Contains(alias));

            if (!string.IsNullOrEmpty(comment))
                query = query.Where(x => x.pr.Comment.Contains(comment));

            if (isPending == true)
            {
                query = query.Where(x => x.pr.Approved == false);
            }

            int totalRecords = query.Count();

            switch (orderBy)
            {
                case ProductReviewSortingType.IdAsc:
                    query = query.OrderBy(x => x.pr.Id);
                    break;
                case ProductReviewSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.pr.Id);
                    break;
                case ProductReviewSortingType.ProductNameAsc:
                    query = query.OrderBy(x => x.p.Name);
                    break;
                case ProductReviewSortingType.ProductNameDesc:
                    query = query.OrderByDescending(x => x.p.Name);
                    break;
                case ProductReviewSortingType.AliasAsc:
                    query = query.OrderBy(x => x.pr.Alias);
                    break;
                case ProductReviewSortingType.AliasDesc:
                    query = query.OrderByDescending(x => x.pr.Alias);
                    break;
                case ProductReviewSortingType.TimeStampAsc:
                    query = query.OrderBy(x => x.pr.TimeStamp);
                    break;
                case ProductReviewSortingType.TimeStampDesc:
                    query = query.OrderByDescending(x => x.pr.TimeStamp);
                    break;
                default:
                    break;
            }
            
            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList().Select(x => new ProductReview
            {
                Id = x.pr.Id,
                ProductId = x.pr.ProductId,
                ProfileId = x.pr.ProfileId,
                Alias = x.pr.Alias,
                Title = x.pr.Title,
                Comment = x.pr.Comment,
                Score = x.pr.Score,
                TimeStamp = x.pr.TimeStamp,
                Approved = x.pr.Approved,
                ProductName = x.p.Name
            });

            return new PagedList<ProductReview>(list, pageIndex, pageSize, totalRecords);
        }

        public PagedList<Colour> GetColourLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> colourIds = null,
            string name = null,
            string brand = null,
            ColourSortingType orderBy = ColourSortingType.IdAsc)
        {
            var query = _colourRepository.Table
                .Join(_brandRepository.Table, c => c.BrandId, b => b.Id, (c, b) => new { c, b });

            if (colourIds != null && colourIds.Count > 0)
                query = query.Where(c_b => colourIds.Contains(c_b.b.Id));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(c_b => c_b.c.Value.Contains(name));

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(c_b => c_b.b.Name.Contains(brand));
            
            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case ColourSortingType.IdAsc:
                    query = query.OrderBy(c_b => c_b.c.Id);
                    break;
                case ColourSortingType.IdDesc:
                    query = query.OrderByDescending(c_b => c_b.c.Id);
                    break;
                case ColourSortingType.ValueAsc:
                    query = query.OrderBy(c_b => c_b.c.Value);
                    break;
                case ColourSortingType.ValueDesc:
                    query = query.OrderByDescending(c_b => c_b.c.Value);
                    break;
                case ColourSortingType.BrandNameAsc:
                    query = query.OrderBy(c_b => c_b.b.Name);
                    break;
                case ColourSortingType.BrandNameDesc:
                    query = query.OrderByDescending(c_b => c_b.b.Name);
                    break;
            }
                       
            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList().Select(c_b => new Colour
            {
                Id = c_b.c.Id,
                Value = c_b.c.Value,
                BrandId = c_b.c.BrandId,
                BrandName = c_b.b.Name,
                ColourFilename = c_b.c.ColourFilename,
                ThumbnailFilename = c_b.c.ThumbnailFilename
            });

            return new PagedList<Colour>(list, pageIndex, pageSize, totalRecords);
        }

        public SearchResultPagedList<ProductOverviewModel> GetPagedProductOverviewModelsByBrandCategoryIds(
            int brandId,
            int pageIndex = 0,
            int pageSize = 2147483647, //Int32.MaxValue
            IList<int> brandCategoryIds = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            bool? enabled = null,
            bool? visibleIndividually = null,
            bool includeDiscontinuedButInStock = false,
            ProductSortingType orderBy = ProductSortingType.Position)
        {
            var list = GetProductLoadPaged(pageIndex: pageIndex,
                                           pageSize: pageSize,
                                           brandIds: new int[] { brandId },
                                           brandCategoryIds: brandCategoryIds,
                                           priceMin: priceMin,
                                           priceMax: priceMax,
                                           enabled: enabled,
                                           visibleIndividually: visibleIndividually,
                                           includeDiscontinuedButInStock: includeDiscontinuedButInStock,
                                           orderBy: orderBy);

            var items = new Collection<ProductOverviewModel>();
            for (int i = 0; i < list.Items.Count; i++)
            {
                var item = _productBuilder.Build(list.Items[i].Id);
                items.Add(BuildProductOverviewModel(item));
            }

            // Price range here
            decimal[] priceRange;
            if (brandCategoryIds.Count > 0)
                priceRange = GetPriceRangeByBrandCategory(brandCategoryIds);
            else
                priceRange = GetPriceRangeByBrand(brandId);

            return new SearchResultPagedList<ProductOverviewModel>(items, pageIndex, pageSize, list.TotalCount,
                priceRange[1], priceRange[0]);
        }

        public SearchResultPagedList<ProductOverviewModel> GetPagedProductOverviewModelsByBrandCategory(
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
            ProductSortingType orderBy = ProductSortingType.Position)
        {
            var brandCategoryIds = _brandService.GetBrandCategoryIdList(
                brandId,
                topUrlKey,
                secondUrlKey,
                thirdUrlKey);

            return GetPagedProductOverviewModelsByBrandCategoryIds(brandId,
                                                                pageIndex,
                                                                pageSize,
                                                                brandCategoryIds,
                                                                priceMin,
                                                                priceMax,
                                                                enabled,
                                                                visibleIndividually,
                                                                includeDiscontinuedButInStock,
                                                                orderBy);
        }
        
        public PagedList<ProductOverviewModel> GetPagedProductOverviewModelsByCategoryHierarchy(
            int pageIndex = 0,
            int pageSize = 2147483647, //Int32.MaxValue
            int? productId = null,
            string name = null,
            int? categoryId = null,
            int? featuredItemType = null,
            int? notFeaturedItemType = null,
            ProductSortingType orderBy = ProductSortingType.Position)
        {
            var pPageIndex = GetParameter("PageIndex", pageIndex, true);
            var pPageSize = GetParameter("PageSize", pageSize);
            var pProductId = GetParameter("ProductId", productId);
            var pName = GetParameter("Name", name);
            var pCategoryId = GetParameter("CategoryId", categoryId);
            var pFeaturedItemType = GetParameter("FeaturedItemType", featuredItemType);
            var pNotFeaturedItemType = GetParameter("NotFeaturedItemType", notFeaturedItemType);
            var pOrderBy = GetParameter("OrderBy", (int)orderBy, true);
            var pTotalRecords = GetParameterIntegerOutput("TotalRecords");
            
            var products = _dbContext.ExecuteStoredProcedureList<Product>(
                    "ProductByCategoryHierarchy_LoadPaged",
                    pProductId,
                    pName,
                    pCategoryId,
                    pFeaturedItemType,
                    pNotFeaturedItemType,
                    pPageIndex,
                    pPageSize,
                    pOrderBy,
                    pTotalRecords);

            var items = new Collection<ProductOverviewModel>();
            foreach (var product in products)
            {
                var temp = _productBuilder.Build(product);
                items.Add(BuildProductOverviewModel(temp));
            }

            //return products
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            
            return new PagedList<ProductOverviewModel>(items, pageIndex, pageSize, totalRecords);
        }

        public PagedList<ProductOverviewModel> GetPagedProductOverviewModelsByBrand(
            int pageIndex = 0,
            int pageSize = 2147483647,
            int? productId = null,
            string name = null,
            int? brandId = null,
            int? featuredItemType = null,
            int? notFeaturedItemType = null,
            ProductSortingType orderBy = ProductSortingType.Position)
        {
            var pPageIndex = GetParameter("PageIndex", pageIndex, true);
            var pPageSize = GetParameter("PageSize", pageSize);
            var pProductId = GetParameter("ProductId", productId);
            var pName = GetParameter("Name", name);
            var pBrandId = GetParameter("BrandId", brandId);
            var pFeaturedItemType = GetParameter("FeaturedItemType", featuredItemType);
            var pNotFeaturedItemType = GetParameter("NotFeaturedItemType", notFeaturedItemType);
            var pOrderBy = GetParameter("OrderBy", (int)orderBy, true);
            var pTotalRecords = GetParameterIntegerOutput("TotalRecords");

            var products = _dbContext.ExecuteStoredProcedureList<Product>(
                    "ProductByBrand_LoadPaged",
                    pProductId,
                    pName,
                    pBrandId,
                    pFeaturedItemType,
                    pNotFeaturedItemType,
                    pPageIndex,
                    pPageSize,
                    pOrderBy,
                    pTotalRecords);

            var items = new Collection<ProductOverviewModel>();
            foreach (var product in products)
            {
                var temp = _productBuilder.Build(product);
                items.Add(BuildProductOverviewModel(temp));
            }

            //return products
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;

            return new PagedList<ProductOverviewModel>(items, pageIndex, pageSize, totalRecords);
        }

        public PagedList<ProductOverviewModel> GetPagedProductOverviewModelsByCategoryFilter(
            int categoryFilterId,
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> productIds = null,
            string name = null,            
            ProductSortingType orderBy = ProductSortingType.Position)
        {
            var query = _productCategoryFilterRepository.Table
                .Join(_productRepository.Table, cf => cf.ProductId, p => p.Id, (cf, p) => new { cf, p })
                .Where(x => x.cf.CategoryFilterId == categoryFilterId);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.p.Name.Contains(name));

            if (productIds != null && productIds.Count > 0)
                query = query.Where(x => productIds.Contains(x.cf.ProductId));

            int totalRecords = query.Count();

            switch (orderBy)
            {
                case ProductSortingType.NameAsc:
                    query = query.OrderBy(x => x.p.Name);
                    break;
                case ProductSortingType.NameDesc:
                    query = query.OrderByDescending(x => x.p.Name);
                    break;                
                case ProductSortingType.IdAsc:
                    query = query.OrderBy(x => x.cf.ProductId);
                    break;
                default:
                case ProductSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.cf.ProductId);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var products = query.Select(x => x.p).ToList();

            var items = new Collection<ProductOverviewModel>();
            foreach (var product in products)
            {
                var temp = _productBuilder.Build(product);
                items.Add(BuildProductOverviewModel(temp));
            }
            
            return new PagedList<ProductOverviewModel>(items, pageIndex, pageSize, totalRecords);
        }

        public PagedList<ProductPriceOverviewModel> GetPagedProductPriceOverviewModels(
            int pageIndex = 0,
            int pageSize = 2147483647, //Int32.MaxValue
            IList<int> productIds = null,
            string name = null,
            string barcode = null,
            bool? productEnabled = null,
            ProductPriceSortingType orderBy = ProductPriceSortingType.ProductIdAsc)
        {
            var list = GetProductPriceLoadPaged(
                pageIndex,
                pageSize,
                productIds,
                name,
                barcode,
                productEnabled,
                orderBy);

            var items = new Collection<ProductPriceOverviewModel>();
            for (int i = 0; i < list.Items.Count; i++)
            {
                items.Add(BuildProductPriceOverviewModel(list.Items[i]));
            }

            return new PagedList<ProductPriceOverviewModel>(items, pageIndex, pageSize, list.TotalCount);
        }

        public PagedList<ProductPrice> GetProductPriceLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> productIds = null,
            string name = null,
            string barcode = null,
            bool? productEnabled = null,
            ProductPriceSortingType orderBy = ProductPriceSortingType.ProductIdAsc)
        {
            var query = _productPriceRepository.Table
                .GroupJoin(_productRepository.Table, pp => pp.ProductId, p => p.Id, (pp, p) => new { pp, p })
                .SelectMany(x => x.p.DefaultIfEmpty(), (x, p) => new { x.pp, p });

            if (productIds != null && productIds.Count > 0)
                query = query.Where(x => productIds.Contains(x.pp.ProductId));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.p.Name.Contains(name));

            if (!string.IsNullOrEmpty(barcode))
                query = query.Where(x => x.pp.Barcode.Contains(barcode));

            if (productEnabled.HasValue)
                query = query.Where(x => x.p.Enabled == productEnabled.Value);

            int totalRecords = query.Count();

            switch (orderBy)
            {
                case ProductPriceSortingType.ProductIdAsc:
                    query = query.OrderBy(x => x.pp.ProductId);
                    break;
                case ProductPriceSortingType.ProductIdDesc:
                    query = query.OrderByDescending(x => x.pp.ProductId);
                    break;
                default:
                case ProductPriceSortingType.NameAsc:
                    query = query.OrderBy(x => x.p.Name);
                    break;
                case ProductPriceSortingType.NameDesc:
                    query = query.OrderByDescending(x => x.p.Name);
                    break;
                case ProductPriceSortingType.WeightAsc:
                    query = query.OrderBy(x => x.pp.Weight);
                    break;
                case ProductPriceSortingType.WeightDesc:
                    query = query.OrderByDescending(x => x.pp.Weight);
                    break;
                case ProductPriceSortingType.PriceAsc:
                    query = query.OrderBy(x => x.pp.Price);
                    break;
                case ProductPriceSortingType.PriceDesc:
                    query = query.OrderByDescending(x => x.pp.Price);
                    break;
                case ProductPriceSortingType.StockAsc:
                    query = query.OrderBy(x => x.pp.Stock);
                    break;
                case ProductPriceSortingType.StockDesc:
                    query = query.OrderByDescending(x => x.pp.Stock);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var prices = query.Select(x => x.pp).ToList();

            return new PagedList<ProductPrice>(prices, pageIndex, pageSize, totalRecords);
        }

        public PagedList<ProductGoogleCustomLabelGroupMappingOverviewModel> GetProductGoogleCustomLabelGroupLoadPaged(
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
            string value5 = null)
        {
            var query = _productRepository.Table
                .GroupJoin(_productGoogleCustomLabelGroupMappingRepository.Table, p => p.Id, g => g.ProductId, (p, g) => new { p, g })
                .SelectMany(x => x.g.DefaultIfEmpty(), (x, g) => new { x.p, g });

            if (productIds != null && productIds.Count > 0)
                query = query.Where(x => productIds.Contains(x.g.ProductId));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.p.Name.Contains(name));

            if (!string.IsNullOrEmpty(customLabel1))
                query = query.Where(x => x.g.CustomLabel1.Contains(customLabel1));

            if (!string.IsNullOrEmpty(customLabel2))
                query = query.Where(x => x.g.CustomLabel2.Contains(customLabel2));

            if (!string.IsNullOrEmpty(customLabel3))
                query = query.Where(x => x.g.CustomLabel3.Contains(customLabel3));

            if (!string.IsNullOrEmpty(customLabel4))
                query = query.Where(x => x.g.CustomLabel4.Contains(customLabel4));

            if (!string.IsNullOrEmpty(customLabel5))
                query = query.Where(x => x.g.CustomLabel5.Contains(customLabel5));

            if (!string.IsNullOrEmpty(value1))
                query = query.Where(x => x.g.Value1.Contains(value1));

            if (!string.IsNullOrEmpty(value2))
                query = query.Where(x => x.g.Value2.Contains(value2));

            if (!string.IsNullOrEmpty(value3))
                query = query.Where(x => x.g.Value3.Contains(value3));

            if (!string.IsNullOrEmpty(value4))
                query = query.Where(x => x.g.Value4.Contains(value4));

            if (!string.IsNullOrEmpty(value5))
                query = query.Where(x => x.g.Value5.Contains(value5));
            
            int totalRecords = query.Count();

            query = query.OrderBy(x => x.p.Name);

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var groups = query
                .Select(x => new ProductGoogleCustomLabelGroupMappingOverviewModel
                {
                    ProductId = x.p.Id,
                    Name = x.p.Name,
                    CustomLabel1 = x.g.CustomLabel1,
                    CustomLabel2 = x.g.CustomLabel2,
                    CustomLabel3 = x.g.CustomLabel3,
                    CustomLabel4 = x.g.CustomLabel4,
                    CustomLabel5 = x.g.CustomLabel5,
                    Value1 = x.g.Value1,
                    Value2 = x.g.Value2,
                    Value3 = x.g.Value3,
                    Value4 = x.g.Value4,
                    Value5 = x.g.Value5
                })
                .ToList();

            return new PagedList<ProductGoogleCustomLabelGroupMappingOverviewModel>(groups, pageIndex, pageSize, totalRecords);
        }

        public ProductPrice GetProductPrice(int productPriceId, DateTimeOffset? absoluteExpiration = null)
        {
            if (productPriceId == 0) return null;
            string key = string.Format(CacheKey.PRODUCT_PRICE_BY_PRODUCT_PRICE_ID_KEY, productPriceId);

            return _cacheManager.GetWithExpiry(key, delegate()
            {
                // We need to get a fully loaded product which contains possible discount.
                var productId = _productPriceRepository.Table
                .Where(pp => pp.Id == productPriceId)
                .Select(pp => pp.ProductId)
                .FirstOrDefault();
                
                var product = GetProductById(productId);
                if (product == null) return null;

                return product.ProductPrices.Where(pp => pp.Id == productPriceId).FirstOrDefault();
            },
            absoluteExpiration);            
        }

        /// <summary>
        /// At the moment, this method is used by store admin (get_item_handler.aspx.cs) which doesn't need discount information.
        /// </summary>
        /// <param name="barcode">Barcode</param>
        /// <returns></returns>
        public ProductPriceOverviewModel GetProductPriceOverviewModelByBarcode(string barcode)
        {
            if (string.IsNullOrEmpty(barcode)) return null;

            var item = _productPriceRepository.Table
                .Where(pp => pp.Barcode == barcode)
                //.Where(pp => pp.Enabled == true)
                .FirstOrDefault();

            if (item != null)
                return BuildProductPriceOverviewModel(item);

            return null;
        }

        public ProductPriceOverviewModel GetProductPriceOverviewModel(int productPriceId)
        {
            if (productPriceId == 0) return null;
            var productPrice = GetProductPrice(productPriceId);

            if (productPrice != null)
                return BuildProductPriceOverviewModel(productPrice);

            return null;
        }

        public decimal[] GetPriceRangeByCategory(IList<int> categoryIds)
        {
            if (categoryIds == null) return new decimal[] { 0M, 0M };

            var items = _productPriceRepository.Table
                .Join(_productCategoryRepository.Table, pp => pp.ProductId, pc => pc.ProductId, (pp, pc) => new { pp, pc })
                .Join(_productRepository.Table, pp_pc => pp_pc.pp.ProductId, p => p.Id, (pp_pc, p) => new { pp_pc.pp, pp_pc.pc, p })
                .Where(pp_pc_p => categoryIds.Contains(pp_pc_p.pc.CategoryId))
                .Where(pp_pc_p => pp_pc_p.p.Enabled == true)
                .Where(pp_p => pp_p.pp.Enabled == true)
                .GroupBy(pp_pc_p => pp_pc_p.p.Id)
                .Select(pp_p => new { min = pp_p.Min(a => a.pp.Price), max = pp_p.Max(a => a.pp.Price) })
                .ToList();

            if (items.Count > 0)
                return new decimal[] { items.Min(x => x.min), items.Max(x => x.max) };
            else
                return new decimal[] { 0M, 0M };
        }
        
        public SearchResultPagedList<ProductOverviewModel> GetPagedProductOverviewModel(
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
            bool displaySearchAnalysis = false)
        {
            var list = GetProductLoadPaged(pageIndex: pageIndex,
                                           pageSize: pageSize,
                                           categoryIds: categoryIds,
                                           categoryFilterIds: categoryFilterIds,
                                           brandIds: brandIds,
                                           productIds: productIds,
                                           brandCategoryIds: brandCategoryIds,
                                           enabled: enabled,
                                           discontinued: discontinued,
                                           visibleIndividually: visibleIndividually,
                                           includeDiscontinuedButInStock: includeDiscontinuedButInStock,
                                           priceMin: priceMin,
                                           priceMax: priceMax,
                                           keywords: keywords,
                                           searchDescriptions: searchDescriptions,
                                           useFullTextSearch: useFullTextSearch,
                                           fullTextMode: fullTextMode,
                                           applySearchAnalysis: applySearchAnalysis,
                                           notCategoryId: notCategoryId,
                                           notBrandId: notBrandId,
                                           notBrandCategoryId: notBrandCategoryId,
                                           notCategoryFilterId: notCategoryFilterId,
                                           orderBy: orderBy);

            var items = new Collection<ProductOverviewModel>();
            for (int i = 0; i < list.Items.Count; i++)
            {
                var item = _productBuilder.Build(list.Items[i]);

                if (displaySearchAnalysis)
                {
                    var priority = CalculateProductPriority(item.Id, keywords);
                    // TODO: Last number of days should be in search setting.
                    var popularity = CalculateProductPopularity(item.Id, 90);
                    var displayRank = CalculateProductDisplayRank(item.Id, priority, popularity);

                    items.Add(BuildProductOverviewModel(item, priority, popularity, displayRank));
                }
                else
                {
                    items.Add(BuildProductOverviewModel(item));
                }                
            }

            return new SearchResultPagedList<ProductOverviewModel>(items, pageIndex, pageSize, list.TotalCount,
                list.MaxPriceFilterByKeyword, list.MinPriceFilterByKeyword);
        }

        public SearchResultPagedList<Product> GetProductLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> categoryIds = null,
            IList<int> categoryFilterIds = null,
            IList<int> brandIds = null,
            IList<int> brandCategoryIds = null,
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
            int? notCategoryId = null,
            int? notBrandId = null,
            int? notBrandCategoryId = null,
            int? notCategoryFilterId = null,
            ProductSortingType orderBy = ProductSortingType.Position)
        {
            var result = new SearchResultPagedList<Product>(new List<Product>());
            try
            {
                var pPageIndex = GetParameter("PageIndex", pageIndex, true);
                var pPageSize = GetParameter("PageSize", pageSize);
                var pEnabled = GetParameter("Enabled", enabled);
                var pDiscontinued = GetParameter("Discontinued", discontinued);
                var pVisibleIndividually = GetParameter("VisibleIndividually", visibleIndividually);
                var pIncludeDiscontinuedButInStock = GetParameter("IncludeDiscontinuedButInStock", includeDiscontinuedButInStock);
                var pPriceMin = GetParameter("PriceMin", priceMin);
                var pPriceMax = GetParameter("PriceMax", priceMax);
                var pKeywords = GetParameter("Keywords", keywords);
                var pSearchDescriptions = GetParameter("SearchDescriptions", searchDescriptions);
                var pUseFullTextSearch = GetParameter("UseFullTextSearch", useFullTextSearch);
                var pFullTextMode = GetParameter("FullTextMode", (int)fullTextMode);
                var pApplySearchAnalysis = GetParameter("ApplySearchAnalysis", applySearchAnalysis);
                var pNotCategoryId = GetParameter("NotCategoryId", notCategoryId);
                var pNotCategoryFilterId = GetParameter("NotCategoryFilterId", notCategoryFilterId);
                var pNotBrandId = GetParameter("NotBrandId", notBrandId);
                var pNotBrandCategoryId = GetParameter("NotBrandCategoryId", notBrandCategoryId);

                var pOrderBy = GetParameter("OrderBy", (int)orderBy, true);
                var pTotalRecords = GetParameterIntegerOutput("TotalRecords");
                var pMaxPriceFilterByKeyword = GetParameterIntegerOutput("MaxPriceFilterByKeyword");
                var pMinPriceFilterByKeyword = GetParameterIntegerOutput("MinPriceFilterByKeyword");

                if (categoryIds != null && categoryIds.Contains(0))
                    categoryIds.Remove(0);
                //pass category identifiers as comma-delimited string
                var commaSeparatedCategoryIds = categoryIds == null ? "" : string.Join(",", categoryIds);
                var pCategoryIds = GetParameter("CategoryIds", commaSeparatedCategoryIds);

                if (categoryFilterIds != null && categoryFilterIds.Contains(0))
                    categoryFilterIds.Remove(0);
                var commaSeparatedCategoryFilterIds = categoryFilterIds == null ? "" : string.Join(",", categoryFilterIds);
                var pCategoryFilterIds = GetParameter("CategoryFilterIds", commaSeparatedCategoryFilterIds);

                if (brandIds != null && brandIds.Contains(0))
                    brandIds.Remove(0);
                //pass brand identifiers as comma-delimited string
                var commaSeparatedBrandIds = brandIds == null ? "" : string.Join(",", brandIds);
                var pBrandIds = GetParameter("BrandIds", commaSeparatedBrandIds);

                if (brandCategoryIds != null && brandCategoryIds.Contains(0))
                    brandCategoryIds.Remove(0);
                //pass brand category identifiers as comma-delimited string
                var commaSeparatedBrandCategoryIds = brandCategoryIds == null ? "" : string.Join(",", brandCategoryIds);
                var pBrandCategoryIds = GetParameter("BrandCategoryIds", commaSeparatedBrandCategoryIds);

                if (productIds != null && productIds.Contains(0))
                    productIds.Remove(0);
                //pass product identifiers as comma-delimited string
                var commaSeparatedProductIds = productIds == null ? "" : string.Join(",", productIds);
                var pProductIds = GetParameter("ProductIds", commaSeparatedProductIds);

                var products = _dbContext.ExecuteStoredProcedureList<Product>(
                    "Product_LoadPaged",
                    pCategoryIds,
                    pNotCategoryId,
                    pCategoryFilterIds,
                    pNotCategoryFilterId,
                    pBrandIds,
                    pNotBrandId,
                    pBrandCategoryIds,
                    pNotBrandCategoryId,
                    pProductIds,
                    pEnabled,
                    pDiscontinued,
                    pVisibleIndividually,
                    pIncludeDiscontinuedButInStock,
                    pPriceMin,
                    pPriceMax,
                    pKeywords,
                    pSearchDescriptions,
                    pUseFullTextSearch,
                    pFullTextMode,
                    pOrderBy,
                    pPageIndex,
                    pPageSize,
                    pApplySearchAnalysis,
                    pTotalRecords,
                    pMaxPriceFilterByKeyword,
                    pMinPriceFilterByKeyword);

                //return products
                var totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
                var maxPriceFilterByKeyword = (pMaxPriceFilterByKeyword.Value != DBNull.Value) ? Convert.ToDecimal(pMaxPriceFilterByKeyword.Value) : 0M;
                var minPriceFilterByKeyword = (pMinPriceFilterByKeyword.Value != DBNull.Value) ? Convert.ToDecimal(pMinPriceFilterByKeyword.Value) : 0M;
                
                result = new SearchResultPagedList<Product>(products, pageIndex, pageSize, totalRecords, maxPriceFilterByKeyword, minPriceFilterByKeyword);
            }
            catch (SqlException sqlEx)
            {
                _logger.InsertLog(
                    LogLevel.Error, 
                    string.Format(@"{0}. 
                        Enabled={{{1}}}, 
                        Discontinued={{{2}}}, 
                        IncludeDiscontinuedButInStock={{{3}}}, 
                        PriceMin={{{4}}}, 
                        PriceMax={{{5}}}, 
                        Keywords={{{6}}},
                        SearchDescriptions={{{7}}},
                        UseFullTextSearch={{{8}}},
                        FullTextMode={{{9}}},
                        ApplySearchAnalysis={{{10}}},
                        NotCategoryId={{{11}}},
                        NotBrandId={{{12}}},
                        NotBrandCategoryId={{{13}}},
                        NotCategoryFilterId={{{14}}}", 
                        sqlEx.Message,
                        enabled,
                        discontinued,
                        includeDiscontinuedButInStock,
                        priceMin,
                        priceMax,
                        keywords,
                        searchDescriptions,
                        useFullTextSearch,
                        fullTextMode,
                        applySearchAnalysis,
                        notCategoryId,
                        notBrandId,
                        notBrandCategoryId,
                        notCategoryFilterId), 
                    sqlEx);
                return new SearchResultPagedList<Product>(new List<Product>());
            }

            return result;            
        }

        public bool GetProductDiscontinuedStatus(int productId)
        {
            bool discontinued = _productRepository.Table
                .Where(p => p.Id == productId)
                .Select(p => p.Discontinued)
                .FirstOrDefault();
            return discontinued;
        }
        
        public int GetPrevProductId(int productId)
        {
            int id = _productRepository.Table
                        .Where(p => p.Id < productId)
                        .OrderByDescending(p => p.Id)
                        .Take(1)
                        .Select(p => p.Id)
                        .SingleOrDefault();

            return id;
        }

        public int GetNextProductId(int productId)
        {
            int id = _productRepository.Table
                        .Where(p => p.Id > productId)
                        .OrderBy(p => p.Id)
                        .Take(1)
                        .Select(p => p.Id)
                        .SingleOrDefault();

            return id;
        }

        public IList<Product> GetOfferRelatedProductByOfferRuleUrlKey(string offerRuleUrlKey)
        {
            var list = _offerRelatedItemRepository.Table
                .Join(_offerRuleRepository.Table, or => or.OfferRuleId, o => o.Id, (or, o) => new { or, o })
                .Join(_productRepository.Table, or_o => or_o.or.ProductId, p => p.Id, (or_o, p) => new { or_o.or, or_o.o, p })
                .Where(or_o_p => or_o_p.o.UrlRewrite == offerRuleUrlKey)
                .Where(or_o_p => or_o_p.or.Enabled == true)
                .OrderBy(or_o_p => or_o_p.or.Priority)
                .Select(or_o_p => or_o_p.p.Id)
                .ToList();

            var products = new List<Product>();

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    products.Add(GetProductById(list[i]));
                }
            }

            return products;
        }

        public Product GetProductById(int id)
        {
            if (id == 0) return null;
            return _productBuilder.Build(id);
        }

        public Product GetProductByProductPriceId(int productPriceId)
        {
            var productId = _productPriceRepository.Table.Where(x => x.Id == productPriceId).Select(x => x.ProductId).FirstOrDefault();
            return GetProductById(productId);
        }

        public int GetBrandIdByProductPriceId(int productPriceId)
        {
            return _productPriceRepository.Table
                .Join(_productRepository.Table, pp => pp.ProductId, p => p.Id, (pp, p) => new { pp, p })
                .Where(x => x.pp.Id == productPriceId)
                .Select(x => x.p.BrandId)
                .FirstOrDefault();
        }

        public ProductOverviewModel GetProductOverviewModelByUrlRewrite(string urlKey)
        {
            var product = GetProductByUrlKey(urlKey);
            return BuildProductOverviewModel(product);
        }

        public ProductOverviewModel GetProductOverviewModelById(int productId)
        {
            var product = GetProductById(productId);
            return BuildProductOverviewModel(product);
        }
        
        public IList<ProductOverviewModel> GetActiveProductOverviewModelsByBrandId(int brandId)
        {
            var items = _productRepository.Table
                .Where(p => p.BrandId == brandId)
                .Where(p => p.Enabled == true)
                .Select(p => p.Id)
                .ToList();

            var models = new List<ProductOverviewModel>();

            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var product = GetProductById(items[i]);                    
                    models.Add(BuildProductOverviewModel(product));
                }
            }

            return models;
        }

        public Product GetProductByUrlKey(string urlKey)
        {
            if (string.IsNullOrEmpty(urlKey)) return null;

            string key = string.Format(CacheKey.PRODUCT_BY_URL_KEY, urlKey);

            return _cacheManager.GetWithExpiry(key, delegate ()
            {
                var productId = _productRepository.Table.Where(p => p.UrlRewrite == urlKey).Select(p => p.Id).FirstOrDefault();

                if (productId != 0)
                {
                    return GetProductById(productId);
                }

                return null;
            });
        }      
        
        public int GetProductCountByBrand(int brandId)
        {
            var list = _productRepository.Table.Where(p => p.BrandId == brandId).ToList();

            if (list != null)
                return list.Count;

            return 0;
        }
        
        public IList<ProductMedia> GetProductMediaListByProductIdList(List<int> list)
        {
            List<ProductMedia> listMedia = new List<ProductMedia>();

            foreach (var id in list)
            {
                var medias = _productMediaRepository.Table.Where(m => m.ProductId == id).ToList();
                if (medias != null) listMedia.AddRange(medias);
            }

            return listMedia.Count > 0 ? listMedia : null;
        }

        public IList<ProductMedia> GetProductMediaByProductId(int productId)
        {
            var list = _productMediaRepository.TableNoTracking.Where(m => m.ProductId == productId).ToList();
            return list;
        }

        public IList<RestrictedGroup> GetRestrictedGroupByProductId(int productId)
        {
            return _restrictedGroupMappingRepository.Table
                .Join(_restrictedGroupRepository.Table, m => m.RestrictedGroupId, g => g.Id, (m, g) => new { m, g })
                .Where(x => x.m.ProductId == productId)
                .Select(x => x.g)
                .ToList();
        }
        
        public int GetProductCountByCategory(int categoryId)
        {
            int count = _productCategoryRepository.Table
                .Where(pc => pc.CategoryId == categoryId)
                .Select(pc => pc.Id)
                .Count();
            return count;
        }

        public string GetProductNameByProductId(int id)
        {
            Product product = _productRepository.Return(id);

            if (product != null) return product.Name;
            return null;
        }
        
        public decimal CalculateProductPopularity(int productId, int lastNumberOfDays)
        {
            var pProductId = GetParameter("ProductId", productId);
            var pLastNumberOfDays = GetParameter("LastNumberOfDays", lastNumberOfDays);

            var popularity = _dbContext.SqlQuery<decimal>("SELECT dbo.CalculateProductPopularity(@ProductId, @LastNumberOfDays);", pProductId, pLastNumberOfDays);

            return popularity.FirstOrDefault();
        }
        
        public int CalculateProductPriority(int productId, string keywords)
        {
            var pProductId = GetParameter("ProductId", productId);
            var pKeywords = GetParameter("Keywords", keywords);

            var priority = _dbContext.SqlQuery<int>("SELECT dbo.CalculateProductPriority(@ProductId, @Keywords);", pProductId, pKeywords);

            return priority.FirstOrDefault();
        }

        public decimal CalculateProductDisplayRank(int productId, int priority, decimal popularity)
        {
            var pProductId = GetParameter("ProductId", productId);
            var pPriority = GetParameter("Priority", priority, includeZero: true);
            var pPopularity = GetParameter("Popularity", popularity, includeZero: true);

            var displayRank = _dbContext.SqlQuery<decimal>("SELECT dbo.CalculateProductDisplayRank(@ProductId, @Priority, @Popularity);", pProductId, pPriority, pPopularity);

            return displayRank.FirstOrDefault();
        }

        public IList<ProductPrice> GetProductPricesByProductId(int productId, bool enabled = true)
        {
            var product = GetProductById(productId);
            return product.ProductPrices.Where(x => x.Enabled == enabled).ToList();
        }
        
        public IList<ProductOverviewModel> GetHomepageProductByGroupId(int groupId)
        {
            var list = _productRepository.Table
                .Join(_productGroupMappingRepository.Table, p => p.Id, pg => pg.ProductId, (p, pg) => new { p, pg })
                .Where(p_pg => p_pg.pg.ProductGroupId == groupId)
                .Where(p_pg => p_pg.p.VisibleIndividually == true)
                .OrderBy(p_pg => p_pg.pg.Priority)
                .Select(p_pg => p_pg.p.Id)
                .ToList();

            var items = new List<ProductOverviewModel>();
            for (int i = 0; i < list.Count; i++)
            {
                var product = GetProductById(list[i]);                
                items.Add(BuildProductOverviewModel(product));
            }

            return items;
        }

        public bool BelongToThisProductGroup(int productId, int productGroupId)
        {
            var count = _productGroupMappingRepository.Table
                       .Join(_productGroupRepository.Table, pgm => pgm.ProductGroupId, pg => pg.Id, (pgm, pg) => new { pgm, pg })
                       .Where(x => x.pgm.ProductId == productId)
                       .Where(x => x.pg.Id == productGroupId)
                       .Count();

            return count > 0;
        }

        public IList<ProductTag> GetProductTagsByProductId(int productId)
        {
            var items = _productTagRepository.Table.Where(x => x.ProductId == productId).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    item.Tag = _tagRepository.Return(item.TagId);
                }
            }

            return items;
        }

        public IList<ProductReview> GetApprovedProductReviewsByProductId(int productId)
        {
            var items = _productReviewRepository.Table
                .Where(x => x.ProductId == productId)
                .Where(x => x.Approved == true)
                .OrderByDescending(x => x.TimeStamp)
                .ToList();

            return items;
        }

        public IList<Product> GetActiveProducts(bool? isGoogleProductSearchDisabled = null, bool visibleIndividually = true)
        {
            var query = _productRepository.Table
                .Where(p => p.Enabled == true)
                .Where(p => p.VisibleIndividually == visibleIndividually);

            if (isGoogleProductSearchDisabled.HasValue)
            {
                query.Where(p => p.IsGoogleProductSearchDisabled == isGoogleProductSearchDisabled.Value);
            }
            
            var productIds = query.Select(p => p.Id).ToList();
            var products = new List<Product>();

            if (productIds.Count == 0) return products;

            for (int i = 0; i < productIds.Count; i++)
            {
                products.Add(GetProductById(productIds[i]));
            }

            return products;
        }

        public ProductGoogleCustomLabelGroupMapping GetProductGoogleCustomLabelGroup(int productId)
        {
            return _productGoogleCustomLabelGroupMappingRepository.Table
                .Where(x => x.ProductId == productId)
                .FirstOrDefault();
        }
        
        #endregion

        #region Create

        public int InsertProductReview(ProductReview review)
        {
            return _productReviewRepository.Create(review);
        }

        public int InsertColour(Colour colour)
        {
            return _colourRepository.Create(colour);
        }

        public int InsertProductRestrictedGroup(RestrictedGroupMapping productRestrictedGroup)
        {
            var id = _restrictedGroupMappingRepository.Create(productRestrictedGroup);

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);

            return id;
        }
        
        public int InsertProduct(Product product)
        {
            int id = _productRepository.Create(product);

            if (product.TaxCategory != null)
            {
                var taxCategoryMapping = new TaxCategoryMapping { ProductId = id, TaxCategoryId = product.TaxCategory.Id };
                _taxCategoryMappingRepository.Create(taxCategoryMapping);
            }

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);

            return id;          
        }

        public int InsertProductPrice(ProductPrice price)
        {
            var id = _productPriceRepository.Create(price);

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);

            return id;
        }

        public int InsertProductMedia(ProductMedia media)
        {
            var id = _productMediaRepository.Create(media);

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);

            return id;
        }

        public int InsertProductTag(ProductTag productTag)
        {
            var id = _productTagRepository.Create(productTag);

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);

            return id;
        }

        #endregion

        #region Delete

        public void DeleteProductMedia(int productMediaId)
        {
            var media = _productMediaRepository.Return(productMediaId);
            if (media != null)
            {
                var productMediaPath = _mediaSettings.ProductMediaLocalPath;
                var mediaFilePath = productMediaPath + media.MediaFilename;
                var thumbnailFilePath = productMediaPath + media.ThumbnailFilename;
                var highResFilenamePath = productMediaPath + media.HighResFilename;

                if (File.Exists(mediaFilePath)) File.Delete(mediaFilePath);
                if (File.Exists(thumbnailFilePath)) File.Delete(thumbnailFilePath);
                if (File.Exists(highResFilenamePath)) File.Delete(highResFilenamePath);

                _productMediaRepository.Delete(media);
            }

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
        }

        public void DeleteProductTag(int productId, int tagId)
        {
            var tag = _productTagRepository.Table
                        .Where(pt => pt.ProductId == productId && pt.TagId == tagId)
                        .FirstOrDefault();

            if (tag != null) _productTagRepository.Delete(tag);

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
        }

        public void DeleteProductPrice(int productPriceId)
        {
            var productPrice = _productPriceRepository.Return(productPriceId);
            if (productPrice != null) _productPriceRepository.Delete(productPriceId);

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PRICE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
        }
        
        public void DeleteProductRestrictedGroup(int productId, int restrictedGroupId)
        {
            var items = _restrictedGroupMappingRepository.Table
                .Where(x => x.ProductId == productId)
                .Where(x => x.RestrictedGroupId == restrictedGroupId)
                .ToList();

            for(int i = 0; i < items.Count; i++)
            {
                _restrictedGroupMappingRepository.Delete(items[i]);
            }

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
        }

        public void DeleteProductReview(int productReviewId)
        {
            _productReviewRepository.Delete(productReviewId);
        }

        public void DeleteProductReviews(IList<int> productReviewIds)
        {
            if (productReviewIds.Count > 0)
            {
                foreach (var id in productReviewIds)
                {
                    _productReviewRepository.Delete(id);
                }
            }            
        }

        #endregion

        #region Update

        public void SaveProductGoogleCustomLabelGroup(ProductGoogleCustomLabelGroupMapping googleCustomLabelGroup)
        {
            var item = _productGoogleCustomLabelGroupMappingRepository.Table.Where(x => x.ProductId == googleCustomLabelGroup.ProductId).FirstOrDefault();
            if (item != null)
            {
                item.CustomLabel1 = googleCustomLabelGroup.CustomLabel1;
                item.CustomLabel2 = googleCustomLabelGroup.CustomLabel2;
                item.CustomLabel3 = googleCustomLabelGroup.CustomLabel3;
                item.CustomLabel4 = googleCustomLabelGroup.CustomLabel4;
                item.CustomLabel5 = googleCustomLabelGroup.CustomLabel5;

                item.Value1 = googleCustomLabelGroup.Value1;
                item.Value2 = googleCustomLabelGroup.Value2;
                item.Value3 = googleCustomLabelGroup.Value3;
                item.Value4 = googleCustomLabelGroup.Value4;
                item.Value5 = googleCustomLabelGroup.Value5;

                _productGoogleCustomLabelGroupMappingRepository.Update(item);                
            }
            else
            {
                _productGoogleCustomLabelGroupMappingRepository.Create(googleCustomLabelGroup);
            }

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
        }

        public void SaveProductGoogleCustomLabelGroups(IList<ProductGoogleCustomLabelGroupMapping> googleCustomLabelGroups)
        {
            foreach (var item in googleCustomLabelGroups)
            {
                SaveProductGoogleCustomLabelGroup(item);
            }           
        }

        public void UpdateProductGoogleTaxonomy(int productId, int googleTaxonomyId)
        {
            var product = _productRepository.Return(productId);
            if (product != null)
            {
                product.GoogleTaxonomyId = googleTaxonomyId;
                _productRepository.Update(product);

                _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
            }
        }

        public void UpdateColour(Colour colour)
        {
            _colourRepository.Update(colour);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.Update(product);

            if (product.TaxCategory != null)
            {
                var existingMappings =_taxCategoryMappingRepository.Table.Where(x => x.ProductId == product.Id).ToList();
                if (existingMappings != null && existingMappings.Count > 0)
                {
                    foreach (var mapping in existingMappings)
                    {
                        _taxCategoryMappingRepository.Delete(mapping);
                    }
                }

                var taxCategoryMapping = new TaxCategoryMapping { ProductId = product.Id, TaxCategoryId = product.TaxCategory.Id };
                _taxCategoryMappingRepository.Create(taxCategoryMapping);
            }

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
        }

        public void UpdateProductReview(ProductReview review)
        {
            _productReviewRepository.Update(review);
        }

        public void UpdateProductReviews(IList<int> list, bool approved)
        {
            if (list.Count > 0)
            {
                foreach (var id in list)
                {
                    var review = _productReviewRepository.Return(id);

                    if (review != null)
                    {
                        review.Approved = approved;
                        _productReviewRepository.Update(review);
                    }
                }

                _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
            }
        }

        public void UpdateProductStatusByProductIdList(IList<int> list, bool status)
        {
            if (list.Count > 0)
            {
                foreach (var id in list)
                {
                    var product = _productRepository.Return(id);

                    if (product != null)
                    {
                        product.Enabled = status;
                        _productRepository.Update(product);
                    }
                }

                _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
            }
        }
        
        public void UpdateProductPriceOverviewModels(IList<ProductPriceOverviewModel> models)
        {
            for (int i = 0; i < models.Count; i++)
            {
                var option = _productPriceRepository.Return(models[i].Id);

                if (option != null)
                {
                    // We choose fields below based on store admin requirement.
                    option.Price = models[i].Price;
                    option.Stock = models[i].Stock;
                    option.Weight = models[i].Weight;
                    option.Barcode = models[i].Barcode;

                    _productPriceRepository.Update(option);
                }
            }
        }

        public void UpdateProductPrice(ProductPrice productPrice)
        {
            _productPriceRepository.Update(productPrice);

            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PRICE_PATTERN_KEY);
        }

        public void UpdateProductDiscontinuedStatusByProductIdList(IList<int> list, bool discontinued)
        {
            if (list.Count > 0)
            {
                foreach (var id in list)
                {
                    var product = _productRepository.Return(id);

                    if (product != null)
                    {
                        product.Discontinued = discontinued;
                        _productRepository.Update(product);
                    }
                }

                _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
            }
        }

        public void UpdateProductPrimaryImage(int productId, int productMediaId)
        {
            var items = _productMediaRepository.TableNoTracking.Where(x => x.ProductId == productId).ToList();
            if (items != null)
            {
                for(int i = 0; i < items.Count(); i++)                
                {
                    items[i].PrimaryImage = items[i].Id == productMediaId;
                    _productMediaRepository.Update(items[i]);
                }
            }
        }

        public void UpdateProductTag(ProductTag productTag)
        {
            _productTagRepository.Update(productTag);
            _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
        }

        public void ToggleProductMediaStatus(int productMediaId)
        {
            var media = _productMediaRepository.Return(productMediaId);
            if (media != null)
            {
                media.Enabled = !media.Enabled;
                _productMediaRepository.Update(media);

                _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
            }
        }

        #endregion

        #region Command

        public bool ProcessProductDeletion(int productId)
        {
            // Check if we have this product in orders
            var count = _lineItemRepository.TableNoTracking
                .Where(x => x.ProductId == productId)
                .Count();

            if (count > 0) return false;

            // Remove product info
            _productRepository.Delete(productId);

            // Remove prices
            var prices = GetProductPricesByProductId(productId);
            for(int i = 0; i < prices.Count; i++)
            {
                DeleteProductPrice(prices[i].Id);
            }

            // Remove media
            var medias = GetProductMediaByProductId(productId);
            for (int j = 0; j < medias.Count; j++)
            {
                DeleteProductMedia(medias[j].Id);
            }
            
            // Remove product tags
            var productTags = GetProductTagsByProductId(productId);
            for (int l = 0; l < productTags.Count; l++)
            {
                DeleteProductTag(productId, productTags[l].TagId);
            }

            // Remove product reviews
            var productReviews = _productReviewRepository.TableNoTracking.Where(x => x.ProductId == productId).ToList();
            for (int m = 0; m < productReviews.Count; m++)
            {
                _productReviewRepository.Delete(productReviews[m].Id);
            }

            // Remove product Google Shopping custom labels
            var labels = _productGoogleCustomLabelGroupMappingRepository.TableNoTracking.Where(x => x.ProductId == productId).ToList();
            for (int n = 0; n < labels.Count; n++)
            {
                _productGoogleCustomLabelGroupMappingRepository.Delete(labels[n].Id);
            }

            // Remove product from cart items
            var cartItems = _cartItemRepository.TableNoTracking.Where(x => x.ProductId == productId).Select(x => x.Id).ToList();
            for (int n = 0; n < cartItems.Count; n++)
            {
                _cartItemRepository.Delete(cartItems[n]);
            }

            // Remove product from cart pharm items
            var cartPharmItems = _cartPharmItemRepository.TableNoTracking.Where(x => x.ProductId == productId).Select(x => x.Id).ToList();
            for (int o = 0; o < cartPharmItems.Count; o++)
            {
                _cartPharmItemRepository.Delete(cartPharmItems[o]);
            }
            
            return true;
        }
        
        public void AutoGenerateFeaturedItemsByCategory(int categoryId, int featuredItemType, int quantity)
        {
            var orderBy = ProductSortingType.Position;

            switch ((FeaturedItemType)featuredItemType)
            {
                case FeaturedItemType.WhatsNew:
                    orderBy = ProductSortingType.CreatedOn;
                    break;
                case FeaturedItemType.TopRated:
                    orderBy = ProductSortingType.ReviewScoreDesc;
                    break;
                case FeaturedItemType.BestSeller:
                    orderBy = ProductSortingType.SoldQuantityDesc;
                    break;
                default:
                    break;
            }

            var result = GetPagedProductOverviewModelsByCategoryHierarchy(pageSize: quantity, categoryId: categoryId, orderBy: orderBy);

            if (result.Items.Count > 0)
            {
                var pCategoryId = GetParameter("CategoryId", categoryId);
                var pFeaturedItemType = GetParameter("FeaturedItemType", featuredItemType);

                _dbContext.ExecuteSqlCommand("DELETE FROM CategoryFeaturedItem WHERE CategoryId = @CategoryId AND FeaturedItemType = @FeaturedItemType",
                    pCategoryId, pFeaturedItemType);

                foreach (var item in result.Items)
                {
                    var newFeaturedItem = new CategoryFeaturedItem
                    {
                        CategoryId = categoryId,
                        FeaturedItemType = featuredItemType,
                        ProductId = item.Id
                    };

                    _categoryService.InsertCategoryFeaturedItem(newFeaturedItem);
                }
            }
        }

        public void AutoGenerateFeaturedItemsByBrand(int brandId, int featuredItemType, int quantity)
        {
            var orderBy = ProductSortingType.Position;

            switch ((FeaturedItemType)featuredItemType)
            {
                case FeaturedItemType.WhatsNew:
                    orderBy = ProductSortingType.CreatedOn;
                    break;
                case FeaturedItemType.TopRated:
                    orderBy = ProductSortingType.ReviewScoreDesc;
                    break;
                case FeaturedItemType.BestSeller:
                    orderBy = ProductSortingType.SoldQuantityDesc;
                    break;
                default:
                    break;
            }

            var result = GetPagedProductOverviewModel(pageSize: quantity, brandIds: new int[] { brandId }, orderBy: orderBy);

            if (result.Items.Count > 0)
            {
                var pBrandId = GetParameter("BrandId", brandId);
                var pFeaturedItemType = GetParameter("FeaturedItemType", featuredItemType);

                _dbContext.ExecuteSqlCommand("DELETE FROM BrandFeaturedItem WHERE BrandId = @BrandId AND FeaturedItemType = @FeaturedItemType",
                    pBrandId, pFeaturedItemType);

                foreach (var item in result.Items)
                {
                    var newFeaturedItem = new BrandFeaturedItem
                    {
                        BrandId = brandId,
                        FeaturedItemType = featuredItemType,
                        ProductId = item.Id
                    };

                    _brandService.InsertBrandFeaturedItem(newFeaturedItem);
                }
            }
        }

        public IList<Tuple<int, string>> ProcessBulkProductInsertion(IList<BulkProductsInfo> items)
        {
            if (items.Count == 0) return new List<Tuple<int, string>>();

            var list = new List<Tuple<int, string>>();

            for(int i = 0; i < items.Count; i++)
            {
                var name = GetParameter("Name", items[i].Name);
                var description = GetParameter("Description", items[i].Description);
                var brandid = GetParameter("BrandId", items[i].BrandId);
                var urlrewrite = GetParameter("UrlRewrite", items[i].UrlRewrite);
                var stepquantity = GetParameter("StepQuantity", items[i].StepQuantity);
                var productcode = GetParameter("ProductCode", items[i].ProductCode);
                var deliveryid = GetParameter("DeliveryId", items[i].DeliveryId);
                var restrictedgroupid = GetParameter("RestrictedGroupId", items[i].RestrictedGroupId);
                var categoryId = GetParameter("CategoryId", items[i].CategoryId);
                var optionType = GetParameter("OptionType", items[i].OptionType, includeZero: true);
                var productEnabled = GetParameter("ProductEnabled", items[i].ProductEnabled);
                var visibleIndividually = GetParameter("VisibleIndividually", items[i].VisibleIndividually);
                var isPharmaceutical = GetParameter("IsPharmaceutical", items[i].IsPharmaceutical);
                var hasFreeWrapping = GetParameter("HasFreeWrapping", items[i].HasFreeWrapping);
                var openForOffer = GetParameter("OpenForOffer", items[i].OpenForOffer);
                var discontinued = GetParameter("Discontinued", items[i].Discontinued);
                var enforceStockCount = GetParameter("EnforceStockCount", items[i].EnforceStockCount);
                var isGoogleProductSearchDisabled = GetParameter("IsGoogleProductSearchDisabled", items[i].IsGoogleProductSearchDisabled);
                var isPhoneOrder = GetParameter("IsPhoneOrder", items[i].IsPhoneOrder);
                var taxCategoryId = GetParameter("TaxCategoryId", items[i].TaxCategoryId);
                var priceCode = GetParameter("PriceCode", items[i].PriceCode);
                var price = GetParameter("Price", items[i].Price);
                var size = GetParameter("Size", items[i].Size);
                var colourId = GetParameter("ColourId", items[i].ColourId);
                var barcode = GetParameter("Barcode", items[i].Barcode);
                var weight = GetParameter("Weight", items[i].Weight);
                var priceEnabled = GetParameter("PriceEnabled", items[i].PriceEnabled);
                var stock = GetParameter("Stock", items[i].Stock, includeZero: true);
                var maximumAllowedPurchaseQuantity = GetParameter("MaximumAllowedPurchaseQuantity", items[i].MaximumAllowedPurchaseQuantity);
                var mediatype = GetParameter("MediaType", items[i].MediaType);
                var mediaFilename = GetParameter("MediaFileName", items[i].MediaFileName);
                var thumbnailFilename = GetParameter("ThumbnailFileName", items[i].ThumbnailFileName);
                var highResFilename = GetParameter("HighResFileName", items[i].HighResFileName);
                var tagActiveIngredientsValue = GetParameter("TagActiveIngredientsValue", items[i].TagActiveIngredientsValue);
                var tagApplicationValue = GetParameter("TagApplicationValue", items[i].TagApplicationValue);
                var tagDirectionForUseValue = GetParameter("TagDirectionForUseValue", items[i].TagDirectionForUseValue);
                var tagPrecautionValue = GetParameter("TagPrecautionValue", items[i].TagPrecautionValue);
                var tagIndicationsValue = GetParameter("TagIndicationsValue", items[i].TagIndicationsValue);
                var tagAllergenInfoValue = GetParameter("TagAllergenInfoValue", items[i].TagAllergenInfoValue);

                var productId = _dbContext.SqlQuery<int>(@"EXEC BulkProductsUpload @Name, @Description, @BrandId, @UrlRewrite, @StepQuantity, @ProductCode, @DeliveryId, 
                                                            @RestrictedGroupId, @CategoryId, @OptionType, @ProductEnabled, @VisibleIndividually, @IsPharmaceutical,
	                                                        @HasFreeWrapping, @OpenForOffer, @Discontinued, @EnforceStockCount, @IsGoogleProductSearchDisabled,
	                                                        @IsPhoneOrder, @TaxCategoryId, @PriceCode, @Price, @Size, @ColourId, @Barcode, 
                                                            @Weight, @PriceEnabled, @Stock, @MaximumAllowedPurchaseQuantity, @MediaType, @MediaFileName, @ThumbnailFileName, @HighResFilename, 
                                                            @TagActiveIngredientsValue, @TagApplicationValue, @TagDirectionForUseValue, @TagPrecautionValue, @TagIndicationsValue,
                                                            @TagAllergenInfoValue",
                                                            name, description, brandid, urlrewrite, stepquantity, productcode, deliveryid, restrictedgroupid, categoryId, optionType,
                                                            productEnabled, visibleIndividually, isPharmaceutical, hasFreeWrapping, openForOffer, discontinued, enforceStockCount, isGoogleProductSearchDisabled,
                                                            isPhoneOrder, taxCategoryId, priceCode, price, size, colourId, barcode, weight, priceEnabled, stock, maximumAllowedPurchaseQuantity, mediatype, mediaFilename,
                                                            thumbnailFilename, highResFilename, tagActiveIngredientsValue, tagApplicationValue, tagDirectionForUseValue, tagPrecautionValue,
                                                            tagIndicationsValue, tagAllergenInfoValue).FirstOrDefault();

                var productName = _productRepository.TableNoTracking.Where(x => x.Id == productId).Select(x => x.Name).FirstOrDefault();

                list.Add(new Tuple<int, string>(productId, productName));
            }
            
            return list;
        }

        public string GenerateGoogleProductFeed(string countryCode)
        {
            var products = GetActiveProducts(isGoogleProductSearchDisabled: false);

            if (products.Count <= 0) return string.Empty;
            
            var xml = string.Empty;
            var generator = _generators.FirstOrDefault(t => t.Metadata.Type == FeedGeneratorType.Google);
            if (generator != null && generator.Value != null)
                xml = generator.Value.BuildFeed(products, countryCode);

            return xml;
        }

        public string GenerateAffiliateWindowProductFeed()
        {
            var products = GetActiveProducts();

            if (products.Count <= 0) return string.Empty;

            var xml = string.Empty;
            var generator = _generators.FirstOrDefault(t => t.Metadata.Type == FeedGeneratorType.AffilicateWindow);
            if (generator != null && generator.Value != null)
                xml = generator.Value.BuildFeed(products);

            return xml;
        }
        
        #endregion

        #region Product attributes

        public void DeleteProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Delete(productAttribute);            
        }

        public PagedList<ProductAttribute> GetAllProductAttributes(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //string key = string.Format(PRODUCTATTRIBUTES_ALL_KEY, pageIndex, pageSize);
            //return _cacheManager.Get(key, () =>
            //{
            //    var query = from pa in _productAttributeRepository.Table
            //                orderby pa.Name
            //                select pa;
            //    var productAttributes = new PagedList<ProductAttribute>(query, pageIndex, pageSize);
            //    return productAttributes;
            //});

            return null;
        }

        public ProductAttribute GetProductAttributeById(int productAttributeId)
        {
            if (productAttributeId == 0) return null;
            
            return _productAttributeRepository.Return(productAttributeId);
        }

        public void InsertProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Create(productAttribute);            
        }

        public void UpdateProductAttribute(ProductAttribute productAttribute)
        {
            if (productAttribute == null)
                throw new ArgumentNullException("productAttribute");

            _productAttributeRepository.Update(productAttribute);
        }

        #endregion

        #region Product attributes mappings

        public void DeleteProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException("productAttributeMapping");

            _productAttributeMappingRepository.Delete(productAttributeMapping);
        }

        public IList<ProductAttributeMapping> GetProductAttributeMappingsByProductId(int productId)
        {
            var productAttributeMappings =_productAttributeMappingRepository.Table
                .Where(pam => pam.ProductId == productId)
                .OrderBy(pam => pam.DisplayOrder)
                .ToList();

            return productAttributeMappings;
        }

        public ProductAttributeMapping GetProductAttributeMappingById(int productAttributeMappingId)
        {
            if (productAttributeMappingId == 0)
                return null;

            return _productAttributeMappingRepository.Return(productAttributeMappingId);
        }

        public void InsertProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException("productAttributeMapping");

            _productAttributeMappingRepository.Create(productAttributeMapping);            
        }

        public virtual void UpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException("productAttributeMapping");

            _productAttributeMappingRepository.Update(productAttributeMapping);            
        }

        #endregion

        #region Public methods
        
        public ProductOverviewModel BuildProductOverviewModel(Product product, int priority = 0, decimal popularity = 0M, decimal displayRank = 0M)
        {
            if (product == null) return null;

            string key = string.Format(CacheKey.PRODUCT_OVERVIEW_BY_URL_KEY, product.UrlRewrite);

            return _cacheManager.Get(key, delegate ()
            {
                #region Images

                var activeProductMedias = product.ProductMedias.Where(m => m.Enabled == true).ToList();

                #endregion

                #region Related Offers

                var relatedOffers = _offerService.FindRelatedOffers(product);

                #endregion

                #region Prices

                var priceRangeExclTax = new decimal[2];
                var priceRangeInclTax = new decimal[2];
                var activePrices = product.ProductPrices.Where(pp => pp.Enabled == true).OrderBy(pp => pp.PriceExclTax).ToList();

                if (activePrices.Count > 0)
                {
                    if (activePrices.Count == 1)
                    {
                        priceRangeExclTax = new decimal[1];
                        priceRangeInclTax = new decimal[1];
                        var price = activePrices[0];

                        if (price.OfferRuleId > 0)
                        {
                            priceRangeExclTax[0] = price.OfferPriceExclTax;
                            priceRangeInclTax[0] = price.OfferPriceInclTax;
                        }
                        else
                        {
                            priceRangeExclTax[0] = price.PriceExclTax;
                            priceRangeInclTax[0] = price.PriceInclTax;
                        }
                    }
                    else
                    {
                        var price1 = activePrices.FirstOrDefault();
                        var price2 = activePrices.LastOrDefault();

                        if (price1.OfferRuleId > 0)
                        {
                            if (price1.OfferPriceExclTax != price2.OfferPriceExclTax)
                            {
                                priceRangeExclTax[0] = price1.OfferPriceExclTax;
                                priceRangeExclTax[1] = price2.OfferPriceExclTax;

                                priceRangeInclTax[0] = price1.OfferPriceInclTax;
                                priceRangeInclTax[1] = price2.OfferPriceInclTax;
                            }
                            else
                            {
                                priceRangeExclTax = new decimal[1];
                                priceRangeExclTax[0] = price1.OfferPriceExclTax;

                                priceRangeInclTax = new decimal[1];
                                priceRangeInclTax[0] = price1.OfferPriceInclTax;
                            }
                        }
                        else
                        {
                            if (price1.PriceExclTax != price2.PriceExclTax)
                            {
                                priceRangeExclTax[0] = price1.PriceExclTax;
                                priceRangeExclTax[1] = price2.PriceExclTax;

                                priceRangeInclTax[0] = price1.PriceInclTax;
                                priceRangeInclTax[1] = price2.PriceInclTax;
                            }
                            else
                            {
                                priceRangeExclTax = new decimal[1];
                                priceRangeExclTax[0] = price1.PriceExclTax;

                                priceRangeInclTax = new decimal[1];
                                priceRangeInclTax[0] = price1.PriceInclTax;
                            }
                        }
                    }
                }
                else
                {
                    priceRangeExclTax = null;
                    priceRangeInclTax = null;
                }

                #endregion

                #region Product Mark

                var productMark = string.Empty;
                var productMarkType = 0;

                if (product.RelatedCatalogOffer != null)
                {
                    productMark = "Offer";
                    productMarkType = (int)ProductMarkType.Red;

                    relatedOffers.Add(product.RelatedCatalogOffer);
                }
                
                // If product mark is not empty, overwrite it
                if (!string.IsNullOrEmpty(product.ProductMark))
                {
                    productMark = product.ProductMark;
                    productMarkType = product.ProductMarkType;
                }

                #endregion

                int totalStock = product.ProductPrices.Select(x => x.Stock).DefaultIfEmpty(0).Sum();

                int taxCategoryId = _taxCategoryMappingRepository.Table
                    .Where(x => x.ProductId == product.Id)
                    .Select(x => x.TaxCategoryId)
                    .FirstOrDefault();

                var overviewModel = new ProductOverviewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    H1Title = product.H1Title,
                    UrlKey = product.UrlRewrite,
                    FullDescription = product.Description,
                    GridMediaFilename = activeProductMedias.Count > 0 ? activeProductMedias[0].MediaFilename : string.Empty,
                    ThumbMediaFilename = activeProductMedias.Count > 0 ? activeProductMedias[0].ThumbnailFilename : string.Empty,
                    Images = activeProductMedias,
                    DefaultOption = product.GetDefaultOption(),
                    Options = product.GetOptionString(),
                    PriceExclTaxRange = priceRangeExclTax,
                    PriceInclTaxRange = priceRangeInclTax,
                    DeliveryTimeLine = product.Delivery.TimeLine,
                    StepQuantity = product.StepQuantity,
                    BrandId = product.BrandId,
                    BrandCategoryId = product.BrandCategoryId,
                    IsPharmaceutical = product.IsPharmaceutical,
                    Discontinued = product.Discontinued,
                    ProductEnforcedStockCount = product.EnforceStockCount,
                    BrandEnforcedStockCount = product.Brand != null ? product.Brand.EnforceStockCount : false,
                    OpenForOffer = product.OpenForOffer,
                    Enabled = product.Enabled,
                    AssignedCategoryIds = _categoryService.GetCategoryIdListByProductId(product.Id),
                    AverageReviewRating = CalculateReviewRating(product),
                    ReviewCount = GetApprovedReviewCount(product),
                    ProductMark = productMark,
                    ProductMarkType = productMarkType,
                    ProductMarkExpiryDate = product.ProductMarkExpiryDate,
                    OptionType = (OptionType)product.OptionType,
                    StockAvailability = totalStock > 0,
                    ShowPreOrderButton = product.ShowPreOrderButton,
                    IsPhoneOrder = product.IsPhoneOrder,
                    CacheExpiryDate = product.CacheExpiryDate,
                    TaxCategoryId = taxCategoryId,
                    VisibleIndividually = product.VisibleIndividually,
                    RelatedOffers = relatedOffers,
                    MetaTitle = product.MetaTitle,
                    MetaDescription = product.MetaDescription,
                    MetaKeywords = product.MetaKeywords,
                    Priority = priority,
                    Popularity = popularity,
                    DisplayRank = displayRank,
                    ApolloRating = product.Rating
                };
                
                return overviewModel;
            });            
        }

        #endregion

        #region Private methods

        private decimal[] GetPriceRangeByBrand(int brandId)
        {
            var items = _productPriceRepository.Table
                .Join(_productRepository.Table, pp => pp.ProductId, p => p.Id, (pp, p) => new { pp, p })
                .Where(pp_p => pp_p.p.BrandId == brandId)
                .Where(pp_p => pp_p.p.Enabled == true)
                .Where(pp_p => pp_p.pp.Enabled == true)
                .GroupBy(pp_p => pp_p.p.Id)
                .Select(pp_p => new { min = pp_p.Min(a => a.pp.Price), max = pp_p.Max(a => a.pp.Price) })
                .ToList();

            if (items.Count > 0)
                return new decimal[] { items.Min(x => x.min), items.Max(x => x.max) };
            else
                return new decimal[] { 0M, 0M };
        }

        private decimal[] GetPriceRangeByBrandCategory(IList<int> brandCategoryIds)
        {
            var items = _productPriceRepository.Table
                .Join(_productRepository.Table, pp => pp.ProductId, p => p.Id, (pp, p) => new { pp, p })
                .Where(pp_p => brandCategoryIds.Contains(pp_p.p.BrandCategoryId))
                .Where(pp_p => pp_p.p.Enabled == true)
                .Where(pp_p => pp_p.pp.Enabled == true)
                .GroupBy(pp_p => pp_p.p.Id)
                .Select(pp_p => new { min = pp_p.Min(a => a.pp.Price), max = pp_p.Max(a => a.pp.Price) })
                .ToList();

            if (items.Count > 0)
                return new decimal[] { items.Min(x => x.min), items.Max(x => x.max) };
            else
                return new decimal[] { 0M, 0M };
        }

        private ProductPriceOverviewModel BuildProductPriceOverviewModel(ProductPrice priceOption)
        {
            var product = _productRepository.Return(priceOption.ProductId);

            if (product == null) _logger.InsertLog(LogLevel.Warning, "Product could not be found. Product ID={{{0}}}", priceOption.ProductId);

            return new ProductPriceOverviewModel
            {
                Id = priceOption.Id,
                ProductId = priceOption.ProductId,
                ProductName = product != null ? product.Name : string.Empty,
                ProductEnabled = product != null ? product.Enabled : false,
                PriceCode = priceOption.PriceCode,
                Weight = priceOption.Weight,
                Price = priceOption.Price,
                Stock = priceOption.Stock,
                Barcode = priceOption.Barcode,
                Size = priceOption.Size,
                ColourId = priceOption.ColourId,
                Option = product != null ? priceOption.GetOption((OptionType)product.OptionType, _cacheManager, _colourRepository) : string.Empty,
                Enabled = priceOption.Enabled,
                MaximumAllowedPurchaseQuantity = priceOption.MaximumAllowedPurchaseQuantity
            };
        }

        private int CalculateReviewRating(Product product)
        {
            if (product == null) return 0;
            if (product.ProductReviews == null) return 0;
            if (product.ProductReviews.Count == 0) return 0;

            var averageRating = product.ProductReviews
                .Where(r => r.Approved == true)
                .Select(r => r.Score)
                .DefaultIfEmpty(0)
                .Average();

            return Convert.ToInt32(Math.Round(averageRating, MidpointRounding.AwayFromZero));
        }

        private int GetApprovedReviewCount(Product product)
        {
            if (product == null) return 0;
            if (product.ProductReviews == null) return 0;
            if (product.ProductReviews.Count == 0) return 0;

            return product.ProductReviews
                .Where(r => r.Approved == true)
                .Count();
        }
        
        #endregion
    }
}
