using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BranchOrderMap : EntityTypeConfiguration<BranchOrder>
    {
        public BranchOrderMap()
        {
            this.ToTable("BranchOrder");
            this.HasKey(b => b.Id);
        }
    }
}
