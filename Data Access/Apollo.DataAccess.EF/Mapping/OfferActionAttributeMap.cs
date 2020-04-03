using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferActionAttributeMap : EntityTypeConfiguration<OfferActionAttribute>
    {
        public OfferActionAttributeMap()
        {
            this.ToTable("OfferActionAttribute");
            this.HasKey(x => x.Id);
        }
    }
}
