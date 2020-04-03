using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OrderShipmentMap : EntityTypeConfiguration<OrderShipment>
    {
        public OrderShipmentMap()
        {
            this.ToTable("OrderShipment");
            this.HasKey(a => a.Id);
            this.Ignore(a => a.ItemShipmentList);
        }
    }
}
