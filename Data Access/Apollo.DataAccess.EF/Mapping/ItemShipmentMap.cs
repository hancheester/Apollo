using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ItemShipmentMap : EntityTypeConfiguration<ItemShipment>
    {
        public ItemShipmentMap()
        {
            this.ToTable("ItemShipment");
            this.HasKey(i => i.Id);
            this.Ignore(i => i.LineItem);
        }
    }
}
