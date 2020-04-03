using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CategoryFilterMap : EntityTypeConfiguration<CategoryFilter>
    {
        public CategoryFilterMap()
        {
            this.ToTable("CategoryFilters");
            this.HasKey(c => c.Id);
        }
    }
}
