using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BrandCategoryMap : EntityTypeConfiguration<BrandCategory>
    {
        public BrandCategoryMap()
        {
            this.ToTable("BrandCategory");
            this.HasKey(bc => bc.Id);
        }
    }
}
