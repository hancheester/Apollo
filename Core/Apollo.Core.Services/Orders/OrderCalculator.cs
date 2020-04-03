using Apollo.Core.Domain.Orders;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Orders
{
    public class OrderCalculator : IOrderCalculator
    {
        private const int FIVE_HUNDRED_GRAM = 500;
        private const int ONE_THOUSAND_GRAM = 1000;
        private const int ONE_THOUSAND_HALF_GRAM = 1500;
        private const int TWO_THOUSAND_GRAM = 2000;
        private const int TWO_THOUSAND_HALF_GRAM = 2500;
        private const int THREE_THOUSAND_GRAM = 3000;
        private const int THREE_THOUSAND_HALF_GRAM = 3500;
        private const int FOUR_THOUSAND_GRAM = 4000;
        private const int FOUR_THOUSAND_HALF_GRAM = 4500;
        private const int FIVE_THOUSAND_GRAM = 5000;

        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<USState> _usStateRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<LineItem> _lineItemRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<ShippingOption> _shippingOptionRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly ShippingSettings _shippingSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ICartItemBuilder _cartItemBuilder;

        public OrderCalculator(
            IRepository<Country> countryRepository,
            IRepository<USState> usStateRepository,
            IRepository<Order> orderRepository,
            IRepository<LineItem> lineItemRepository,
            IRepository<CartItem> cartItemRepository,
            IRepository<ShippingOption> shippingOptionRepository,
            IRepository<Currency> currencyRepository,
            IRepository<ProductPrice> productPriceRepository,
            ICartItemBuilder cartItemBuilder,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            _countryRepository = countryRepository;
            _usStateRepository = usStateRepository;
            _orderRepository = orderRepository;
            _lineItemRepository = lineItemRepository;
            _cartItemRepository = cartItemRepository;
            _shippingOptionRepository = shippingOptionRepository;
            _currencyRepository = currencyRepository;
            _productPriceRepository = productPriceRepository;
            _cartItemBuilder = cartItemBuilder;
            _shippingSettings = shippingSettings;
            _shoppingCartSettings = shoppingCartSettings;
        }

        /// <summary>
        /// Get order total using exchange rate.
        /// </summary>
        public decimal GetOrderTotal(int orderId, bool useDefaultCurrency = false)
        {
            var order = GetCompleteOrderById(orderId);

            if (order == null) return 0M;

            return GetOrderTotal(order, useDefaultCurrency);
        }

        /// <summary>
        /// Get order total using exchange rate.
        /// </summary>
        public decimal GetOrderTotal(Order order, bool useDefaultCurrency = false)
        {
            if (order == null) return 0M;

            var exchangeRate = order.ExchangeRate;

            // TODO: Default currency should come from global settings.
            if (useDefaultCurrency) exchangeRate = 1M;
            
            decimal subtotal = GetLineTotalExclTax(order.LineItemCollection, useDefaultCurrency);
            decimal vat = order.ShippingCountry.IsEC ? GetVAT(order, useDefaultCurrency) : 0M;
            decimal discount = order.DiscountAmount * exchangeRate;
            decimal usedPointValue = Convert.ToDecimal((order.AllocatedPoint / 100M) * exchangeRate);
            decimal shippingCost = order.ShippingCost * exchangeRate;
            
            return subtotal + vat - discount - usedPointValue + shippingCost;
        }

        /// <summary>
        /// Get line total using exchange rate.
        /// </summary>
        public decimal GetLineTotalExclTax(IEnumerable<LineItem> items, bool useDefaultCurrency = false)
        {
            var query = items.Where(i => i.StatusCode != LineStatusCode.CANCELLED);

            if (useDefaultCurrency)
            {
                return query.Select(i => i.PriceExclTax * i.Quantity).DefaultIfEmpty(0).Sum();
            }
            else
            {
                return query.Select(i => i.PriceExclTax * i.Quantity * i.ExchangeRate).DefaultIfEmpty(0).Sum();
            }            
        }
        
        /// <summary>
        /// Get taxable line total using exchange rate.
        /// </summary>
        public decimal GetLineTotalInclTax(IEnumerable<LineItem> items, bool useDefaultCurrency = false)
        {
            var query = items.Where(i => i.StatusCode != LineStatusCode.CANCELLED);

            if (useDefaultCurrency)
            {
                return query.Select(i => i.PriceInclTax * i.Quantity).DefaultIfEmpty(0).Sum();
            }
            else
            {
                return query.Select(i => i.PriceInclTax * i.Quantity * i.ExchangeRate).DefaultIfEmpty(0).Sum();
            }
        }

        /// <summary>
        /// Get cart item total in GBP.
        /// </summary>
        public decimal GetCartItemTotalInclTax(IEnumerable<CartItem> items)
        {
            decimal total = items.Select(i => i.ProductPrice.OfferPriceInclTax * i.Quantity).Sum();
            return total;
        }
        
        public decimal GetVAT(IEnumerable<CartItem> items)
        {
            var query = items.Where(i => i.CartItemMode != (int)CartItemMode.FreeItem);
            
            var totalInclTax = query.Select(
                i => (i.CartItemMode == (int)CartItemMode.InitialPrice ? i.ProductPrice.PriceInclTax : i.ProductPrice.OfferPriceInclTax) * i.Quantity)
                .Sum();
            
            var totalExclTax = query.Select(
                i => (i.CartItemMode == (int)CartItemMode.InitialPrice ? i.ProductPrice.PriceExclTax : i.ProductPrice.OfferPriceExclTax) * i.Quantity)
                .Sum();

            return totalInclTax - totalExclTax;
        }
        
        /// <summary>
        /// Get VAT using exchange rate.
        /// </summary>
        public decimal GetVAT(Order order, bool useDefaultCurrency = false)
        {
            var subtotalInclTax = GetLineTotalInclTax(order.LineItemCollection, useDefaultCurrency);
            var subtotalExclTax = GetLineTotalExclTax(order.LineItemCollection, useDefaultCurrency);            
            
            return subtotalInclTax - subtotalExclTax;
        }
        
        /// <summary>
        /// Get shipping cost in GBP.
        /// </summary>
        public decimal GetShippingCost(IEnumerable<CartItem> cartItems,
                                       ShippingOption shippingOption,
                                       decimal discount,
                                       decimal usedPointValue)
        {
            var isFreeDeliveryInUK = IsFreeDeliveryInUK(cartItems, shippingOption.Country.Id);
            var subtotal = GetCartItemTotalInclTax(cartItems) - discount - usedPointValue;
            var additionalShippingCost = CalculateTotalAdditionalShippingCost(cartItems, shippingOption.Country.ISO3166Code);
            var totalWeight = cartItems.Sum(c => c.Quantity * c.ProductPrice.Weight);
            var itemQuantity = cartItems.Sum(c => c.Quantity);

            return GetShippingCost(isFreeDeliveryInUK, itemQuantity, totalWeight, subtotal, additionalShippingCost, shippingOption);
        }

        /// <summary>
        /// Get shipping cost in GBP.
        /// </summary>
        public decimal GetShippingCost(ProductPrice priceOption,
                                       ShippingOption shippingOption,
                                       decimal discount,
                                       decimal usedPointValue)
        {
            var isFreeDeliveryInUK = IsFreeDeliveryInUK(priceOption, shippingOption.Country.Id);
            var subtotal = priceOption.PriceExclTax - discount - usedPointValue;
            var additionalShippingCost = CalculateTotalAdditionalShippingCost(priceOption, shippingOption.Country.ISO3166Code);

            return GetShippingCost(isFreeDeliveryInUK, 1, priceOption.Weight, subtotal, additionalShippingCost, shippingOption);
        }

        /// <summary>
        /// Calculate earned points based on currency GBP. Customers earn points before VAT deduction.
        /// </summary>
        public int CalculateEarnedLoyaltyPointsFromCart(int profileId, int allocatedPoint, string currencyCode, decimal discount)
        {
            var earnedPoints = Retry.Do(delegate ()
            {
                // Loyalty point is calculated based on non offer items
                var currency = _currencyRepository.Table.Where(c => c.CurrencyCode == currencyCode).FirstOrDefault();
                var usedPointValue = Convert.ToDecimal(allocatedPoint / 100M);
                var cartTotal = GetOfferExcludedCartTotalInclTax(profileId);
                var orderTotal = cartTotal - discount - usedPointValue;               
                var loyaltyRate = _shoppingCartSettings.LoyaltyRate;

                // If orderTotal value is less than 0, return 0
                // Make sure order value is converted to GBP
                return orderTotal <= 0M ? 0 : (int)Math.Round(orderTotal * (loyaltyRate / 100M) * 100M);
            }, TimeSpan.FromSeconds(3));

            return earnedPoints;
        }

        /// <summary>
        /// Get shipping cost in GBP.
        /// </summary>
        private decimal GetShippingCost(bool isFreeDeliveryInUK,
                                        int itemQuantity,
                                        int totalWeight,
                                        decimal subtotal,
                                        decimal additionalShippingCost,
                                        ShippingOption shippingOption)
        {
            if (isFreeDeliveryInUK)
                return 0M;

            decimal freeThreshold = shippingOption.FreeThreshold;
            decimal singleItemValue = shippingOption.SingleItemValue;
            decimal upToOneKg = shippingOption.UpToOneKg;
            decimal upToOneHalfKg = shippingOption.UpToOneHalfKg;
            decimal upToTwoKg = shippingOption.UpToTwoKg;
            decimal upToTwoHalfKg = shippingOption.UpToTwoHalfKg;
            decimal upToThreeKg = shippingOption.UpToThreeKg;
            decimal upToThreeHalfKg = shippingOption.UpToThreeHalfKg;
            decimal upToFourKg = shippingOption.UpToFourKg;
            decimal upToFourHalfKg = shippingOption.UpToFourHalfKg;
            decimal upToFiveKg = shippingOption.UpToFiveKg;
            decimal halfKgRate = shippingOption.HalfKgRate;

            decimal tenPoundsWorth = 10M;
            decimal twentyPoundsWorth = 20M;

            // Orders less than 2KG, standard delivery and not UK
            var primaryStoreCountryId = _shippingSettings.PrimaryStoreCountryId;
            if (shippingOption.Country.Id != primaryStoreCountryId
                && shippingOption.Name.ToLower().Contains("standard delivery")
                && totalWeight <= TWO_THOUSAND_GRAM)
            {
                if (subtotal <= tenPoundsWorth)
                {
                    return 3M + additionalShippingCost;
                }
                // International and less than GBP 20
                else if (subtotal <= twentyPoundsWorth)
                {
                    return 2M + additionalShippingCost;
                }
            }

            // delivery priority list
            // 1. > 5KG
            // 2. Free threshold
            // 3. Single item
            // 4. <= 1KG
            // 5. <= 1.5KG
            // 6. <= 2KG
            // 7. <= 2.5KG
            // 8. <= 3KG
            // 9. <= 3.5KG
            // 10. <= 4KG
            // 11. <= 4.5KG
            // 12. <= 5KG
            // 13. Default value

            decimal value = shippingOption.Value;

            if (totalWeight > FIVE_THOUSAND_GRAM)
            {
                decimal extraKg = totalWeight - FIVE_THOUSAND_GRAM;
                decimal extraCost = Math.Ceiling(extraKg / FIVE_HUNDRED_GRAM) * halfKgRate;
                value = upToFiveKg + extraCost;
            }
            else if (freeThreshold > 0M && (freeThreshold <= subtotal || subtotal == 0M))
                value = 0M;
            else if (itemQuantity == 1 && singleItemValue > 0M)
                value = singleItemValue;
            else if (totalWeight <= ONE_THOUSAND_GRAM)
                value = upToOneKg;
            else if (totalWeight <= ONE_THOUSAND_HALF_GRAM)
                value = upToOneHalfKg;
            else if (totalWeight <= TWO_THOUSAND_GRAM)
                value = upToTwoKg;
            else if (totalWeight <= TWO_THOUSAND_HALF_GRAM)
                value = upToTwoHalfKg;
            else if (totalWeight <= THREE_THOUSAND_GRAM)
                value = upToThreeKg;
            else if (totalWeight <= THREE_THOUSAND_HALF_GRAM)
                value = upToThreeHalfKg;
            else if (totalWeight <= FOUR_THOUSAND_GRAM)
                value = upToFourKg;
            else if (totalWeight <= FOUR_THOUSAND_HALF_GRAM)
                value = upToFourHalfKg;
            else if (totalWeight <= FIVE_THOUSAND_GRAM)
                value = upToFiveKg;

            return value + additionalShippingCost;
        }

        /// <summary>
        /// Calculate total additional shipping cost in GBP.
        /// </summary>
        private decimal CalculateTotalAdditionalShippingCost(IEnumerable<CartItem> items, string countryCode)
        {
            var result = Retry.Do(delegate ()
            {
                decimal totalValue = 0M;

                var country = _countryRepository.Table.Where(x => x.ISO3166Code == countryCode).FirstOrDefault();
                if (country == null) return totalValue;

                // Exclude UK                
                int primaryStoreCountryId = _shippingSettings.PrimaryStoreCountryId;
                if (country.Id != primaryStoreCountryId)
                {
                    foreach (var item in items)
                    {
                        var productPrice = _productPriceRepository.Return(item.ProductPriceId);

                        if (productPrice.AdditionalShippingCost > 0M)
                            totalValue += productPrice.AdditionalShippingCost * item.Quantity;
                    }
                }

                return totalValue;
            }, TimeSpan.FromSeconds(3));

            return result;
        }

        /// <summary>
        /// Calculate total additional shipping cost in GBP.
        /// </summary>
        private decimal CalculateTotalAdditionalShippingCost(ProductPrice priceOption, string countryCode)
        {
            decimal totalValue = 0M;

            var country = _countryRepository.Table.Where(x => x.ISO3166Code == countryCode).FirstOrDefault();
            if (country == null) return totalValue;

            // Exclude UK
            int primaryStoreCountryId = _shippingSettings.PrimaryStoreCountryId;
            if (country.Id != primaryStoreCountryId)
            {
                return priceOption.AdditionalShippingCost;
            }

            return totalValue;
        }

        /// <summary>
        /// Get cart item total (including tax) which is not on offer in GBP.
        /// </summary>
        private decimal GetOfferExcludedCartTotalInclTax(int profileId)
        {
            var total = _cartItemRepository.Table
                .Where(i => i.ProfileId == profileId)
                .ToList()
                .Select(i => _cartItemBuilder.Build(i))
                .Where(i => i.ProductPrice.OfferRuleId == 0)
                .Select(i => i.ProductPrice.OfferPriceInclTax * i.Quantity)
                .DefaultIfEmpty(0M)
                .Sum();

            return total;
        }

        /// <summary>
        /// Check if cart contains NHS prescription.
        /// </summary>
        private bool IsFreeDeliveryInUK(IEnumerable<CartItem> items, int countryId)
        {
            // As prescription from NHS is eligible for free delivery, if there is only NHS prescription in the basket then delivery will be free.
            var prescriptionProductId = 0;
            var nhsPrescriptionFound = items.Where(x => x.ProductId == prescriptionProductId).Count() > 0;
            var primaryStoreCountryId = _shippingSettings.PrimaryStoreCountryId;

            if (nhsPrescriptionFound && items.Count() == 1 && countryId == primaryStoreCountryId)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the product contains NHS prescription.
        /// </summary>
        private bool IsFreeDeliveryInUK(ProductPrice priceOption, int countryId)
        {
            // As prescription from NHS is eligible for free delivery, if there is only NHS prescription in the basket then delivery will be free.
            var prescriptionProductId = 0;
            var primaryStoreCountryId = _shippingSettings.PrimaryStoreCountryId;

            return priceOption.ProductId == prescriptionProductId && countryId == primaryStoreCountryId;
        }

        private Order GetCompleteOrderById(int orderId)
        {
            var order = _orderRepository.TableNoTracking.Where(x => x.Id == orderId).FirstOrDefault();

            if (order != null)
            {
                order.LineItemCollection = _lineItemRepository.TableNoTracking.Where(l => l.OrderId == order.Id).ToList();
                order.Country = _countryRepository.Return(order.CountryId);
                order.USState = _usStateRepository.Return(order.USStateId);
                order.ShippingCountry = _countryRepository.Return(order.ShippingCountryId);
                order.ShippingUSState = _usStateRepository.Return(order.ShippingUSStateId);
                order.ShippingOption = _shippingOptionRepository.Return(order.ShippingOptionId);
            }

            return order;
        }
    }
}
