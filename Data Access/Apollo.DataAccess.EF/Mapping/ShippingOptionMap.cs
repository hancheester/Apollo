using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ShippingOptionMap : EntityTypeConfiguration<ShippingOption>
    {
        public ShippingOptionMap()
        {
            this.ToTable("ShippingOption");
            this.HasKey(s => s.Id);
            this.Property(s => s.Value).HasPrecision(10, 4);
            this.Property(s => s.FreeThreshold).HasPrecision(10, 4);
            this.Property(s => s.SingleItemValue).HasPrecision(10, 4);
            this.Property(s => s.UpToOneKg).HasPrecision(10, 4);
            this.Property(s => s.UpToOneHalfKg).HasPrecision(10, 4);
            this.Property(s => s.UpToTwoKg).HasPrecision(10, 4);
            this.Property(s => s.UpToTwoHalfKg).HasPrecision(10, 4);
            this.Property(s => s.UpToThreeKg).HasPrecision(10, 4);
            this.Property(s => s.UpToThreeHalfKg).HasPrecision(10, 4);
            this.Property(s => s.UpToFourKg).HasPrecision(10, 4);
            this.Property(s => s.UpToFourHalfKg).HasPrecision(10, 4);
            this.Property(s => s.UpToFiveKg).HasPrecision(10, 4);
            this.Property(s => s.HalfKgRate).HasPrecision(10, 4);
            this.Ignore(s => s.Country);
        }
    }
}
