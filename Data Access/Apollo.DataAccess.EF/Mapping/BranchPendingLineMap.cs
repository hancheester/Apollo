using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BranchPendingLineMap : EntityTypeConfiguration<BranchPendingLine>
    {
        public BranchPendingLineMap()
        {
            this.ToTable("BranchPendingLines");
            this.HasKey(b => b.Id);
            this.Ignore(b => b.BranchItemStatus);
            this.Ignore(b => b.Product);
            this.Ignore(b => b.ProductPrice);
        }
    }
}
