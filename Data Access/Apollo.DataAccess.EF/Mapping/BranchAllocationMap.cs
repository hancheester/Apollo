using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BranchAllocationMap : EntityTypeConfiguration<BranchAllocation>
    {
        public BranchAllocationMap()
        {
            this.ToTable("BranchAllocations");
            this.HasKey(b => b.Id);
        }
    }
}
