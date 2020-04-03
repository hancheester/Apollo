using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class GCSupportedCountryMap : EntityTypeConfiguration<GCSupportedCountry>
    {
        public GCSupportedCountryMap()
        {
            this.ToTable("GCSupportedCountry");
            this.HasKey(g => g.Id);
        }
    }
}
