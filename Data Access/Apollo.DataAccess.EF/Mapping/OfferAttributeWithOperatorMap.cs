using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferAttributeWithOperatorMap : EntityTypeConfiguration<OfferAttributeWithOperator>
    {
        public OfferAttributeWithOperatorMap()
        {
            this.ToTable("OfferAttributeWithOperator");
            this.HasKey(o => o.Id);
        }
    }
}
