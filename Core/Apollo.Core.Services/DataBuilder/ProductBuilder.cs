using Apollo.Core.Caching;
using Apollo.Core.Domain.Tax;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.Core.Services.Offer.OfferProcessor;
using Apollo.DataAccess.Interfaces;
using System;
using System.Linq;

namespace Apollo.Core.Services.DataBuilder
{
    public class ProductBuilder : IProductBuilder
    {
        private readonly IBrandService _brandService;
        private readonly ICacheManager _cacheManager;
        private readonly ICatalogOfferProcessor _offerProcessor;
        private readonly IRepository<Colour> _colourRepository;
        private readonly IRepository<Delivery> _deliveryRepository;        
        private readonly IRepository<ProductGoogleCustomLabelGroupMapping> _productGoogleCustomLabelGroupMappingRepository;
        private readonly IRepository<ProductMedia> _productMediaRepository;        
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IRepository<ProductTag> _productTagRepository;
        private readonly IRepository<RestrictedGroupMapping> _restrictedGroupMappingRepository;
        private readonly IRepository<RestrictedGroup> _restrictedGroupRepository;        
        private readonly IRepository<TaxCategoryMapping> _taxCategoryMappingRepository;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly TaxSettings _taxSettings;

        public ProductBuilder(
            IRepository<Product> productRepository,
            IRepository<ProductPrice> productPriceRepository,
            IRepository<ProductMedia> productMediaRepository,
            IRepository<Colour> colourRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductTag> productTagRepository,
            IRepository<RestrictedGroup> restrictedGroupRepository,
            IRepository<RestrictedGroupMapping> restrictedGroupMappingRepository,
            IRepository<Delivery> deliveryRepository,
            IRepository<ProductGoogleCustomLabelGroupMapping> productGoogleCustomLabelGroupMappingRepository,
            IRepository<TaxCategoryMapping> taxCategoryMappingRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IBrandService brandService,
            ICatalogOfferProcessor offerProcessor,           
            ICacheManager cacheManager,
            TaxSettings taxSettings)
        {
            _productRepository = productRepository;
            _productPriceRepository = productPriceRepository;
            _productMediaRepository = productMediaRepository;
            _colourRepository = colourRepository;
            _productReviewRepository = productReviewRepository;
            _productTagRepository = productTagRepository;
            _restrictedGroupRepository = restrictedGroupRepository;
            _restrictedGroupMappingRepository = restrictedGroupMappingRepository;
            _deliveryRepository = deliveryRepository;
            _taxCategoryMappingRepository = taxCategoryMappingRepository;
            _taxCategoryRepository = taxCategoryRepository;
            _productGoogleCustomLabelGroupMappingRepository = productGoogleCustomLabelGroupMappingRepository;
            _brandService = brandService;
            _offerProcessor = offerProcessor;
            _cacheManager = cacheManager;
            _taxSettings = taxSettings;
        }

        public Product Build(int id)
        {
            var product = _productRepository.Return(id);
            if (product == null) throw new ArgumentNullException("product");

            return Build(product);
        }

        public Product Build(Product product)
        {
            if (product == null) throw new ArgumentNullException("product");
            string key = string.Format(CacheKey.PRODUCT_BY_ID_KEY, product.Id);

            product = _cacheManager.GetWithExpiry(key, delegate ()
            {
                // Get tax category
                product.TaxCategory = _taxCategoryMappingRepository.Table
                    .Join(_taxCategoryRepository.Table, m => m.TaxCategoryId, t => t.Id, (m, t) => new { m, t })
                    .Where(x => x.m.ProductId == product.Id)
                    .Select(x => x.t)
                    .FirstOrDefault();

                // Get product prices
                product.ProductPrices = _productPriceRepository.TableNoTracking.Where(x => x.ProductId == product.Id).ToList();

                // Get price option
                // Set default offer price
                for (int i = 0; i < product.ProductPrices.Count; i++)
                {
                    product.ProductPrices[i].Option = product.ProductPrices[i].GetOption((OptionType)product.OptionType, _cacheManager, _colourRepository);

                    if (_taxSettings.PricesIncludeTax)
                    {
                        product.ProductPrices[i].PriceInclTax = product.ProductPrices[i].Price;
                        product.ProductPrices[i].PriceExclTax = product.ProductPrices[i].Price * 100M / (100 + product.TaxCategory.Rate);

                        product.ProductPrices[i].OfferPriceInclTax = product.ProductPrices[i].PriceInclTax;
                        product.ProductPrices[i].OfferPriceExclTax = product.ProductPrices[i].PriceExclTax;
                    }
                    else
                    {
                        product.ProductPrices[i].PriceInclTax = product.ProductPrices[i].Price * 100M / (100 + product.TaxCategory.Rate);
                        product.ProductPrices[i].PriceExclTax = product.ProductPrices[i].Price;

                        product.ProductPrices[i].OfferPriceInclTax = product.ProductPrices[i].PriceInclTax;
                        product.ProductPrices[i].OfferPriceExclTax = product.ProductPrices[i].PriceExclTax;
                    }
                }

                // Sort prices in ascending order
                product.ProductPrices.ToList().Sort(delegate (ProductPrice price1, ProductPrice price2)
                { return price1.PriceExclTax.CompareTo(price2.PriceExclTax); });

                // Get all reviews (including not approved ones)
                product.ProductReviews = _productReviewRepository.Table.Where(r => r.ProductId == product.Id).ToList();
                
                // Get product tags
                product.ProductTags = _productTagRepository.Table.Where(pt => pt.ProductId == product.Id).ToList();

                // Get restricted groups
                product.RestrictedGroups = _restrictedGroupMappingRepository.Table
                    .Join(_restrictedGroupRepository.Table, m => m.RestrictedGroupId, g => g.Id, (m, g) => new { m, g })
                    .Where(x => x.m.ProductId == product.Id)
                    .Select(x => x.g)
                    .ToList();

                // Get delivery
                product.Delivery = _deliveryRepository.Return(product.DeliveryId);

                // Get brand                
                product.Brand = _brandService.GetBrandById(product.BrandId);

                // Get media
                product.ProductMedias = _productMediaRepository.TableNoTracking.Where(m => m.ProductId == product.Id).ToList();

                // TODO: To implement Google Shopping custom label
                // Get Google Shopping custom label group
                product.ProductGoogleCustomLabelGroup = _productGoogleCustomLabelGroupMappingRepository.Table
                    .Where(x => x.ProductId == product.Id)
                    .FirstOrDefault();
                
                // Process catalog offer
                if (product.OpenForOffer)
                    product = _offerProcessor.ProcessCatalog(product);

                return product;
            });

            return product;
        }
    }
}
