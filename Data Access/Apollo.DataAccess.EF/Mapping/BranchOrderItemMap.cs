using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BranchOrderItemMap : EntityTypeConfiguration<BranchOrderItem>
    {
        public BranchOrderItemMap()
        {
            this.ToTable("BranchOrderItem");
            this.HasKey(b => b.Id);
        }
    }
}
