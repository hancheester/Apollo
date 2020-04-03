using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductMediaMap : EntityTypeConfiguration<ProductMedia>
    {
        public ProductMediaMap()
        {
            this.ToTable("ProductMedia");
            this.HasKey(c => c.Id);
        }
    }
}
