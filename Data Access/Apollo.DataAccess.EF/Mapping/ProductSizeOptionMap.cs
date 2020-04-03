using Apollo.Core.Model;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductSizeOptionMap : EntityTypeConfiguration<ProductSizeOption>
    {
        public ProductSizeOptionMap()
        {
            this.ToTable("ProductSizeOption");
            this.HasKey(ps => ps.Id);
        }
    }
}
