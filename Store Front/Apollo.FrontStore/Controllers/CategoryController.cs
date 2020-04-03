using Apollo.Core;
using Apollo.Core.Domain;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Models;
using Apollo.FrontStore.Models.Category;
using Apollo.FrontStore.Models.Media;
using Apollo.FrontStore.Models.Product;
using Apollo.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class CategoryController : BasePublicController
    {
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;        
        private readonly MediaSettings _mediaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;

        public CategoryController(
            IBrandService brandService,
            ICategoryService categoryService,
            IProductService productService,
            MediaSettings mediaSettings,
            StoreInformationSettings storeInformationSettings)
        {
            _brandService = brandService;
            _categoryService = categoryService;
            _productService = productService;
            _mediaSettings = mediaSettings;
            _storeInformationSettings = storeInformationSettings;
        }

        [HttpGet]
        public ActionResult CategoryWithProducts(string top, string second, string third,
            CategoryProductPagingFilteringModel command)
        {
            var topCategory = _categoryService.GetActiveCategoryOverviewModel(top);
            if (topCategory == null) return InvokeHttp404();
            var secondCategory = _categoryService.GetActiveCategoryOverviewModel(top, second);
            if (secondCategory == null) return InvokeHttp404();
            var categoryIds = new List<int>(secondCategory.Children.Select(x => x.Id).ToArray());
            if (categoryIds.Count <= 0) categoryIds = new List<int> { -1 }; // To indicate that no products should be loaded.

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
                visibleIndividually: true,
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
        
        [ChildActionOnly]
        //[OutputCache(Duration = 43200, VaryByParam = "none")]
        public ActionResult Menu()
        {
            const string REGEX = "/category/([^/]+)";
            var match = Regex.Match(Request.Url.PathAndQuery, REGEX);

            var model = new MenuModel
            {
                Categories = _categoryService.GetCategoryOverviewModelForMenu()
            };

            if (match.Success)
            {
                var foundUrl = match.Groups[1].Value;
                model.CategorySelectedIndex = model.Categories.IndexOf(model.Categories.Where(x => x.UrlKey == foundUrl).FirstOrDefault());
            }

            return PartialView("_Menu", model);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 43200, VaryByParam = "none")]
        public ActionResult SimpleMenu()
        {
            var items = _categoryService.GetCategoryOverviewModelForMenu();
            var model = new MenuModel
            {
                SimpleCategories = PrepareCategorySimpleModels(items)
            };

            return PartialView("_SimpleMenu", model);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 43200, VaryByParam = "none")]
        public ActionResult FooterMenu()
        {
            var items = _categoryService.GetCategoryOverviewModelForMenu();
            var model = new MenuModel
            {
                SimpleCategories = PrepareCategorySimpleModels(items)
            };

            return PartialView("_FooterMenu", model);
        }

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

        #endregion
    }
}