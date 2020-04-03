using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OrderPaymentMap : EntityTypeConfiguration<OrderPayment>
    {
        public OrderPaymentMap()
        {
            this.ToTable("OrderPayment");
            this.HasKey(o => o.Id);
            this.Property(o => o.Amount).HasPrecision(10, 4);
            this.Property(o => o.ExchangeRate).HasPrecision(10, 4);
        }
    }
}
