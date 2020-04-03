using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferRelatedItemMap : EntityTypeConfiguration<OfferRelatedItem>
    {
        public OfferRelatedItemMap()
        {
            this.ToTable("OfferRelatedItem");
            this.HasKey(o => o.Id);
            this.Ignore(o => o.ProductName);
        }
    }
}
