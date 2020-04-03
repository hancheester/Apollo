using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class WarehouseAllocationMap : EntityTypeConfiguration<WarehouseAllocation>
    {
        public WarehouseAllocationMap()
        {
            this.ToTable("WarehouseAllocation");
            this.HasKey(w => w.Id);
        }
    }
}
