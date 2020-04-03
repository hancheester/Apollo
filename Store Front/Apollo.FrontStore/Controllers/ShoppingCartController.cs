using Apollo.Core;
using Apollo.Core.Caching;
using Apollo.Core.Domain.Tax;
using Apollo.Core.Model;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Infrastructure;
using Apollo.FrontStore.Models;
using Apollo.FrontStore.Models.Prescription;
using Apollo.FrontStore.Models.ShoppingCart;
using Apollo.Web.Framework.Security;
using Apollo.Web.Framework.Services.Catalog;
using Apollo.Web.Framework.Services.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class ShoppingCartController : BasePublicController
    {
        private static ILog _logger = LogManager.GetLogger(typeof(ShoppingCartController).FullName);
        
        private readonly IAccountService _accountService;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IUtilityService _utilityService;
        private readonly IOfferService _offerService;        
        private readonly IWorkContext _workContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICacheManager _cacheManager;
        private readonly TaxSettings _taxSettings;
        private readonly ApolloSessionState _session;

        public ShoppingCartController(
            IAccountService accountService,
            ICartService cartService,
            IProductService productService,
            IShippingService shippingService,
            IUtilityService utilityService,
            IOfferService offerService,
            IWorkContext workContext,
            IPriceFormatter priceFormatter,
            ICacheManager cacheManager,
            TaxSettings taxSettings,
            ApolloSessionState session)
        {
            _accountService = accountService;
            _cartService = cartService;
            _productService = productService;
            _shippingService = shippingService;
            _utilityService = utilityService;
            _offerService = offerService;
            _workContext = workContext;
            _priceFormatter = priceFormatter;
            _cacheManager = cacheManager;
            _taxSettings = taxSettings;
            _session = session;
        }

        public ActionResult Cart()
        {
            var profileId = _workContext.CurrentProfile.Id;
            
            if (!string.IsNullOrEmpty(_session["error"] as string))
            {
                ViewBag.ErrorMessage = Convert.ToString(_session["error"]);
                _session["error"] = null;
            }
            
            var messages = _cartService.ProcessPostalRestrictionRules(profileId, _workContext.CurrentCountry.ISO3166Code);
            if (messages != null && messages.Length > 0)
            {
                if (string.IsNullOrEmpty(ViewBag.ErrorMessage as string))
                    ViewBag.ErrorMessage = string.Join("<br/><br/>", messages);
                else
                    ViewBag.ErrorMessage += "<br/>" + string.Join("<br/><br/>", messages);
            }

            var items = _cartService.GetCartItemOverviewModelByProfileId(profileId);
            var model = PrepareShoppingCartModel(items, isAllowedToProceed: messages.Length == 0);

            return View(model);
        }

        [HttpPost]
        public ActionResult ApplyCode(string code)
        {
            var profileId = _workContext.CurrentProfile.Id;

            _utilityService.SaveAttribute(
                profileId,
                "Profile",
                SystemCustomerAttributeNames.DiscountCouponCode,
                code);

            var cartOffer = _offerService.ProcessCartOfferByProfileId(profileId, _workContext.CurrentCountry.ISO3166Code);
            
            if (cartOffer.IsValid == false && !string.IsNullOrEmpty(code))
            {
                _session["error"] = "Sorry, the code has been entered incorrectly, has expired or the items aren't eligible. Please check terms and conditions for the code.";
            }

            return RedirectToAction("Cart");
        }

        [HttpGet]
        public ActionResult RemoveCode()
        {
            var profileId = _workContext.CurrentProfile.Id;
            var promocode = _workContext.CurrentProfile.GetAttribute<string>("Profile", SystemCustomerAttributeNames.DiscountCouponCode);
            if (string.IsNullOrEmpty(promocode) == false) _offerService.RemoveCartOfferByPromoCode(profileId, promocode);

            _utilityService.SaveAttribute(
                profileId,
                "Profile",
                SystemCustomerAttributeNames.DiscountCouponCode,
                string.Empty);
            
            _offerService.ProcessCartOfferByProfileId(profileId, _workContext.CurrentCountry.ISO3166Code);

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public ActionResult ApplyPoint(LoyaltyPointModel model)
        {
            int allocatedPoints = 0;
            int.TryParse(model.Points, out allocatedPoints);
            var profileId = _workContext.CurrentProfile.Id;

            var result = _cartService.ProcessAllocatedPoints(profileId, allocatedPoints);

            if (!string.IsNullOrEmpty(result.Message))
            {
                _session["error"] = result.Message;
            }
            
            _utilityService.SaveAttribute(
                profileId,
                "Profile",
                SystemCustomerAttributeNames.AllocatedPoint,
                result.AllocatedPoints.ToString());
            
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public ActionResult ClearPoint()
        {
            _utilityService.SaveAttribute(
                _workContext.CurrentProfile.Id,
                "Profile",
                SystemCustomerAttributeNames.AllocatedPoint,
                "0");

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public ActionResult UpdateShippingOption(int optionid)
        {
            var profileId = _workContext.CurrentProfile.Id;

            _utilityService.SaveAttribute(
                profileId, 
                "Profile",
                SystemCustomerAttributeNames.SelectedShippingOption,
                optionid.ToString());
            
            return RedirectToAction("Cart");
        }

        [AjaxOnly]
        public ActionResult LoadShippingOptions(int countryid)
        {
            var country = _shippingService.GetCountryById(countryid);
            _workContext.CurrentCountry = country;

            var profileId = _workContext.CurrentProfile.Id;
            var options = _cartService.GetCustomerShippingOptionByCountryAndPriority(profileId);

            // Select default option
            _utilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.SelectedShippingOption, options[0].Id.ToString());

            var models = options.PrepareShippingOptionModel(options[0].Id);
            var ShippingOptionSectionHtml = RenderPartialViewToString("_ShippingOptions", models);

            return Json(new
            {
                success = true,
                shippingOptionSectionHtml = ShippingOptionSectionHtml
            });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddItemWithDetails(int productid, FormCollection form)
        {
            try
            {
                int profileId = _workContext.CurrentProfile.Id;
                int productPriceId = Convert.ToInt32(form[string.Format("product_price_{0}", productid)]);
                int quantity = Convert.ToInt32(form[string.Format("addtocart_{0}.EnteredQuantity", productid)]);

                string error = _cartService.ProcessItemAddition(
                    profileId,
                    productid,
                    productPriceId,
                    _workContext.CurrentCountry.ISO3166Code,
                    quantity);

                if (string.IsNullOrEmpty(error))
                {
                    return PrepareAjaxBasketResult(profileId);
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = error
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Product could not be added to cart. Product ID={{{0}}}", productid), ex);
                return Json(new
                {
                    success = false,
                    message = "Sorry, an error occurred while creating/updating item."
                });
            }
        }

        [HttpPost]
        [AjaxOnly]
        public virtual ActionResult AddProductToCartFromCatalog(int productid, int shoppingcarttypeid, int quantity, int? productpriceid = null)
        {
            try
            {
                // TODO: We need to prepare wishlist for future.
                var cartType = (ShoppingCartType)shoppingcarttypeid;

                var product = _productService.GetProductOverviewModelById(productid);
                if (product == null)
                    //no product found
                    return Json(new
                    {
                        success = false,
                        message = "No product found with the specified ID"
                    });

                var productPrices = _productService.GetProductPricesByProductId(productid);
                if (productPrices.Count > 1 && productpriceid.HasValue == false)
                {
                    var model = new QuickAddToCartModel
                    {
                        Product = product,
                        Options = productPrices.PrepareProductPriceModels(
                        product.OptionType,
                        product.Discontinued,
                        product.ProductEnforcedStockCount,
                        product.BrandEnforcedStockCount)
                    };

                    return Json(new
                    {
                        updateQuickAddToCartSectionHtml = RenderPartialViewToString("QuickAddToCart", model)
                    });
                }
                else
                {
                    if (productpriceid.HasValue != true)
                        productpriceid = productPrices[0].Id;
                }

                //products with "step quantity" more than a specified qty
                if (product.StepQuantity > quantity)
                {
                    //we cannot add to the cart such products from category pages
                    //it can confuse customers. That's why we redirect customers to the product details page
                    return Json(new
                    {
                        redirect = Url.RouteUrl("Product", new { urlKey = product.UrlKey }),
                    });
                }

                if (quantity > 0 && (quantity % product.StepQuantity) != 0)
                {
                    //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                    return Json(new
                    {
                        redirect = Url.RouteUrl("Product", new { SeName = product.UrlKey }),
                    });
                }

                int profileId = _workContext.CurrentProfile.Id;
                string countryCode = _workContext.CurrentCountry.ISO3166Code;
                string error = _cartService.ProcessItemAddition(
                        profileId,
                        productid,
                        productpriceid.Value,
                        countryCode,
                        quantity);

                if (string.IsNullOrEmpty(error))
                {
                    string offerMessage = null;
                    if (product.RelatedOffers.Count > 0)
                    {
                        var offer = product.RelatedOffers.Where(x => x.OfferRuleType == OfferRuleType.Cart).FirstOrDefault();
                        if (offer != null && offer.ShowInOfferPage)
                        {
                            offerMessage = offer.ShortDescription + string.Format(" This offer is subject to <a href=\"{0}\">terms & conditions</a>.", Url.RouteUrl("Individual Offer", new { urlKey = offer.UrlKey }));
                        }
                    }

                    return PrepareAjaxBasketResult(
                        profileId, 
                        offerMessage: offerMessage, 
                        hideQuickAddToCartSection: true);
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = error
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Product could not be added to cart. Product ID={{{0}}}", productid), ex);
                return Json(new
                {
                    success = false,
                    message = "Sorry, an error occurred while creating/updating item."
                });
            }
        }
        
        public ActionResult RemoveItem(int cartitemid)
        {
            _cartService.DeleteCartItemsByProfileIdAndCartItemId(_workContext.CurrentProfile.Id, cartitemid);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddPrescription(PrescriptionOrderModel form)
        {
            try
            {
                int profileId = _workContext.CurrentProfile.Id;

                int productPriceId = 0;
                if (form.HasExemption.Value)
                {
                    productPriceId = Convert.ToInt32(form.EnteredExemptionId);
                }
                else
                {
                    var nhsPrescriptionProductId = 0;
                    var prescriptionProduct = _productService.GetProductById(nhsPrescriptionProductId);

                    productPriceId = prescriptionProduct.ProductPrices.Where(x => x.Price > 0M).Select(x => x.Id).FirstOrDefault();
                }

                int quantity = form.EnteredQuantity;
                decimal price = form.HasExemption.Value ? 0M : 123;

                string error = _cartService.ProcessPrescriptionUpdate(profileId, productPriceId, quantity);
                
                if (string.IsNullOrEmpty(error))
                {
                    return PrepareAjaxBasketResult(profileId);
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = error
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Prescription could not be added to cart.", ex);
                return Json(new
                {
                    success = false,
                    message = "Sorry, an error occurred while updating your prescription."
                });
            }
        }

        [HttpPost]
        public ActionResult ChangeQuantity(int cartitemid, int quantity)
        {
            string message = _cartService.ProcessItemQuantityUpdate(
                _workContext.CurrentProfile.Id,
                _workContext.CurrentCountry.ISO3166Code,
                cartitemid,
                quantity);

            if (!string.IsNullOrEmpty(message))
                _session["error"] = message;

            return RedirectToAction("Cart");
        }

        [ChildActionOnly]
        public ActionResult OrderSummary(bool? prepareAndDisplayOrderReviewData)
        {
            var items = _cartService.GetCartItemOverviewModelByProfileId(_workContext.CurrentProfile.Id);
            var model = PrepareShoppingCartModel(items,
                isEditable: false,
                prepareEstimateShippingIfEnabled: false,
                prepareAndDisplayOrderReviewData: prepareAndDisplayOrderReviewData.GetValueOrDefault());
            
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult FlyoutShoppingCart()
        {
            var model = PrepareMiniShoppingCartModel();
            return PartialView(model);
        }
        
        [ChildActionOnly]
        public ActionResult ShoppingCartMiniSummary()
        {
            int profileId = _workContext.CurrentProfile.Id;         
            ViewBag.Quantity = _cartService.GetTotalQuantityCartItemByProfileId(profileId);
            
            var items = _cartService.GetCartItemOverviewModelByProfileId(profileId);
            ViewBag.Subtotal = GetSubtotalString(items);

            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult OrderTotals(bool isEditable)
        {
            var model = PrepareOrderTotalsModel(isEditable);
            return PartialView(model);
        }

        #region Utilities

        [NonAction]
        protected string GetSubtotalString(IList<CartItemOverviewModel> items)
        {
            if (_workContext.CurrentCountry.IsEC)            
                return _priceFormatter.FormatPrice(items.Select(x => x.OfferPriceInclTax * x.Quantity).DefaultIfEmpty(0).Sum());
            
            return _priceFormatter.FormatPrice(items.Select(x => x.OfferPriceExclTax * x.Quantity).DefaultIfEmpty(0).Sum());
        }

        [NonAction]
        protected ShoppingCartModel PrepareShoppingCartModel(
            IList<CartItemOverviewModel> items,
            bool isEditable = true,
            bool isAllowedToProceed = true,
            bool prepareEstimateShippingIfEnabled = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            var model = new ShoppingCartModel
            {
                IsEditable = isEditable,
                IsAllowedToProceed = isAllowedToProceed,
                Items = items.ApplyMaximumQuantityRule().PrepareCartItemModels(isEditable),
                OrderTotal = items.Select(i => i.OfferPriceExclTax * i.Quantity).Sum()
            };

            #region Discount box

            var profileId = _workContext.CurrentProfile.Id;
            var country = _workContext.CurrentCountry;
            var cartOffer = _offerService.ProcessCartOfferByProfileId(profileId, country.ISO3166Code);

            var discountBoxModel = new ShoppingCartModel.DiscountBoxModel();
            var promocode = _workContext.CurrentProfile.GetAttribute<string>("Profile", SystemCustomerAttributeNames.DiscountCouponCode);

            if (cartOffer.IsValid)
            {
                discountBoxModel.IsApplied = true;
                discountBoxModel.CurrentCode = promocode;
                discountBoxModel.Message = cartOffer.Description;
            }

            model.DiscountBox = discountBoxModel;

            #endregion

            #region Loyalty box

            var loyaltyBox = new ShoppingCartModel.LoyaltyBoxModel();
            loyaltyBox.IsLoggedIn = !_workContext.CurrentProfile.IsAnonymous;

            if (loyaltyBox.IsLoggedIn)
            {
                var allocatedPoints = _workContext.CurrentProfile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.AllocatedPoint, _utilityService);
                loyaltyBox.AllocatedPoints = allocatedPoints;

                var loyaltyPoints = _accountService.GetLoyaltyPointsBalanceByProfileId(profileId);
                loyaltyBox.MyPoints = loyaltyPoints;
            }

            model.LoyaltyBox = loyaltyBox;

            #endregion

            #region Estimate shipping

            if (prepareEstimateShippingIfEnabled)
            {
                var defaultCountryId = country.Id;
                var countries = _shippingService.GetActiveCountries();
                model.EstimateShipping.AvailableCountries = countries
                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == defaultCountryId })
                    .ToList();

                if (defaultCountryId != 0)
                {
                    var options = _cartService.GetCustomerShippingOptionByCountryAndPriority(profileId);
                    var selectedShippingOption = _workContext.CurrentProfile.GetAttribute<int>("Profile", SystemCustomerAttributeNames.SelectedShippingOption);
                    
                    if (selectedShippingOption != 0)
                    {           
                        model.EstimateShipping.ShippingOptions = options.PrepareShippingOptionModel(selectedShippingOption);
                    }
                    else
                    {
                        //If no selected shipping option, choose a default option                         
                        model.EstimateShipping.ShippingOptions = options.PrepareShippingOptionModel(options[0].Id);
                        _utilityService.SaveAttribute(
                            profileId,
                            "Profile",
                            SystemCustomerAttributeNames.SelectedShippingOption,
                            options[0].Id.ToString());
                    }
                }
            }

            #endregion

            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                var accountId = _accountService.GetAccountIdByProfileId(profileId);
                var hasOnlyFreeNHSPrescriptionItem = _cartService.HasOnlyFreeNHSPrescriptionItem(profileId);
                var orderTotals = _cartService.CalculateOrderTotals(profileId);
                var displayBillingAddress = orderTotals.Total > 0M && hasOnlyFreeNHSPrescriptionItem == false;

                if (displayBillingAddress)
                { 
                    model.OrderReviewData.DisplayBillingAddress = true;
                    //billing address
                    var billing = _accountService.GetBillingAddressByAccountId(accountId);
                    if (billing != null) model.OrderReviewData.BillingAddress = billing.PrepareAddressModel();
                }

                //shipping address
                var shipping = _accountService.GetShippingAddressByAccountId(accountId);
                if (shipping != null) model.OrderReviewData.ShippingAddress = shipping.PrepareAddressModel();

                //contact number option
                var contactNumber = _accountService.GetContactNumberIfOptedToDisplay(accountId);
                model.OrderReviewData.DisplayContactNumberInDespatch = !string.IsNullOrEmpty(contactNumber);
                model.OrderReviewData.ContactNumber = contactNumber;

            }

            #endregion

            return model;
        }

        [NonAction]
        protected MiniShoppingCartModel PrepareMiniShoppingCartModel()
        {
            int userId = _workContext.CurrentProfile.Id;
            var items = _cartService.GetCartItemOverviewModelByProfileId(userId);

            var model = new MiniShoppingCartModel
            {
                Items = items.ApplyMaximumQuantityRule().PrepareCartItemModels(),
                SubTotal = GetSubtotalString(items),
                Message = "Enjoy FREE UK standard shipping on all orders plus free sample on every order* !<br/>*T&amp;C's apply.",
                CurrencyCode = _workContext.WorkingCurrency.HtmlEntity
            };

            return model;
        }

        [NonAction]
        protected virtual OrderTotalsModel PrepareOrderTotalsModel(bool isEditable)
        {
            var model = new OrderTotalsModel();
            model.IsEditable = isEditable;
            
            var orderTotals = _cartService.CalculateOrderTotals(_workContext.CurrentProfile.Id);
            
            if (orderTotals != null && orderTotals.ItemCount > 0)
            {
                //subtotal
                model.SubTotal = _priceFormatter.FormatPrice(orderTotals.Subtotal);

                //discount
                if (orderTotals.Discount > 0M)
                {
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(orderTotals.Discount * -1);

                    foreach (var discount in orderTotals.Discounts)
                    {
                        model.CartOffers.Add(discount.Key);
                    }                    
                }
                
                //allocated points
                if (orderTotals.AllocatedPoints > 0)
                {
                    model.AllocatedPoints = orderTotals.AllocatedPoints;
                    model.AllocatedPointsAmount = _priceFormatter.FormatPrice(orderTotals.AllocatedPoints / 100M * -1);
                }

                //shipping info
                if (orderTotals.ShippingCost > 0M)
                    model.Shipping = _priceFormatter.FormatPrice(orderTotals.ShippingCost);
                else
                    model.Shipping = "FREE";
                model.SelectedShippingMethod = orderTotals.ShippingMethod;

                //tax
                if (orderTotals.DisplayTax)
                {
                    model.DisplayTax = true;
                    model.Tax = _priceFormatter.FormatTaxRate(orderTotals.Tax);
                }

                model.OrderTotal = _priceFormatter.FormatPrice(orderTotals.Total);
            }

            return model;
        }

        [NonAction]
        protected virtual JsonResult PrepareAjaxBasketResult(int profileId, string offerMessage = null, bool hideQuickAddToCartSection = false)
        {
            var model = PrepareMiniShoppingCartModel();

            int totalQuantity = _cartService.GetTotalQuantityCartItemByProfileId(profileId);
            var items = _cartService.GetCartItemOverviewModelByProfileId(profileId);
            var subtotal = items.Select(i => i.Quantity * i.OfferPriceExclTax).Sum();
            var itemQuantity = items.Select(x => x.Quantity).Sum();

            var UpdateTopCartQuantitySectionHtml = string.Format("({0})", totalQuantity);
            var UpdateTopCartSubtotalSectionHtml = GetSubtotalString(items);
            var UpdateFlyoutCartSectionHtml = RenderPartialViewToString("FlyoutShoppingCart", model);
            var UpdateMiniBasketNotifySectionHtml = itemQuantity > 0 ? string.Format("<span class=\"badge badge-notify\">{0}</span>", itemQuantity) : string.Empty;

            return Json(new
            {
                success = true,
                message = string.Format("Item has been added to <a href=\"{0}\">shopping cart</a>.", Url.RouteUrl("Shopping Cart")),
                offerMessage = offerMessage,
                updateTopCartQuantitySectionHtml = UpdateTopCartQuantitySectionHtml,
                updateTopCartSubtotalSectionHtml = UpdateTopCartSubtotalSectionHtml,
                updateFlyoutCartSectionHtml = UpdateFlyoutCartSectionHtml,
                updateMiniBasketNotifySectionHtml = UpdateMiniBasketNotifySectionHtml,
                hideQuickAddToCartSection = hideQuickAddToCartSection
            });
        }
        
        #endregion
    }
}