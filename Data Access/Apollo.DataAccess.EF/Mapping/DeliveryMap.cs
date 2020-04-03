using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class DeliveryMap : EntityTypeConfiguration<Delivery>
    {
        public DeliveryMap()
        {
            this.ToTable("Delivery");
            this.HasKey(d => d.Id);
        }
    }
}
