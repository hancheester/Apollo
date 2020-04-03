using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CategoryWhatsNewMap : EntityTypeConfiguration<CategoryWhatsNew>
    {
        public CategoryWhatsNewMap()
        {
            this.ToTable("CategoryWhatsNew");
            this.HasKey(c => c.Id);
        }
    }
}
