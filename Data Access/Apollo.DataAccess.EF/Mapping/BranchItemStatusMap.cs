using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BranchItemStatusMap : EntityTypeConfiguration<BranchItemStatus>
    {
        public BranchItemStatusMap()
        {
            this.ToTable("BranchItemStatus");
            this.HasKey(b => b.Id);
        }
    }
}
