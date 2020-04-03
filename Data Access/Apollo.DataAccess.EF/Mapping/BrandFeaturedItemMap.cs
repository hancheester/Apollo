using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BrandFeaturedItemMap : EntityTypeConfiguration<BrandFeaturedItem>
    {
        public BrandFeaturedItemMap()
        {
            this.ToTable("BrandFeaturedItem");
            this.HasKey(f => f.Id);
        }
    }
}
