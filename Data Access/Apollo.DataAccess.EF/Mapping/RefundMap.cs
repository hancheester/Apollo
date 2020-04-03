using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class RefundMap : EntityTypeConfiguration<Refund>
    {
        public RefundMap()
        {
            this.ToTable("Refund");
            this.HasKey(r => r.Id);
            this.Property(r => r.ValueToRefund).HasPrecision(10, 4);
            this.Property(r => r.ExchangeRate).HasPrecision(10, 4);
        }
    }
}
