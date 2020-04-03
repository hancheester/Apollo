using Apollo.Core.Caching;
using Apollo.Core.Domain.Media;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Apollo.Core.Services.Catalog
{
    public class CategoryService : BaseRepository, ICategoryService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategoryFilter> _categoryFilterRepository;
        private readonly IRepository<CategoryMedia> _categoryMediaRepository;
        private readonly IRepository<CategoryFeaturedItem> _categoryFeaturedItemRepository;
        private readonly IRepository<CategoryFeaturedBrand> _categoryFeaturedBrandRepository;
        private readonly IRepository<CategoryWhatsNew> _categoryWhatsNewRepository;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IRepository<GoogleTaxonomy> _googleTaxonomyRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductCategoryFilter> _productCategoryFilterRepository;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IRepository<CategoryLargeBannerMapping> _categoryLargeBannerMappingRepository;
        private readonly IRepository<LargeBanner> _largeBannerRepository;
        private readonly ICacheManager _cacheManager;
        private readonly MediaSettings _mediaSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public CategoryService(
            IDbContext dbContext,
            IRepository<Category> categoryRepository,
            IRepository<CategoryFilter> categoryFilterRepository,
            IRepository<CategoryMedia> categoryMediaRepository,
            IRepository<CategoryFeaturedItem> categoryFeaturedItemRepository,
            IRepository<CategoryFeaturedBrand> categoryFeaturedBrandRepository,
            IRepository<CategoryWhatsNew> categoryWhatsNewRepository,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            IRepository<GoogleTaxonomy> googleTaxonomyRepository,
            IRepository<Brand> brandRepository,
            IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductCategoryFilter> productCategoryFilterRepository,
            IRepository<ProductPrice> productPriceRepository,
            IRepository<CategoryLargeBannerMapping> categoryLargeBannerMappingRepository,
            IRepository<LargeBanner> largeBannerRepository,
            ILogBuilder logBuilder,
            ICacheManager cacheManager,
            MediaSettings mediaSettings)
        {
            _dbContext = dbContext;
            _categoryRepository = categoryRepository;
            _categoryFilterRepository = categoryFilterRepository;
            _categoryMediaRepository = categoryMediaRepository;
            _categoryFeaturedItemRepository = categoryFeaturedItemRepository;
            _categoryFeaturedBrandRepository = categoryFeaturedBrandRepository;
            _categoryWhatsNewRepository = categoryWhatsNewRepository;
            _categoryTemplateRepository = categoryTemplateRepository;
            _googleTaxonomyRepository = googleTaxonomyRepository;
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _productCategoryFilterRepository = productCategoryFilterRepository;
            _productPriceRepository = productPriceRepository;
            _categoryLargeBannerMappingRepository = categoryLargeBannerMappingRepository;
            _largeBannerRepository = largeBannerRepository;
            _cacheManager = cacheManager;
            _mediaSettings = mediaSettings;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion
        
        #region Create

        public int InsertCategory(Category category)
        {
            return _categoryRepository.Create(category);
        }

        public int InsertCategoryFilter(CategoryFilter categoryFilter)
        {
            return _categoryFilterRepository.Create(categoryFilter);
        }

        public int InsertCategoryMedia(CategoryMedia media)
        {
            return _categoryMediaRepository.Create(media);
        }

        public int InsertCategoryFeaturedItem(CategoryFeaturedItem item)
        {
            return _categoryFeaturedItemRepository.Create(item);
        }

        public int InsertCategoryWhatsNew(CategoryWhatsNew item)
        {
            return _categoryWhatsNewRepository.Create(item);
        }
        
        #endregion

        #region Return

        public CategoryWhatsNew GetCategoryWhatsNewById(int categoryWhatsNewById)
        {
            return _categoryWhatsNewRepository.Return(categoryWhatsNewById);
        }

        public CategoryFilter GetCategoryFilterById(int categoryFilterId)
        {
            return _categoryFilterRepository.Return(categoryFilterId);
        }

        public GoogleTaxonomy GetGoogleTaxonomyById(int googleTaxonomyId)
        {
            return _googleTaxonomyRepository.Return(googleTaxonomyId);
        }

        public CategoryTemplate GetCategoryTemplateById(int categoryTemplateId)
        {
            return _categoryTemplateRepository.Return(categoryTemplateId);
        }

        public IList<CategoryMedia> GetCategoryMediasByCategoryId(int categoryId)
        {
            return _categoryMediaRepository.TableNoTracking.Where(x => x.CategoryId == categoryId).ToList();
        }

        public CategoryOverviewModel GetActiveCategoryOverviewModel(string topUrlKey, string secondUrlKey, string thirdUrlKey)
        {
            var query = _categoryRepository.Table
                        .GroupJoin(_categoryRepository.Table,
                                   c1 => c1.Id,
                                   c2 => c2.ParentId,
                                   (c1, c2) => new { c1, c2 = c2.DefaultIfEmpty() })
                        .SelectMany(c1_c2 => c1_c2.c2.DefaultIfEmpty(),
                                    (c1_c2, c2) => new { c1_c2.c1, c2 })
                        .GroupJoin(_categoryRepository.Table,
                                   c1_c2 => c1_c2.c2.Id,
                                   c3 => c3.ParentId,
                                   (c1_c2, c3) => new { c1_c2.c1, c1_c2.c2, c3 = c3.DefaultIfEmpty() })
                        .SelectMany(c1_c2_c3 => c1_c2_c3.c3.DefaultIfEmpty(),
                                    (c1_c2, c3) => new { c1_c2.c1, c1_c2.c2, c3 });


            if (!string.IsNullOrEmpty(topUrlKey))
            {
                CategoryOverviewModel item;

                query = query
                        .Where(c1_c2_c3 => c1_c2_c3.c1.UrlRewrite == topUrlKey)
                        .Where(c1_c2_c3 => c1_c2_c3.c1.Visible == true)
                        .OrderBy(c1_c2_c3 => c1_c2_c3.c1.Priority)
                        .Select(c1_c2_c3 => c1_c2_c3);

                if (!string.IsNullOrEmpty(secondUrlKey))
                {
                    var query1 = query
                                .Where(c1_c2_c3 => c1_c2_c3.c2.UrlRewrite == secondUrlKey)
                                .Where(c1_c2_c3 => c1_c2_c3.c2.Visible == true)
                                .OrderBy(c1_c2_c3 => c1_c2_c3.c2.Priority)
                                .Select(c1_c2_c3 => new { c1_c2_c3.c2, c1_c2_c3.c3 });

                    if (!string.IsNullOrEmpty(thirdUrlKey))
                    {
                        var query2 = query1
                            .Where(c1_c2_c3 => c1_c2_c3.c3.UrlRewrite == thirdUrlKey)
                            .Where(c1_c2_c3 => c1_c2_c3.c3.Visible == true)
                            .OrderBy(c1_c2_c3 => c1_c2_c3.c3.Priority)
                            .Select(c2_c3 => c2_c3.c3);

                        item = query2.Select(c3 => new CategoryOverviewModel
                        {
                            Id = c3.Id,
                            Name = c3.CategoryName,
                            UrlKey = c3.UrlRewrite,
                            Description = c3.Description,
                            ShortDescription = c3.ShortDescription,
                            ThumbnailFilename = c3.ThumbnailFilename,
                            CategoryTemplateId = c3.CategoryTemplateId
                        })
                        .FirstOrDefault();
                    }
                    else
                    {
                        item = query1.Select(c2_c3 => new CategoryOverviewModel
                        {
                            Id = c2_c3.c2.Id,
                            Name = c2_c3.c2.CategoryName,
                            UrlKey = c2_c3.c2.UrlRewrite,
                            Description = c2_c3.c2.Description,
                            ShortDescription = c2_c3.c2.ShortDescription,
                            ThumbnailFilename = c2_c3.c2.ThumbnailFilename,
                            CategoryTemplateId = c2_c3.c2.CategoryTemplateId
                        })
                        .FirstOrDefault();
                    }
                }
                else
                {
                    item = query.Select(c1_c2_c3 => new CategoryOverviewModel
                    {
                        Id = c1_c2_c3.c1.Id,
                        Name = c1_c2_c3.c1.CategoryName,
                        UrlKey = c1_c2_c3.c1.UrlRewrite,
                        Description = c1_c2_c3.c1.Description,
                        ShortDescription = c1_c2_c3.c1.ShortDescription,
                        ThumbnailFilename = c1_c2_c3.c1.ThumbnailFilename,
                        CategoryTemplateId = c1_c2_c3.c1.CategoryTemplateId
                    })
                    .FirstOrDefault();

                    if (item != null)
                    {
                        item.Banners = GetLargeBannersByCategory(item.Id, enabled: true);
                        item.FeatureItemTypes = _categoryFeaturedItemRepository.Table
                            .Where(x => x.CategoryId == item.Id)
                            .Where(x => x.FeaturedItemType != 0)                            
                            .Select(x => x.FeaturedItemType)
                            .Distinct()
                            .ToList()
                            .Where(x => x != Convert.ToInt32(FeaturedItemType.Position))
                            .ToList();
                    }
                }

                if (item != null)
                {
                    item.Children = GetActiveCategoryViewModelByParentId(item.Id);

                    if (item.Children != null)
                    {
                        foreach (var child in item.Children)
                        {
                            child.Children = GetActiveCategoryViewModelByParentId(child.Id);
                        }
                    }

                    return item;
                }
            }

            return null;
        }

        private IList<CategoryOverviewModel> GetActiveCategoryViewModelByParentId(int categoryId)
        {
            return _categoryRepository.Table
                .Where(c => c.ParentId == categoryId)
                .Where(c => c.Visible == true)
                .OrderBy(c => c.Priority)
                .Select(c => new CategoryOverviewModel
                {
                    Id = c.Id,
                    Name = c.CategoryName,
                    UrlKey = c.UrlRewrite,
                    Description = c.Description,
                    ShortDescription = c.ShortDescription,
                    ThumbnailFilename = c.ThumbnailFilename,
                    CategoryTemplateId = c.CategoryTemplateId,
                    MetaTitle = c.MetaTitle,
                    MetaDescription = c.MetaDescription,
                    MetaKeywords = c.MetaKeywords,
                    H1Title = c.H1Title
                })
                .ToList();
        }

        public IList<CategoryOverviewModel> GetCategoryOverviewModelForMenu()
        {
            // Parent
            var list = _categoryRepository.Table.Where(c => c.ParentId == 0)
                        .Where(c => c.Visible == true)
                        .OrderBy(c => c.Priority)
                        .Select(c => new CategoryOverviewModel
                        {
                            Id = c.Id,
                            Name = c.CategoryName,
                            UrlKey = c.UrlRewrite,
                            ThumbnailFilename = c.ThumbnailFilename,
                            ColourScheme = c.ColourScheme
                        })
                        .ToList();

            // Second level
            foreach (var item in list)
            {
                item.Children = _categoryRepository.Table
                                .Where(c => c.ParentId == item.Id)
                                .Where(c => c.Visible == true)
                                .OrderBy(c => c.Priority)
                                .Select(c => new CategoryOverviewModel
                                {
                                    Id = c.Id,
                                    Name = c.CategoryName,
                                    UrlKey = c.UrlRewrite,
                                    ThumbnailFilename = c.ThumbnailFilename
                                })
                                .ToList();

                // Third level
                foreach (var child in item.Children)
                {
                    // Get Whats New
                    child.ActiveWhatsNew = _categoryWhatsNewRepository.Table
                        .Where(x => x.CategoryId == child.Id)
                        .Where(x => x.Enabled == true)
                        .Where(x =>
                                (x.StartDate == null || x.StartDate.Value.CompareTo(DateTime.Now) <= 0)
                                &&
                                (x.EndDate == null || x.EndDate.Value.CompareTo(DateTime.Now) >= 0))
                        .OrderBy(x => x.Priority)
                        .FirstOrDefault();

                    child.Children = _categoryRepository.Table
                                     .Where(c => c.ParentId == child.Id)
                                     .Where(c => c.Visible == true)
                                     .OrderBy(c => c.Priority)
                                     .Select(c => new CategoryOverviewModel
                                     {
                                         Id = c.Id,
                                         Name = c.CategoryName,
                                         UrlKey = c.UrlRewrite,
                                         ThumbnailFilename = c.ThumbnailFilename
                                     })
                                     .ToList();

                    child.FeaturedBrands = _categoryFeaturedBrandRepository.Table
                                            .Join(_brandRepository.Table, fb => fb.BrandId, b => b.Id, (fb, b) => new { fb, b })
                                            .Where(fb_b => fb_b.fb.CategoryId == child.Id)
                                            .OrderBy(fb_b => fb_b.fb.Priority)
                                            .Select(fb_b => new CategoryOverviewModel
                                            {
                                                Name = fb_b.b.Name,
                                                UrlKey = fb_b.b.UrlRewrite
                                            })
                                            .ToList();

                    child.FeaturedItems = _categoryFeaturedItemRepository.Table
                                            .Join(_productRepository.Table, cf => cf.ProductId, p => p.Id, (cf, p) => new { cf, p })
                                            .Where(x => x.cf.CategoryId == child.Id)
                                            .OrderBy(x => x.cf.Priority)
                                            .Select(x => new CategoryFeaturedItemOverviewModel
                                            {
                                                CategoryId = x.cf.CategoryId,
                                                FeaturedItemType = x.cf.FeaturedItemType,
                                                ProductId = x.cf.ProductId,
                                                ProductName = x.p.Name,
                                                ProductUrlKey = x.p.UrlRewrite
                                            })
                                            .ToList();                                            
                }
            }

            return list;
        }

        public IList<int> GetGoogleTaxonomyTreeList(int googleTaxonomyId)
        {
            var treeList = new List<int>();

            var taxonomy = _googleTaxonomyRepository.Return(googleTaxonomyId);

            if (taxonomy != null)
            {
                while (taxonomy.ParentId != 0)
                {
                    treeList.Insert(0, taxonomy.Id);
                    taxonomy = _googleTaxonomyRepository.Return(taxonomy.ParentId);
                }

                treeList.Insert(0, taxonomy.Id);
            }

            return treeList;
        }

        public IList<GoogleTaxonomy> GetGoogleTaxonomyTreeByParentId(int parentId)
        {
            var list = _googleTaxonomyRepository.Table
                        .Where(gt => gt.ParentId == parentId)
                        .GroupJoin(_googleTaxonomyRepository.Table,
                                   gt1 => gt1.ParentId,
                                   gt2 => gt2.Id,
                                   (gt1, gt2) => new { gt1, gt2 = gt2.DefaultIfEmpty() })
                        .SelectMany(gt1_gt2 => gt1_gt2.gt2.DefaultIfEmpty(),
                                   (gt1_gt2, gt2) => gt1_gt2.gt1)
                        .ToList();

            return list;
        }

        public int GetCategoryTreeLevel(int categoryId)
        {
            int level = _categoryRepository.Table.Where(c => c.Id == categoryId).Select(c => c.TreeLevel).FirstOrDefault();
            return level;
        }

        public Category GetCategoryByUrlKey(string urlKey)
        {
            var category = _categoryRepository.Table.Where(c => c.UrlRewrite == urlKey).FirstOrDefault();

            if (category != null)
                category = BuildCategory(category);

            return category;
        }

        public Category GetActiveCategory(int categoryId)
        {
            var category = _categoryRepository.TableNoTracking.Where(c => c.Id == categoryId && c.Visible == true).FirstOrDefault();

            if (category != null)
                category = BuildCategory(category);

            return category;
        }

        public Category GetCategory(int categoryId)
        {
            var category = _categoryRepository.Return(categoryId);

            if (category != null)
                category = BuildCategory(category);

            return category;
        }

        public IList<int> GetCategoryIdListByProductId(int productId)
        {
            var list = _productCategoryRepository.Table
                .Join(_categoryRepository.Table, pc => pc.CategoryId, c => c.Id, (pc, c) => new { pc, c })
                .Where(pc_c => pc_c.pc.ProductId == productId)
                .Select(pc_c => pc_c.c.Id)
                .ToList();

            return list;
        }

        public IList<Category> GetCategoriesByProductId(int productId)
        {
            var list = _productCategoryRepository.Table
                .Join(_categoryRepository.Table, pc => pc.CategoryId, c => c.Id, (pc, c) => new { pc, c })
                .Where(pc_c => pc_c.pc.ProductId == productId)
                .Select(pc_c => pc_c.c)
                .ToList();

            return list;
        }

        public IList<Category> GetCategoriesByLargeBannerId(int largeBannerId)
        {
            var list = _categoryLargeBannerMappingRepository.Table
                .Join(_categoryRepository.Table, m => m.CategoryId, c => c.Id, (m, c) => new { m, c })
                .Where(m_c => m_c.m.LargeBannerId == largeBannerId)
                .Select(m_c => m_c.c)
                .ToList();

            return list;
        }

        public Category GetFirstActiveCategoryByProductId(int productId)
        {
            var category = _productCategoryRepository.Table
                .Join(_categoryRepository.Table, pc => pc.CategoryId, c => c.Id, (pc, c) => new { pc, c })
                .Where(pc_c => pc_c.pc.ProductId == productId)
                .Where(pc_c => pc_c.c.Visible == true)
                .Where(pc_c => pc_c.c.CategoryName.Contains("&#163;") == false) // We do not need to include £ (&#163;)
                .Select(pc_c => pc_c.c)
                .FirstOrDefault();

            return category;
        }
        
        public IList<CategoryFilter> GetCategoryFilterListByCategoryIdAndBrandId(int categoryId, int brandId)
        {
            var list = _categoryFilterRepository.Table
                .Join(_categoryRepository.Table, cf => cf.CategoryId, c => c.Id, (cf, c) => new { cf, c })
                .Join(_productCategoryFilterRepository.Table, cf_c => cf_c.cf.Id, pcf => pcf.CategoryFilterId, (cf_c, pcf) => new { cf_c.cf, cf_c.c, pcf })
                .Join(_productRepository.Table, cf_pcf => cf_pcf.pcf.ProductId, p => p.Id, (cf_pcf, p) => new { cf_pcf.cf, cf_pcf.pcf, p })
                .Where(cf_pcf_p => cf_pcf_p.cf.CategoryId == categoryId && cf_pcf_p.p.Enabled == true && cf_pcf_p.p.BrandId == brandId)
                .Select(cf_pcf_p => cf_pcf_p.cf)
                .ToList();

            return list;
        }

        public IList<CategoryFilter> GetCategoryFilterListByCategoryFilterIdAndType(int categoryFilterId, string type)
        {
            var list = _categoryFilterRepository.Table
                .Where(cf => cf.Id == categoryFilterId && cf.Type == type)
                .ToList();

            return list;
        }

        public IList<CategoryFilter> GetCategoryFilterByCategoryId(int categoryId)
        {
            var list = _categoryFilterRepository.Table
                .Where(cf => cf.CategoryId == categoryId)
                .ToList();

            return list;
        }

        public IList<Category> GetCategoryByParent(int parentId)
        {
            var list = _categoryRepository.Table
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.Priority)
                .ToList();

            return list;
        }

        public IList<Category> GetCategoryListByParentId(int parentId)
        {
            var list = _categoryRepository.Table.Where(c => c.ParentId == parentId).ToList();

            if (list != null)
                for (int i = 0; i < list.Count; i++)
                    list[i] = BuildCategory(list[i]);

            return list;
        }

        public IList<int> GetTreeList(int categoryId)
        {
            IList<int> treeList = new List<int>();
            treeList = BuildTree(categoryId, treeList);
            
            return treeList;
        }

        public IList<KeyValuePair<int, string>> GetTreeListWithName(int categoryId)
        {
            string key = string.Format(CacheKey.CATEGORY_TREE_LIST_WITH_NAME_BY_ID_KEY, categoryId);

            var list = _cacheManager.Get(key, delegate ()
            {
                var tree = GetTreeList(categoryId);
                var treeWithName = new List<KeyValuePair<int, string>>();

                foreach (var item in tree)
                {
                    var name = _categoryRepository.Table.Where(x => x.Id == item).Select(x => x.CategoryName).FirstOrDefault();
                    treeWithName.Add(new KeyValuePair<int, string>(item, name));
                }

                return treeWithName;
            });

            return list;
        }

        public IList<KeyValuePair<int, string>> GetGoogleTaxonomyTreeListWithName(int googleTaxonomyId)
        {
            string key = string.Format(CacheKey.CATEGORY_GOOGLE_TAXONOMY_TREE_LIST_WITH_NAME_BY_GOOGLE_TAXONOMY_ID_KEY, googleTaxonomyId);

            var list = _cacheManager.Get(key, delegate ()
            {
                var treeWithName = new List<KeyValuePair<int, string>>();

                var taxonomy = _googleTaxonomyRepository.Return(googleTaxonomyId);

                if (taxonomy != null)
                {
                    treeWithName.Insert(0, new KeyValuePair<int, string>(taxonomy.Id, taxonomy.Name));

                    while (taxonomy.ParentId != 0)
                    {
                        taxonomy = _googleTaxonomyRepository.Return(taxonomy.ParentId);
                        treeWithName.Insert(0, new KeyValuePair<int, string>(taxonomy.Id, taxonomy.Name));
                    }
                }

                return treeWithName;
            });

            return list;
        }

        public IList<Tuple<int, string, int>> GetCategoryFiltersRangeByCategory(IList<int> categoryIds)
        {
            if (categoryIds == null) return new List<Tuple<int, string, int>>();

            var items = _productCategoryFilterRepository.Table
                .Join(_categoryFilterRepository.Table, pcf => pcf.CategoryFilterId, cf => cf.Id, (pcf, cf) => new { pcf, cf })
                .Join(_productRepository.Table, pcf_cf => pcf_cf.pcf.ProductId, p => p.Id, (pcf_cf, p) => new { pcf_cf.pcf, pcf_cf.cf, p })                
                .Where(x => categoryIds.Contains(x.cf.CategoryId))
                .Where(x => x.p.Enabled == true)
                .Where(x => x.p.Discontinued == false || (x.p.Discontinued == true && _productPriceRepository.Table.Where(pp => pp.ProductId == x.p.Id).Select(pp => pp.Stock).Max() > 0))
                .GroupBy(x => new { x.cf.Id, x.cf.Type })
                .ToList()
                .Select(a => new Tuple<int, string, int>(a.Key.Id, a.Key.Type, a.Count()))
                .ToList();

            return items;
        }

        public IList<CategoryTemplate> GetAllCategoryTemplates()
        {
            return _categoryTemplateRepository.Table.OrderBy(x => x.DisplayOrder).ToList();
        }

        #endregion

        #region Delete
        
        public void DeleteCategoryWhatsNew(int categoryWhatsNewId)
        {
            _categoryWhatsNewRepository.Delete(categoryWhatsNewId);
        }

        public void DeleteProductCategoryByCategoryId(int categoryId)
        {
            var list = _productCategoryRepository.Table
                .Where(pc => pc.CategoryId == categoryId).Select(pc => pc.Id).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                _productCategoryRepository.Delete(list[i]);
            }
        }

        public void DeleteCategoryFromProductIdList(int categoryId, IList<int> productIds)
        {
            for (int i = 0; i < productIds.Count; i++)
            {
                DeleteCategoryFromProduct(categoryId, productIds[i]);
            }
        }

        public void DeleteCategoryFromProduct(int categoryId, int productId)
        {
            var list = _productCategoryRepository.Table
                .Where(pc => pc.CategoryId == categoryId)
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.Id)
                .ToList();

            for (int i = 0; i < list.Count; i++)
            {
                _productCategoryRepository.Delete(list[i]);
            }
        }

        public void DeleteCategoryFromLargeBanner(int categoryId, int largeBannerId)
        {
            var list = _categoryLargeBannerMappingRepository.Table
                .Where(m => m.CategoryId == categoryId)
                .Where(m => m.LargeBannerId == largeBannerId)
                .Select(m => m.Id)
                .ToList();

            for (int i = 0; i < list.Count; i++)
            {
                _categoryLargeBannerMappingRepository.Delete(list[i]);
            }
        }

        public void DeleteCategoryMedia(int categoryMediaId)
        {
            var media = _categoryMediaRepository.Return(categoryMediaId);

            if (media != null)
            {
                var path = _mediaSettings.CategoryMediaLocalPath;
                var filePath = path + media.MediaFilename;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _categoryMediaRepository.Delete(media);
            }
        }

        public void DeleteCategoryFilterByProductIdList(int categoryFilterId, IList<int> productIds)
        {
            var list = _productCategoryFilterRepository.Table
                .Where(f => f.CategoryFilterId == categoryFilterId)
                .Where(f => productIds.Contains(f.ProductId))
                .ToList();

            for (int i = 0; i < list.Count; i++)
            {
                _productCategoryFilterRepository.Delete(list[i]);
            }
        }

        public void DeleteCategoryFeaturedItem(int categoryId, int featuredItemType, int productId)
        {
            var list = _categoryFeaturedItemRepository.Table
                .Where(f => f.CategoryId == categoryId)
                .Where(f => f.ProductId == productId)
                .Where(f => f.FeaturedItemType == featuredItemType)
                .Select(f => f.Id)
                .ToList();

            for (int i = 0; i < list.Count; i++)
            {
                _categoryFeaturedItemRepository.Delete(list[i]);
            }
        }

        public void DeleteCategoryFeaturedBrandByCategoryId(int categoryId)
        {
            var list = _categoryFeaturedBrandRepository.Table
                .Where(f => f.CategoryId == categoryId)
                .Select(f => f.Id)
                .ToList();

            for (int i = 0; i < list.Count; i++)
            {
                _categoryFeaturedBrandRepository.Delete(list[i]);
            }
        }

        public void DeleteCategoryFeaturedItem(int? categoryFeaturedItemId = null, int? productId = null, int? categoryId = null, int? featuredItemType = null)
        {
            var query = _categoryFeaturedItemRepository.TableNoTracking;

            if (categoryFeaturedItemId.HasValue)
                query = query.Where(x => x.Id == categoryFeaturedItemId.Value);

            if (productId.HasValue)
                query = query.Where(x => x.ProductId == productId.Value);

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);

            if (featuredItemType.HasValue)
                query = query.Where(x => x.FeaturedItemType == featuredItemType.Value);

            var items = query.ToList();

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    _categoryFeaturedItemRepository.Delete(item.Id);
                }
            }
        }

        #endregion

        #region Update

        public void UpdateCategoryFeaturedItemForPriority(int productId, int categoryId, int featuredItemType, int priority)
        {
            var item = _categoryFeaturedItemRepository.TableNoTracking
                .Where(x => x.ProductId == productId)
                .Where(x => x.CategoryId == categoryId)
                .Where(x => x.FeaturedItemType == featuredItemType)
                .FirstOrDefault();

            if (item != null)
            {
                item.Priority = priority;
                _categoryFeaturedItemRepository.Update(item);
            }
        }

        public void UpdateCategoryFeaturedBrandForPriorityById(int categoryFeaturedBrandId, int priority)
        {
            var item = _categoryFeaturedBrandRepository.Return(categoryFeaturedBrandId);
            if (item != null)
            {
                item.Priority = priority;
                _categoryFeaturedBrandRepository.Update(item);
            }
        }

        public void UpdateCategoryFilterType(string type, int categoryFilterId)
        {
            var item = _categoryFilterRepository.Return(categoryFilterId);
            if (item != null)
            {
                item.Type = type;
                _categoryFilterRepository.Update(item);
            }
        }

        public void UpdateCategory(Category category)
        {
            _categoryRepository.Update(category);
        }

        public void UpdateCategoryWhatsNew(CategoryWhatsNew whatsNew)
        {
            _categoryWhatsNewRepository.Update(whatsNew);
        }

        public void UpdateCategoryLargeBannerForDisplayOrder(int categoryLargeBannerMappingId, int displayOrder)
        {
            var item = _categoryLargeBannerMappingRepository.Return(categoryLargeBannerMappingId);
            
            if (item != null)
            {
                item.DisplayOrder = displayOrder;
                _categoryLargeBannerMappingRepository.Update(item);
            }
        }

        #endregion

        #region Command
        
        public void ProcessNewGoogleTaxonomy(DataTable dt)
        {
            // Clean up first
            _dbContext.ExecuteSqlCommand("TRUNCATE TABLE GoogleTaxonomy;");
            _dbContext.ExecuteSqlCommand("UPDATE Products SET GoogleTaxonomyId = 0;");

            var query = dt.AsEnumerable();
            var parents = query
                .Where(x => string.IsNullOrEmpty(x[1].ToString()) == false)
                .Select(x => x[1].ToString()).Distinct().ToList();

            CreateGoogleTaxonomy(query, parents, 0, 1);
        }

        public void ProcessCategoryAssignmentForProduct(int categoryId, int productId)
        {
            // Remove category if any
            var list = _productCategoryRepository.Table
                .Where(pc => pc.CategoryId == categoryId && pc.ProductId == productId).Select(pc => pc.Id).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                _productCategoryRepository.Delete(list[i]);
            }

            // Insert category
            var productCategory = new ProductCategory
            {
                CategoryId = categoryId,
                ProductId = productId
            };

            _productCategoryRepository.Create(productCategory);
        }

        public void ProcessCategoryAssignmentForLargeBanner(int categoryId, int largeBannerId)
        {
            // Remove mapping if any
            var list = _categoryLargeBannerMappingRepository.Table
                .Where(m => m.CategoryId == categoryId)
                .Where(m => m.LargeBannerId == largeBannerId)
                .Select(m => m.Id)                
                .ToList();

            for (int i = 0; i < list.Count; i++)
            {
                _categoryLargeBannerMappingRepository.Delete(list[i]);
            }

            // Insert mapping
            var mapping = new CategoryLargeBannerMapping
            {
                CategoryId = categoryId,
                LargeBannerId = largeBannerId
            };

            _categoryLargeBannerMappingRepository.Create(mapping);
        }

        public void ProcessCategoryRemoval(int categoryId)
        {
            var category = _categoryRepository.Return(categoryId);
            category = BuildCategory(category);

            for (int i = 0; i < category.ChildrenId.Length; i++)
            {
                // Loop thru the children
                ProcessCategoryRemoval(category.ChildrenId[i]);
            }

            // Remove featured items from this category
            DeleteCategoryFeaturedItem(categoryId);

            // Remove featured brands from this category
            DeleteCategoryFeaturedBrandByCategoryId(categoryId);

            // Remove products from this category
            DeleteProductCategoryByCategoryId(categoryId);

            // Remove category from database
            _categoryRepository.Delete(categoryId);

            // Remove media from this category
            for (int i = 0; i < category.CategoryMedias.Count; i++)
                _categoryMediaRepository.Delete(category.CategoryMedias[i].Id);
        }

        public string ProcessCategoryFeaturedItemInsertion(CategoryFeaturedItem featuredItem)
        {
            var count = _categoryFeaturedItemRepository.TableNoTracking
                .Where(x => x.CategoryId == featuredItem.CategoryId)
                .Where(x => x.ProductId == featuredItem.ProductId)
                .Where(x => x.FeaturedItemType == featuredItem.FeaturedItemType)
                .Select(x => x.Id)
                .Count();

            if (count == 0)
            {
                _categoryFeaturedItemRepository.Create(featuredItem);
                return string.Empty;
            }

            return "Item is already assigned.";
        }

        public void ProcessProductCategoryByCategoryIdAndProductIdListInsertion(int categoryId, IList<int> productIds)
        {
            for (int i = 0; i < productIds.Count; i++)
            {
                int productId = productIds[i];
                // Search if there is any existing item
                int count = _productCategoryRepository.Table
                    .Where(x => x.ProductId == productId)
                    .Where(x => x.CategoryId == categoryId)
                    .Select(x => x.Id)                    
                    .Count();

                if (count == 0)
                {
                    var item = new ProductCategory()
                    {
                        CategoryId = categoryId,
                        ProductId = productId
                    };

                    _productCategoryRepository.Create(item);
                }
            }
        }

        public void ProcessProductCategoryFilterInsertion(int categoryFilterId, IList<int> productIds)
        {
            for (int i = 0; i < productIds.Count; i++)
            {
                var item = new ProductCategoryFilter()
                {
                    CategoryFilterId = categoryFilterId,
                    ProductId = productIds[i]
                };

                _productCategoryFilterRepository.Create(item);
            }
        }

        public void ProcessCategoryFilterRemoval(int categoryFilterId)
        {
            var items = _productCategoryFilterRepository.Table.Where(x => x.CategoryFilterId == categoryFilterId).ToList();

            if (items.Count > 0)
            {
                for(int i = 0; i < items.Count; i++)
                {
                    _productCategoryFilterRepository.Delete(items[i].Id);
                }
            }

            _categoryFilterRepository.Delete(categoryFilterId);
        }

        public bool ProductHasThisCategory(int productId, int categoryId = 0, int categoryFilterId = 0)
        {
            if (productId <= 0) throw new ApolloException("Product ID should be a positive integer.");
            if (categoryId <= 0 && categoryFilterId <= 0) return false;

            var found = false;
            var filterCount = 0;
            var categoryCount = 0;

            if (categoryFilterId > 0)
            {
                filterCount = _productCategoryFilterRepository.TableNoTracking
                    .Where(x => x.ProductId == productId)
                    .Where(x => x.CategoryFilterId == categoryFilterId)
                    .Select(x => x.Id)
                    .Count();

                found = filterCount > 0;
            }

            if (categoryId > 0)
            {
                categoryCount = _productCategoryRepository.TableNoTracking
                    .Where(x => x.ProductId == productId)
                    .Where(x => x.CategoryId == categoryId)
                    .Select(x => x.Id)
                    .Count();

                found = categoryCount > 0;
            }

            if (categoryId > 0 && categoryFilterId > 0)
                found = (filterCount > 0) && (categoryCount > 0);

            return found;
        }

        #endregion

        #region Private methods

        private void CreateGoogleTaxonomy(IEnumerable<DataRow> query, List<string> parents, int parentId, int columnIndex)
        {
            foreach (var item in parents)
            {
                // Create row
                var id = _googleTaxonomyRepository.Create(new GoogleTaxonomy { Name = HttpUtility.HtmlEncode(item), ParentId = parentId });

                // Get rows under this item
                var nextLevelItems = query
                   .Where(x => x.ItemArray.Length > (columnIndex + 1))
                   .Where(x => x[columnIndex].ToString() == item)
                   .Where(x => string.IsNullOrEmpty(x[columnIndex + 1].ToString()) == false)
                   .Select(x => x[columnIndex + 1].ToString())
                   .Distinct().ToList();

                // If no rows found, skip
                if (nextLevelItems.Count <= 0) continue;

                CreateGoogleTaxonomy(query, nextLevelItems, id, columnIndex + 1);
            }
        }

        private Category BuildCategory(Category category)
        {
            // Get pictures
            category.CategoryMedias = _categoryMediaRepository.Table.Where(cm => cm.CategoryId == category.Id).ToList();

            // Get fetured brands and items
            category.FeaturedBrands = _categoryFeaturedBrandRepository.Table.Where(fb => fb.CategoryId == category.Id).ToList();
            category.FeaturedItems = _categoryFeaturedItemRepository.TableNoTracking.Where(fi => fi.CategoryId == category.Id).ToList();

            // Get category filters
            category.CategoryFilters = _categoryFilterRepository.Table.Where(cf => cf.CategoryId == category.Id).ToList();

            // Get category what's new items
            category.CategoryWhatsNews = _categoryWhatsNewRepository.Table.Where(cw => cw.CategoryId == category.Id).ToList();
            
            // Get children
            category.ChildrenId = _categoryRepository.Table
                .Where(c => c.ParentId == category.Id)                
                .Select(c => c.Id)                
                .ToArray();

            // Get banners
            category.Banners = GetLargeBannerMappingsByCategory(category.Id);

            return category;
        }

        private IList<CategoryLargeBannerMapping> GetLargeBannerMappingsByCategory(int categoryId, bool? enabled = null)
        {
            var mappings = _categoryLargeBannerMappingRepository.Table
                .Where(x => x.CategoryId == categoryId)
                .OrderByDescending(x => x.DisplayOrder)
                .ToList();

            if (mappings.Count > 0)
            {
                foreach (var mapping in mappings)
                {
                    mapping.LargeBanner = _largeBannerRepository.Return(mapping.LargeBannerId);
                }

                if (enabled.HasValue)
                {
                    mappings = mappings.Where(x => x.LargeBanner.Enabled == enabled.Value).ToList();
                }
            }

            return mappings;
        }

        private IList<LargeBanner> GetLargeBannersByCategory(int categoryId, bool? enabled = null)
        {
            var mappings = GetLargeBannerMappingsByCategory(categoryId, enabled);
            
            if (mappings.Count > 0)
            {
                return mappings.Select(x => x.LargeBanner).ToList();
            }

            return new List<LargeBanner>();
        }

        private IList<int> BuildTree(int categoryId, IList<int> tree)
        {
            var category = _categoryRepository.TableNoTracking
                .Where(c => c.Id == categoryId)
                .Select(c => new { Id = c.Id, ParentId = c.ParentId.Value })
                .FirstOrDefault();

            if (category == null) return tree;

            tree.Insert(0, category.Id);
            return BuildTree(category.ParentId, tree);
        }
        
        #endregion
    }
}
