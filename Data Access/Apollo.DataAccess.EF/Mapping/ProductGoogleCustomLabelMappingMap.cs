using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductGoogleCustomLabelMappingMap : EntityTypeConfiguration<ProductGoogleCustomLabelGroupMapping>
    {
        public ProductGoogleCustomLabelMappingMap()
        {
            this.ToTable("Product_GoogleCustomLabelGroup_Mapping");
            this.HasKey(p => p.Id);
        }
    }
}
