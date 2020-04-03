using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferActionMap : EntityTypeConfiguration<OfferAction>
    {
        public OfferActionMap()
        {
            this.ToTable("OfferAction");
            this.HasKey(a => a.Id);
            this.Ignore(a => a.Condition);
        }
    }
}
