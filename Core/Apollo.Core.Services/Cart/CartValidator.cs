using Apollo.Core.Caching;
using Apollo.Core.Domain.Orders;
using Apollo.Core.Model.Entity;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Cart
{
    public class CartValidator : ICartValidator
    {
        #region Fields

        #region European Countries

        private readonly List<string> EuropeanCountries = new List<string>
        {
            "AL",
            "AD",
            "AT",
            "BY",
            "BE",
            "BA",
            "BG",
            "HR",
            "CY",
            "CZ",
            "DK",
            "EE",
            "FO",
            "FI",
            "FR",
            "DE",
            "GI",
            "GR",
            "HU",
            "IS",
            "IE",
            "IT",
            "LV",
            "LI",
            "LT",
            "LU",
            "MK",
            "MT",
            "MD",
            "MC",
            "NL",
            "NO",
            "PL",
            "PT",
            "RO",
            "SM",
            "RS",
            "SK",
            "SI",
            "ES",
            "SE",
            "CH",
            "UA",
            "GB",
            "VA",
            "RS",
            "IM",
            "RS",
            "ME"
        };

        #endregion
        
        private readonly IRepository<RestrictedGroup> _restrictedGroupRepository;
        private readonly IRepository<RestrictedGroupMapping> _restrictedGroupMappingRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<Country> _countryRepository;        
        private readonly ICacheManager _cacheManager;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        public CartValidator(IRepository<RestrictedGroup> restrictedGroupRepository,
                             IRepository<RestrictedGroupMapping> restrictedGroupMappingRepository,
                             IRepository<ProductPrice> productPriceRepository,
                             IRepository<Product> productRepository,
                             IRepository<CartItem> cartItemRepository,
                             IRepository<Country> countryRepository,
                             ICacheManager cacheManager,
                             ShoppingCartSettings shoppingCartSettings)
        {
            _productRepository = productRepository;
            _productPriceRepository = productPriceRepository;
            _restrictedGroupRepository = restrictedGroupRepository;
            _restrictedGroupMappingRepository = restrictedGroupMappingRepository;
            _cartItemRepository = cartItemRepository;
            _countryRepository = countryRepository;
            _shoppingCartSettings = shoppingCartSettings;
            _cacheManager = cacheManager;
        }

        public string ValidateCartItem(int profileId,
                                       int productId,
                                       int brandId,
                                       string shippingCountryCode,
                                       bool isPharmaceutical,
                                       bool discontinued,
                                       int stock,
                                       string name,
                                       bool productEnforcedStockCount,
                                       bool brandEnforcedStockCount,
                                       bool enabled,
                                       bool productPriceEnabled,
                                       int quantityInCart,
                                       int quantity,
                                       int stepQuantity,
                                       bool isPhoneOrder,
                                       decimal price,
                                       bool allowZeroPrice = true)
        {
            if (enabled == false) return string.Format("Apologies, {0} is not available anymore.", name);
            if (productPriceEnabled == false) return string.Format("Apologies, {0} is not available anymore.", name);
            if (isPhoneOrder == true) return string.Format("Apologies, {0} is only available for phone order. Please contact our customer service to order.", name);
            if ((brandEnforcedStockCount || productEnforcedStockCount) && stock <= 0) return string.Format("Apologies, {0} is out of stock.", name);
            if (isPharmaceutical && (quantityInCart + quantity) > _shoppingCartSettings.MaxPharmaceuticalProduct) return string.Format("Apologies, only {0} unit(s) maximum allowed.", _shoppingCartSettings.MaxPharmaceuticalProduct);
            if (allowZeroPrice == false && price <= 0M) return "Apologies, free item is not allowed to add to your basket.";

            if (discontinued)
            {
                var prices = _productPriceRepository.Table.Where(p => p.ProductId == productId).ToList();

                if (prices == null
                    || (prices.Count > 0
                        && !prices.Exists(delegate (ProductPrice priceItem) { return priceItem.Stock > 0; })))
                    return string.Format("Apologies, {0} is discontinued.", name);
            }

            var maxAllowedCartQuantity = CalculateMaxAllowedCartQuantity(
                isPharmaceutical,
                discontinued,
                productEnforcedStockCount,
                stepQuantity, 
                brandEnforcedStockCount,
                stock);

            if (quantity > maxAllowedCartQuantity) return string.Format("Apologies, only {0} unit(s) maximum allowed.", maxAllowedCartQuantity);

            var activeRestrictedGroups = GetActiveRestrictedGroupsByProductId(productId);
            if (activeRestrictedGroups.Count() > 0)
            {
                var cartItems = _cartItemRepository.Table.Where(c => c.ProfileId == profileId).ToList();
                var message = string.Empty;

                for (int i = 0; i < activeRestrictedGroups.Count(); i++)
                {
                    var restrictedGroupId = activeRestrictedGroups[i].Id;

                    #region Maximum quantity

                    var maxQuantity = activeRestrictedGroups[i].MaximumQuantity;
                    var restrictedQuantityInCart = GetQuantityByRestrictedGroupId(cartItems, restrictedGroupId);

                    // Check maximum quantity only if it is not zero
                    if (maxQuantity > 0 && ((restrictedQuantityInCart + quantity) > maxQuantity))
                    {
                        var restrictedGroupName = activeRestrictedGroups[i].Name;

                        if (string.IsNullOrEmpty(activeRestrictedGroups[i].Message))
                            return string.Format("Apologies, due to postal restrictions we are only able to send {0} unit(s) of {1} maximum.", maxQuantity, restrictedGroupName);
                        else
                            return activeRestrictedGroups[i].Message;
                    }

                    #endregion

                    #region Allowed countries

                    message = CheckForAllowedCountryRestriction(activeRestrictedGroups[i], shippingCountryCode);
                    if (!string.IsNullOrEmpty(message))
                        return message;

                    #endregion

                    #region Disallowed countries

                    message = CheckForDisallowedCountryRestriction(activeRestrictedGroups[i], shippingCountryCode);
                    if (!string.IsNullOrEmpty(message))
                        return message;
                    
                    #endregion
                }
            }

            return string.Empty;
        }

        public string[] ValidateCartAgainstShippingCountry(int profileId, string shippingCountryCode)
        {
            var messages = new List<string>();

            var products = _cartItemRepository.Table
                .Where(c => c.ProfileId == profileId)
                .Select(c => new { c.ProductId })
                .ToList();

            if (products.Count == 0)
                return messages.ToArray();
            
            var groups = new Dictionary<RestrictedGroup, List<string>>();

            for (int i = 0; i < products.Count; i++)
            {
                var activeGroups = GetActiveRestrictedGroupsByProductId(products[i].ProductId);

                for (int j = 0; j < activeGroups.Count; j++)
                {
                    var productId = products[i].ProductId;
                    var productName = _productRepository.Table.Where(x => x.Id == productId).Select(x => x.Name).FirstOrDefault();
                    if (groups.Keys.Where(x => x.Id == activeGroups[j].Id).Any())                    
                    {
                        var key = groups.Keys.Where(x => x.Id == activeGroups[j].Id).First();
                        groups[key].Add(string.Format("- {0}", productName));
                        groups[key] = groups[key].Distinct().OrderBy(x => x).ToList();
                    }
                    else
                    {
                        groups.Add(activeGroups[j], new List<string>() { string.Format("- {0}", productName) });
                    }
                }
            }
            
            if (groups.Count == 0)
                return messages.ToArray();

            foreach (var group in groups.Keys)
            {
                #region Allowed countries

                var message = CheckForAllowedCountryRestriction(group, shippingCountryCode);
                if (!string.IsNullOrEmpty(message))
                {
                    messages.Add(message + "<br/>" + string.Join("<br/>", groups[group].ToArray()));
                }
                #endregion

                #region Disallowed countries

                message = CheckForDisallowedCountryRestriction(group, shippingCountryCode);
                if (!string.IsNullOrEmpty(message))
                {
                    messages.Add(message + "<br/>" + string.Join("<br/>", groups[group].ToArray()));
                }

                #endregion
            }            

            return messages.ToArray();
        }

        private string CheckForAllowedCountryRestriction(RestrictedGroup group, string shippingCountryCode)
        {
            // Check allowed countries
            if (string.IsNullOrEmpty(group.AllowedCountries) == false)
            {
                var allowedCountries = group.AllowedCountries.Trim().Split(',').ToArray();
                if (allowedCountries.Contains(shippingCountryCode) == false)
                {
                    if (string.IsNullOrEmpty(group.Message))
                    {
                        var country = GetCountryByCountryCode(shippingCountryCode);

                        if (country != null)
                            return string.Format("Apologies, due to postal restrictions we are not allowed to send to {0}.", country.Name);
                        else
                            return "Apologies, due to postal restrictions we are not allowed to send.";
                    }
                    else
                        return group.Message;
                }
            }

            return string.Empty;
        }

        private string CheckForDisallowedCountryRestriction(RestrictedGroup group, string shippingCountryCode)
        {
            // Check disallowed countries
            if (string.IsNullOrEmpty(group.DisallowedCountries) == false)
            {
                var disallowedCountries = group.DisallowedCountries.Trim().Split(',').ToArray();
                if (disallowedCountries.Contains(shippingCountryCode))
                {
                    if (string.IsNullOrEmpty(group.Message))
                    {
                        var country = GetCountryByCountryCode(shippingCountryCode);

                        if (country != null)
                            return string.Format("Apologies, due to postal restrictions we are not allowed to send to {0}.", country.Name);
                        else
                            return "Apologies, due to postal restrictions we are not allowed to send.";
                    }
                    else
                        return group.Message;
                }
            }

            return string.Empty;
        }

        private Country GetCountryByCountryCode(string countryCode)
        {
            string key = string.Format(CacheKey.COUNTRY_BY_COUNTRY_CODE_KEY, countryCode);

            var item = _cacheManager.Get(key, delegate ()
            {
                return _countryRepository.Table.Where(x => x.ISO3166Code.ToLower() == countryCode.ToLower()).FirstOrDefault();
            });

            return item;
        }

        private IList<RestrictedGroup> GetActiveRestrictedGroupsByProductId(int productId)
        {
            string key = string.Format(CacheKey.RESTRICTED_GROUP_ACTIVE_BY_PRODUCT_ID_KEY, productId);

            return _cacheManager.Get(key, delegate ()
            {
                return _restrictedGroupMappingRepository.Table
                    .Join(_restrictedGroupRepository.Table, m => m.RestrictedGroupId, g => g.Id, (m, g) => new { m, g })
                    .Where(x => x.m.ProductId == productId)
                    .Where(x =>
                            (x.g.StartDate == null || x.g.StartDate.Value.CompareTo(DateTime.Now) <= 0)
                            &&
                            (x.g.EndDate == null || x.g.EndDate.Value.CompareTo(DateTime.Now) >= 0))
                    .Select(x => x.g)
                    .ToList();
            });   
        }

        private int GetQuantityByRestrictedGroupId(IList<CartItem> items, int restrictedGroupId)
        {
            int count = 0;

            for (int i = 0; i < items.Count; i++)
            {
                var groups = GetActiveRestrictedGroupsByProductId(items[i].ProductId);

                if (groups.Where(x => x.Id == restrictedGroupId).Count() > 0)                
                    count += items[i].Quantity;                
            }

            return count;
        }

        private int CalculateMaxAllowedCartQuantity(bool isPharmaceutical, bool discontinued, bool productEnforcedStockCount, int stepQuantity, bool brandEnforcedStockCount, int stock)
        {
            //TODO: It should come from configuration
            var maxQuantity = 10;

            if (isPharmaceutical)
            {
                // 1st priority is pharmaceutical item
                var maxPharmProduct = _shoppingCartSettings.MaxPharmaceuticalProduct;

                maxQuantity = maxPharmProduct;
            }
            else if ((discontinued == true)
                    || ((brandEnforcedStockCount || productEnforcedStockCount)))
            {
                // 2nd priority is discontinued item
                maxQuantity = maxQuantity > stock ? stock : 10;
            }

            var maxAllowedCartQuantity = 10;

            for (int i = stepQuantity; i <= maxQuantity; i = i + stepQuantity)
            {
                maxAllowedCartQuantity = i;
            }

            return maxAllowedCartQuantity;
        }
    }
}
