using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CategoryFeaturedBrandMap : EntityTypeConfiguration<CategoryFeaturedBrand>
    {
        public CategoryFeaturedBrandMap()
        {
            this.ToTable("CategoryFeaturedBrand");
            this.HasKey(b => b.Id);
        }
    }
}
