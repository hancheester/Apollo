using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class BranchProductStockMap : EntityTypeConfiguration<BranchProductStock>
    {
        public BranchProductStockMap()
        {
            this.ToTable("BranchProductStock");
            this.HasKey(b => new { b.Id, b.ProductPriceId, b.BranchId });
        }
    }
}
