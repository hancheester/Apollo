using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CartItemMap : EntityTypeConfiguration<CartItem>
    {
        public CartItemMap()
        {
            this.ToTable("CartItem");
            this.HasKey(i => i.Id);
            this.Ignore(i => i.Product);
            this.Ignore(i => i.ProductPrice);
        }
    }
}
