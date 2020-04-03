using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferBannerMap : EntityTypeConfiguration<OfferBanner>
    {
        public OfferBannerMap()
        {
            this.ToTable("OfferBanner");
            this.HasKey(bc => bc.Id);
        }
    }
}
