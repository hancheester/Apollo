using Apollo.Core.Model;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductColourOptionMap : EntityTypeConfiguration<ProductColourOption>
    {
        public ProductColourOptionMap()
        {
            this.ToTable("ProductColourOption");
            this.HasKey(ps => ps.Id);
        }
    }
}
