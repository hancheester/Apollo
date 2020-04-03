using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BrandMap : EntityTypeConfiguration<Brand>
    {
        public BrandMap()
        {
            this.ToTable("Brands");
            this.HasKey(b => b.Id);
            this.Ignore(b => b.FeaturedItems);
            this.Ignore(b => b.BrandMedias);
            this.Ignore(b => b.Delivery);
        }
    }
}
