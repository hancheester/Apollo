using Apollo.Core;
using Apollo.Core.Caching;
using Apollo.Core.Domain;
using Apollo.Core.Domain.Email;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Infrastructure;
using Apollo.FrontStore.Models.Common;
using Apollo.FrontStore.Models.Media;
using Apollo.FrontStore.Models.ShoppingCart;
using Apollo.Web.Framework.Security;
using Apollo.Web.Framework.Services.Catalog;
using Apollo.Web.Framework.Services.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class CommonController : BasePublicController
    {
        private readonly IUtilityService _utilityService;
        private readonly ICampaignService _campaignService;
        private readonly ICartService _cartService;
        private readonly IOfferService _offerService;
        private readonly IAccountService _accountService;
        private readonly IShippingService _shippingService;
        private readonly IProductService _productService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        private readonly HttpContextBase _httpContext;
        private readonly EmailSettings _emailSettings;
        private readonly StoreInformationSettings _storeInformationSettings;

        public CommonController(
            IUtilityService utilityService,
            ICampaignService campaignService,
            ICartService cartService,
            IOfferService offerService,
            IAccountService accountService,
            IShippingService shippingService,
            IProductService productService,
            IPriceFormatter priceFormatter,
            IWorkContext workContext,
            ICacheManager cacheManager,
            HttpContextBase httpContext,
            EmailSettings emailSettings,
            StoreInformationSettings storeInformationSettings)
        {
            _utilityService = utilityService;
            _campaignService = campaignService;
            _cartService = cartService;
            _offerService = offerService;
            _accountService = accountService;
            _shippingService = shippingService;
            _productService = productService;
            _priceFormatter = priceFormatter;
            _workContext = workContext;
            _cacheManager = cacheManager;
            _httpContext = httpContext;
            _emailSettings = emailSettings;
            _storeInformationSettings = storeInformationSettings;
        }

        //bad request
        [ApolloHttpsRequirement(SslRequirement.NoMatter)]
        public ActionResult BadRequest()
        {
            Response.StatusCode = 400;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }

        //unauthorized
        [ApolloHttpsRequirement(SslRequirement.NoMatter)]
        public ActionResult Unauthorized()
        {
            Response.StatusCode = 401;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }

        //page not found
        [ApolloHttpsRequirement(SslRequirement.NoMatter)]
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }

        //gone
        [ApolloHttpsRequirement(SslRequirement.NoMatter)]
        public ActionResult Gone()
        {
            Response.StatusCode = 410;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(ContactUsModel model)
        {
            if (ModelState.IsValid)
            {
                var contactUsEmail = _emailSettings.ContactUsEmail;
                _utilityService.ProcessContactUs(contactUsEmail, model.Email, model.Name, model.Message);

                return RedirectToRoute("Contact Us Result", new { resultId = (int)ContactUsResults.Sent });
            }
            else
            {
                return RedirectToRoute("Contact Us Result", new { resultId = (int)ContactUsResults.NotSent });
            }
        }

        public ActionResult ContactUsResult(int resultId)
        {
            var resultText = string.Empty;
            switch ((ContactUsResults)resultId)
            {
                case ContactUsResults.NotSent:
                    resultText = "Sorry, there was a problem with your submission. Please try again later or contact us via <a href='mailto:customerservices@Apollo.co.uk'>customerservices@Apollo.co.uk</a>.";
                    break;
                default:
                case ContactUsResults.Sent:
                    resultText = "Thanks. Your message has been successfully submitted and we appreciate you contacting us. We'll be in touch soon.";
                    break;
            }

            var model = new ContactUsResultModel
            {
                Result = resultText
            };

            return View(model);
        }

        public ActionResult AboutUs()
        {
            return View();
        }
        
        public ActionResult DeliveryInformation()
        {
            return View();
        }
        
        public ActionResult InternationalDeliveryCountry()
        {
            var options = _shippingService.GetActiveTrackedDeliveryShippingOptions();
            var models = PrepareTrackedDeliveryModels(options);
            return View(models);
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        #region Affiliates

        public ActionResult Affiliates()
        {
            return View();
        }

        public ActionResult AffiliateAgreement()
        {
            return View();
        }

        #endregion

        public ActionResult SpecialOffers(int offertype = 0)
        {
            var items = _offerService.GetActiveOfferTypes();
            var result = _offerService.GetOfferRuleLoadPaged(
                isActive: true, 
                offerTypeId: offertype != 0 ? offertype : default(int?), 
                orderBy: OfferRuleSortingType.PriorityAsc);
            var banners = _campaignService.GetActiveLargeBanners(BannerDisplayType.OffersPage);
            
            var model = new SpecialOfferModel
            {
                OfferTypes = items,
                Offers = result.Items.FilterOffers().PrepareSingleOfferModel(),
                Banners = banners.PrepareLargeBannerModels(),
                SelectedOfferTypeId = offertype,
                SelectedOfferType = items.Where(x => x.Id == offertype).Select(x => x.Name).FirstOrDefault()
            };

            return View(model);
        }

        public ActionResult IndividualOffer(string urlkey, int offerType = 0)
        {
            if (offerType > 0)
                return RedirectToAction("SpecialOffers", new { offertype = offerType });

            var offer = _offerService.GetOfferRuleByUrlKey(urlkey);

            if (offer == null)
                return RedirectToRoute("Special Offers");
            
            var model = new IndividualOfferModel
            {
                OfferTypes = _offerService.GetActiveOfferTypes(),
                Id = offer.Id,
                Alias = offer.Alias,
                Description = offer.LongDescription,
                Image = new PictureModel
                {
                    ImageUrl = offer.LargeImage,
                    Title = offer.Alias,
                    AlternateText = offer.Alias
                },
                OfferTypeId = offer.OfferTypeId,
                UrlKey = offer.UrlRewrite,
                OfferUrl = offer.OfferUrl,
                Products = offer.RelatedItems.PrepareProductBoxModels()
            };

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult HeaderStripOffer(string elementId, bool isMobile)
        {
            if (_workContext.CurrentProfile.GetAttribute<bool>("Profile", SystemCustomerAttributeNames.HideHeaderStripOffer) == false)
            {
                var items = _cacheManager.Get(CacheKey.OFFER_RULE_ACTIVE_ORDER_BY_PRIORITY_KEY, delegate ()
                {
                    var result = _offerService.GetOfferRuleLoadPaged(
                    isActive: true,
                    orderBy: OfferRuleSortingType.PriorityAsc);

                    return result.Items;
                });

                var offers = items.FilterOffers(displayOnHeaderStrip: true);

                if (offers.Count > 0)
                {
                    var offer = offers.First();
                    var model = new HeaderStripOfferModel
                    {
                        Alias = offer.Alias,
                        Description = offer.ShortDescription,
                        UrlKey = offer.UrlRewrite,
                        ElementId = elementId,
                        IsMobile = isMobile
                    };

                    return PartialView("_HeaderStripOffer", model);
                }
            }

            return Content("");
        }
        
        public ActionResult LoyaltyScheme()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubscribeBox(SubscriptionModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _accountService.ProcessNewEmailSubscription(model.Email);
                return RedirectToRoute("Subscription Result", new { resultId = (int)result });
            }
            else
            {
                return RedirectToRoute("Subscription Result", new { resultId = (int)SubscriptionResults.InvalidEmail });
            }
        }

        public ActionResult SubscribeResult(int resultId)
        {
            var resultText = string.Empty;
            switch ((SubscriptionResults)resultId)
            {
                case SubscriptionResults.Successful:
                    resultText = "You've successfully joined our email list. Stay tuned for latest news, offers and ideas.";
                    break;
                case SubscriptionResults.InvalidEmail:
                    resultText = "Sorry, please try again with a valid email address.";
                    break;
                default:
                case SubscriptionResults.Error:
                    resultText = "Sorry, there is an error in the system. Please let us know about it by sending an email.";
                    break;
            }

            var model = new SubscriptionResultModel
            {
                Result = resultText
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult HelpAlternativeProduct(AlternativeHelpModel model)
        {
            if (ModelState.IsValid)
            {
                var contactUsEmail = _emailSettings.ContactUsEmail;                
                _utilityService.ProcessHelpAlternativeProduct(contactUsEmail, model.Email, model.ProductId);

                return RedirectToRoute("Help Alternative Result", new { resultId = (int)HelpAlternativeResults.Sent });
            }
            else
            {
                return RedirectToRoute("Help Alternative Result", new { resultId = (int)HelpAlternativeResults.NotSent });
            }
        }
        
        public ActionResult HelpAlternativeProductResult(int resultId)
        {
            var resultText = string.Empty;
            switch ((HelpAlternativeResults)resultId)
            {
                case HelpAlternativeResults.NotSent:
                    resultText = "Sorry, there was a problem with your submission. Please try again later or contact us via <a href='mailto:customerservices@Apollo.co.uk'>customerservices@Apollo.co.uk</a>.";
                    break;
                default:
                case HelpAlternativeResults.Sent:
                    resultText = "Thanks. Your email has been successfully submitted and we appreciate you contacting us. We'll be in touch soon.";
                    break;
            }

            var model = new HelpAlternativeProductResultModel
            {
                Result = resultText
            };

            return View(model);
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult HelpStockNotification(StockNotifierModel model)
        {
            if (ModelState.IsValid)
            {
                _utilityService.ProcessHelpStockNofification(model.Email, model.ProductId, model.ProductPriceId);

                return HelpStockNotificationResult(HelpStockNoficationResults.Sent);                
            }
            else
            {
                return HelpStockNotificationResult(HelpStockNoficationResults.NotSent);            
            }
        }

        [AjaxOnly]
        public ActionResult QuickStockNofification(int productid)
        {
            var product = _productService.GetProductOverviewModelById(productid);

            if (product == null)
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            var model = new QuickStockNotifyModel
            {
                Product = product
            };

            return Json(new
            {
                success = true,
                resultSectionHtml = RenderPartialViewToString("QuickStockNotify", model)
            });
        }

        [ChildActionOnly]
        [OutputCache(Duration = 43200, VaryByParam = "none")]
        public ActionResult MainBanners()
        {
            var banners = _campaignService.GetActiveLargeBanners(BannerDisplayType.HomePage);
            var models = banners.PrepareLargeBannerModels();
            return PartialView("_Banners", models);
        }

        [ChildActionOnly]
        public ActionResult OfferBanners()
        {
            var banners = _campaignService.GetActiveOfferBanners();
            var models = PrepareOfferBannerModelList(banners);
            return PartialView("_Banners", models);
        }

        [ChildActionOnly]        
        public ActionResult CurrencySelector()
        {
            var model = PrepareCurrencySelectorModel();
            if (model.AvailableCurrencies.Count == 1) Content(string.Empty);

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult CountrySelector()
        {
            var model = PrepareCountryPreferenceModel();
            if (model.AvailableCountries.Count == 0) Content(string.Empty);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ChangeCountryPreference(CountryPreferenceModel model, string returnUrl = "")
        {
            var country = _shippingService.GetCountryById(model.CountryId);
            if (country != null) _workContext.CurrentCountry = country;

            var profileId = _workContext.CurrentProfile.Id;
            var options = _cartService.GetCustomerShippingOptionByCountryAndPriority(profileId);

            // Select default option
            _utilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.SelectedShippingOption, options[0].Id.ToString());

            // Home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Home");

            // Prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Home");

            return Redirect(returnUrl);
        }

        [ChildActionOnly]        
        public ActionResult CurrencySymbolSelector()
        {
            var model = PrepareCurrencySelectorModel();
            if (model.AvailableCurrencies.Count == 1) Content(string.Empty);

            return PartialView(model);
        }

        public ActionResult SetCurrency(int customercurrency, string returnUrl = "")
        {
            var currency = _utilityService.GetCurrency(customercurrency);
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            _httpContext.Response.RemoveOutputCacheItem(Url.Action("DeliveryInformation", "Common"));
            _httpContext.Response.RemoveOutputCacheItem(Url.Action("InternationalDeliveryCountry", "Common"));

            // Home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Home");

            // Prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Home");
            
            return Redirect(returnUrl);
        }

        [ChildActionOnly]
        public ActionResult LocationPreference()
        {
            var model = PrepareLocationPreferenceModel();

            return PartialView("_LocationPreference", model);
        }

        [HttpPost]
        public ActionResult ChangeLocationPreference(LocationPreferenceModel model, string returnUrl = "")
        {
            var currency = _utilityService.GetCurrencyByCurrencyCode(model.CurrencyCode);
            if (currency != null) _workContext.WorkingCurrency = currency;

            var country = _shippingService.GetCountryById(model.CountryId);
            if (country != null) _workContext.CurrentCountry = country;

            var profileId = _workContext.CurrentProfile.Id;
            var options = _cartService.GetCustomerShippingOptionByCountryAndPriority(profileId);

            // Select default option
            _utilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.SelectedShippingOption, options[0].Id.ToString());

            // Home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("Home");

            // Prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Home");

            return Redirect(returnUrl);
        }

        [ChildActionOnly]
        public ActionResult EuCookieLaw()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                return Content("");

            //ignore search engines because some pages could be indexed with the EU cookie as description
            if (_workContext.CurrentProfile.IsSystemProfile)
                return Content("");
            
            if (_workContext.CurrentProfile.GetAttribute<bool>("Profile", SystemCustomerAttributeNames.EuCookieLawAccepted))
                return Content("");

            return PartialView();
        }

        [HttpPost]
        public ActionResult EuCookieLawAccept()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Json(new { stored = false });

            //save setting
            _utilityService.SaveAttribute(
                _workContext.CurrentProfile.Id,
                "Profile",
                SystemCustomerAttributeNames.EuCookieLawAccepted,
                true.ToString());

            return Json(new { stored = true });
        }

        [HttpPost]
        [AjaxOnly]
        public ActionResult HeaderStripOfferHide()
        {
            //save setting
            _utilityService.SaveAttribute(
                _workContext.CurrentProfile.Id,
                "Profile",
                SystemCustomerAttributeNames.HideHeaderStripOffer,
                true.ToString());

            return Json(new { hidden = true });
        }

        public ActionResult Careers()
        {
            return View();
        }

        #region Utilities

        [NonAction]
        protected CurrencySelectorModel PrepareCurrencySelectorModel()
        {
            var list = _cacheManager.Get(CacheKey.CURRENCY_MODEL_LIST_KEY, delegate () {
                var result = _utilityService.GetAllCurrency();

                return result.Select(c => new CurrencyModel
                {
                    Id = c.Id,
                    Name = c.CurrencyCode,
                    CurrencyCode = c.CurrencyCode,
                    CurrencySymbol = c.Symbol
                })
                .ToList();
            });

            var model = new CurrencySelectorModel
            {
                CurrentCurrencyId = _workContext.WorkingCurrency.Id,
                CurrencyCurrencyCode = _workContext.WorkingCurrency.CurrencyCode,
                AvailableCurrencies = list
            };

            return model;            
        }

        [NonAction]
        protected LocationPreferenceModel PrepareLocationPreferenceModel()
        {
            var selectedCountry = _workContext.CurrentCountry;
            var selectedCurrency = _workContext.WorkingCurrency;
            var allCurrency = _utilityService.GetAllCurrency();
            var countries = _shippingService.GetActiveCountries();
            var options = _shippingService.GetShippingOptionByCountryAndEnabled(selectedCountry.Id, true);
            var freeDeliveryOption = options.Where(x => x.FreeThreshold > 0).FirstOrDefault();
            var freeDeliveryNote = ", FREE Delivery*";

            if (selectedCountry.ISO3166Code != "GB")
            {
                freeDeliveryNote = freeDeliveryOption == null ? 
                    string.Empty : 
                    string.Format(", FREE {0} over {1}", freeDeliveryOption.Name, _priceFormatter.FormatPrice(freeDeliveryOption.FreeThreshold));
            }

            var model = new LocationPreferenceModel
            {
                SelectedCountryName = selectedCountry.Name,
                SelectedCountryCode = selectedCountry.ISO3166Code,                
                FreeDeliveryNote = freeDeliveryNote
            };

            model.Note = string.Format("Delivery options are set based on shipping to {0}, but can be amended below.", selectedCountry.Name);
            model.AvailableCountries = countries
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = x.Id == selectedCountry.Id
                })
                .ToList();
            model.AvailableLocaleCurrencies = allCurrency
                .Select(x => new SelectListItem
                {
                    Text = x.Symbol + " " + x.CurrencyCode,
                    Value = x.CurrencyCode,
                    Selected = x.CurrencyCode == selectedCurrency.CurrencyCode
                })
                .ToList();

            return model;
        }

        [NonAction]
        protected CountryPreferenceModel PrepareCountryPreferenceModel()
        {
            var selectedCountry = _workContext.CurrentCountry;            
            var countries = _shippingService.GetActiveCountries();
            var options = _shippingService.GetShippingOptionByCountryAndEnabled(selectedCountry.Id, true);
            var freeDeliveryOption = options.Where(x => x.FreeThreshold > 0).FirstOrDefault();
            var freeDeliveryNote = ", FREE Delivery*";

            if (selectedCountry.ISO3166Code != "GB")
            {
                freeDeliveryNote = freeDeliveryOption == null ?
                    string.Empty :
                    string.Format(", FREE {0} over {1}", freeDeliveryOption.Name, _priceFormatter.FormatPrice(freeDeliveryOption.FreeThreshold));
            }

            var model = new CountryPreferenceModel
            {
                SelectedCountryName = selectedCountry.Name,
                SelectedCountryCode = selectedCountry.ISO3166Code,                
                FreeDeliveryNote = freeDeliveryNote
            };

            model.Note = string.Format("Delivery options are set based on shipping to <b>{0}</b>, but can be amended below.", selectedCountry.Name);
            model.AvailableCountries = countries
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = x.Id == selectedCountry.Id
                })
                .ToList();
            
            return model;
        }

        [NonAction]
        protected IList<BannerModel> PrepareOfferBannerModelList(IList<OfferBanner> banners)
        {
            var models = new List<BannerModel>();

            if (banners != null)
            {
                models = banners.Select(x => new BannerModel
                {
                    Link = x.Link,
                    Picture = new PictureModel
                    {
                        ImageUrl = string.Format("/media/banner/offer/{0}", x.MediaFilename), // TODO: Move to settings
                        Title = x.Title,
                        AlternateText = x.MediaAlt
                    }
                }).ToList();
            }
            
            return models;
        }

        [NonAction]
        protected IList<TrackedDeliveryModel> PrepareTrackedDeliveryModels(IList<ShippingOptionOverviewModel> options)
        {
            var models = new List<TrackedDeliveryModel>();

            for(int i = 0; i < options.Count; i++)
            {
                var country = _shippingService.GetCountryById(options[i].CountryId);

                if (country != null)
                {
                    var description = string.Empty;
                    if (options[i].FreeThreshold > 0)
                    {
                        description = string.Format("FREE when you spend over {0}", _priceFormatter.FormatPrice(options[i].FreeThreshold));
                    }

                    models.Add(new TrackedDeliveryModel
                    {
                        CountryName = country.Name,
                        Name = options[i].Description,
                        Cost = _priceFormatter.FormatPrice(options[i].Cost),
                        Description = description,
                        Timeline = options[i].Timeline
                    });
                }
            }

            return models;
        }

        [NonAction]
        protected JsonResult HelpStockNotificationResult(HelpStockNoficationResults result)
        {
            var resultText = string.Empty;
            var title = string.Empty;

            switch (result)
            {
                case HelpStockNoficationResults.NotSent:
                    title = "Apologies";
                    resultText = "Sorry, there was a problem with your submission. Please refresh the page and try again later or contact us via <a href='mailto:customerservices@Apollo.co.uk'>customerservices@Apollo.co.uk</a>.";
                    break;
                default:
                case HelpStockNoficationResults.Sent:
                    title = "We'll be in touch";
                    resultText = "Thanks. Your email has been successfully submitted and we'll contact you again when the product is available. Make sure you keep an eye out as once it's back as it is likely to sell out fast.";
                    break;
            }

            var model = new HelpStockNotifierResultModel
            {
                Title = title,
                Result = resultText
            };

            return Json(new
            {
                helpStockNoficationResultSectionHtml = RenderPartialViewToString("HelpStockNotificationResult", model)
            });
        }

        #endregion
    }
}