using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BranchPendingAllocationMap : EntityTypeConfiguration<BranchPendingAllocation>
    {
        public BranchPendingAllocationMap()
        {
            this.ToTable("BranchPendingAllocation");
            this.HasKey(b => b.Id);
        }
    }
}
