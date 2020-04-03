using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            this.ToTable("Products");
            this.HasKey(p => p.Id);
            
            this.Ignore(p => p.CacheExpiryDate);
            this.Ignore(p => p.Brand);
            this.Ignore(p => p.Delivery);
            this.Ignore(p => p.RestrictedGroups);
            this.Ignore(p => p.ProductMedias);
            this.Ignore(p => p.ProductPrices);
            this.Ignore(p => p.ProductTags);
            this.Ignore(p => p.ProductReviews);
            this.Ignore(p => p.Brand);
            this.Ignore(p => p.Delivery);
            this.Ignore(p => p.ProductGoogleCustomLabelGroup);
            this.Ignore(p => p.TaxCategory);
            this.Ignore(p => p.RelatedCatalogOffer);
        }
    }
}
