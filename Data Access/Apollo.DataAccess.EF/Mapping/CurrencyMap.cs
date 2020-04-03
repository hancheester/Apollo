using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CurrencyMap : EntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            this.ToTable("Currencies");
            this.HasKey(c => c.Id);
            this.Property(c => c.ExchangeRate).HasPrecision(10, 4);
            this.Property(c => c.GoogleExchangeRate).HasPrecision(10, 4);
        }
    }
}
