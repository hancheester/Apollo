using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BrandMediaMap : EntityTypeConfiguration<BrandMedia>
    {
        public BrandMediaMap()
        {
            this.ToTable("BrandMedia");
            this.HasKey(bm => bm.Id);
        }
    }
}
