using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Offer.OfferProcessor
{
    public class CartOfferProcessor : ICartOfferProcessor
    {
        private readonly ICartItemBuilder _cartItemBuilder;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly CartUtility _cartUtility;
        private readonly IOfferBuilder _offerBuilder;
        private readonly AttributeUtility _attributeUtility;

        public CartOfferProcessor(
            IOfferBuilder offerBuilder,
            ICartItemBuilder cartItemBuilder,
            IRepository<CartItem> cartItemRepository,
            CartUtility cartUtility,
            AttributeUtility attributeUtility)
        {
            _offerBuilder = offerBuilder;
            _cartItemBuilder = cartItemBuilder;
            _cartUtility = cartUtility;
            _cartItemRepository = cartItemRepository;
            _attributeUtility = attributeUtility;
        }

        public CartOffer ProcessCart(int profileId, string shippingCountryCode, string promoCode = "", int testOfferRuleId = 0)
        {
            promoCode = promoCode.ToLower().Trim();
            CartOffer cartOffer = new CartOffer();

            var cartRules = _offerBuilder.GetActiveOfferRulesByType(OfferRuleType.Cart);

            if (testOfferRuleId > 0)
            {
                var testRule = _offerBuilder.PrepareTestOfferRule(testOfferRuleId);
                cartRules = _offerBuilder.CloneActiveOfferRulesByType(OfferRuleType.Cart);
                cartRules.Remove(testRule);
                cartRules.Add(testRule);
                cartRules = cartRules.OrderBy(x => x.Priority).ToList();
            }

            if (cartRules.Count > 0)
            {
                // Get all items by profile ID
                var items = _cartItemRepository.Table
                    .Where(x => x.ProfileId == profileId)
                    .ToList()
                    .Select(x => _cartItemBuilder.Build(x))
                    .ToList();

                // Do a basket cleanup
                items = _cartUtility.RemoveFreeItemWithOffers(items, cartRules.Select(x => x.Id).ToArray());
                
                for (int i = 0; i < cartRules.Count; i++)
                {
                    // Check status and dates
                    // In theory, all inactive / expired rules will not be loaded from database
                    if ((cartRules[i].IsActive) &&
                        ((cartRules[i].StartDate.HasValue == false || cartRules[i].StartDate.Value.CompareTo(DateTime.Now) <= 0) &&
                        (cartRules[i].EndDate.HasValue == false || cartRules[i].EndDate.Value.CompareTo(DateTime.Now) >= 0)))
                    {
                        bool proceed = true;

                        if (profileId != 0 && !string.IsNullOrEmpty(promoCode) && promoCode.CompareTo(cartRules[i].PromoCode.ToLower()) == 0)
                        {
                            int orderQuantity = _cartUtility.GetNumberOfPaidOrders(promoCode, profileId);

                            // Check promocode and uses per customer
                            // value <= 0 will be considered as proceed
                            proceed = cartRules[i].UsesPerCustomer <= 0 || orderQuantity < cartRules[i].UsesPerCustomer;

                            if (cartRules[i].NewCustomerOnly && orderQuantity > 0)
                                proceed = false;
                        }

                        // Set html message from this offer
                        if (string.IsNullOrEmpty(cartRules[i].PromoCode) == false && cartRules[i].PromoCode.ToLower().CompareTo(promoCode) == 0)
                            cartOffer.Description = cartRules[i].HtmlMessage;

                        // If entered promocode is empty and promocode from offer is not empty, do not proceed
                        if (string.IsNullOrEmpty(cartRules[i].PromoCode) == false && string.IsNullOrEmpty(promoCode))
                            proceed = false;

                        bool conditionMatched = false;

                        if (proceed)
                        {
                            conditionMatched = _cartUtility.ProcessCartCondition(items, cartRules[i].Condition, cartRules[i].OfferedItemIncluded, cartRules[i].UseInitialPrice);

                            if (conditionMatched)
                            {
                                // If cart rule does not have promocode, proceed
                                // If cart rule does not match the given promocode, do not proceed
                                if (string.IsNullOrEmpty(cartRules[i].PromoCode)
                                    || cartRules[i].PromoCode.ToLower().CompareTo(promoCode) == 0)
                                {
                                    // Run action here
                                    cartOffer = _cartUtility.ProcessCartOfferAction(profileId, items, shippingCountryCode, cartRules[i], cartRules[i].Action, cartOffer);
                                }
                            }

                            if (!cartRules[i].ProceedForNext)
                                break;
                        }
                    }
                }

                // Update all cart items in database at the end of the loop.
                foreach (var item in items)
                {
                    if (item.Id > 0)
                        _cartItemRepository.Update(item);
                    else
                        _cartItemRepository.Create(item);
                }
            }

            return cartOffer;
        }

        public void RemoveCartOfferByPromoCode(int profileId, string promoCode)
        {
            if (string.IsNullOrEmpty(promoCode)) return;

            var cartRules = _offerBuilder.GetActiveOfferRulesByType(OfferRuleType.Cart);
            var offerIds = cartRules
                .Where(x => x.PromoCode.ToLower().CompareTo(promoCode.ToLower()) == 0)
                .Select(x => x.Id)
                .ToArray();

            if (offerIds.Count() > 0)
            {
                // Get all items by profile ID
                var items = _cartItemRepository.Table
                    .Where(x => x.ProfileId == profileId)
                    .Select(x => _cartItemBuilder.Build(x))
                    .ToList();

                _cartUtility.RemoveFreeItemWithOffers(items, offerIds);
                //RemoveItemWithOffers(profileId, offerIds);
            }
        }

        public IList<OfferRuleOverviewModel> FindRelatedOffers(Product product)
        {
            var cartRules = _offerBuilder.GetActiveOfferRulesByType(OfferRuleType.Cart);
            var relatedOffers = new List<OfferRuleOverviewModel>();

            if (cartRules.Count > 0)
            {
                var offerIds = new List<int>();

                foreach (var rule in cartRules)
                {
                    if ((rule.IsActive) &&
                        ((rule.StartDate.HasValue == false || rule.StartDate.Value.CompareTo(DateTime.Now) <= 0) &&
                        (rule.EndDate.HasValue == false || rule.EndDate.Value.CompareTo(DateTime.Now) >= 0)))
                    {
                        for (int j = 0; j < rule.Condition.ChildOfferConditions.Count; j++)
                        {
                            bool matched = IsRelatedToOfferRule(rule.Condition, product);

                            if (matched)
                            {
                                relatedOffers.Add(new OfferRuleOverviewModel
                                {
                                    Name = rule.Name,
                                    OfferTag = _cartUtility.GetOfferLabel(rule, rule.Action),
                                    UrlKey = rule.UrlRewrite,
                                    OfferRuleType = OfferRuleType.Cart,
                                    ShortDescription = rule.ShortDescription,
                                    DisableOfferLabel = rule.DisableOfferLabel,
                                    ShowInOfferPage = rule.ShowInOfferPage
                                });

                                break;
                            }
                        }

                        if (rule.ProceedForNext == false) break;
                    }
                }
            }
            
            return relatedOffers;
        }

        private bool IsRelatedToOfferRule(OfferCondition condition, Product product)
        {
            foreach (var childCondition in condition.ChildOfferConditions)
            {
                if (childCondition.OfferAttribute != null)
                {
                    if (childCondition.OfferAttribute.IsCatalog)
                    {
                        bool matched = _attributeUtility.ValidateAttribute(
                                            _attributeUtility.GetAttributeValue(childCondition.OfferAttribute.Id, product),
                                            childCondition.OfferOperator.Operator,
                                            childCondition.Operand);

                        if (matched) return true;
                    }
                    else if (childCondition.OfferAttribute.IsCart)
                    {
                        if (childCondition.ChildOfferConditions.Count > 0)
                            return IsRelatedToOfferRule(childCondition, product);

                        // At the moment, we have "total amount" and "total quantity" attributes that has IsCart = true. 
                        // Thus, we assume that this condition will always return true.
                        return true;
                    }
                }
                
                return IsRelatedToOfferRule(childCondition, product);
            }

            return false;
        }
    }
}
