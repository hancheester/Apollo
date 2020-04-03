using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            this.ToTable("Categories");
            this.HasKey(c => c.Id);
            this.Ignore(c => c.CategoryWhatsNews);
            this.Ignore(c => c.CategoryMedias);
            this.Ignore(c => c.FeaturedBrands);
            this.Ignore(c => c.FeaturedItems);
            this.Ignore(c => c.CategoryFilters);
            this.Ignore(c => c.ChildrenId);
            this.Ignore(c => c.Banners);
        }
    }
}
