using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class LineItemMap : EntityTypeConfiguration<LineItem>
    {
        public LineItemMap()
        {
            this.ToTable("LineItem");
            this.HasKey(i => i.Id);
            this.Property(i => i.PriceInclTax).HasPrecision(10, 4);
            this.Property(i => i.PriceExclTax).HasPrecision(10, 4);
            this.Property(i => i.CostPrice).HasPrecision(10, 4);
            this.Property(i => i.ExchangeRate).HasPrecision(10, 4);
            this.Ignore(i => i.Product);
            this.Ignore(i => i.ProductPrice);
        }
    }
}
