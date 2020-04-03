using Apollo.Core.Caching;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Models.Brand;
using Apollo.FrontStore.Models.Common;
using Apollo.FrontStore.Models.Media;
using Apollo.FrontStore.Models.Product;
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
    public class BrandController : BasePublicController
    {
        private readonly IBrandService _brandService;
        private readonly IProductService _productService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICacheManager _cacheManager;

        public BrandController(
            IBrandService brandService,
            IProductService productService,
            IPriceFormatter priceFormatter,
            ICacheManager cacheManager)
        {
            _brandService = brandService;            
            _productService = productService;
            _priceFormatter = priceFormatter;
            _cacheManager = cacheManager;
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
        public ActionResult Brand(string urlKey, ProductPagingFilteringModel command)
        {
            if (string.IsNullOrEmpty(urlKey))            
                return RedirectToAction("ShopByBrand", "Brand");
            
            var brand = _brandService.GetBrandByUrlKey(urlKey);

            if (brand == null)
                return InvokeHttp404();

            BrandModel model;
            if (brand.HasMicrosite == true)
            {
                var key = string.Format(CacheKey.BRAND_MODEL_URL_KEY, urlKey);

                model = _cacheManager.Get(key, delegate ()
                {
                    return PrepareBrandModel(brand, includeProductsIfNoCategory: true);
                });
            }
            else
            {
                model = PrepareBrandModelWithProducts(brand, command);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ShopByBrand()
        {
            return View();
        }

        [HttpGet]
        [ChildActionOnly]
        public ActionResult BrandListByAlpha(char letter, bool forMobile = false)
        {
            var key = string.Format(CacheKey.BRAND_MODEL_LETTER_KEY, letter);
            var models = _cacheManager.Get(key, delegate ()
            {
                var brands = new List<Brand>();
                if (letter == '#')
                {
                    for (int i = 0; i < 10; i++)
                    {
                        brands.AddRange(_brandService.GetActiveBrandsByFirstLetter(i.ToString()));
                    }
                }
                else
                {
                    brands = _brandService.GetActiveBrandsByFirstLetter(letter.ToString()).ToList();
                }

                return brands.Select(b => PrepareBrandModel(b)).ToList();
            });

            if (forMobile)
                return PartialView("_BrandListByAlphaForMobile", models);
            else
                return PartialView("_BrandListByAlpha", models);
        }
        
        #region Utilities

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

        #endregion
    }
}