using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductGroupMappingMap : EntityTypeConfiguration<ProductGroupMapping>
    {
        public ProductGroupMappingMap()
        {
            this.ToTable("Product_ProductGroup_Mapping");
            this.HasKey(p => p.Id);
        }
    }
}
