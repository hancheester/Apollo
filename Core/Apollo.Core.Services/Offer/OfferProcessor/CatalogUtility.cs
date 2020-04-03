using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;

namespace Apollo.Core.Services.Offer.OfferProcessor
{
    public class CatalogUtility
    {
        private const int BY_PERCENT_OF_THE_ORIGINAL_PRICE = 1;
        private const int TO_PERCENT_OF_THE_ORIGINAL_PRICE = 2;
        private const int BY_FIXED_AMOUNT = 3;
        private const int TO_FIXED_AMOUNT = 4;
        private const int DO_NOTHING = 9;

        private readonly ILogger _logger;
        private readonly AttributeUtility _attributeUtility;

        public CatalogUtility(
            ILogBuilder logBuilder,
            AttributeUtility attributeUtility)
        {
            _logger = logBuilder.CreateLogger(typeof(CatalogUtility).FullName);
            _attributeUtility = attributeUtility;
        }

        public Product ProcessCatalogOfferAction(Product product, OfferRule offerRule)
        {
            if (product == null)
            {
                _logger.InsertLog(LogLevel.Error, "Product is null.");
                return product;
            }
            if (offerRule == null)
            {
                _logger.InsertLog(LogLevel.Error, "Offer rule is null.");
                return product;
            }
            if (offerRule.Action == null)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Offer action is null. Offer rule ID={{{0}}}", offerRule.Id));
                return product;
            }

            bool isBySize = offerRule.Action.OptionOperator != null; // it could be colour too
            bool proceed = true;

            for (int i = 0; i < product.ProductPrices.Count; i++)
            {
                if (product.ProductPrices[i].OfferRuleId <= 0)
                {
                    if (isBySize)
                    {
                        string option = product.ProductPrices[i].Size;
                        proceed = _attributeUtility.ValidateAttribute(option, offerRule.Action.OptionOperator.Operator, offerRule.Action.OptionOperand);
                    }

                    if (proceed)
                    {
                        var assignOfferRuleId = true;
                        switch (offerRule.Action.OfferActionAttributeId)
                        {
                            case BY_FIXED_AMOUNT:
                                // Fixed amount is assumed to have tax included.
                                product.ProductPrices[i].OfferPriceInclTax = product.ProductPrices[i].PriceInclTax - offerRule.Action.DiscountAmount.Value;
                                product.ProductPrices[i].OfferPriceExclTax = product.ProductPrices[i].PriceExclTax - (offerRule.Action.DiscountAmount.Value / (100 + product.TaxCategory.Rate) * 100M);
                                break;

                            case BY_PERCENT_OF_THE_ORIGINAL_PRICE:
                                product.ProductPrices[i].OfferPriceExclTax = product.ProductPrices[i].PriceExclTax / 100M * (100M - offerRule.Action.DiscountAmount.Value);
                                product.ProductPrices[i].OfferPriceInclTax = product.ProductPrices[i].OfferPriceExclTax * (100 + product.TaxCategory.Rate) / 100M;
                                break;

                            case TO_FIXED_AMOUNT:
                                // Fixed amount is assumed to have tax included.
                                product.ProductPrices[i].OfferPriceInclTax = offerRule.Action.DiscountAmount.Value;
                                product.ProductPrices[i].OfferPriceExclTax = offerRule.Action.DiscountAmount.Value / (100 + product.TaxCategory.Rate) * 100M;
                                break;

                            case TO_PERCENT_OF_THE_ORIGINAL_PRICE:
                                product.ProductPrices[i].OfferPriceExclTax = product.ProductPrices[i].PriceExclTax / 100M * offerRule.Action.DiscountAmount.Value;
                                product.ProductPrices[i].OfferPriceInclTax = product.ProductPrices[i].OfferPriceExclTax * (100 + product.TaxCategory.Rate) / 100M;
                                break;

                            case DO_NOTHING:
                                assignOfferRuleId = false;
                                break;
                        }

                        // Flag this item with offer rule id to isolate from other offers
                        // It is important to check if we need to assign offer rule Id. If action is DO NOTHING, then we should not assign any offer rule Id at all
                        if (assignOfferRuleId) product.ProductPrices[i].OfferRuleId = offerRule.Action.OfferRuleId;

                        // Set fields for offer visibility on store front
                        product.ProductPrices[i].ShowOfferTag = offerRule.ShowOfferTag;
                        product.ProductPrices[i].ShowRRP = offerRule.ShowRRP;

                        // Set expiry date
                        if (offerRule.EndDate.HasValue)
                            product.CacheExpiryDate = offerRule.EndDate;

                        // Must make sure price excl tax is not zero
                        // If action is DO NOTHING, do not add to related offer
                        if (assignOfferRuleId && product.RelatedCatalogOffer == null && product.ProductPrices[i].PriceExclTax > 0)
                        {
                            product.RelatedCatalogOffer = new OfferRuleOverviewModel
                            {
                                Name = offerRule.Name,
                                OfferTag = string.IsNullOrEmpty(offerRule.OfferLabel) ?                                 
                                    string.Format("{0:#.##}% Off", (product.ProductPrices[i].PriceExclTax - product.ProductPrices[i].OfferPriceExclTax) / product.ProductPrices[i].PriceExclTax * 100) : 
                                    offerRule.OfferLabel,
                                UrlKey = offerRule.UrlRewrite,
                                OfferRuleType = OfferRuleType.Catalog,
                                DisableOfferLabel = offerRule.DisableOfferLabel,
                                ShowInOfferPage = offerRule.ShowInOfferPage
                            };
                        }
                    }
                }
            }

            return product;
        }

        public bool ProcessCatalogCondition(int offerRuleId, OfferCondition condition, Product product)
        {
            if (condition == null)
            {
                _logger.InsertLog(LogLevel.Error, "Condition is null. Offer Rule ID={{{0}}}", offerRuleId);
                return false;
            }

            if (product == null)
            {
                _logger.InsertLog(LogLevel.Error, "Product is null.");
                return false;
            }

            bool resultFlag;
            bool expected = condition.Matched.Value;
            bool matched;
            bool proceed = true;
            bool decider = condition.IsAll.Value; //true = IsAll, false = IsAny

            for (int i = 0; i < condition.ChildOfferConditions.Count; i++)
            {
                // process condition / attribute condition
                switch (condition.ChildOfferConditions[i].Type)
                {
                    case OfferConditionType.Attribute:
                        resultFlag = ProcessCatalogAttributeCondition(condition.ChildOfferConditions[i], product);
                        break;
                    case OfferConditionType.Subselection:
                    case OfferConditionType.ItemMatched:
                    case OfferConditionType.None:
                    default:
                        resultFlag = ProcessCatalogCondition(offerRuleId, condition.ChildOfferConditions[i], product);
                        break;
                }

                // does it match the expected?
                matched = expected == resultFlag;

                // does it need to proceed?
                proceed = decider == matched;

                if (!proceed) break;
            }

            // does it match the condition?
            return decider == proceed;
        }

        public bool ProcessCatalogAttributeCondition(OfferCondition condition, Product product)
        {
            var value = _attributeUtility.GetAttributeValue(condition.OfferAttribute.Id, product);
            return _attributeUtility.ValidateAttribute(
                value,
                condition.OfferOperator.Operator,
                condition.Operand);
        }        
    }
}
