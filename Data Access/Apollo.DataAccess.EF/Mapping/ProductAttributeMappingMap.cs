using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductAttributeMappingMap : EntityTypeConfiguration<ProductAttributeMapping>
    {
        public ProductAttributeMappingMap()
        {
            this.ToTable("Product_ProductAttribute_Mapping");
            this.HasKey(pam => pam.Id);
            this.Ignore(pam => pam.AttributeControlType);
            this.Ignore(pam => pam.ProductAttribute);
        }
    }
}
