using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductPriceMap : EntityTypeConfiguration<ProductPrice>
    {
        public ProductPriceMap()
        {
            this.ToTable("ProductPrices");
            this.HasKey(pp => pp.Id);
            this.Property(pp => pp.Price).HasPrecision(10, 4);            
            this.Property(pp => pp.CostPrice).HasPrecision(10, 4);
            this.Property(pp => pp.AdditionalShippingCost).HasPrecision(10, 4);
            this.Ignore(pp => pp.Option);
            this.Ignore(pp => pp.PriceExclTax);
            this.Ignore(pp => pp.PriceInclTax);
            this.Ignore(pp => pp.OfferPriceExclTax);
            this.Ignore(pp => pp.OfferPriceInclTax);
            this.Ignore(pp => pp.OfferRuleId);
            this.Ignore(pp => pp.Note);
            this.Ignore(pp => pp.ShowOfferTag);
            this.Ignore(pp => pp.ShowRRP);            
        }
    }
}
