using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferOperatorMap : EntityTypeConfiguration<OfferOperator>
    {
        public OfferOperatorMap()
        {
            this.ToTable("OfferOperator");
            this.HasKey(o => o.Id);
        }
    }
}
