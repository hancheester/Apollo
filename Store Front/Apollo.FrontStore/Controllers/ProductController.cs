using Apollo.Core;
using Apollo.Core.Domain.Catalog;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Infrastructure;
using Apollo.FrontStore.Models.Media;
using Apollo.FrontStore.Models.Product;
using Apollo.Web.Framework.Controllers;
using Apollo.Web.Framework.Security;
using Apollo.Web.Framework.Services.Catalog;
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
    public class ProductController : BasePublicController
    {
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly IOfferService _offerService;
        private readonly ISearchService _searchService;
        private readonly ICategoryService _categoryService;
        private readonly IShippingService _shippingService;
        private readonly ICartService _cartService;
        private readonly IUtilityService _utilityService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly IWorkContext _workContext;
        private readonly CatalogSettings _catalogSettings;

        public ProductController(
            IBrandService brandService,
            IProductService productService,
            IOfferService offerService,
            ISearchService searchService,
            ICategoryService categoryService,
            IShippingService shippingService,
            ICartService cartService,
            IUtilityService utilityService,
            IPriceFormatter priceFormatter,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IWorkContext workContext,
            CatalogSettings catalogSettings)
        {
            _brandService = brandService;
            _productService = productService;
            _offerService = offerService;
            _searchService = searchService;
            _categoryService = categoryService;
            _shippingService = shippingService;
            _cartService = cartService;
            _utilityService = utilityService;
            _priceFormatter = priceFormatter;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _catalogSettings = catalogSettings;
            _workContext = workContext;
        }

        #region Products

        [HttpGet]
        public ActionResult ProductDetails(string urlkey)
        {
            var product = _productService.GetProductOverviewModelByUrlRewrite(urlkey);
            
            // If the product is not found, then return 404
            if (product == null || product.VisibleIndividually == false)                
                return RedirectToRoute("Product Not Found");

            // If product is disabled, show alternative products
            if (product.Enabled == false)
                return View("ProductNotAvailable", new ProductNotAvailableModel { ProductId = product.Id, Name = product.Name });
            
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            // Prepare the model
            var model = PrepareProductDetailsModel(product);

            return View(model);
        }

        [HttpGet]
        public ActionResult ProductDetailsWithOption(string urlkey, int priceid)
        {
            var product = _productService.GetProductOverviewModelByUrlRewrite(urlkey);

            // If the product is not found, then return 404
            if (product == null || product.VisibleIndividually == false)
                return RedirectToRoute("Product Not Found");

            // If product is disabled, show alternative products
            if (product.Enabled == false)
                return View("ProductNotAvailable", new ProductNotAvailableModel { ProductId = product.Id, Name = product.Name });

            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            // Prepare the model
            var model = PrepareProductDetailsModel(product, selectedPriceId: priceid);

            return View("ProductDetails", model);
        }

        [HttpGet]
        public ActionResult ProductDetailsWithCountryAndOption(string urlkey, int priceid, string countrycode, string currencycode)
        {
            var product = _productService.GetProductOverviewModelByUrlRewrite(urlkey);

            // If the product is not found, then return 404
            if (product == null || product.VisibleIndividually == false)
                return RedirectToRoute("Product Not Found");

            // If product is disabled, show alternative products
            if (product.Enabled == false)
                return View("ProductNotAvailable", new ProductNotAvailableModel { ProductId = product.Id, Name = product.Name });

            var country = _shippingService.GetCountryByCountryCode(countrycode.ToUpper());
            if (country != null) _workContext.CurrentCountry = country;

            // There is no need to save attribute if the profile is a system profile (eg. Search Engine)
            if (_workContext.CurrentProfile.IsSystemProfile == false)
            {
                var profileId = _workContext.CurrentProfile.Id;
                var options = _cartService.GetCustomerShippingOptionByCountryAndPriority(profileId);

                // Select default option
                _utilityService.SaveAttribute(profileId, "Profile", SystemCustomerAttributeNames.SelectedShippingOption, options[0].Id.ToString());
            }

            var currency = _utilityService.GetCurrencyByCurrencyCode(currencycode.ToUpper());
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            return RedirectToRoute("Product With Option", new { @urlkey = urlkey, @priceid = priceid });
        }

        [HttpGet]
        public ActionResult AddProductReview(int productId)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var product = _productService.GetProductOverviewModelById(productId);

            // If the product is not found, then return 404
            if (product == null || product.VisibleIndividually == false)
                return RedirectToRoute("Product Not Found");

            // If product is disabled, show alternative products
            if (product.Enabled == false)
                return View("ProductNotAvailable", new ProductNotAvailableModel { ProductId = product.Id, Name = product.Name });

            var model = PrepareAddProductReviewModel(product);

            return View(model);
        }

        [HttpGet]
        public ActionResult ProductNotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;

            return View();
        }
        
        [HttpPost]
        [PublicAntiForgery]
        [FormValueRequired("submit-review")]
        public ActionResult AddProductReview(int productId, AddProductReviewModel model)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var product = _productService.GetProductOverviewModelById(productId);

            //TODO: I think we should redirect to a page that recommends other related products instead.
            // If the product is offline, then return 404
            if (product == null || product.Enabled == false || product.VisibleIndividually == false)
                return InvokeHttp404();

            if (ModelState.IsValid)
            {
                var review = new ProductReview
                {
                    ProductId = product.Id,
                    ProfileId = _workContext.CurrentProfile.Id,
                    Alias = model.Alias,
                    Title = model.Title,
                    Comment = model.Comment,
                    Score = model.Rating,
                    TimeStamp = DateTime.Now,                    
                    ProductName = product.Name
                };

                _productService.InsertProductReview(review);

                model = PrepareAddProductReviewModel(product);
                model.SuccessfullyAdded = true;
                model.Result = "You will see the product review after approving by a store administrator.";

                return View(model);
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult HomepageProducts(int groupId)
        {
            var items = _productService.GetHomepageProductByGroupId(groupId);
            var models = new List<ProductBoxModel>();
            if (items != null)
            {
                models = items.PrepareProductBoxModels(styleClass: "wide", imageLoadType: "owlCarousel") as List<ProductBoxModel>;
            }

            return PartialView("_ProductGroup", models);
        }

        [ChildActionOnly]
        public ActionResult CategoryFeaturedProducts(int categoryId, int featuredItemTypeId)
        {
            var items = _productService.GetActiveProductOverviewModelByCategoryFeaturedItemType(categoryId, featuredItemTypeId);
            var models = items.PrepareProductBoxModels(styleClass: "wide", imageLoadType: "owlCarousel");

            return PartialView("_ProductGroup", models);
        }

        [ChildActionOnly]
        public ActionResult BrandFeaturedProducts(int brandId, int featuredItemTypeId)
        {
            var items = _productService.GetActiveProductOverviewModelByBrandFeaturedItemType(brandId, featuredItemTypeId);
            var models = items.PrepareProductBoxModels(styleClass: "wide", imageLoadType: "owlCarousel");

            return PartialView("_ProductGroup", models);
        }

        [ChildActionOnly]
        public ActionResult AlternativeProducts(int productId)
        {
            var product = _productService.GetProductOverviewModelById(productId);

            if (product == null) return Content("");

            IList<ProductBoxModel> products = new List<ProductBoxModel>();

            if (product.BrandCategoryId != 0)
            {
                // Try searching by brand category first            
                var result = _productService.GetPagedProductOverviewModelsByBrandCategoryIds(
                    product.BrandId,
                    pageSize: 12,
                    brandCategoryIds: new List<int> { product.BrandCategoryId },
                    enabled: true,
                    includeDiscontinuedButInStock: true);

                if (result != null && result.Items.Count > 0)
                    products = result.Items.PrepareProductBoxModels(styleClass: "wide");
            }

            // If nothing found, try searching by category
            if (products.Count == 0 && product.AssignedCategoryIds.Count > 0)
            {
                var result = _productService.GetPagedProductOverviewModel(                
                    pageSize: 12,
                    categoryIds: product.AssignedCategoryIds,                
                    enabled: true,
                    visibleIndividually: true,
                    includeDiscontinuedButInStock: true);

                if(result != null && result.Items.Count > 0)
                    products = result.Items.PrepareProductBoxModels(styleClass: "wide");
            }
            
            var model = new AlternativeProductModel { ProductId = productId, Products = products };

            return PartialView("_AlternativeProducts", model);
        }

        [ChildActionOnly]
        public ActionResult RecentlyViewedProducts()
        {
            var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(9);
            if (!products.Any()) return Content("");
            var models = products.PrepareProductBoxModels();

            return PartialView("_RecentlyViewedProducts", models);
        }

        [AjaxOnly]        
        public ActionResult GetPriceInfo(int productId, int productPriceId, int type)
        {
            var product = _productService.GetProductOverviewModelById(productId);
            var prices = _productService.GetProductPricesByProductId(productId);
            var price = prices.Where(x => x.Id == productPriceId).FirstOrDefault();

            var priceModel = price.PrepareProductPriceModel(                
                (OptionType)type,
                product.Discontinued,
                product.ProductEnforcedStockCount,
                product.BrandEnforcedStockCount);
            var PriceInfoSectionHtml = RenderPartialViewToString("ProductPriceInfo", priceModel);

            var AddToCartModel = PrepareAddToCartModel(product, price);
            var AddToCartInfoSectionHtml = RenderPartialViewToString("_AddToCart", AddToCartModel, string.Format("addtocart_{0}", productId));

            return Json(new
            {
                success = true,
                message = string.Empty,
                priceInfoSectionHtml = PriceInfoSectionHtml,
                addToCartInfoSectionHtml = AddToCartInfoSectionHtml
            });
        }
        
        [HttpPost]
        public ActionResult RemoveReviewedItem(int productId)
        {
            _recentlyViewedProductsService.RemoveProductFromRecentlyViewedList(productId);
            return Content("");
        }

        #endregion

        #region Searching

        [ValidateInput(false)]
        public ActionResult Search(SearchModel model, ProductPagingFilteringModel command)
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

            var productId = 0;
            string keywords = null;
            string originalKeywords = model.q;
            if (int.TryParse(model.q, out productId) == false)
                keywords = model.q;

            if (!string.IsNullOrEmpty(keywords)) keywords = keywords.Trim();

            var result = _searchService.SearchProduct(
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize,
                brandIds: brandList.Count > 0 ? brandList : null,
                productIds: productId != 0 ? new int[] { productId } : null,
                enabled: true,
                visibleIndividually: true,
                includeDiscontinuedButInStock: true,
                priceMin: priceMin,
                priceMax: priceMax,
                keywords: keywords,
                searchDescriptions: false,
                useFullTextSearch: true,
                fullTextMode: FulltextSearchMode.Or,
                applySearchAnalysis: true,
                applyKeywordSuggestion: true,
                orderBy: command.OrderBy);

            //If we have predefined search term, we would like to bring customers to this url.
            if (result.HasSearchTerm) return Redirect(result.SearchTerm.RedirectUrl);
            
            //If we only have one product, redirect to product page.
            if (result.Items.Count == 1 && command.PageNumber == 1) return RedirectToRoute("Product", new { urlkey = result.Items[0].UrlKey });

            model = new SearchModel();
            model.OriginalKeywords = string.IsNullOrEmpty(keywords) ? originalKeywords : keywords;
            model.q = model.OriginalKeywords;
            if (!string.IsNullOrEmpty(result.SuggestedKeywords))
                model.SuggestedKeywords = result.SuggestedKeywords.ToLower() != keywords.ToLower() ? result.SuggestedKeywords : string.Empty;

            //If we have no result, redirect to empty result page.
            if (result.Items.Count == 0
                && result.MinPriceFilterByKeyword == 0M
                && result.MaxPriceFilterByKeyword == 0M)
                return View("SearchResultNotFound", model);

            model.Products = result.Items.PrepareProductBoxModels();            
            model.PagingFilteringContext.LoadPagedList(result);
            
            string min = Math.Round(result.MinPriceFilterByKeyword).ToString();
            string max = Math.Ceiling(result.MaxPriceFilterByKeyword).ToString();

            model.PagingFilteringContext.PriceRangeFilter = new PriceRangeFilterModel()
            {
                Min = min,
                Max = max,
                From = (command.From == string.Empty) ? min : command.From,
                To = (command.To == string.Empty) ? max : command.To
            };

            //model.PagingFilteringContext.BrandRangeFilter.Brands = brandRange;
            model.PagingFilteringContext.BrandRangeFilter.SelectedBrands = brandList.ToArray();
            model.PagingFilteringContext.SelectedPageSize = command.PageSize;
            model.PagingFilteringContext.OrderBy = command.OrderBy;
            model.PagingFilteringContext.PrepareSortingOptions();
            model.PagingFilteringContext.ViewMode = command.ViewMode;
            model.PagingFilteringContext.Brands = command.Brands;
            model.PagingFilteringContext.PrepareViewModeOptions();

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            var model = new SearchBoxModel();
            return PartialView(model);
        }

        #endregion

        #region Utilities
        
        [NonAction]
        protected AddToCartModel PrepareAddToCartModel(ProductOverviewModel product, ProductPrice productPrice, CartItemOverviewModel cartItem = null)
        {
            var model = new AddToCartModel();
            model.ProductId = product.Id;
            model.AvailableForPreOrder = product.ShowPreOrderButton;

            if (product.Enabled == false)  return model;
            if (productPrice == null) return model;
            if (productPrice.PriceExclTax <= 0M) return model;
            if (productPrice.Enabled == false) return model;
            if ((product.ProductEnforcedStockCount || product.BrandEnforcedStockCount) && productPrice.Stock <= 0) return model;
            
            var maxQuantity = product.CalculateMaxQuantity(productPrice.Stock, productPrice.MaximumAllowedPurchaseQuantity);

            //allowed quantities
            if (maxQuantity > 0)
            {
                model.Visible = true;

                var increment = product.StepQuantity <= 0 ? 1 : product.StepQuantity;

                for (int i = product.StepQuantity; i <= maxQuantity; i = i + increment)
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
                UrlKey = product.UrlKey,
                Enabled = product.Enabled,
                Discontinued = product.Discontinued,
                MetaDescription = product.MetaDescription,
                MetaKeywords = product.MetaKeywords,
                MetaTitle = product.MetaTitle
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

            var medias = product.Images;
            var defaultPicture = medias.Where(x => x.Enabled == true).FirstOrDefault();
            var defaultPictureModel = new PictureModel();
            if (defaultPicture != null)
            {
                defaultPictureModel = new PictureModel
                {
                    Title = product.Name,
                    AlternateText = product.Name,
                    ImageUrl = string.Format("/media/product/{0}", defaultPicture.ThumbnailFilename),
                    FullSizeImageUrl = string.Format("/media/product/{0}", string.IsNullOrEmpty(defaultPicture.HighResFilename) ? defaultPicture.MediaFilename : defaultPicture.HighResFilename)
                };
            }

            var pictureModels = new List<PictureModel>();
            foreach (var picture in medias)
            {
                if (picture.Enabled)
                {
                    pictureModels.Add(new PictureModel
                    {
                        Id = picture.Id,
                        Title = product.Name,
                        AlternateText = product.Name,
                        ImageUrl = string.Format("/media/product/{0}", picture.ThumbnailFilename),
                        FullSizeImageUrl = string.Format("/media/product/{0}", string.IsNullOrEmpty(picture.HighResFilename) ? picture.MediaFilename : picture.HighResFilename)
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
                defaultProductPriceModel = defaultProductPrice.PrepareProductPriceModel(                    
                    product.OptionType,                    
                    product.Discontinued,
                    product.ProductEnforcedStockCount,
                    product.BrandEnforcedStockCount,
                    isPreSelected: true);
            }
            model.DefaultProductPrice = defaultProductPriceModel;
            
            foreach (var item in productPrices)
            {
                var price = item.PrepareProductPriceModel(                    
                    product.OptionType,                    
                    product.Discontinued,
                    product.ProductEnforcedStockCount,
                    product.BrandEnforcedStockCount,
                    isPreSelected: item.Id == defaultProductPriceModel.Id);

                model.ProductPrices.Add(price);
            }
            
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
            
            model.ProductBreadcrumb = PrepareProductBreadcrumbModel(product.Id);

            #endregion

            return model;
        }

        [NonAction]
        protected ProductBreadcrumbModel PrepareProductBreadcrumbModel(int productId)
        {
            var thirdLevelCategories = _categoryService.GetCategoriesByProductId(productId);
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

            return breadcrumbModel;
        }

        [NonAction]
        protected AddProductReviewModel PrepareAddProductReviewModel(ProductOverviewModel product)
        {
            var model = new AddProductReviewModel
            {
                Name = string.IsNullOrWhiteSpace(product.H1Title) ? product.Name : product.H1Title,
                UrlKey = product.UrlKey
            };

            model.ProductBox = product.PrepareProductBoxModel();
            model.ProductBreadcrumb = PrepareProductBreadcrumbModel(product.Id);

            return model;
        }
       
        #endregion
    }
}