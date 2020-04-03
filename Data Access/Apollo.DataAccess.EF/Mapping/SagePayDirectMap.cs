using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class SagePayDirectMap : EntityTypeConfiguration<SagePayDirect>
    {
        public SagePayDirectMap()
        {
            this.ToTable("SagePayDirect");
            this.HasKey(s => s.Id);
            this.Property(s => s.Amount).HasPrecision(10, 4);
            this.Property(s => s.ExchangeRate).HasPrecision(10, 4);
        }
    }
}
