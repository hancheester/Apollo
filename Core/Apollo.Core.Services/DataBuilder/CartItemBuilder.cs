using Apollo.Core.Caching;
using Apollo.Core.Domain.Tax;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.Core.Services.Offer.OfferProcessor;
using Apollo.DataAccess.Interfaces;
using System;
using System.Linq;

namespace Apollo.Core.Services.DataBuilder
{
    public class CartItemBuilder : ProductBuilder, ICartItemBuilder
    {
        public CartItemBuilder(
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
            TaxSettings taxSettings,
            IBrandService brandService, 
            ICatalogOfferProcessor offerProcessor, 
            ICacheManager cacheManager) 
            : base(
                  productRepository, 
                  productPriceRepository, 
                  productMediaRepository, 
                  colourRepository, 
                  productReviewRepository,
                  productTagRepository, 
                  restrictedGroupRepository, 
                  restrictedGroupMappingRepository, 
                  deliveryRepository, 
                  productGoogleCustomLabelGroupMappingRepository, 
                  taxCategoryMappingRepository, 
                  taxCategoryRepository,
                  brandService, 
                  offerProcessor, 
                  cacheManager,
                  taxSettings)
        {
            
        }

        public CartItem Build(CartItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            item.Product = Build(item.ProductId);
            item.ProductPrice = item.Product.ProductPrices.Where(x => x.Id == item.ProductPriceId).FirstOrDefault();
            return item;
        }
    }
}
