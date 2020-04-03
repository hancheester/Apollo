using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CartPharmOrderMap : EntityTypeConfiguration<CartPharmOrder>
    {
        public CartPharmOrderMap()
        {
            this.ToTable("CartPharmOrder");
            this.HasKey(p => p.Id);
            this.Ignore(p => p.Items);
        }
    }
}
