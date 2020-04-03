using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BranchStockStatusMap : EntityTypeConfiguration<BranchStockStatus>
    {
        public BranchStockStatusMap()
        {
            this.ToTable("BranchStockStatus");
            this.HasKey(b => new { b.Id, b.StatusCode });
        }
    }
}
