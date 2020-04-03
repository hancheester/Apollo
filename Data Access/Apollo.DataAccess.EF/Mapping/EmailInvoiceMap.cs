using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class EmailInvoiceMap : EntityTypeConfiguration<EmailInvoice>
    {
        public EmailInvoiceMap()
        {
            this.ToTable("EmailInvoice");
            this.HasKey(i => i.Id);
            this.Property(i => i.Amount).HasPrecision(10, 4);
            this.Property(i => i.ExchangeRate).HasPrecision(10, 4);
            this.Ignore(i => i.Country);
            this.Ignore(i => i.USState);
        }
    }
}
