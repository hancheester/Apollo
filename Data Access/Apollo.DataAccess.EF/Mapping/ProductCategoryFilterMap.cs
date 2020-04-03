using Apollo.Core.Model.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ProductCategoryFilterMap : EntityTypeConfiguration<ProductCategoryFilter>
    {
        public ProductCategoryFilterMap()
        {
            this.ToTable("ProductCategoryFilters");
            this.HasKey(x => new { x.Id, x.CategoryFilterId, x.ProductId });
            this.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
