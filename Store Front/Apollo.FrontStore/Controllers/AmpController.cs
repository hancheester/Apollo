using Apollo.Core;
using Apollo.Core.Domain;
using Apollo.Core.Domain.Catalog;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Infrastructure;
using Apollo.FrontStore.Models;
using Apollo.FrontStore.Models.Brand;
using Apollo.FrontStore.Models.Category;
using Apollo.FrontStore.Models.Common;
using Apollo.FrontStore.Models.Media;
using Apollo.FrontStore.Models.Product;
using Apollo.Web.Framework.ActionFilters;
using Apollo.Web.Framework.Security;
using Apollo.Web.Framework.Services.Catalog;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
    [UseAmpImage]
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class AmpController : BasePublicController
    {
        #region Fields

        private static ILog _logger = LogManager.GetLogger(typeof(ShoppingCartController).FullName);

        private readonly IWorkContext _workContext;
        private readonly ICampaignService _campaignService;
        private readonly IAccountService _accountService;
        private readonly ICartService _cartService;
        private readonly IShippingService _shippingService;
        private readonly IUtilityService _utilityService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly IOfferService _offerService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly MediaSettings _mediaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        public AmpController(
            IWorkContext workContext,
            IAccountService accountService,
            ICampaignService campaignService,
            ICartService cartService,
            IUtilityService utilityService,
            IShippingService shippingService,            
            ICategoryService categoryService,
            IProductService productService,
            IBrandService brandService,
            IOfferService offerService,
            IPriceFormatter priceFormatter,
            MediaSettings mediaSettings,
            StoreInformationSettings storeInformationSettings,
            CatalogSettings catalogSettings)
        {
            _accountService = accountService;
            _campaignService = campaignService;
            _cartService = cartService;
            _utilityService = utilityService;
            _shippingService = shippingService;
            _workContext = workContext;
            _categoryService = categoryService;
            _productService = productService;
            _brandService = brandService;
            _offerService = offerService;
            _priceFormatter = priceFormatter;
            _mediaSettings = mediaSettings;           
            _storeInformationSettings = storeInformationSettings;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Home

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Common

        [HttpPost]
        [AmpForm]
        public ActionResult SubscribeBox(SubscriptionModel model)
        {
            if (ModelState.IsValid == false)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    message = "Sorry, please try again with a valid email address."
                });
            }

            var result = _accountService.ProcessNewEmailSubscription(model.Email);
            var resultMessage = string.Empty;

            switch (result)
            {
                case SubscriptionResults.Successful:
                    resultMessage = "You've successfully joined our email list. Stay tuned for latest news, offers and ideas.";
                    break;
                case SubscriptionResults.InvalidEmail:
                    Response.StatusCode = 400;
                    resultMessage = "Sorry, please try again with a valid email address.";
                    break;
                case SubscriptionResults.Error:
                default:
                    Response.StatusCode = 500;
                    resultMessage = "Sorry, there is an error in the system. Please let us know about it by sending an email.";
                    break;
            }

            return Json(new
            {
                message = resultMessage
            });
        }

        [HttpPost]
        [AmpForm]
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
            
            string baseUrl = "https://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            string redirectTo = baseUrl + returnUrl;
            HttpContext.Response.AddHeader("AMP-Redirect-To", redirectTo);

            return Json(new
            {
                url = redirectTo
            });
        }

        [ChildActionOnly]
        public ActionResult LocationPreference()
        {
            var model = PrepareLocationPreferenceModel();

            return PartialView("_LocationPreference", model);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 43200, VaryByParam = "none")]
        public ActionResult MainBanners()
        {
            var banners = _campaignService.GetActiveLargeBanners(BannerDisplayType.HomePage);
            var models = PrepareLargeBannerModelList(banners);
            return PartialView("_Banners", models);
        }

        #endregion

        #region Category

        [HttpGet]
        public ActionResult Category(string top)
        {
            var topCategory = _categoryService.GetActiveCategoryOverviewModel(top);

            if (topCategory == null) return InvokeHttp404();

            var template = _categoryService.GetCategoryTemplateById(topCategory.CategoryTemplateId);
            if (template == null) template = _categoryService.GetAllCategoryTemplates().FirstOrDefault();
            if (template == null) throw new ApolloException("No default template could be loaded");

            var model = new CategoryModel
            {
                Category = topCategory,
                Banners = topCategory.Banners.PrepareLargeBannerModels(),
                CategoryTemplateViewPath = template.ViewPath,
                FeatureItemTypes = topCategory.FeatureItemTypes
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult CategoryWithProducts(string top, string second, string third, ProductPagingFilteringModel command)
        {
            var topCategory = _categoryService.GetActiveCategoryOverviewModel(top);
            if (topCategory == null) return InvokeHttp404();
            var secondCategory = _categoryService.GetActiveCategoryOverviewModel(top, second);
            if (secondCategory == null) return InvokeHttp404();
            var categoryIds = new List<int>(secondCategory.Children.Select(x => x.Id).ToArray());

            CategoryOverviewModel thirdCategory = null;
            if (!string.IsNullOrEmpty(third))
            {
                thirdCategory = _categoryService.GetActiveCategoryOverviewModel(top, second, third);
                categoryIds.Clear();
                categoryIds.Add(thirdCategory.Id);
            }

            #region Price filter
            decimal price;
            decimal? priceMin = null;
            if (!string.IsNullOrEmpty(command.From) && decimal.TryParse(command.From, out price))
            {
                priceMin = price;
            }
            decimal? priceMax = null;
            if (!string.IsNullOrEmpty(command.To) && decimal.TryParse(command.To, out price))
            {
                priceMax = price;
            }
            #endregion

            #region Brand filter
            var brandList = new List<int>();
            if (!string.IsNullOrEmpty(command.Brands))
            {
                var brandStringArray = command.Brands.Split(',');
                if (brandStringArray.Length > 0)
                {
                    int[] brandArray = Array.ConvertAll(brandStringArray, int.Parse);
                    brandList = new List<int>(brandArray);
                }
            }
            #endregion

            #region Type filter
            var filterList = new List<int>();
            if (!string.IsNullOrEmpty(command.Filters))
            {
                var filterStringArray = command.Filters.Split(',');
                if (filterStringArray.Length > 0)
                {
                    int[] filterArray = Array.ConvertAll(filterStringArray, int.Parse);
                    filterList = new List<int>(filterArray);
                }
            }
            #endregion

            var result = _productService.GetPagedProductOverviewModel(
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize,
                categoryIds: categoryIds,
                categoryFilterIds: filterList.Count > 0 ? filterList : null,
                brandIds: brandList.Count > 0 ? brandList : null,
                enabled: true,
                includeDiscontinuedButInStock: true,
                priceMin: priceMin,
                priceMax: priceMax,
                orderBy: command.OrderBy);

            var model = new CategoryModel
            {
                SelectedSecondCategoryName = secondCategory.Name,
                SelectedSecondCategoryUrl = secondCategory.UrlKey,
                SelectedThirdCategoryName = thirdCategory != null ? thirdCategory.Name : null,
                SelectedThirdCategoryUrl = thirdCategory != null ? thirdCategory.UrlKey : null,
                SelectedCategoryDescription = thirdCategory != null ? thirdCategory.Description : secondCategory.Description,
                Category = topCategory,
                Products = result.Items.PrepareProductBoxModels()
            };

            model.PagingFilteringContext.LoadPagedList(result);

            #region Price range
            var priceRange = _productService.GetPriceRangeByCategory(categoryIds);
            model.PagingFilteringContext.PriceRangeFilter = new PriceRangeFilterModel
            {
                Min = Math.Round(priceRange[0]).ToString(),
                Max = Math.Ceiling(priceRange[1]).ToString(),
                From = (command.From == string.Empty) ? Math.Round(priceRange[0]).ToString() : command.From,
                To = (command.To == string.Empty) ? Math.Ceiling(priceRange[1]).ToString() : command.To
            };
            #endregion

            #region Brand range
            var brandRange = _brandService.GetBrandRangeByCategory(categoryIds);
            model.PagingFilteringContext.BrandRangeFilter.Brands = brandRange;
            model.PagingFilteringContext.BrandRangeFilter.SelectedBrands = brandList.ToArray();
            #endregion

            #region Type range
            var categoryFilterRange = _categoryService.GetCategoryFiltersRangeByCategory(categoryIds);
            model.PagingFilteringContext.CategoryFilterRangeFilter.Filters = categoryFilterRange;
            model.PagingFilteringContext.CategoryFilterRangeFilter.SelectedFilters = filterList.ToArray();
            #endregion

            model.PagingFilteringContext.Brands = command.Brands;
            model.PagingFilteringContext.SelectedPageSize = command.PageSize;
            model.PagingFilteringContext.OrderBy = command.OrderBy;
            model.PagingFilteringContext.PrepareSortingOptions();
            model.PagingFilteringContext.ViewMode = command.ViewMode;
            model.PagingFilteringContext.PrepareViewModeOptions();

            return View(model);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 43200, VaryByParam = "isForSearchResult")]
        public ActionResult BoxMenu(bool isForSearchResult = false)
        {
            var items = _categoryService.GetCategoryOverviewModelForMenu();
            var model = new MenuModel
            {
                IsForSearchResult = isForSearchResult,
                SimpleCategories = PrepareCategorySimpleModels(items, true)
            };

            return PartialView("_BoxMenu", model);
        }

        #endregion

        #region Brand

        [HttpGet]
        public ActionResult Brand(string urlKey, ProductPagingFilteringModel command)
        {
            if (string.IsNullOrEmpty(urlKey))
                return RedirectToAction("ShopByBrand", "Amp");

            var brand = _brandService.GetBrandByUrlKey(urlKey);

            if (brand == null)
                return InvokeHttp404();

            BrandModel model;

            if (brand.HasMicrosite == true)
            {
                model = PrepareBrandModel(brand, includeProductsIfNoCategory: true);
            }
            else
            {
                model = PrepareBrandModelWithProducts(brand, command);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult BrandWithProducts(string urlKey, string top, string second, string third, ProductPagingFilteringModel command)
        {
            var brand = _brandService.GetBrandByUrlKey(urlKey);

            if (brand == null)
                return InvokeHttp404();

            var model = PrepareBrandModelWithProducts(brand, command, top, second, third);

            return View(model);
        }

        [HttpGet]
        public ActionResult ShopByBrand()
        {
            return View();
        }

        #endregion

        #region Product

        [HttpGet]
        public ActionResult ProductDetails(string urlkey)
        {
            var product = _productService.GetProductOverviewModelByUrlRewrite(urlkey);

            //TODO: I think we should redirect to a page that recommends other related products instead.
            // If the product is offline, then return 404
            if (product == null || product.Enabled == false || product.VisibleIndividually == false)
                return InvokeHttp404();

            // Prepare the model
            var model = PrepareProductDetailsModel(product);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult HomepageProducts(int groupId)
        {
            var items = _productService.GetHomepageProductByGroupId(groupId);
            var models = items.PrepareProductBoxModels(styleClass: "wide");

            return PartialView("_ProductGroup", models);
        }

        [ChildActionOnly]
        public ActionResult CategoryFeaturedProducts(int categoryId, int featuredItemTypeId)
        {
            var items = _productService.GetActiveProductOverviewModelByCategoryFeaturedItemType(categoryId, featuredItemTypeId);
            var models = items.PrepareProductBoxModels(styleClass: "wide");

            return PartialView("_ProductGroup", models);
        }

        [ChildActionOnly]
        public ActionResult BrandFeaturedProducts(int brandId, int featuredItemTypeId)
        {
            var items = _productService.GetActiveProductOverviewModelByBrandFeaturedItemType(brandId, featuredItemTypeId);
            var models = items.PrepareProductBoxModels(styleClass: "wide");

            return PartialView("_ProductGroup", models);
        }

        #endregion

        #region Shopping

        [HttpPost]
        [AmpForm]
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
                    string returnUrl = Url.RouteUrl("Shopping Cart");
                    string baseUrl = "https://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                    string redirectTo = baseUrl + returnUrl;
                    HttpContext.Response.AddHeader("AMP-Redirect-To", redirectTo);

                    return Json(new
                    {
                        success = true
                    });
                }
                else
                {
                    HttpContext.Response.StatusCode = 400;

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
                HttpContext.Response.StatusCode = 500;

                return Json(new
                {
                    success = false,
                    message = "Sorry, an error occurred while creating/updating item."
                });
            }
        }

        #endregion

        #region Utilities

        [NonAction]
        protected IList<CategorySimpleModel> PrepareCategorySimpleModels(IList<CategoryOverviewModel> items, bool forBoxMenu = false)
        {
            var models = new List<CategorySimpleModel>();
            var categoryMediaPath = _mediaSettings.CategoryMediaPath;
            foreach (var item in items)
            {
                var model = new CategorySimpleModel
                {
                    Name = item.Name,
                    //Url = Url.RouteUrl("Category", new { top = item.UrlKey }),
                    Url = string.Format("{0}category/{1}", _storeInformationSettings.StoreFrontLink, item.UrlKey),
                    Picture = new PictureModel
                    {
                        ImageUrl = string.Format("{0}{1}", categoryMediaPath, item.ThumbnailFilename),
                        AlternateText = item.Name,
                        Title = item.Name
                    }
                };

                models.Add(model);
            }

            if (forBoxMenu)
            {
                models.Add(new CategorySimpleModel
                {
                    Name = "Offers",
                    //Url = Url.RouteUrl("Special Offers"),
                    Url = string.Format("{0}special-offers", _storeInformationSettings.StoreFrontLink),
                    Picture = new PictureModel
                    {
                        ImageUrl = string.Format("{0}{1}", categoryMediaPath, "box-menu-offer.png"),
                        AlternateText = "Offers",
                        Title = "Offers"
                    }
                });

                models.Add(new CategorySimpleModel
                {
                    Name = "i-Zone",
                    //Url = Url.RouteUrl("i-Zone"),
                    Url = string.Format("{0}blog", _storeInformationSettings.StoreFrontLink),
                    Picture = new PictureModel
                    {
                        ImageUrl = string.Format("{0}{1}", categoryMediaPath, "box-menu-izone.png"),
                        AlternateText = "i-Zone",
                        Title = "i-Zone"
                    }
                });

                models.Add(new CategorySimpleModel
                {
                    Name = "Shop By Brands",
                    //Url = Url.RouteUrl("Shop By Brand"),
                    Url = string.Format("{0}brands", _storeInformationSettings.StoreFrontLink),
                    Picture = new PictureModel
                    {
                        ImageUrl = string.Format("{0}{1}", categoryMediaPath, "box-menu-shopbybrand.png"),
                        AlternateText = "Shop By Brands",
                        Title = "Shop By Brands"
                    }
                });
            }

            return models;
        }

        [NonAction]
        protected LocationPreferenceModel PrepareLocationPreferenceModel()
        {
            var selectedCountry = _workContext.CurrentCountry;
            var selectedCurrency = _workContext.WorkingCurrency;
            var allCurrency = _utilityService.GetAllCurrency();
            var countries = _shippingService.GetActiveCountries();

            var model = new LocationPreferenceModel
            {
                SelectedCountryName = selectedCountry.Name,
                SelectedCountryCode = selectedCountry.ISO3166Code,
                FreeDeliveryNote = selectedCountry.ISO3166Code == "GB" ? "FREE Delivery*" : string.Format("FREE Delivery over {0}*", _priceFormatter.FormatPrice(20M))
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
                    Text = x.CurrencyCode,
                    Value = x.CurrencyCode,
                    Selected = x.CurrencyCode == selectedCurrency.CurrencyCode
                })
                .ToList();

            return model;
        }

        [NonAction]
        protected IList<BannerModel> PrepareLargeBannerModelList(IList<LargeBanner> banners)
        {
            var models = new List<BannerModel>();

            if (banners != null)
            {
                models = banners.Select(x => new BannerModel
                {
                    Link = x.Link,
                    Picture = new PictureModel
                    {
                        ImageUrl = "/media/banner/large/" + x.MediaFilename,
                        Title = x.Title,
                        AlternateText = x.MediaAlt
                    }
                }).ToList();
            }

            return models;
        }

        [NonAction]
        protected BrandModel PrepareBrandModel(
            Brand brand,
            bool includeProductsIfNoCategory = false,
            string topUrlKey = "",
            string secondUrlKey = "",
            string thirdUrlKey = "")
        {
            var categories = _brandService.GetActiveBrandCategoryOverviewModelTree(brand.Id);
            IList<BannerModel> banners = new List<BannerModel>();

            if (brand.BrandMedias != null)
            {
                banners = brand.BrandMedias.Where(x => x.Enabled == true).ToList().PrepareBrandBannerModels();
            }

            var model = new BrandModel
            {
                Id = brand.Id,
                Name = brand.Name,
                UrlKey = brand.UrlRewrite,
                HasMicrosite = brand.HasMicrosite,
                Description = brand.Description,
                Categories = PrepareBrandCategoryModelList(categories),
                Banners = banners,
                FeatureItemTypes = brand.FeaturedItems == null ? new List<int>() : brand.FeaturedItems.Select(x => x.FeaturedItemType).Distinct().ToList(),
                MetaTitle = brand.MetaTitle,
                MetaDescription = brand.MetaDescription,
                MetaKeywords = brand.MetaKeywords
            };

            if (!string.IsNullOrEmpty(brand.FlashImage))
            {
                model.Logo = new PictureModel
                {
                    Title = brand.Name,
                    AlternateText = brand.Name,
                    ImageUrl = string.Format("/media/brand/{0}", brand.FlashImage),
                    FullSizeImageUrl = string.Format("/media/brand/{0}", brand.FlashImage)
                };
            }

            // If there is no categories assigned to this brand, load products instead
            if (model.Categories.Count == 0 && includeProductsIfNoCategory)
            {
                var products = _productService.GetActiveProductOverviewModelsByBrandId(brand.Id);
                if (products.Count > 0)
                    model.Products = products.PrepareProductBoxModels();
            }

            if (!string.IsNullOrEmpty(topUrlKey))
            {
                var category = SearchBrandCategory(model.Categories, topUrlKey);
                if (category != null)
                {
                    model.TopCategoryName = category.Name;
                    model.TopUrlKey = category.UrlKey;
                }
            }

            if (!string.IsNullOrEmpty(secondUrlKey))
            {
                var category = SearchBrandCategory(model.Categories, secondUrlKey);
                if (category != null)
                {
                    model.SecondCategoryName = category.Name;
                    model.SecondUrlKey = category.UrlKey;
                }
            }

            if (!string.IsNullOrEmpty(thirdUrlKey))
            {
                var category = SearchBrandCategory(model.Categories, thirdUrlKey);
                if (category != null)
                {
                    model.ThirdCategoryName = category.Name;
                    model.ThirdUrlKey = category.UrlKey;
                }
            }
            return model;
        }

        [NonAction]
        protected BrandModel PrepareBrandModelWithProducts(Brand brand, ProductPagingFilteringModel command, string top = null, string second = null, string third = null)
        {
            decimal price;
            decimal? priceMin = null;
            if (!string.IsNullOrEmpty(command.From) && decimal.TryParse(command.From, out price))
            {
                priceMin = price;
            }
            decimal? priceMax = null;
            if (!string.IsNullOrEmpty(command.To) && decimal.TryParse(command.To, out price))
            {
                priceMax = price;
            }

            var result = _productService.GetPagedProductOverviewModelsByBrandCategory(
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize,
                brandId: brand.Id,
                topUrlKey: top,
                secondUrlKey: second,
                thirdUrlKey: third,
                priceMin: priceMin,
                priceMax: priceMax,
                enabled: true,
                includeDiscontinuedButInStock: true,
                orderBy: command.OrderBy);

            var model = PrepareBrandModel(
                brand,
                topUrlKey: string.IsNullOrEmpty(top) ? string.Empty : top,
                secondUrlKey: string.IsNullOrEmpty(second) ? string.Empty : second,
                thirdUrlKey: string.IsNullOrEmpty(third) ? string.Empty : third);
            model.Products = result.Items.PrepareProductBoxModels();
            model.PagingFilteringContext.LoadPagedList(result);
            model.PagingFilteringContext.PriceRangeFilter = new PriceRangeFilterModel
            {
                Min = Math.Round(result.MinPriceFilterByKeyword).ToString(),
                Max = Math.Ceiling(result.MaxPriceFilterByKeyword).ToString(),
                From = (command.From == string.Empty) ? Math.Round(result.MinPriceFilterByKeyword).ToString() : command.From,
                To = (command.To == string.Empty) ? Math.Ceiling(result.MaxPriceFilterByKeyword).ToString() : command.To
            };
            model.PagingFilteringContext.SelectedPageSize = command.PageSize;
            model.PagingFilteringContext.OrderBy = command.OrderBy;
            model.PagingFilteringContext.PrepareSortingOptions();
            model.PagingFilteringContext.ViewMode = command.ViewMode;
            model.PagingFilteringContext.PrepareViewModeOptions();

            return model;
        }

        [NonAction]
        protected List<BrandCategoryModel> PrepareBrandCategoryModelList(IList<BrandCategoryOverviewModel> categories, int brandCategoryId = -1)
        {
            var models = new List<BrandCategoryModel>();
            var foundList = categories.Where(c => c.ParentId == brandCategoryId).ToList();

            if (foundList != null)
            {
                foreach (var item in foundList)
                {
                    var model = PrepareBrandCategoryModel(item);
                    model.Children.AddRange(PrepareBrandCategoryModelList(item.Children, item.Id));
                    models.Add(model);
                }
            }

            return models;
        }

        [NonAction]
        protected BrandCategoryModel PrepareBrandCategoryModel(BrandCategoryOverviewModel brandCategory)
        {
            return new BrandCategoryModel
            {
                Id = brandCategory.Id,
                Name = brandCategory.Name,
                UrlKey = brandCategory.UrlKey,
                ImageUrl = "/media/brand/" + brandCategory.ImageUrl,
                Description = brandCategory.Description
            };
        }

        [NonAction]
        protected BrandCategoryModel SearchBrandCategory(IList<BrandCategoryModel> list, string urlKey)
        {
            BrandCategoryModel category = null;
            bool found = false;

            foreach (var item1 in list)
            {
                foreach (var item2 in item1.Children)
                {
                    foreach (var item3 in item2.Children)
                    {
                        if (urlKey.ToLower() == item3.UrlKey.ToLower())
                        {
                            category = item3;
                            found = true;
                            break;
                        }
                    }

                    if (found) break;
                    if (urlKey.ToLower() == item2.UrlKey.ToLower())
                    {
                        category = item2;
                        found = true;
                        break;
                    }
                }
                if (found) break;
                if (urlKey.ToLower() == item1.UrlKey.ToLower())
                {
                    category = item1;
                    found = true;
                    break;
                }
            }

            return category;
        }

        [NonAction]
        protected string GetOptionByOptionType(OptionType type, ProductPrice price)
        {
            switch (type)
            {
                case OptionType.Size:
                    return price.Size;
                case OptionType.Colour:
                    if (price.ColourId.HasValue)
                    {
                        var colour = _productService.GetColour(price.ColourId.Value);
                        if (colour != null) return colour.Value;
                    }
                    return string.Empty;
                case OptionType.GiftCard:
                case OptionType.None:
                default:
                    return string.Empty;
            }
        }

        [NonAction]
        protected PictureModel GetOptionImageByOptionType(OptionType type, ProductPrice price)
        {
            switch (type)
            {
                case OptionType.Colour:
                    if (price.ColourId.HasValue)
                    {
                        var colour = _productService.GetColour(price.ColourId.Value);
                        if (colour != null)
                            return new PictureModel
                            {
                                ImageUrl = string.Format("/media/colour/{0}", colour.ColourFilename),
                                Title = colour.Value,
                                AlternateText = colour.Value


                            };
                    }
                    return new PictureModel();
                case OptionType.Size:
                case OptionType.GiftCard:
                case OptionType.None:
                default:
                    return new PictureModel();
            }
        }

        [NonAction]
        protected ProductPriceModel PrepareProductPriceModel(
            ProductPrice item,
            OptionType type,
            Currency workingCurrency,
            bool discontinued,
            bool productEnforceStockCount,
            bool brandEnforceStockCount,
            bool isPreSelected = false)
        {
            var visible = true;
            var messageAfterHidden = string.Empty;
            var stockAvailability = true;

            if (item.Stock <= 0)
            {
                // 1st priority is discontinued
                if (discontinued == true)
                {
                    visible = false;
                    messageAfterHidden = "Discontinued";
                    stockAvailability = false;
                }
                else if (brandEnforceStockCount || productEnforceStockCount)
                {
                    stockAvailability = false;
                }
                else if (item.PriceExclTax <= 0M)
                {
                    visible = false;
                }
            }

            var displayRRP = false;

            if (item.OfferRuleId > 0)
            {
                var offerRule = _offerService.GetOfferRuleById(item.OfferRuleId);
                if (offerRule != null && offerRule.ShowRRP)
                {
                    displayRRP = true;
                }
            }

            var price = item.PriceExclTax;
            var offerPrice = item.OfferPriceExclTax;

            if (_workContext.CurrentCountry.IsEC)
            {
                price = item.PriceInclTax;
                offerPrice = item.OfferPriceInclTax;
            }
            
            var savePercentageNote = string.Empty;

            if (item.OfferRuleId > 0)            
                savePercentageNote = "SAVE " + _priceFormatter.FormatPrice(price - offerPrice) + " (" + ((price - offerPrice) / price * 100).ToString("#.##") + "%)";

            var currencies = _utilityService.GetAllCurrency();
            var metaPrices = new Dictionary<SchemaMetaTagModel, SchemaMetaTagModel>();
            var metaOfferPrices = new Dictionary<SchemaMetaTagModel, SchemaMetaTagModel>();

            if (isPreSelected)
            {
                foreach (var currency in currencies)
                {
                    var metaCurrency = new SchemaMetaTagModel
                    {
                        ItemProp = "priceCurrency",
                        Content = currency.CurrencyCode
                    };

                    metaPrices.Add(metaCurrency, new SchemaMetaTagModel
                    {
                        ItemProp = "price",
                        Content = string.Format("{0:0.00}", price * currency.ExchangeRate)
                    });

                    metaOfferPrices.Add(metaCurrency, new SchemaMetaTagModel
                    {
                        ItemProp = "price",
                        Content = string.Format("{0:0.00}", offerPrice * currency.ExchangeRate)
                    });
                }
            }

            return new ProductPriceModel
            {
                Id = item.Id,
                Option = GetOptionByOptionType(type, item),
                PictureModel = GetOptionImageByOptionType(type, item),
                Price = price > 0M ? _priceFormatter.FormatPrice(price) : null,
                OfferPrice = offerPrice > 0M ? _priceFormatter.FormatPrice(offerPrice, showCurrencySymbol: true) : null,
                CurrencyCode = workingCurrency.CurrencyCode,
                CurrencySymbol = workingCurrency.Symbol,
                OfferPriceValue = offerPrice > 0M ? _priceFormatter.FormatPrice(offerPrice, showCurrency: false) : null,
                PriceValue = price > 0M ? _priceFormatter.FormatPrice(price, showCurrency: false) : null,
                OfferRuleId = item.OfferRuleId,
                SavePercentageNote = savePercentageNote,
                Visible = visible,
                MessageAfterHidden = messageAfterHidden,
                StockAvailability = stockAvailability,
                DisplayRRP = displayRRP,
                IsPreSelected = isPreSelected,
                SchemaMetaPrices = metaPrices,
                SchemaMetaOfferPrices = metaOfferPrices
            };
        }

        [NonAction]
        protected AddToCartModel PrepareAddToCartModel(ProductOverviewModel product, ProductPrice productPrice, CartItemOverviewModel cartItem = null)
        {
            var model = new AddToCartModel();
            model.ProductId = product.Id;
            model.AvailableForPreOrder = product.ShowPreOrderButton;

            if (product.Enabled == false) return model;
            if (productPrice == null) return model;
            if (productPrice.PriceExclTax <= 0M) return model;
            if (productPrice.Enabled == false) return model;
            if ((product.ProductEnforcedStockCount || product.BrandEnforcedStockCount) && productPrice.Stock <= 0) return model;

            var maxQuantity = product.CalculateMaxQuantity(productPrice.Stock, productPrice.MaximumAllowedPurchaseQuantity);

            //allowed quantities
            if (maxQuantity > 0)
            {
                model.Visible = true;

                for (int i = product.StepQuantity; i <= maxQuantity; i = i + product.StepQuantity)
                {
                    model.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
            }

            return model;
        }

        [NonAction]
        protected ProductDetailsModel PrepareProductDetailsModel(ProductOverviewModel product, int? selectedPriceId = null)
        {
            if (product == null) throw new ArgumentNullException("product");

            #region Standard properties

            var model = new ProductDetailsModel
            {
                Id = product.Id,
                Name = string.IsNullOrWhiteSpace(product.H1Title) ? product.Name : product.H1Title,
                ShortDescription = product.ShortDescription,
                FullDescription = product.FullDescription,
                DeliveryTimeLine = product.DeliveryTimeLine,
                OptionType = product.OptionType,
                AverageReviewRating = product.AverageReviewRating,
                IsPhoneOrder = product.IsPhoneOrder,
                PhoneOrderMessage = product.IsPhoneOrder ? _catalogSettings.PhoneOrderMessage : string.Empty,
                UrlKey = product.UrlKey
            };

            #endregion

            #region Brand

            var brand = _brandService.GetBrandById(product.BrandId);
            if (brand != null)
            {
                PictureModel brandPicture = null;
                if (string.IsNullOrEmpty(brand.FlashImage) == false)
                {
                    brandPicture = new PictureModel
                    {
                        AlternateText = brand.Name,
                        Title = brand.Name,
                        ImageUrl = "/media/brand/" + brand.FlashImage
                    };
                }
                var productBrandModel = new ProductBrandModel();
                productBrandModel.Picture = brandPicture;
                productBrandModel.Name = brand.Name;
                productBrandModel.BrandUrl = Url.RouteUrl("Brand", new { urlKey = brand.UrlRewrite }, Request.Url.Scheme);
                productBrandModel.Visible = true;

                model.ProductBrand = productBrandModel;
            }

            #endregion

            #region Pictures

            var medias = _productService.GetProductMediaByProductId(product.Id);
            var defaultPicture = medias.Where(x => x.Enabled).FirstOrDefault();
            var defaultPictureModel = new PictureModel();
            if (defaultPicture != null)
            {
                defaultPictureModel = new PictureModel
                {
                    Title = product.Name,
                    AlternateText = product.Name,
                    ImageUrl = defaultPicture.ThumbnailFilename,
                    FullSizeImageUrl = string.IsNullOrEmpty(defaultPicture.HighResFilename) ? defaultPicture.MediaFilename : defaultPicture.HighResFilename
                };
            }

            var pictureModels = new List<PictureModel>();
            foreach (var picture in medias)
            {
                if (picture.Enabled)
                {
                    pictureModels.Add(new PictureModel
                    {
                        Title = product.Name,
                        AlternateText = product.Name,
                        ImageUrl = picture.ThumbnailFilename,
                        FullSizeImageUrl = string.IsNullOrEmpty(picture.HighResFilename) ? picture.MediaFilename : picture.HighResFilename
                    });
                }
            }

            model.DefaultPicture = defaultPictureModel;
            model.PictureModels = pictureModels;

            #endregion

            #region Product prices

            var productPrices = _productService.GetProductPricesByProductId(product.Id);
            productPrices = productPrices.OrderBy(pp => pp.Priority).ToList();

            var defaultProductPrice = productPrices.Where(x => x.Stock > 0).FirstOrDefault();
            if (defaultProductPrice == null)
            {
                defaultProductPrice = productPrices.Where(x => x.Enabled == true).FirstOrDefault();
            }

            if (selectedPriceId.HasValue)
            {
                var found = productPrices.Where(x => x.Id == selectedPriceId.Value).FirstOrDefault();
                if (found != null) defaultProductPrice = found;
            }
            
            var defaultProductPriceModel = new ProductPriceModel();
            if (defaultProductPrice != null)
            {
                defaultProductPriceModel = PrepareProductPriceModel(
                    defaultProductPrice,
                    product.OptionType,
                    _workContext.WorkingCurrency,
                    product.Discontinued,
                    product.ProductEnforcedStockCount,
                    product.BrandEnforcedStockCount,
                    isPreSelected: true);
            }
            model.DefaultProductPrice = defaultProductPriceModel;

            foreach (var item in productPrices)
            {
                var price = PrepareProductPriceModel(
                    item,
                    product.OptionType,
                    _workContext.WorkingCurrency,
                    product.Discontinued,
                    product.ProductEnforcedStockCount,
                    product.BrandEnforcedStockCount,
                    isPreSelected: item.Id == defaultProductPriceModel.Id);

                model.ProductPrices.Add(price);
            }

            //if (model.ProductPrices.Count > 0)
            //{
            //    var preselectedOption = model.ProductPrices[0];

            //    if (selectedPriceId.HasValue)
            //    {
            //        var found = model.ProductPrices.Where(x => x.Id == selectedPriceId.Value).FirstOrDefault();
            //        if (found != null) preselectedOption = found;
            //    }

            //    preselectedOption.IsPreSelected = true;
            //}

            #endregion

            #region Add to cart

            model.AddToCart = PrepareAddToCartModel(product, defaultProductPrice);

            #endregion

            #region Product offer

            if (defaultProductPrice != null && (defaultProductPrice.OfferRuleId > 0))
            {
                var offerRule = _offerService.GetOfferRuleById(defaultProductPrice.OfferRuleId);
                if (offerRule != null
                    && offerRule.ShowCountDown
                    && offerRule.EndDate != null
                    && offerRule.EndDate.Value != DateTime.MinValue
                    && offerRule.EndDate.Value.CompareTo(DateTime.Now) >= 0)
                {
                    model.ProductOffer = new ProductOfferModel
                    {
                        ExpiryDate = offerRule.EndDate.Value,
                        Visible = true
                    };
                }
            }

            #endregion

            #region Product attributes

            #endregion

            #region Product review overview

            //model.ProductReviewOverview = new ProductReviewOverviewModel
            //{
            //    ProductId = product.Id,
            //    RatingSum = product.ApprovedRatingSum,
            //    TotalReviews = product.ApprovedTotalReviews,
            //    AllowCustomerReviews = product.AllowCustomerReviews
            //};

            #endregion

            #region Product tags

            var productTags = _productService.GetProductTagsByProductId(product.Id);

            if (productTags != null)
            {
                foreach (var productTag in productTags)
                {
                    model.ProductTags.Add(new ProductTagModel
                    {
                        Id = productTag.TagId,
                        Tag = productTag.Tag.Name,
                        Description = productTag.Value
                    });
                }
            }

            #endregion

            #region Product reviews

            var reviews = _productService.GetApprovedProductReviewsByProductId(product.Id);

            if (reviews != null)
            {
                foreach (var review in reviews)
                {
                    model.Reviews.Add(new ProductReviewModel
                    {
                        Title = review.Title,
                        Comment = review.Comment,
                        Score = review.Score,
                        Alias = review.Alias,
                        TimeStamp = review.TimeStamp
                    });
                }
            }

            #endregion

            #region Breadcrumb

            var thirdLevelCategories = _categoryService.GetCategoriesByProductId(product.Id);
            var thirdLevelCategory = thirdLevelCategories.Where(x => x.Visible == true).FirstOrDefault();

            var secondLevelCategory = thirdLevelCategory != null ? _categoryService.GetCategory(thirdLevelCategory.ParentId.Value) : null;
            var firstLevelCategory = secondLevelCategory != null ? _categoryService.GetCategory(secondLevelCategory.ParentId.Value) : null;

            var breadcrumbModel = new ProductBreadcrumbModel();

            if (firstLevelCategory != null)
            {
                breadcrumbModel.BreadCrumb.Add(new ProductBreadcrumbModel
                {
                    Name = firstLevelCategory.CategoryName,
                    Url = Url.RouteUrl("Category", new { top = firstLevelCategory.UrlRewrite }, Request.Url.Scheme)
                });

                if (secondLevelCategory != null)
                {
                    breadcrumbModel.BreadCrumb.Add(new ProductBreadcrumbModel
                    {
                        Name = secondLevelCategory.CategoryName,
                        Url = Url.RouteUrl("Category With Products", new { top = firstLevelCategory.UrlRewrite, second = secondLevelCategory.UrlRewrite, third = "" }, Request.Url.Scheme)
                    });

                    if (thirdLevelCategory != null)
                    {
                        breadcrumbModel.BreadCrumb.Add(new ProductBreadcrumbModel
                        {
                            Name = thirdLevelCategory.CategoryName,
                            Url = Url.RouteUrl("Category With Products",
                                new { top = firstLevelCategory.UrlRewrite, second = secondLevelCategory.UrlRewrite, third = thirdLevelCategory.UrlRewrite }, Request.Url.Scheme)
                        });
                    }
                }
            }

            model.ProductBreadcrumb = breadcrumbModel;

            #endregion

            return model;
        }
        
        #endregion
    }
}