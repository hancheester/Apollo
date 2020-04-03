using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces.DataBuilder
{
    public interface IOfferBuilder
    {
        IList<OfferRule> GetActiveOfferRulesByType(OfferRuleType type);
        IList<OfferRule> CloneActiveOfferRulesByType(OfferRuleType type);
        OfferRule PrepareTestOfferRule(int offerRuleId);
    }
}
