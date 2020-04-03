using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CategoryMediaMap : EntityTypeConfiguration<CategoryMedia>
    {
        public CategoryMediaMap()
        {
            this.ToTable("CategoryMedia");
            this.HasKey(c => c.Id);
        }
    }
}
