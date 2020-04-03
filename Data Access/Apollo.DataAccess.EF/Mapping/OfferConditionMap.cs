using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferConditionMap : EntityTypeConfiguration<OfferCondition>
    {
        public OfferConditionMap()
        {
            this.ToTable("OfferCondition");
            this.HasKey(i => i.Id);
            this.Ignore(i => i.Type);
            this.Ignore(i => i.ChildOfferConditions);
            this.Ignore(i => i.OfferAttribute);
            this.Ignore(i => i.OfferOperator);
        }
    }
}
