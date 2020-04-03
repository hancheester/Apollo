using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OfferRuleMap : EntityTypeConfiguration<OfferRule>
    {
        public OfferRuleMap()
        {
            this.ToTable("OfferRule");
            this.HasKey(o => o.Id);
            this.Ignore(o => o.Condition);
            this.Ignore(o => o.Action);
            this.Ignore(o => o.RelatedItems);
            this.Ignore(o => o.CacheExpiryDate);
        }
    }
}
