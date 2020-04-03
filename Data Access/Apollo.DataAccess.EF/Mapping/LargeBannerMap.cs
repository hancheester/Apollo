using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class LargeBannerMap : EntityTypeConfiguration<LargeBanner>
    {
        public LargeBannerMap()
        {
            this.ToTable("LargeBanner");
            this.HasKey(b => b.Id);
        }
    }
}
