using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface IOrderCalculator
    {
        /// <summary>
        /// Get order total using exchange rate.
        /// </summary>
        decimal GetOrderTotal(int orderId, bool useDefaultCurrency = false);
        /// <summary>
        /// Get order total using exchange rate.
        /// </summary>
        decimal GetOrderTotal(Order order, bool useDefaultCurrency = false);
        /// <summary>
        /// Get line total (excluding tax) using exchange rate.
        /// </summary>
        decimal GetLineTotalExclTax(IEnumerable<LineItem> items, bool useDefaultCurrency = false);
        /// <summary>
        /// Get line total (including tax) using exchange rate.
        /// </summary>
        decimal GetLineTotalInclTax(IEnumerable<LineItem> items, bool useDefaultCurrency = false);
        /// <summary>
        /// Get cart item (including tax) total in GBP.
        /// </summary>
        decimal GetVAT(IEnumerable<CartItem> items);        
        /// <summary>
        /// Get VAT using exchange rate.
        /// </summary>
        decimal GetVAT(Order order, bool useDefaultCurrency = false);        
        /// <summary>
        /// Get shipping cost in GBP.
        /// </summary>
        decimal GetShippingCost(IEnumerable<CartItem> cartItems,
                                ShippingOption shippingOption,
                                decimal discount,
                                decimal usedPointValue);
        /// <summary>
        /// Get shipping cost in GBP.
        /// </summary>
        decimal GetShippingCost(ProductPrice priceOption,
                                ShippingOption shippingOption,
                                decimal discount,
                                decimal usedPointValue);
        /// <summary>
        /// Calculate earned points based on currency GBP.
        /// </summary>
        int CalculateEarnedLoyaltyPointsFromCart(int profileId,
                                                 int allocatedPoint,
                                                 string currencyCode,
                                                 decimal discount);
    }
}
