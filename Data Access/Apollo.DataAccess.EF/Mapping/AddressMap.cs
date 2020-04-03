using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            this.ToTable("Address");
            this.HasKey(a => a.Id);
            this.Property(a => a.Name).HasMaxLength(100);
            this.Property(a => a.AddressLine1).HasMaxLength(256);
            this.Property(a => a.AddressLine2).HasMaxLength(256);
            this.Property(a => a.City).HasMaxLength(100);
            this.Property(a => a.County).HasMaxLength(100);
            this.Property(a => a.PostCode).HasMaxLength(50);
            this.Ignore(a => a.Country);
            this.Ignore(a => a.USState);
        }
    }
}
