using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferTypeMap : EntityTypeConfiguration<OfferType>
    {
        public OfferTypeMap()
        {
            this.ToTable("OfferType");
            this.HasKey(x => x.Id);
        }
    }
}
