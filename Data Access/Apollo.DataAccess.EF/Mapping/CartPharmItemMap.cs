using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CartPharmItemMap : EntityTypeConfiguration<CartPharmItem>
    {
        public CartPharmItemMap()
        {
            this.ToTable("CartPharmItem");
            this.HasKey(p => p.Id);
        }
    }
}
