using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CategoryLargeBannerMappingMap : EntityTypeConfiguration<CategoryLargeBannerMapping>
    {
        public CategoryLargeBannerMappingMap()
        {
            this.ToTable("Category_LargeBanner_Mapping");
            this.HasKey(m => m.Id);
            this.Ignore(m => m.LargeBanner);
        }
    }
}
