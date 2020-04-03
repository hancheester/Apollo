using System;
using System.Linq;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.Core.Model;

namespace Apollo.Core.Services.Offer.OfferProcessor
{
    public class CatalogOfferProcessor : ICatalogOfferProcessor
    {
        private readonly CatalogUtility _catalogUtility;
        private readonly IOfferBuilder _offerBuilder;

        public CatalogOfferProcessor(
            IOfferBuilder offerBuilder,
            CatalogUtility catalogUtility)
        {
            _offerBuilder = offerBuilder;
            _catalogUtility = catalogUtility;
        }

        public Product ProcessCatalog(Product product, int testOfferRuleId = 0)
        {
            if (product.OpenForOffer)
            {
                var catalogRules = _offerBuilder.GetActiveOfferRulesByType(OfferRuleType.Catalog);

                if (testOfferRuleId > 0)
                {
                    var testRule = _offerBuilder.PrepareTestOfferRule(testOfferRuleId);
                    catalogRules = _offerBuilder.CloneActiveOfferRulesByType(OfferRuleType.Catalog);
                    catalogRules.Remove(testRule);
                    catalogRules.Add(testRule);
                    catalogRules = catalogRules.OrderBy(x => x.Priority).ToList();
                }

                if (catalogRules.Count > 0)
                {
                    for (int i = 0; i < catalogRules.Count; i++)
                    {
                        // check status and dates
                        // in theory, all inactive / expired rules will not be loaded from database
                        if (catalogRules[i].IsActive)
                        {
                            // check dates
                            if ((catalogRules[i].StartDate.HasValue == false || catalogRules[i].StartDate.Value.CompareTo(DateTime.Now) <= 0) &&
                                (catalogRules[i].EndDate.HasValue == false || catalogRules[i].EndDate.Value.CompareTo(DateTime.Now) >= 0))
                            {
                                bool conditionMatched = _catalogUtility.ProcessCatalogCondition(catalogRules[i].Id, catalogRules[i].Condition, product);

                                if (conditionMatched)
                                {
                                    // run action here
                                    product = _catalogUtility.ProcessCatalogOfferAction(product, catalogRules[i]);
                                }

                                if (!catalogRules[i].ProceedForNext)
                                    break;
                            }
                        }
                    }
                }
            }

            return product;
        }
    }
}
