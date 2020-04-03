using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferAttributeMap : EntityTypeConfiguration<OfferAttribute>
    {
        public OfferAttributeMap()
        {
            this.ToTable("OfferAttribute");
            this.HasKey(a => a.Id);
        }
    }
}
