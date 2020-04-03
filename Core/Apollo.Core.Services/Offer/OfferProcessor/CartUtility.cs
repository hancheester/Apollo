using Apollo.Core.Domain.Media;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Cart;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Offer.OfferProcessor
{
    public class CartUtility
    {
        private const int FIXED_PERCENT_DISCOUNT_FOR_WHOLE_CART = 5;
        private const int FREE_ITEM_OFFER = 6;
        private const int FIXED_PERCENT_DISCOUNT_FOR_PRODUCT_SUBSELECTION = 7;
        private const int FIXED_AMOUNT_DISCOUNT_FOR_WHOLE_CART = 8;
        private const int DO_NOTHING = 9;
        private const int BUY_X_QUANTITY_FOR_Y_QUANTITY = 10;
        private const int BUY_X_QUANTITY_FOR_Y_PRICE = 11;
        private const int BUY_X_QUANTITY_CHEAPEST_FREE = 12;

        private readonly ILogger _logger;
        private readonly MediaSettings _mediaSettings;
        private readonly ICartValidator _cartValidator;
        private readonly AttributeUtility _attributeUtility;
        private readonly IProductBuilder _productBuilder;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<OfferType> _offerTypeRepository;

        public CartUtility(
            ILogBuilder logBuilder,
            ICartValidator cartValidator,
            IProductBuilder productBuilder,
            IRepository<Order> orderRepository,
            IRepository<Country> countryRepository,
            IRepository<CartItem> cartItemRepository,
            IRepository<OfferType> offerTypeRepository,
            AttributeUtility attributeUtility,
            MediaSettings mediaSettings)
        {
            _logger = logBuilder.CreateLogger(typeof(CartUtility).FullName);
            _mediaSettings = mediaSettings;
            _cartValidator = cartValidator;
            _attributeUtility = attributeUtility;
            _productBuilder = productBuilder;
            _orderRepository = orderRepository;
            _countryRepository = countryRepository;
            _cartItemRepository = cartItemRepository;
            _offerTypeRepository = offerTypeRepository;
        }

        public bool ProcessCartCondition(IList<CartItem> items, OfferCondition condition, bool offeredItemIncluded, bool useInitialPrice)
        {
            if (condition == null) return false;
            if (condition.Matched == null) return false;
            bool resultFlag;
            bool expected = condition.Matched.Value;
            bool matched;
            bool proceed = true;
            bool decider = condition.IsAll.Value; //true = IsAll, false = IsAny

            for (int i = 0; i < condition.ChildOfferConditions.Count; i++)
            {
                // process cart attribute / subselection / item matching condition
                switch (condition.ChildOfferConditions[i].Type)
                {
                    case OfferConditionType.Attribute:
                        resultFlag = ProcessCartAttributeCondition(items, condition.ChildOfferConditions[i], offeredItemIncluded, useInitialPrice);
                        break;
                    case OfferConditionType.Subselection:
                        resultFlag = ProcessCartSubselectionCondition(items, condition.ChildOfferConditions[i], offeredItemIncluded);
                        break;
                    case OfferConditionType.ItemMatched:
                        resultFlag = ProcessCartItemMatchCondition(items, condition.ChildOfferConditions[i], offeredItemIncluded);
                        break;
                    case OfferConditionType.None:
                    default:
                        resultFlag = ProcessCartCondition(items, condition.ChildOfferConditions[i], offeredItemIncluded, useInitialPrice);
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

        public bool ProcessCartAttributeCondition(IList<CartItem> items, OfferCondition condition, bool offeredItemIncluded, bool useInitialPrice)
        {
            var value = _attributeUtility.GetAttributeValue(condition.OfferAttribute.Id, items, offeredItemIncluded, useInitialPrice, false);
            return _attributeUtility.ValidateAttribute(value, condition.OfferOperator.Operator, condition.Operand);
        }

        public bool ProcessCartSubselectionCondition(IList<CartItem> items, OfferCondition condition, bool offeredItemIncluded)
        {
            bool resultFlag;
            bool proceed = true;
            bool isTotalQty = condition.IsTotalQty ?? false; // false = total amount, true = total quantity            
            bool decider = condition.IsAll ?? false; //true = IsAll, false = IsAny

            int qty = 0;
            decimal amount = 0M;

            for (int i = 0; i < condition.ChildOfferConditions.Count; i++)
            {
                if (isTotalQty)
                {
                    int resultQty = CalculateQty(items, condition.ChildOfferConditions[i], offeredItemIncluded);

                    if (decider == false)
                        qty += resultQty;
                    else
                        qty = resultQty;

                    resultFlag = _attributeUtility.ValidateAttribute(qty.ToString(), condition.OfferOperator.Operator, condition.Operand);
                }
                else
                {
                    decimal resultAmount = CalculateAmount(items, condition.ChildOfferConditions[i], offeredItemIncluded);

                    if (decider == false)
                        amount += resultAmount;
                    else
                        amount = resultAmount;

                    resultFlag = _attributeUtility.ValidateAttribute(amount.ToString(), condition.OfferOperator.Operator, condition.Operand);
                }

                // does it need to proceed?
                proceed = decider == resultFlag;

                if (!proceed) break;
            }

            // does it match the condition?
            return decider == proceed;
        }

        public bool ProcessCartItemMatchCondition(IList<CartItem> items, OfferCondition condition, bool offeredItemIncluded)
        {
            bool resultFlag;
            bool isFound = condition.ItemFound.Value; // false = not found, true = found
            bool proceed = true;
            bool matched;
            bool decider = condition.IsAll.Value; //true = IsAll, false = IsAny

            for (int i = 0; i < condition.ChildOfferConditions.Count; i++)
            {
                resultFlag = FindItem(items, condition.ChildOfferConditions[i], offeredItemIncluded);

                // does it match the expected?
                matched = isFound == resultFlag;

                // does it need to proceed?
                proceed = decider == matched;

                if (!proceed) break;
            }

            // does it match the condition?
            return decider == proceed;
        }

        public CartOffer ProcessCartOfferAction(int profileId, IList<CartItem> items, string shippingCountryCode, OfferRule offerRule, OfferAction action, CartOffer cartOffer)
        {
            if (action == null) return cartOffer;

            List<CartItem> list;
            decimal matchedAmount = 0M;
            decimal discount = 0M;
            int steps = 0;
            Country country = _countryRepository.Table.Where(x => x.ISO3166Code == shippingCountryCode).FirstOrDefault();

            cartOffer.FreeDelivery = action.FreeShipping;
            cartOffer.RewardPoint += action.RewardPoint;

            switch (action.OfferActionAttributeId)
            {
                #region FIXED_PERCENT_DISCOUNT_FOR_PRODUCT_SUBSELECTION

                case FIXED_PERCENT_DISCOUNT_FOR_PRODUCT_SUBSELECTION:                    
                    // Run acton condition first
                    if (offerRule.Action.Condition != null && offerRule.Action.Condition.ChildOfferConditions.Count > 0)
                        matchedAmount = CalculateActionMatchedAmount(items, offerRule, action.Condition, country.IsEC);
                    else // Else use condition from offer rule..
                        matchedAmount = CalculateActionMatchedAmount(items, offerRule, offerRule.Condition, country.IsEC);

                    discount = matchedAmount / 100M * action.DiscountAmount.Value;
                    cartOffer.Discounts.Add(offerRule.Id, discount);
                    cartOffer.DiscountAmount += discount;
                    cartOffer.IsValid = true;
                    break;

                #endregion

                #region FIXED_PERCENT_DISCOUNT_FOR_WHOLE_CART

                case FIXED_PERCENT_DISCOUNT_FOR_WHOLE_CART:
                    if (country.IsEC)
                    {
                        discount = items.Select(x => x.ProductPrice.OfferPriceInclTax * x.Quantity)
                            .DefaultIfEmpty(0).Sum() * Convert.ToInt32(action.DiscountAmount.Value) / 100;
                    }
                    else
                    {
                        discount = items.Select(x => x.ProductPrice.OfferPriceExclTax * x.Quantity)
                            .DefaultIfEmpty(0).Sum() * Convert.ToInt32(action.DiscountAmount.Value) / 100;                        
                    }

                    cartOffer.DiscountAmount += discount;
                    cartOffer.Discounts.Add(offerRule.Id, discount);

                    cartOffer.IsValid = true;
                    break;

                #endregion

                #region FIXED_AMOUNT_DISCOUNT_FOR_WHOLE_CART

                case FIXED_AMOUNT_DISCOUNT_FOR_WHOLE_CART:
                    cartOffer.DiscountAmount += action.DiscountAmount.Value;
                    cartOffer.Discounts.Add(offerRule.Id, action.DiscountAmount.Value);
                    cartOffer.IsValid = true;
                    break;

                #endregion

                #region FREE_ITEM_OFFER

                case FREE_ITEM_OFFER:                    
                    bool proceed = false;
                    
                    if (action.MinimumAmount.HasValue)
                    {
                        if (action.Condition == null)
                            proceed = true;
                        else
                            proceed = CalculateActionMatchedAmount(items, offerRule, action.Condition, country.IsEC) > action.MinimumAmount.Value;
                    }
                    else
                        proceed = true;

                    int qtyStep = 1;
                    int qtyFound = 1;

                    if (action.DiscountQtyStep.HasValue && proceed)
                    {
                        // To calculate how many free items need to be added
                        qtyStep = action.DiscountQtyStep.Value;

                        if (action.Condition == null || action.Condition.ChildOfferConditions.Count == 0)
                            qtyFound = CalculateActionMatchedQty(items, offerRule, offerRule.Condition);
                        else
                            qtyFound = CalculateActionMatchedQty(items, offerRule, action.Condition);

                        proceed = qtyFound > 0;
                    }

                    bool isByOption = action.OptionOperator != null;

                    var foundList = new List<CartItem>();

                    // If there is an action condition
                    if (offerRule.Action.Condition != null && offerRule.Action.Condition.ChildOfferConditions.Count > 0)
                        foundList = GetActionMatchedItems(items, offerRule, action.Condition);
                    else // Else use condition from offer rule
                        foundList = GetActionMatchedItems(items, offerRule, offerRule.Condition);

                    list = new List<CartItem>();

                    if (isByOption)
                    {
                        for (int i = 0; i < foundList.Count; i++)
                        {
                            if (foundList[i].ProductPrice.OfferRuleId <= 0)
                            {
                                string targetOption = foundList[i].ProductPrice.Option;
                                bool isOkay = _attributeUtility.ValidateAttribute(targetOption, action.OptionOperator.Operator, action.OptionOperand);

                                if (isOkay)
                                {
                                    list.Add(foundList[i]);
                                }
                            }
                        }

                        if (list.Count > 0) proceed = true;
                    }
                    else
                    {
                        list = foundList;
                    }

                    if (proceed)
                    {
                        // TODO: Do we need to prevent other process?
                        // Flag item with offer rule id to prevent other process
                        //FlagOfferRuleIdInBasket(action.OfferRuleId, list.Select(x => x.Id).ToList());

                        // Add free item
                        if (action.FreeProductItself.Value)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i].Product == null)
                                {
                                    _logger.InsertLog(LogLevel.Warning, string.Format("Free item could not be loaded. Product ID={{{0}}}", list[i].ProductId));
                                    break;
                                }

                                if (list[i].ProductPrice == null)
                                {
                                    _logger.InsertLog(LogLevel.Warning, string.Format("Product price for free item could not be loaded. Product Price ID={{{0}}}", list[i].ProductPriceId));
                                    break;
                                }

                                if (list[i].Product.Brand == null)
                                {
                                    _logger.InsertLog(LogLevel.Warning, string.Format("Brand for free item could not be loaded. Brand ID={{{0}}}", list[i].Product.BrandId));
                                    break;
                                }

                                var message = _cartValidator.ValidateCartItem(
                                    profileId,
                                    list[i].ProductId,
                                    list[i].Product.BrandId,
                                    shippingCountryCode,
                                    list[i].Product.IsPharmaceutical,
                                    list[i].Product.Discontinued,
                                    list[i].ProductPrice.Stock,
                                    list[i].Product.Name,
                                    list[i].Product.EnforceStockCount,
                                    list[i].Product.Brand.EnforceStockCount,
                                    list[i].Product.Enabled,
                                    list[i].ProductPrice.Enabled,
                                    list[i].Quantity,
                                    list[i].Quantity / qtyStep,
                                    list[i].Product.StepQuantity,
                                    list[i].Product.IsPhoneOrder,
                                    list[i].ProductPrice.OfferPriceExclTax,
                                    allowZeroPrice: true);

                                // Empty message is ok status from cart item validation
                                if (string.IsNullOrEmpty(message))
                                {
                                    var cartItem = new CartItem
                                    {
                                        ProfileId = profileId,
                                        ProductPriceId = list[i].ProductPriceId,
                                        ProductId = list[i].ProductId,
                                        Quantity = list[i].Quantity / qtyStep,
                                        Product = list[i].Product,
                                        ProductPrice = list[i].ProductPrice,
                                        CartItemMode = (int)CartItemMode.FreeItem
                                    };

                                    items.Add(cartItem);
                                }
                            }
                        }
                        else
                        {
                            // Load free item
                            var freeItem = _productBuilder.Build(action.FreeProductId.Value);
                            if (freeItem == null)
                            {
                                _logger.InsertLog(LogLevel.Warning, string.Format("Free item could not be loaded. Product ID={{{0}}}", action.FreeProductId.Value));
                                break;
                            }

                            var freeItemPriceOption = freeItem.ProductPrices.Where(x => x.Id == action.FreeProductPriceId).FirstOrDefault();
                            if (freeItemPriceOption == null)
                            {
                                _logger.InsertLog(LogLevel.Warning, string.Format("Product price for free item could not be loaded. Product Price ID={{{0}}}", action.FreeProductPriceId));
                                break;
                            }

                            if (freeItem.Brand == null)
                            {
                                _logger.InsertLog(LogLevel.Warning, string.Format("Brand for free item could not be loaded. Brand ID={{{0}}}", freeItem.BrandId));
                                break;
                            }

                            // While stock lasts
                            if (freeItemPriceOption.Stock > 0)
                            {
                                var message = _cartValidator.ValidateCartItem(
                                    profileId,
                                    freeItem.Id,
                                    freeItem.BrandId,
                                    shippingCountryCode,
                                    freeItem.IsPharmaceutical,
                                    freeItem.Discontinued,
                                    freeItemPriceOption.Stock,
                                    freeItem.Name,
                                    freeItem.EnforceStockCount,
                                    freeItem.Brand.EnforceStockCount,
                                    freeItem.Enabled,
                                    freeItemPriceOption.Enabled,
                                    0,
                                    action.FreeProductQty.Value * (qtyFound / qtyStep),
                                    freeItem.StepQuantity,
                                    freeItem.IsPhoneOrder,
                                    freeItemPriceOption.OfferPriceExclTax,
                                    allowZeroPrice: true);

                                // Empty message is ok status from cart item validation
                                if (string.IsNullOrEmpty(message))
                                {
                                    var cartItem = new CartItem
                                    {
                                        ProfileId = profileId,
                                        ProductPriceId = freeItemPriceOption.Id,
                                        ProductId = freeItem.Id,
                                        Quantity = action.FreeProductQty.Value * (qtyFound / qtyStep),
                                        Product = freeItem,
                                        ProductPrice = freeItemPriceOption,
                                        CartItemMode = (int)CartItemMode.FreeItem
                                    };

                                    items.Add(cartItem);
                                }
                            }
                        }
                    }

                    cartOffer.IsValid = true;

                    break;

                #endregion

                #region DO_NOTHING

                case DO_NOTHING:
                    // do nothing
                    cartOffer.IsValid = true;
                    break;

                #endregion

                #region BUY_X_QUANTITY_FOR_Y_QUANTITY

                case BUY_X_QUANTITY_FOR_Y_QUANTITY:

                    // Must have X quantity
                    // Must have Y quantity
                    // X > Y
                    if (offerRule.Action.XValue.HasValue && offerRule.Action.YValue.HasValue
                        && offerRule.Action.XValue > offerRule.Action.YValue)
                    {
                        list = new List<CartItem>();

                        if (offerRule.Action.Condition != null && offerRule.Action.Condition.ChildOfferConditions.Count > 0) // If there is condition for action
                            list = GetActionMatchedItems(items, offerRule, action.Condition);
                        else // Else use condition from offer rule..
                            list = GetActionMatchedItems(items, offerRule, offerRule.Condition);

                        // Revert to old price if necessary
                        if (offerRule.UseInitialPrice)
                            SetToUseInitialPrice(list);
                        
                        // List down individual price
                        List<decimal> listIndividualPrice = new List<decimal>();

                        for (int i = 0; i < list.Count; i++)
                        {
                            for (int j = 0; j < list[i].Quantity; j++)
                            {
                                if (country.IsEC)
                                    listIndividualPrice.Add(list[i].ProductPrice.OfferPriceInclTax);
                                else
                                    listIndividualPrice.Add(list[i].ProductPrice.OfferPriceExclTax);
                            }
                        }

                        // Calculate how many steps for X quantity
                        steps = listIndividualPrice.Count / offerRule.Action.XValue.Value;

                        // Find quantity X of total value
                        decimal totalXValue = 0M;

                        for (int i = 0; i < offerRule.Action.XValue.Value; i++)
                        {
                            if (i < listIndividualPrice.Count)
                                totalXValue += listIndividualPrice[i];
                        }

                        // Find quantity Y of total value
                        decimal totalYValue = 0M;

                        for (int i = 0; i < offerRule.Action.YValue.Value; i++)
                        {
                            if (i < listIndividualPrice.Count)
                                totalYValue += listIndividualPrice[i];
                        }

                        // Get the difference
                        decimal differenceValue = totalXValue - totalYValue;

                        discount = steps * differenceValue;

                        cartOffer.DiscountAmount += discount;
                        cartOffer.Discounts.Add(offerRule.Id, discount);
                    }

                    cartOffer.IsValid = true;
                    break;

                #endregion

                #region BUY_X_QUANTITY_FOR_Y_PRICE

                case BUY_X_QUANTITY_FOR_Y_PRICE:

                    // Must have X quantity
                    // Must have Y price
                    if (offerRule.Action.XValue.HasValue && offerRule.Action.YValue.HasValue)
                    {
                        list = new List<CartItem>();

                        if (offerRule.Action.Condition != null && offerRule.Action.Condition.ChildOfferConditions.Count > 0) // If there is condition for action
                            list = GetActionMatchedItems(items, offerRule, action.Condition);
                        else // Else use condition from offer rule..
                            list = GetActionMatchedItems(items, offerRule, offerRule.Condition);

                        // Revert to old price if necessary
                        if (offerRule.UseInitialPrice)
                            SetToUseInitialPrice(list);
                        
                        // List down individual price
                        List<decimal> listIndividualPrice = new List<decimal>();
                        decimal totalValue = 0M;

                        for (int i = 0; i < list.Count; i++)
                        {
                            for (int j = 0; j < list[i].Quantity; j++)
                            {
                                if (country.IsEC)
                                {
                                    listIndividualPrice.Add(list[i].ProductPrice.OfferPriceInclTax);
                                    totalValue += list[i].ProductPrice.OfferPriceInclTax;
                                }
                                else
                                {
                                    listIndividualPrice.Add(list[i].ProductPrice.OfferPriceExclTax);
                                    totalValue += list[i].ProductPrice.OfferPriceExclTax;
                                }
                                
                            }
                        }

                        // Calculate how many steps for X quantity
                        steps = listIndividualPrice.Count / offerRule.Action.XValue.Value;

                        // Find total value
                        int lastRemoveCount = listIndividualPrice.Count % offerRule.Action.XValue.Value;

                        if (lastRemoveCount != 0)
                        {
                            for (int i = 0; i < lastRemoveCount; i++)
                            {
                                totalValue -= listIndividualPrice[listIndividualPrice.Count - 1 - i];
                            }
                        }

                        // Find total value of Y in current currency
                        decimal totalYValue = steps * (offerRule.Action.YValue.Value);

                        // Get the difference
                        discount = totalValue - totalYValue;

                        cartOffer.DiscountAmount += discount;
                        cartOffer.Discounts.Add(offerRule.Id, discount);
                    }

                    cartOffer.IsValid = true;

                    break;

                #endregion

                #region BUY_X_QUANTITY_CHEAPEST_FREE

                case BUY_X_QUANTITY_CHEAPEST_FREE:

                    // Must have X quantity
                    if (offerRule.Action.XValue.HasValue)
                    {
                        list = new List<CartItem>();

                        if (offerRule.Action.Condition != null && offerRule.Action.Condition.ChildOfferConditions.Count > 0) // If there is condition for action
                            list = GetActionMatchedItems(items, offerRule, action.Condition);
                        else // Else use condition from offer rule..
                            list = GetActionMatchedItems(items, offerRule, offerRule.Condition);
                        
                        // List down individual price
                        List<decimal> listIndividualPrice = new List<decimal>();
                        decimal totalValue = 0M;

                        for (int i = 0; i < list.Count; i++)
                        {
                            for (int j = 0; j < list[i].Quantity; j++)
                            {
                                if (country.IsEC)
                                {
                                    listIndividualPrice.Add(list[i].ProductPrice.OfferPriceInclTax);
                                    totalValue += list[i].ProductPrice.OfferPriceInclTax;
                                }
                                else
                                {
                                    listIndividualPrice.Add(list[i].ProductPrice.OfferPriceExclTax);
                                    totalValue += list[i].ProductPrice.OfferPriceExclTax;
                                }
                            }
                        }

                        // Calculate how many steps for X quantity
                        steps = listIndividualPrice.Count / offerRule.Action.XValue.Value;

                        // Get the smallest price                        
                        listIndividualPrice.Sort(delegate (decimal price1, decimal price2)
                        {
                            return price1.CompareTo(price2);
                        });

                        for (int i = 0; i < steps; i++)                        
                            discount += listIndividualPrice[i];
                        
                        cartOffer.DiscountAmount += discount;
                        cartOffer.Discounts.Add(offerRule.Id, discount);
                    }

                    cartOffer.IsValid = true;
                    break;

                    #endregion
            }

            cartOffer.DiscountAmount = Math.Round(cartOffer.DiscountAmount, 2, MidpointRounding.AwayFromZero);

            return cartOffer;
        }

        public int CalculateActionMatchedQty(IList<CartItem> items, OfferRule offerRule, OfferCondition condition)
        {
            var matchedItems = GetActionMatchedItems(items, offerRule, condition);
            int qty = 0;

            if (matchedItems != null)
            {
                qty = matchedItems
                    .Where(x => x.Product.OpenForOffer == true)
                    .Select(x => x.Quantity).DefaultIfEmpty(0).Sum();
            }

            return qty;
        }

        public decimal CalculateActionMatchedAmount(IList<CartItem> items, OfferRule offerRule, OfferCondition condition, bool isECcountry)
        {
            var matchedItems = GetActionMatchedItems(items, offerRule, condition);
            decimal amount = 0M;

            if (matchedItems != null)
            {
                if (offerRule.UseInitialPrice)
                    SetToUseInitialPrice(matchedItems);

                if (isECcountry)
                {
                    if (offerRule.UseInitialPrice)
                        amount = matchedItems
                            .Where(x => x.Product.OpenForOffer == true)
                            .Select(x => x.ProductPrice.PriceInclTax * x.Quantity).DefaultIfEmpty(0).Sum();
                    else
                        amount = matchedItems
                            .Where(x => x.Product.OpenForOffer == true)
                            .Select(x => x.ProductPrice.OfferPriceInclTax * x.Quantity).DefaultIfEmpty(0).Sum();
                }                    
                else
                {
                    if (offerRule.UseInitialPrice)
                        amount = matchedItems
                            .Where(x => x.Product.OpenForOffer == true)
                            .Select(x => x.ProductPrice.PriceExclTax * x.Quantity).DefaultIfEmpty(0).Sum();
                    else
                        amount = matchedItems
                            .Where(x => x.Product.OpenForOffer == true)
                            .Select(x => x.ProductPrice.OfferPriceExclTax * x.Quantity).DefaultIfEmpty(0).Sum();
                }                    
            }

            return amount;
        }

        public List<CartItem> GetActionMatchedItems(IList<CartItem> items, OfferRule offerRule, OfferCondition condition)
        {
            var matchedItems = items.Where(x =>
            {
                // Exclude item bound to an offer provided that rule says offered item is NOT included
                if (x.ProductPrice.OfferRuleId > 0 && !offerRule.OfferedItemIncluded)
                    return false;

                // If there is no condition, matched as default
                if (condition.ChildOfferConditions.Count <= 0)
                    return true;

                return ProcessCartActionCondition(condition, x);
            }).ToList();

            return matchedItems;
        }

        public bool ProcessCartActionCondition(OfferCondition condition, CartItem item)
        {
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
                        resultFlag = ProcessCartActionAttributeCondition(condition.ChildOfferConditions[i], item);
                        break;
                    case OfferConditionType.None:
                    case OfferConditionType.Subselection:
                    case OfferConditionType.ItemMatched:
                    default:
                        resultFlag = ProcessCartActionCondition(condition.ChildOfferConditions[i], item);
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

        public bool ProcessCartActionAttributeCondition(OfferCondition condition, CartItem item)
        {
            var value = _attributeUtility.GetAttributeValue(condition.OfferAttribute.Id, item.ProductId);
            return _attributeUtility.ValidateAttribute(value, condition.OfferOperator.Operator, condition.Operand);
        }
        
        public bool FindItem(IList<CartItem> items, OfferCondition condition, bool offeredItemIncluded)
        {
            var matchedItems = GetMatchedConditionItems(items, condition, offeredItemIncluded);
            return matchedItems == null ? false : matchedItems.Count > 0;
        }

        public decimal CalculateAmount(IList<CartItem> items, OfferCondition condition, bool offeredItemIncluded)
        {
            decimal amount = 0M;
            var matchedItems = GetMatchedConditionItems(items, condition, offeredItemIncluded);

            if (matchedItems != null)
                amount = matchedItems.Select(x => x.Quantity * x.ProductPrice.OfferPriceInclTax).DefaultIfEmpty(0).Sum();

            return amount;
        }

        public int CalculateQty(IList<CartItem> items, OfferCondition condition, bool offeredItemIncluded)
        {
            int qty = 0;
            var matchedItems = GetMatchedConditionItems(items, condition, offeredItemIncluded);

            if (matchedItems != null)
                qty = matchedItems.Select(x => x.Quantity).DefaultIfEmpty(0).Sum();

            return qty;
        }

        public IList<CartItem> GetMatchedConditionItems(IList<CartItem> items, OfferCondition condition, bool offeredItemIncluded)
        {
            var foundItems = items.Where(x =>
            {
                if (x.Product.OpenForOffer)
                {
                    if (offeredItemIncluded)
                    {
                        return _attributeUtility.ValidateAttribute(
                            _attributeUtility.GetAttributeValue(condition.OfferAttribute.Id, x.Product),
                            condition.OfferOperator.Operator,
                            condition.Operand);
                    }
                    else
                    {
                        // Exclude items with assigned offer rule id
                        if (x.ProductPrice.OfferRuleId <= 0)
                            return _attributeUtility.ValidateAttribute(
                                _attributeUtility.GetAttributeValue(condition.OfferAttribute.Id, x.Product),
                                condition.OfferOperator.Operator,
                                condition.Operand);
                        return false;
                    }
                }
                return false;
            })
            .ToList();

            return foundItems;
        }

        public void SetToUseInitialPrice(IList<CartItem> items)
        {
            if (items.Count == 0) return;

            foreach (var item in items)
                item.CartItemMode = (int)CartItemMode.InitialPrice;
        }

        public List<CartItem> RemoveFreeItemWithOffers(IList<CartItem> items, IList<int> offerRuleIds)
        {
            var list = items.ToList();
            var deletedItems = list
                .Where(x => x.ProductPrice.PriceExclTax <= 0 
                    || offerRuleIds.Contains(x.ProductPrice.OfferRuleId)
                    || x.CartItemMode == (int)CartItemMode.FreeItem).ToList();

            if (deletedItems.Count > 0)
            {
                list.RemoveAll(x => deletedItems.Any(y => y.Id == x.Id));

                foreach (var item in deletedItems)
                {
                    _cartItemRepository.Delete(item.Id);
                }
            }

            return list;
        }

        public int GetNumberOfPaidOrders(string promoCode, int profileId)
        {
            int count = _orderRepository.Table
                .Where(o => o.PromoCode == promoCode && o.ProfileId == profileId && o.Paid == true)
                .Count();

            return count;
        }

        public string GetOfferLabel(OfferRule offerRule, OfferAction action)
        {
            switch (action.OfferActionAttributeId)
            {
                case FIXED_PERCENT_DISCOUNT_FOR_PRODUCT_SUBSELECTION:
                case FIXED_PERCENT_DISCOUNT_FOR_WHOLE_CART:
                case FIXED_AMOUNT_DISCOUNT_FOR_WHOLE_CART:
                    return GetDefaultOfferLabel(offerRule) ?? "Sales";
                
                case FREE_ITEM_OFFER:
                    return GetDefaultOfferLabel(offerRule) ?? "Free Gift";
               
                case BUY_X_QUANTITY_FOR_Y_QUANTITY:
                    return GetDefaultOfferLabel(offerRule) ?? string.Format("{0} For {1}", offerRule.Action.XValue, offerRule.Action.YValue);
                
                case BUY_X_QUANTITY_FOR_Y_PRICE:
                    return GetDefaultOfferLabel(offerRule) ?? string.Format("{0} For Price Of {1}", offerRule.Action.XValue, offerRule.Action.YValue);

                case BUY_X_QUANTITY_CHEAPEST_FREE:
                    return GetDefaultOfferLabel(offerRule) ?? "Cheapest Free";

                case DO_NOTHING:
                default:
                    return null;
            }
        }

        private string GetDefaultOfferLabel(OfferRule offerRule)
        {
            if (string.IsNullOrEmpty(offerRule.OfferLabel) == false) return offerRule.OfferLabel;

            if (offerRule.OfferTypeId > 0)
            {
                var type = _offerTypeRepository.Return(offerRule.OfferTypeId);
                if (type != null) return type.Type;
            }

            return null;
        }
    }
}
