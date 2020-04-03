using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class TaxCategoryMappingMap : EntityTypeConfiguration<TaxCategoryMapping>
    {
        public TaxCategoryMappingMap()
        {
            this.ToTable("Product_TaxCategory_Mapping");
            this.HasKey(p => p.Id);
        }
    }
}
