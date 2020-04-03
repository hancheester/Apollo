using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CategoryFeaturedItemMap : EntityTypeConfiguration<CategoryFeaturedItem>
    {
        public CategoryFeaturedItemMap()
        {
            this.ToTable("CategoryFeaturedItem");
            this.HasKey(c => c.Id);
        }
    }
}
