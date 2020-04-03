using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductGroupMap : EntityTypeConfiguration<ProductGroup>
    {
        public ProductGroupMap()
        {
            this.ToTable("ProductGroup");
            this.HasKey(p => p.Id);
        }
    }
}
