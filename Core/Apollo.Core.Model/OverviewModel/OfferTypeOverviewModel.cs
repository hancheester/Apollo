using System;
using System.Collections.Generic;

namespace Apollo.Core.Model.OverviewModel
{
    public class OfferTypeOverviewModel : IEquatable<OfferTypeOverviewModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<OfferRuleOverviewModel> OfferRules { get; set; }

        public OfferTypeOverviewModel()
        {
            this.OfferRules = new List<OfferRuleOverviewModel>();
        }
        
        public bool Equals(OfferTypeOverviewModel other)
        {
            return Id == other.Id;
        }
    }
}
