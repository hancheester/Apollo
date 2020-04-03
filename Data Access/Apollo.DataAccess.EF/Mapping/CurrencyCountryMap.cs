using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CurrencyCountryMap : EntityTypeConfiguration<CurrencyCountry>
    {
        public CurrencyCountryMap()
        {
            this.ToTable("CurrencyCountry");
            this.HasKey(c => c.Id);
        }
    }
}
