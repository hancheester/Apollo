using Apollo.Core.Caching;
using Apollo.Core.Domain.Media;
using Apollo.Core.Domain.Orders;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Domain.Tax;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Directory;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Apollo.Core.Services.Cart
{
    public class CartService : BaseRepository, ICartService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<OfferRule> _offerRuleRepository;
        private readonly IRepository<ProductMedia> _productMediaRepository;
        private readonly IRepository<ShippingOption> _shippingOptionRepository;
        private readonly IRepository<CartPharmOrder> _cartPharmOrderRepository;
        private readonly IRepository<CartPharmItem> _cartPharmItemRepository;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;       
        private readonly IOrderCalculator _orderCalculator;
        private readonly IOfferService _offerService;        
        private readonly ICurrencyService _currencyService;
        private readonly IShippingService _shippingService;
        private readonly IAccountService _accountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICartValidator _cartValidator;
        private readonly ICacheManager _cacheManager;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IProductBuilder _productBuilder;
        private readonly ICartItemBuilder _cartItemBuilder;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public CartService(
            IDbContext dbContext,
            IRepository<Account> accountRepository,
            IRepository<CartItem> cartItemRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Country> countryRepository,
            IRepository<ProductMedia> productMediaRepository,
            IRepository<ShippingOption> shippingOptionRepository,
            IRepository<OfferRule> offerRuleRepository,
            IRepository<CartPharmOrder> cartPharmOrderRepository,
            IRepository<CartPharmItem> cartPharmItemRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IOrderCalculator orderCalculator,
            IOfferService offerService,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            IShippingService shippingService,
            IAccountService accountService,
            ICartValidator cartValidator,
            ICacheManager cacheManager,
            ILogBuilder logBuilder,
            IProductBuilder productBuilder,
            ICartItemBuilder cartItemBuilder,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings,
            MediaSettings mediaSettings,
            ShippingSettings shippingSettings)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _cartItemRepository = cartItemRepository;
            _currencyRepository = currencyRepository;
            _countryRepository = countryRepository;
            _productMediaRepository = productMediaRepository;
            _shippingOptionRepository = shippingOptionRepository;
            _offerRuleRepository = offerRuleRepository;
            _cartPharmOrderRepository = cartPharmOrderRepository;
            _cartPharmItemRepository = cartPharmItemRepository;
            _taxCategoryRepository = taxCategoryRepository;
            _orderCalculator = orderCalculator;
            _offerService = offerService;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _shippingService = shippingService;
            _accountService = accountService;
            _cartValidator = cartValidator;
            _cacheManager = cacheManager;
            _productBuilder = productBuilder;
            _cartItemBuilder = cartItemBuilder;
            _shoppingCartSettings = shoppingCartSettings;
            _taxSettings = taxSettings;
            _mediaSettings = mediaSettings;
            _shippingSettings = shippingSettings;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        #region Create

        public int InsertCartItem(CartItem item)
        {
            return _cartItemRepository.Create(item);
        }

        #endregion

        #region Return

        public PagedList<CartItemOverviewModel> GetPagedCartItemOverviewModels(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> productIds = null,
            IList<int> userIds = null,
            string name = null,
            CartItemSortingType orderBy = CartItemSortingType.IdAsc)
        {
            var list = GetCartItemLoadPaged(
                pageIndex,
                pageSize,
                productIds,
                userIds,
                name,
                orderBy);

            var items = new Collection<CartItemOverviewModel>();

            for (int i = 0; i < list.Items.Count; i++)
            {
                var item = BuildCartItemOverviewModel(list.Items[i]);
                items.Add(item);
            }

            return new PagedList<CartItemOverviewModel>(items, pageIndex, pageSize, list.TotalCount);
        }

        public PagedList<CartItem> GetCartItemLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> productIds = null,
            IList<int> userIds = null,
            string name = null,
            CartItemSortingType orderBy = CartItemSortingType.IdAsc)
        {
            var pName = GetParameter("Name", name);            
            var pOrderBy = GetParameter("OrderBy", (int)orderBy, true);
            var pPageIndex = GetParameter("PageIndex", pageIndex, true);
            var pPageSize = GetParameter("PageSize", pageSize);
            var pTotalRecords = GetParameterIntegerOutput("TotalRecords");

            if (productIds != null && productIds.Contains(0))
                productIds.Remove(0);
            string commaSeparatedProductIds = productIds == null ? "" : string.Join(",", productIds);
            var pProductIds = GetParameter("ProductIds", commaSeparatedProductIds);

            if (userIds != null && userIds.Contains(0))
                userIds.Remove(0);
            string commaSeparatedUserIds = userIds == null ? "" : string.Join(",", userIds);
            var pUserIds = GetParameter("UserIds", commaSeparatedUserIds);

            var items = _dbContext.ExecuteStoredProcedureList<CartItem>(
                    "CartItem_LoadPaged",
                    pProductIds,
                    pUserIds,
                    pName,
                    pOrderBy,
                    pPageIndex,
                    pPageSize,
                    pTotalRecords);

            //return accounts
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;

            return new PagedList<CartItem>(items, pageIndex, pageSize, totalRecords);
        }

        public IList<CartItemOverviewModel> GetCartItemOverviewModelByProfileId(int profileId, bool autoRemovePhoneOrderItems = true, bool? isPharmaceutical = null)
        {
            var items = GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems, isPharmaceutical);

            var overviewItems = new List<CartItemOverviewModel>();

            foreach (var item in items)
                overviewItems.Add(BuildCartItemOverviewModel(item));
            
            return overviewItems;
        }
        
        public int GetTotalQuantityCartItemByProfileId(int profileId, bool autoRemovePhoneOrderItems = true)
        {
            var items = GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems);
            return items.Select(i => i.Quantity).DefaultIfEmpty(0).Sum();
        }
        
        public IList<CartItem> GetCartItemsByProfileId(int profileId, bool autoRemovePhoneOrderItems, bool ? isPharmaceutical = null)
        {
            var items = _cartItemRepository.Table
                .Where(c => c.ProfileId == profileId)
                .ToList()
                .Select(c => _cartItemBuilder.Build(c));

            if (isPharmaceutical.HasValue)
                items = items.Where(c => c.Product.IsPharmaceutical == isPharmaceutical.Value);

            if (autoRemovePhoneOrderItems)
            {
                var phoneOrderItemIds = items.Where(x => x.Product.IsPhoneOrder == true).Select(x => x.Id);
                if (phoneOrderItemIds.Count() > 0)
                {
                    foreach (var phoneOrderItemId in phoneOrderItemIds)
                    {
                        _cartItemRepository.Delete(phoneOrderItemId);
                    }

                    items = items.Where(x => x.Product.IsPhoneOrder == false);
                }
            }

            var list = items.ToList();

            return list;
        }

        public IList<ShippingOptionOverviewModel> GetCustomerShippingOptionByCountryAndPriority(int profileId, bool autoRemovePhoneOrderItems = true)
        {
            var props = _genericAttributeService.GetAttributesForEntity(profileId, "Profile");

            var gaCountryId = props.FirstOrDefault(ga =>
                 ga.Key.Equals(SystemCustomerAttributeNames.CountryId, StringComparison.InvariantCultureIgnoreCase));
            
            var countryId = gaCountryId != null ? Convert.ToInt32(gaCountryId.Value) : 0;
            if (countryId <= 0)
            {
                // If the profile doesn't have country selected, assign with a default country.
                var defaultCountryId = _shippingSettings.PrimaryStoreCountryId;
                _genericAttributeService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.CountryId, defaultCountryId.ToString());

                countryId = defaultCountryId;
            }
            var country = _shippingService.GetCountryById(countryId);

            var options = _shippingOptionRepository.Table
                .Where(x => x.CountryId == countryId && x.Enabled == true)
                .OrderBy(x => x.Priority)
                .Select(x => new
                {
                    x,
                    m = new ShippingOptionOverviewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Enabled = x.Enabled,
                        Priority = x.Priority,
                        CountryId = countryId,
                        FreeThreshold = x.FreeThreshold
                    }
                })
                .ToList();

            options.ForEach(x => x.x.Country = _shippingService.GetCountryById(x.x.CountryId));

            var cartItems = GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems);            

            var gaAllocatedPoint = props.FirstOrDefault(ga =>
                ga.Key.Equals(SystemCustomerAttributeNames.AllocatedPoint, StringComparison.InvariantCultureIgnoreCase));

            var usedPointValue = 0M;
            if (gaAllocatedPoint != null)
                usedPointValue = Convert.ToDecimal(gaAllocatedPoint.Value) / 100M;
            
            var cartOffer = ProcessCartOfferByProfileId(profileId, country.ISO3166Code);

            foreach (var option in options)
            {
                option.m.Cost = _orderCalculator.GetShippingCost(cartItems, option.x, cartOffer.DiscountAmount, usedPointValue);
            }
            
            return options.Select(o => o.m).ToList();
        }

        #endregion

        #region Command

        public void ClearCart(int profileId)
        {
            // Remove all cart items;
            DeleteCartItemsByProfileId(profileId);

            // Remove pharmaceutical form
            ClearCartPharmForm(profileId);

            // Clear points
            _genericAttributeService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.AllocatedPoint, "0");

            // Clear promocode
            _genericAttributeService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.DiscountCouponCode, string.Empty);
        }

        public OrderTotals CalculateOrderTotals(int profileId, bool autoRemovePhoneOrderItems = true, int testOfferRuleId = 0)
        {
            var orderTotals = new OrderTotals();
            var gas = _genericAttributeService.GetAttributesForEntity(profileId, "Profile");

            var gaCurrencyId = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.CurrencyId);
            if (gaCurrencyId == null) throw new ApolloException(string.Format("Currency is not selected. Profile ID={{{0}}}", profileId));
            var currencyId = Convert.ToInt32(gaCurrencyId.Value);
            var currency = _currencyRepository.Return(currencyId);

            orderTotals.CurrencyCode = currency.CurrencyCode;
            orderTotals.ExchangeRate = currency.ExchangeRate;

            var itemCount = GetTotalQuantityCartItemByProfileId(profileId, autoRemovePhoneOrderItems);

            if (itemCount <= 0) return orderTotals;

            orderTotals.ItemCount = itemCount;

            //get country
            var gaCountryId = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.CountryId);
            if (gaCountryId == null) throw new ApolloException(string.Format("Country is not selected. Profile ID={{{0}}}", profileId));
            var countryId = Convert.ToInt32(gaCountryId.Value);
            var country = _countryRepository.Return(countryId);

            //subtotal
            var items = GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems);

            orderTotals.Subtotal = items
                .Where(x => x.CartItemMode != (int)CartItemMode.FreeItem)
                .Select(x => 
                    ((x.CartItemMode == (int)CartItemMode.InitialPrice) ? x.ProductPrice.PriceExclTax : x.ProductPrice.OfferPriceExclTax) * x.Quantity)
                    .DefaultIfEmpty(0).Sum();
                        
            //discount
            var gaDiscountCouponCode = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.DiscountCouponCode);
            var discountCouponCode = string.Empty;
            if (gaDiscountCouponCode != null)
                discountCouponCode = gaDiscountCouponCode.Value;

            var cartOffer = ProcessCartOfferByProfileId(profileId, country.ISO3166Code, testOfferRuleId);
            if (cartOffer.IsValid)
            {
                foreach (var discount in cartOffer.Discounts)
                {
                    var offer = _offerService.GetOfferRuleById(discount.Key);
                    if (offer != null) orderTotals.Discounts.Add(offer.Alias, discount.Value);
                }

                orderTotals.Discount = cartOffer.DiscountAmount;
                orderTotals.AwardedPoints = cartOffer.RewardPoint;
            }

            //shipping info
            var gaSelectedShippingOption = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.SelectedShippingOption);
            if (gaSelectedShippingOption != null)
            {
                var selectedShippingOptionId = Convert.ToInt32(gaSelectedShippingOption.Value);
                var shippingOption = _shippingOptionRepository.Return(selectedShippingOptionId);

                orderTotals.ShippingCost = CalculateShippingCost(profileId, selectedShippingOptionId);
                orderTotals.ShippingMethod = shippingOption.Name;
            }

            //allocated points
            var gaAllocatedPoint = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.AllocatedPoint);
            var allocatedPoints = 0;
            if (gaAllocatedPoint != null) allocatedPoints = Convert.ToInt32(gaAllocatedPoint.Value);
            orderTotals.AllocatedPoints = allocatedPoints;
            var allocatedPointsValue = allocatedPoints / 100M;

            //tax
            if (country.IsEC)
            {
                orderTotals.Tax = _orderCalculator.GetVAT(items);
                orderTotals.DisplayTax = country.IsEC;
            }
            
            //total
            var total = orderTotals.Subtotal + orderTotals.Tax - orderTotals.Discount - allocatedPointsValue + orderTotals.ShippingCost;
            orderTotals.Total = total;

            //earned points
            var earnedPoint = _orderCalculator.CalculateEarnedLoyaltyPointsFromCart(
                                profileId,
                                orderTotals.AllocatedPoints,
                                currency.CurrencyCode,
                                orderTotals.Discount);
            orderTotals.EarnedPoints = earnedPoint;

            return orderTotals;
        }
                
        public bool CheckIfNeedPharmForm(int profileId, bool autoRemovePhoneOrderItems = true)
        {
            var hasPharmItem = HasPharmItem(profileId);

            // Pharm item availability
            if (hasPharmItem == false)
            {
                // Clear cart pharm form then
                ClearCartPharmForm(profileId);
                return false;
            }
            
            // If it is here, there is pharm item in the cart
            // Cart pharm form availability
            var form = _cartPharmOrderRepository.Table
                .Where(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (form == null) return true;

            // Cart pharm items availability
            var pharmItems = _cartPharmItemRepository.Table
                .Where(x => x.CartPharmOrderId == form.Id)
                .ToList();

            if (pharmItems.Count == 0) return true;

            var items = GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems, isPharmaceutical: true);

            // Check if pharm items quantity matches with cart items quantity
            if (pharmItems.Count != items.Count) return true;

            for(int i = 0; i < pharmItems.Count; i++)
            {
                var count = items.Where(x => x.ProductPriceId == pharmItems[i].ProductPriceId).Count();
                if (count == 0)
                    return true;
            }
            
            return false;
        }

        public bool HasPharmItem(int profileId)
        {
            int count = _cartItemRepository.TableNoTracking                
                .Where(x => x.ProfileId == profileId)
                .ToList()
                .Select(x => _cartItemBuilder.Build(x))
                .Where(x => x.Product.IsPharmaceutical == true)
                .Count();

            return count > 0;
        }

        public bool HasOnlyFreeNHSPrescriptionItem(int profileId)
        {
            var nhsPrescriptionProductId = 0;
            var items = _cartItemRepository.TableNoTracking
                .Where(x => x.ProfileId == profileId)
                .ToList()
                .Select(x => _cartItemBuilder.Build(x))
                .ToList();               

            if (items.Count == 1)
            {
                var product = items.Select(x => new { x.ProductId, x.ProductPrice.OfferPriceExclTax }).FirstOrDefault();
                return product.ProductId == nhsPrescriptionProductId && product.OfferPriceExclTax == 0M;
            }

            return false;
        }
        
        public decimal CalculateShippingCost(int profileId, int shippingOptionId)
        {
            var items = _cartItemRepository.Table.Where(c => c.ProfileId == profileId).ToList();
            var shippingOption = _shippingService.GetShippingOptionById(shippingOptionId);
            if (shippingOption == null) throw new ArgumentException(string.Format("Shipping option could not found with this id '{0}'", shippingOptionId));
            shippingOption.Country = _countryRepository.Return(shippingOption.CountryId);

            var props = _genericAttributeService.GetAttributesForEntity(profileId, "Profile");
            var gaAllocatedPoint = props.FirstOrDefault(ga => ga.Key.Equals(SystemCustomerAttributeNames.AllocatedPoint, StringComparison.InvariantCultureIgnoreCase));
            var allocatedPoint = 0;
            if (gaAllocatedPoint != null) allocatedPoint = Convert.ToInt32(gaAllocatedPoint.Value);
            var usedPointValue = allocatedPoint / 100M;
            
            var cartOffer = ProcessCartOfferByProfileId(profileId, shippingOption.Country.ISO3166Code);

            return _orderCalculator.GetShippingCost(items,
                                               shippingOption,
                                               cartOffer.DiscountAmount,
                                               usedPointValue);
        }

        public int CalculateEarnedLoyaltyPointsFromCart(int profileId)
        {
            var props = _genericAttributeService.GetAttributesForEntity(profileId, "Profile");

            var gaCountryId = props.FirstOrDefault(ga => ga.Key.Equals(SystemCustomerAttributeNames.CountryId, StringComparison.InvariantCultureIgnoreCase));
            if (gaCountryId == null) throw new ApolloException("Country ID is not found with this profile. Profile ID={{{0}}}", profileId);
            var country = _shippingService.GetCountryById(Convert.ToInt32(gaCountryId.Value));

            var gaCurrencyId = props.FirstOrDefault(ga => ga.Key.Equals(SystemCustomerAttributeNames.CurrencyId, StringComparison.InvariantCultureIgnoreCase));
            if (gaCountryId == null) throw new ApolloException("Currency ID is not found with this profile. Profile ID={{{0}}}", profileId);
            var currency = _currencyService.GetCurrency(Convert.ToInt32(gaCurrencyId.Value));

            var gaAllocatedPoint = props.FirstOrDefault(ga => ga.Key.Equals(SystemCustomerAttributeNames.AllocatedPoint, StringComparison.InvariantCultureIgnoreCase));
            var allocatedPoint = 0;
            if (gaAllocatedPoint != null) allocatedPoint = Convert.ToInt32(gaAllocatedPoint.Value);

            var cartOffer = ProcessCartOfferByProfileId(profileId, country.ISO3166Code);

            return _orderCalculator.CalculateEarnedLoyaltyPointsFromCart(profileId, allocatedPoint, currency.CurrencyCode, cartOffer.DiscountAmount);
        }

        public string ProcessItemQuantityUpdate(int profileId, string shippingCountryCode, int cartItemId, int quantity)
        {
            var message = string.Empty;

            try
            {
                var cartItem = _cartItemRepository.Return(cartItemId);
                if (cartItem == null)            
                    throw new ApolloException(string.Format("Cart item could not be loaded. Cart Item ID={{{0}}}", cartItemId));

                cartItem = _cartItemBuilder.Build(cartItem);

                // Validate cart item first
                message = _cartValidator.ValidateCartItem(
                    profileId,
                    cartItem.ProductId,
                    cartItem.Product.BrandId,
                    shippingCountryCode,
                    cartItem.Product.IsPharmaceutical,
                    cartItem.Product.Discontinued,
                    cartItem.ProductPrice.Stock,
                    cartItem.Product.Name,
                    cartItem.Product.EnforceStockCount,
                    cartItem.Product.Brand.EnforceStockCount,
                    cartItem.Product.Enabled,
                    cartItem.ProductPrice.Enabled,
                    cartItem.Quantity,
                    quantity,
                    cartItem.Product.StepQuantity,
                    cartItem.Product.IsPhoneOrder,
                    cartItem.ProductPrice.OfferPriceInclTax);

                // Empty message is ok status from cart item validation
                if (string.IsNullOrEmpty(message))
                {
                    cartItem.Quantity = quantity;
                    cartItem.UpdatedOnDate = DateTime.Now;
                    _cartItemRepository.Update(cartItem);

                    ProcessCartOfferByProfileId(profileId, shippingCountryCode);
                }
            }
            catch (Exception ex)
            {
                message = "Failed to update quantity. Please try to delete and add the item again.";
                _logger.InsertLog(LogLevel.Error, "Error occurred while updating cart item quantity. " + ex.Message, ex);
            }

            return message;
        }
        
        public string ProcessItemAddition(
            int profileId,
            int productId,
            int productPriceId,
            string shippingCountryCode,
            int quantity,            
            int testOfferRuleId = 0,
            bool disablePhoneOrderCheck = false)
        {
            var message = string.Empty;
            try
            {
                var product = _productBuilder.Build(productId);
                if (product == null) throw new ApolloException("Product could not be loaded. Product ID={{{0}}}", productId);

                var productPrice = product.ProductPrices.Where(x => x.Id == productPriceId).FirstOrDefault();
                if (productPrice == null) throw new ApolloException("Product price could not be loaded. Product ID={{{0}}}, Product Price ID={{{0}}}", productId, productPriceId);

                // Validate cart item first
                var quantityInCart = _cartItemRepository.Table
                    .Where(c => c.ProfileId == profileId)
                    .Where(c => c.ProductPriceId == productPriceId)
                    .Where(c => c.CartItemMode != (int)CartItemMode.FreeItem)
                    .Select(c => c.Quantity)
                    .DefaultIfEmpty(0)
                    .Sum();

                message = _cartValidator.ValidateCartItem(profileId,
                                                          productId,
                                                          product.Brand.Id,
                                                          shippingCountryCode,
                                                          product.IsPharmaceutical,
                                                          product.Discontinued,
                                                          productPrice.Stock,
                                                          product.Name,
                                                          product.EnforceStockCount,
                                                          product.Brand.EnforceStockCount,
                                                          product.Enabled,
                                                          productPrice.Enabled,
                                                          quantityInCart,
                                                          quantity,
                                                          product.StepQuantity,
                                                          disablePhoneOrderCheck ? false : product.IsPhoneOrder,
                                                          productPrice.OfferPriceExclTax,
                                                          product.Id == 0);

                // Empty message is ok status from cart item validation
                if (string.IsNullOrEmpty(message))
                {
                    var status = false;
                    message = "Sorry, an error occurred while creating/updating item.";

                    var cartItem = new CartItem
                    {
                        ProfileId = profileId,
                        ProductPriceId = productPriceId,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    
                    status = EditCartItem(profileId, cartItem);

                    if (status)
                    {
                        message = string.Empty;
                        ProcessCartOfferByProfileId(profileId, shippingCountryCode, testOfferRuleId);
                    }
                }
                
            }
            catch(Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, "Error occurred while creating/updating item. " + ex.Message, ex);
                message = "Sorry, an error occurred while creating/updating item.";
            }

            return message;
        }
        
        public void MigrateShoppingCart(int fromProfileId, int toProfileId, string shippingCountryCode, bool autoRemovePhoneOrderItems = true)
        {
            if (fromProfileId == 0) throw new ArgumentException("Value should not be 0.", "fromProfileId");
            if (toProfileId == 0) throw new ArgumentException("Value should not be 0.", "toProfileId");

            if (fromProfileId == toProfileId) return;
            var items = GetCartItemsByProfileId(fromProfileId, autoRemovePhoneOrderItems);

            foreach (var item in items)
            {
                if (item.CartItemMode != (int)CartItemMode.FreeItem)
                {
                    ProcessItemAddition(
                        toProfileId,
                        item.ProductId,
                        item.ProductPriceId,
                        shippingCountryCode,
                        item.Quantity);
                }                
            }

            DeleteCartItemsByProfileId(fromProfileId);
        }

        public void ProcessCartPharmOrder(CartPharmOrder cartPharmOrder)
        {
            // Clear everything first.
            var form = _cartPharmOrderRepository.Table.Where(x => x.ProfileId == cartPharmOrder.ProfileId).FirstOrDefault();
            if (form != null)
            {
                var pCartPharmOrderId = GetParameter("CartPharmOrderId", form.Id);
                _dbContext.ExecuteSqlCommand("DELETE FROM CartPharmItem WHERE CartPharmOrderId = @CartPharmOrderId", pCartPharmOrderId);
                _cartPharmOrderRepository.Delete(form.Id);
            }

            _cartPharmOrderRepository.Create(cartPharmOrder);
            
            for(int i = 0; i < cartPharmOrder.Items.Count; i++)
            {
                cartPharmOrder.Items[i].CartPharmOrderId = cartPharmOrder.Id;
                _cartPharmItemRepository.Create(cartPharmOrder.Items[i]);
            }
        }

        public AllocatedPointResult ProcessAllocatedPoints(int profileId, int allocatedPoints, bool autoRemovePhoneOrderItems = true)
        {
            var result = new AllocatedPointResult { AllocatedPoints = allocatedPoints };
            var items = GetCartItemsByProfileId(profileId, autoRemovePhoneOrderItems).ToList();
            
            // Only for offers that allow points to be spent
            var subtotal = 0M;
            items.ForEach(delegate (CartItem item)
            {
                if (item.ProductPrice.OfferRuleId == 0)
                {
                    subtotal += item.Quantity * item.ProductPrice.OfferPriceInclTax;
                    return;
                }

                var offer = _offerService.GetOfferRuleOnlyById(item.ProductPrice.OfferRuleId);
                if (offer == null || offer.PointSpendable)
                    subtotal += item.Quantity * item.ProductPrice.OfferPriceInclTax;                
            });

            var pointsBalance = _accountService.GetLoyaltyPointsBalanceByProfileId(profileId);

            if (allocatedPoints > pointsBalance)
            {
                result.Message = "Sorry, you do not have enough points to use.";
                result.AllocatedPoints = 0;
                return result;
            }
            
            int maxPoints = Convert.ToInt32(subtotal * 100);

            if (allocatedPoints > maxPoints)
            {
                result.AllocatedPoints = maxPoints;

                if (maxPoints <= 0)
                {
                    result.Message = string.Format("Sorry, you are not allowed to use any points.");
                }
                else
                {
                    result.Message = string.Format("Sorry, you have overused your maximum points. Your maximum points are {0}.", maxPoints);
                }
            }                

            return result;
        }

        public string[] ProcessPostalRestrictionRules(int profileId, string shippingCountryCode)
        {
            return _cartValidator.ValidateCartAgainstShippingCountry(profileId, shippingCountryCode);
        }

        public string ProcessPrescriptionUpdate(int profileId, int productPriceId, int quantity)
        {
            var message = string.Empty;

            try
            {
                var prescriptionProductId = 0;

                var prescriptionCartItem = _cartItemRepository.Table
                    .Where(x => x.ProfileId == profileId)
                    .Where(x => x.ProductId == prescriptionProductId)
                    .Where(x => x.ProductPriceId == productPriceId)
                    .FirstOrDefault();

                if (prescriptionCartItem != null)
                {
                    prescriptionCartItem.Quantity = quantity;
                    _cartItemRepository.Update(prescriptionCartItem);
                }
                else
                {
                    var product = _productBuilder.Build(prescriptionProductId);

                    if (product == null)
                        throw new ApolloException("Product for prescription could not be loaded. Prescription Product ID={{{0}}}", prescriptionProductId);

                    var priceOption = product.ProductPrices.Where(x => x.Id == productPriceId).FirstOrDefault();
                    
                    if (priceOption == null)
                        throw new ApolloException("Product price for prescription could not be loaded. Prescription Product ID={{{0}}}", prescriptionProductId);

                    var media = _productMediaRepository.Table.Where(x => x.ProductId == product.Id).FirstOrDefault();
                    var image = media != null ? media.ThumbnailFilename : _mediaSettings.NoImagePath;

                    var newCartItem = new CartItem
                    {
                        ProfileId = profileId,
                        ProductId = prescriptionProductId,
                        ProductPriceId = priceOption.Id,
                        Quantity = quantity
                    };

                    _cartItemRepository.Create(newCartItem);
                }
            }
            catch (Exception ex)
            {
                message = "Sorry, an error occurred while creating/updating your prescription.";
                _logger.InsertLog(LogLevel.Error, "Error occurred while creating/updating prescription item. " + ex.Message, ex);
            }

            return message;
        }

        #endregion

        #region Update
        
        public void UpdateLineQuantityByProfileIdAndProductPriceId(int profileId, int productPriceId, int quantity)
        {
            var item = _cartItemRepository.Table
                .Where(c => c.ProfileId == profileId)
                .Where(c => c.ProductPriceId == productPriceId)                
                .FirstOrDefault();

            item.Quantity = quantity;
            _cartItemRepository.Update(item);
        }
        
        #endregion
        
        #region Delete

        public void DeleteCartItemsByProfileId(int profileId)
        {
            var cartItems = _cartItemRepository.Table.Where(c => c.ProfileId == profileId).ToList();

            foreach (var cartItem in cartItems)
            {
                // Clear related product cache in order to update stock level
                _cacheManager.Remove(string.Format(CacheKey.PRODUCT_BY_ID_KEY, cartItem.ProductId));

                _cartItemRepository.Delete(cartItem);
            }
        }

        public void DeleteCartItemsByProfileIdAndPriceId(int profileId, int productPriceId, bool freeItemIncluded = false)
        {
            var items = _cartItemRepository.Table
                .Where(c => c.ProfileId == profileId && c.ProductPriceId == productPriceId)
                .ToList()
                .Select(c => _cartItemBuilder.Build(c));

            if (freeItemIncluded == false)
                items = items.Where(c => c.ProductPrice.OfferPriceExclTax > 0).ToList();
             
            var item = items.FirstOrDefault();

            if (item != null) _cartItemRepository.Delete(item);
        }

        public void DeleteCartItemsByProfileIdAndCartItemId(int profileId, int cartItemId)
        {
            var item = _cartItemRepository.Table
                .Where(c => c.ProfileId == profileId && c.Id == cartItemId)
                .FirstOrDefault();

            if (item != null)
                _cartItemRepository.Delete(item);

            ProcessCartOfferByProfileId(profileId);
        }

        public int DeleteOldCartItem(DateTime? lastUpdateOnDateFromUtc)
        {
            var pLastUpdatedOnDateFromUtc = GetParameter("LastUpdatedOnDateFromUtc", lastUpdateOnDateFromUtc);
            var pTotalRecordsDeleted = GetParameterIntegerOutput("TotalRecordsDeleted");

            //invoke stored procedure
            _dbContext.ExecuteSqlCommand(
                "EXEC [DeleteOldCartItem] @LastUpdatedOnDateFromUtc, @TotalRecordsDeleted = @TotalRecordsDeleted OUTPUT",
                pLastUpdatedOnDateFromUtc,
                pTotalRecordsDeleted);

            int totalRecordsDeleted = (pTotalRecordsDeleted.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecordsDeleted.Value) : 0;
            return totalRecordsDeleted;
        }

        #endregion

        #region Private methods
        
        private CartItemOverviewModel BuildCartItemOverviewModel(CartItem item)
        {
            return new CartItemOverviewModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductPriceId = item.ProductPriceId,
                Name = item.Product.Name,
                Quantity = item.Quantity,
                OfferPriceExclTax = item.GetPrice(inclTax: false),
                OfferPriceInclTax = item.GetPrice(),
                PriceExclTax = item.ProductPrice.PriceExclTax,
                PriceInclTax = item.ProductPrice.PriceInclTax,
                Option = item.ProductPrice.Option,
                ThumbnailFilename = item.Product.ProductMedias.Count > 0 ? item.Product.ProductMedias[0].ThumbnailFilename : string.Empty,
                UrlKey = item.Product.UrlRewrite,
                StepQuantity = item.Product.StepQuantity,
                MaxAllowedQuantity = item.Product.IsPharmaceutical ? _shoppingCartSettings.MaxPharmaceuticalProduct : 10,
                IsPharmaceutical = item.Product.IsPharmaceutical,
                Discontinued = item.Product.Discontinued,
                BrandEnforcedStockCount = item.Product.Brand.EnforceStockCount,
                ProductEnforcedStockCount = item.Product.EnforceStockCount,
                Stock = item.ProductPrice.Stock,
                MaximumAllowedPurchaseQuantity = item.ProductPrice.MaximumAllowedPurchaseQuantity
            };
        }

        private bool EditCartItem(int profileId, CartItem cartItem)
        {
            try
            {
                var foundItem = _cartItemRepository.Table
                    .Where(c => c.ProfileId == cartItem.ProfileId)
                    .Where(c => c.ProductId == cartItem.ProductId)
                    .Where(c => c.ProductPriceId == cartItem.ProductPriceId)
                    .FirstOrDefault();
                
                if (foundItem == null)
                {
                    _cartItemRepository.Create(cartItem);
                    return true;
                }            
                
                foundItem = _cartItemBuilder.Build(foundItem);

                var maxAllowedQuantity = 10;
                
                // 1st priority is discontinued
                if ((foundItem.Product.Discontinued == true)
                    || (((foundItem.Product.Brand != null && foundItem.Product.Brand.EnforceStockCount) || foundItem.Product.EnforceStockCount)))
                {
                    maxAllowedQuantity = maxAllowedQuantity > foundItem.ProductPrice.Stock ? foundItem.ProductPrice.Stock : 10;
                }
                
                if ((foundItem.Quantity + cartItem.Quantity) <= maxAllowedQuantity)
                    foundItem.Quantity = foundItem.Quantity + cartItem.Quantity;
                else
                    foundItem.Quantity = maxAllowedQuantity;

                _cartItemRepository.Update(foundItem);

                return true;
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, ex.Message, ex);
                return false;
            }
        }

        private CartOffer ProcessCartOfferByProfileId(int profileId, string shippingCountryCode = null, int testOfferRuleId = 0)
        {
            var gas = _genericAttributeService.GetAttributesForEntity(profileId, "Profile");

            if (string.IsNullOrEmpty(shippingCountryCode))
            {
                // Get country
                var gaCountryId = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.CountryId);
                var countryId = gaCountryId != null ? Convert.ToInt32(gaCountryId.Value) : 0;
                if (countryId <= 0)
                {
                    // If the profile doesn't have country selected, assign with a default country.
                    var defaultCountryId = _shippingSettings.PrimaryStoreCountryId;
                    _genericAttributeService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.CountryId, defaultCountryId.ToString());

                    countryId = defaultCountryId;
                }
                
                var country = _shippingService.GetCountryById(countryId);
                shippingCountryCode = country.ISO3166Code;
            }

            var cartOffer = _offerService.ProcessCartOfferByProfileId(profileId, shippingCountryCode, testOfferRuleId);

            return cartOffer;
        }

        private void ClearCartPharmForm(int profileId)
        {
            // Remove pharmaceutical form
            var pharmForm = _cartPharmOrderRepository.Table.Where(x => x.ProfileId == profileId).FirstOrDefault();

            if (pharmForm != null)
            {
                var pharmItems = _cartPharmItemRepository.Table.Where(x => x.CartPharmOrderId == pharmForm.Id).ToList();

                for (int i = 0; i < pharmItems.Count; i++)
                {
                    _cartPharmItemRepository.Delete(pharmItems[i]);
                }

                _cartPharmOrderRepository.Delete(pharmForm);
            }
        }

        #endregion
    }
}
