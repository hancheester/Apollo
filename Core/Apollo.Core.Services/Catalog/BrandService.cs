using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Catalog
{
    public class BrandService : BaseRepository, IBrandService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<BrandMedia> _brandMediaRepository;
        private readonly IRepository<BrandCategory> _brandCategoryRepository;        
        private readonly IRepository<Delivery> _deliveryRepository;
        private readonly IRepository<BrandFeaturedItem> _brandFeaturedItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategoryFilter> _categoryFilterRepository;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IRepository<CategoryFeaturedBrand> _categoryFeaturedBrandRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public BrandService(IDbContext dbContext,
                            IRepository<Brand> brandRepository,
                            IRepository<BrandMedia> brandMediaRepository,
                            IRepository<BrandCategory> brandCategoryRepository,                            
                            IRepository<Delivery> deliveryRepository,
                            IRepository<BrandFeaturedItem> brandFeaturedItemRepository,
                            IRepository<Product> productRepository,
                            IRepository<ProductCategory> productCategoryRepository,
                            IRepository<Category> categoryRepository,
                            IRepository<CategoryFilter> categoryFilterRepository,
                            IRepository<ProductPrice> productPriceRepository,
                            IRepository<CategoryFeaturedBrand> categoryFeaturedBrandRepository,
                            ILogBuilder logBuilder,
                            ICacheManager cacheManager) 
        {
            _dbContext = dbContext;
            _brandRepository = brandRepository;
            _brandMediaRepository = brandMediaRepository;
            _brandCategoryRepository = brandCategoryRepository;
            _deliveryRepository = deliveryRepository;
            _brandFeaturedItemRepository = brandFeaturedItemRepository;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _categoryRepository = categoryRepository;
            _categoryFilterRepository = categoryFilterRepository;
            _productPriceRepository = productPriceRepository;
            _categoryFeaturedBrandRepository = categoryFeaturedBrandRepository;            
            _cacheManager = cacheManager;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        #region Return

        public IList<Brand> GetActiveBrands()
        {
            string key = CacheKey.BRAND_ALL_ACTIVE_KEY;

            return _cacheManager.Get(key, delegate ()
            {
                return _brandRepository.Table
                    .Where(b => b.Enabled == true)
                    .OrderBy(b => b.Name).ToList();
            });
        }

        public IList<int> GetBrandCategoryIdList(
            int brandId,
            string topUrlKey = null,
            string secondUrlKey = null,
            string thirdUrlKey = null)
        {
            var topBrandCategory = GetBrandCategoryByUrlKey(topUrlKey);
            var secondBrandCategory = GetBrandCategoryByUrlKey(secondUrlKey);
            var thirdBrandCategory = GetBrandCategoryByUrlKey(thirdUrlKey);

            IList<int> list = new List<int>();

            if (thirdBrandCategory != null)
            {
                list.Add(thirdBrandCategory.Id);
                return list;
            }

            if (secondBrandCategory != null)
            {
                var tree = GetBrandCategoryOverviewModelTree(brandId, secondBrandCategory.Id);
                list = GenerateBrandCategoryIdList(tree);
                list.Add(secondBrandCategory.Id);
                return list;
            }

            if (topBrandCategory != null)
            {
                var tree = GetBrandCategoryOverviewModelTree(brandId, topBrandCategory.Id);
                list = GenerateBrandCategoryIdList(tree);
                list.Add(topBrandCategory.Id);
                return list;
            }

            return list;
        }

        public PagedList<BrandOverviewModel> GetPagedBrandOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> brandIds = null,
            string name = null,
            bool? isCategoryFeaturedBrand = null,
            int? categoryId = null,
            bool? hideDisabled = null,
            BrandSortingType orderBy = BrandSortingType.IdAsc)
        {
            var query = _brandRepository.Table;

            if (!string.IsNullOrEmpty(name))
                query = query.Where(b => b.Name.Contains(name));

            if (brandIds != null && brandIds.Count > 0)
                query = query.Where(b => brandIds.Contains(b.Id));

            if (hideDisabled.HasValue)            
                query = query.Where(b => b.Enabled == true);
            
            if (isCategoryFeaturedBrand.HasValue && categoryId.HasValue && categoryId.Value != 0)
            {
                var temp = query.GroupJoin(_categoryFeaturedBrandRepository.Table, b => b.Id, cfb => cfb.BrandId, (b, cfb) => new { b, cfb })
                    .SelectMany(b_cfb => b_cfb.cfb.DefaultIfEmpty(), (b_cfb, cfb) => new { b_cfb.b, cfb });

                if (isCategoryFeaturedBrand.Value == true)
                {
                    temp = temp.Where(b_cfb => b_cfb.cfb.CategoryId == categoryId.Value);
                }
                else
                {
                    temp = temp.Where(b_cfb => b_cfb.cfb.CategoryId != categoryId.Value);
                }

                query = temp.Select(b_cfb => b_cfb.b);
            }

            int totalRecords = query.Count();

            switch (orderBy)
            {
                case BrandSortingType.IdAsc:
                    query = query.OrderBy(b => b.Id);
                    break;
                case BrandSortingType.IdDesc:
                    query = query.OrderByDescending(b => b.Id);
                    break;
                case BrandSortingType.NameAsc:
                    query = query.OrderBy(b => b.Name);
                    break;
                case BrandSortingType.NameDesc:
                default:
                    query = query.OrderByDescending(b => b.Name);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var brands = query.ToList()
                .Select(b => new BrandOverviewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    UrlKey = b.UrlRewrite,
                    Enabled = b.Enabled,
                    AssignedCategoryIdForFeaturedBrand = _categoryFeaturedBrandRepository.Table.Where(c => c.BrandId == b.Id).Select(c => c.CategoryId).ToList()
                }
            );

            return new PagedList<BrandOverviewModel>(brands, pageIndex, pageSize, totalRecords);
        }
        
        public IList<Tuple<int, string, int>> GetBrandRangeByCategory(IList<int> categoryIds)
        {
            if (categoryIds == null) return new List<Tuple<int, string, int>>();

            var items = _productCategoryRepository.Table
                .Join(_productRepository.Table, pc => pc.ProductId, p => p.Id, (pc, p) => new { pc, p })
                .Join(_brandRepository.Table, pc_p => pc_p.p.BrandId, b => b.Id, (pc_p, b) => new { pc_p.pc, pc_p.p, b })
                .Where(pc_p_b => categoryIds.Contains(pc_p_b.pc.CategoryId))
                .Where(pc_p_b => pc_p_b.p.Enabled == true)
                .Where(pc_p_b => pc_p_b.p.Discontinued == false || (pc_p_b.p.Discontinued == true && _productPriceRepository.Table.Where(pp => pp.ProductId == pc_p_b.p.Id).Select(pp => pp.Stock).Max() > 0))
                .GroupBy(pc_p_b => new { pc_p_b.b.Id, pc_p_b.b.Name })                
                .ToList()
                .Select(a => new Tuple<int, string, int>(a.Key.Id, a.Key.Name, a.Select(x => x.p.Id).Distinct().Count()))
                .OrderBy(a => a.Item2)
                .ToList();

            return items;
        }

        public IList<Brand> GetActiveBrandsByFirstLetter(string letter)
        {
            string key = string.Format(CacheKey.BRAND_ACTIVE_BY_LETTER_KEY, letter);

            return _cacheManager.Get(key, delegate ()
            {
                return _brandRepository.Table
                    .Where(b => b.Name.StartsWith(letter) && b.Enabled == true)
                    .OrderBy(b => b.Name)
                    .ToList();
            });
        }

        public IList<BrandCategoryOverviewModel> GetActiveBrandCategoryOverviewModelTree(int brandId)
        {
            var categories = _brandCategoryRepository.Table
                .Where(bc => bc.BrandId == brandId)
                .Where(bc => bc.Visible == true)
                .ToList();
            return BuildBrandCategoryOverviewModelTree(categories);
        }

        public IList<BrandCategoryOverviewModel> GetBrandCategoryOverviewModelTree(int brandId, int parentId)
        {
            var categories = _brandCategoryRepository.Table
                .Where(bc => bc.BrandId == brandId)
                .ToList();

            return BuildBrandCategoryOverviewModelTree(categories, parentId);
        }

        public IList<BrandCategory> GetBrandCategoriesByBrandParent(int brandId, int parentId)
        {
            List<BrandCategory> list = _brandCategoryRepository.Table.Where(bc => bc.BrandId == brandId && bc.ParentId == parentId).ToList();
            return list;
        }

        public IList<BrandCategory> GetBrandCategoriesByParentId(int parentId)
        {
            List<BrandCategory> list = _brandCategoryRepository.Table.Where(b => b.ParentId == parentId).ToList();
            return list;
        }

        public IList<Brand> GetBrandListHavingMicrosite()
        {
            List<Brand> brands = _brandRepository.Table.Where(b => b.HasMicrosite == true).OrderBy(b => b.Name).ToList();

            return brands;
        }

        public IList<Brand> GetBrandList()
        {
            string key = CacheKey.BRAND_ALL_KEY;

            return _cacheManager.Get(key, delegate ()
            {
                return _brandRepository.Table.OrderBy(b => b.Name).ToList();
            });
        }
        
        public BrandCategory GetBrandCategoryById(int id)
        {
            var brandCategory = _brandCategoryRepository.Return(id);
            return brandCategory;
        }

        public BrandCategory GetBrandCategoryByUrlKey(string brandCategoryUrlKey)
        {
            BrandCategory brandCategory = _brandCategoryRepository.Table
                .FirstOrDefault(b => b.UrlRewrite == brandCategoryUrlKey);

            return brandCategory;
        }
        
        public Brand GetBrandById(int brandId)
        {
            string key = string.Format(CacheKey.BRAND_BY_ID_KEY, brandId);

            return _cacheManager.Get(key, delegate ()
            {
                Brand brand = _brandRepository.Return(brandId);
                if (brand != null)
                {
                    brand.BrandMedias = GetBrandMediaListByBrandId(brand.Id);
                    brand.Delivery = _deliveryRepository.Return(brand.DeliveryId);
                    brand.FeaturedItems = _brandFeaturedItemRepository.TableNoTracking.Where(x => x.BrandId == brandId).ToList();
                }

                return brand;
            });
        }

        public Brand GetBrandByUrlKey(string urlKey)
        {
            string key = string.Format(CacheKey.BRAND_BY_URL_KEY, urlKey);

            return _cacheManager.Get(key, delegate ()
            {
                var id = _brandRepository.Table.Where(b => b.UrlRewrite == urlKey).Select(b => b.Id).FirstOrDefault();
                if (id == 0) return null;
                return GetBrandById(id);
            });
        }

        public Brand GetBrandByName(string name)
        {
            return _brandRepository.Table.FirstOrDefault(b => b.Name.ToLower() == name.ToLower().Trim());
        }
        
        public BrandMedia GetBrandMediaById(int id)
        {
            return _brandMediaRepository.Return(id);
        }

        public int GetBrandCategoryCountByFilters(int? brandCategoryId, string name)
        {
            var BrandCategoryId = GetParameter("BrandCategoryId", brandCategoryId);
            var Name = GetParameter("Name", name);

            var returnValue = _dbContext.SqlQuery<int>("EXEC BrandCategory_GetCount @BrandCategoryId, @Name", BrandCategoryId, Name).ToList();
            int count = Convert.ToInt32(returnValue[0].ToString());

            return count;
        }

        public IList<BrandMedia> GetBrandMediaListByBrandId(int brandId)
        {
            return _brandMediaRepository.Table.Where(b => b.BrandId == brandId).ToList();
        }
        
        public IList<int> GetBrandCategoryTreeList(int brandCategoryId)
        {
            IList<int> treeList = new List<int>();
            treeList = BuildBrandCategory(brandCategoryId, treeList);
            
            return treeList;
        }
        
        #endregion

        #region Create
        
        public int InsertBrandMedia(BrandMedia media)
        {
            return _brandMediaRepository.Create(media);
        }

        public int InsertBrand(Brand brand)
        {
            var id = _brandRepository.Create(brand);

            _cacheManager.RemoveCacheStartsWith(CacheKey.BRAND_PATTERN_KEY);

            return id;
        }

        public int InsertFeaturedBrandInCategory(int brandId, int categoryId)
        {
            CategoryFeaturedBrand newRow = new CategoryFeaturedBrand();
            newRow.CategoryId = categoryId;
            newRow.BrandId = brandId;
            newRow.Priority = 0;
            int id = _categoryFeaturedBrandRepository.Create(newRow);

            return id;
        }

        public int InsertBrandCategory(BrandCategory brandCategory)
        {
            return _brandCategoryRepository.Create(brandCategory);
        }

        public int InsertBrandFeaturedItem(BrandFeaturedItem brandFeaturedItem)
        {
            return _brandFeaturedItemRepository.Create(brandFeaturedItem);
        }

        #endregion

        #region Update

        public void UpdateBrandCategory(BrandCategory category)
        {
            _brandCategoryRepository.Update(category);
        }

        public void UpdateBrand(Brand brand)
        {
            _brandRepository.Update(brand);

            _cacheManager.RemoveByPattern(CacheKey.BRAND_PATTERN_KEY);
        }

        public void UpdateBrandMedia(BrandMedia media)
        {
            _brandMediaRepository.Update(media);

            _cacheManager.RemoveByPattern(CacheKey.BRAND_PATTERN_KEY);
        }

        public void UpdateProductWithBrand(int brandId, IList<int> productIdList)
        {
            if (productIdList == null) throw new ArgumentNullException("productIdList");            
            if (productIdList.Count == 0) throw new ArgumentException("List is empty.", "productIdList");
            
            foreach (var productId in productIdList)
            {
                Product product = _productRepository.Return(productId);
                if (product != null) product.BrandId = brandId;

                _productRepository.Update(product);
            }
        }

        public void UpdateProductWithBrandCategory(int brandCategoryId, IList<int> productIdList)
        {
            if (productIdList == null) throw new ArgumentNullException("productIdList");
            if (productIdList.Count == 0) throw new ArgumentException("List is empty.", "productIdList");

            foreach (var productId in productIdList)
            {
                Product product = _productRepository.Return(productId);
                if (product != null) product.BrandCategoryId = brandCategoryId;

                _productRepository.Update(product);
            }
        }

        public void UpdateFeaturedBrandInCategory(int categoryFeaturedBrandId, int? brandId, int? categoryId, int priority)
        {
            if (brandId == null) throw new ArgumentNullException("brandId");
            if (categoryId == null) throw new ArgumentNullException("categoryId");

            var feature = _categoryFeaturedBrandRepository.Return(categoryFeaturedBrandId);
            if (feature != null)
            {
                feature.BrandId = brandId.Value;
                feature.CategoryId = categoryId.Value;
                feature.Priority = priority;

                _categoryFeaturedBrandRepository.Update(feature);
            }
        }

        public void UpdateBrandFeaturedItemForPriority(int productId, int brandId, int featuredItemType, int priority)
        {
            var item = _brandFeaturedItemRepository.TableNoTracking
                .Where(x => x.ProductId == productId)
                .Where(x => x.BrandId == brandId)
                .Where(x => x.FeaturedItemType == featuredItemType)
                .FirstOrDefault();

            if (item != null)
            {
                item.Priority = priority;
                _brandFeaturedItemRepository.Update(item);

                _cacheManager.RemoveByPattern(CacheKey.BRAND_PATTERN_KEY);
            }
        }

        #endregion

        #region Delete

        public void DeleteBrand(int brandId)
        {
            _brandRepository.Delete(brandId);
            string key = string.Format(CacheKey.BRAND_BY_ID_KEY, brandId);
            _cacheManager.Remove(key);
        }

        public void DeleteBrandMedia(int brandMediaId)
        {
            _brandMediaRepository.Delete(brandMediaId);
            _cacheManager.RemoveCacheStartsWith(CacheKey.BRAND_PATTERN_KEY);
        }

        public void DeleteFeaturedBrandListInCategory(int categoryId, IList<int> brandIdList)
        {
            for (int i = 0; i < brandIdList.Count; i++)
            {
                int brandId = brandIdList[i];
                CategoryFeaturedBrand fb = _categoryFeaturedBrandRepository.Table.FirstOrDefault(s => s.CategoryId == categoryId && s.BrandId == brandId);
                
                if (fb != null)
                {
                    _categoryFeaturedBrandRepository.Delete(fb);                    
                }
            }
        }
        
        public void DeleteBrandFeaturedItem(int? brandFeaturedItemId = null, int? productId = null, int? brandId = null, int? featuredItemType = null)
        {
            var query = _brandFeaturedItemRepository.TableNoTracking;

            if (brandFeaturedItemId.HasValue)
                query = query.Where(x => x.Id == brandFeaturedItemId.Value);

            if (productId.HasValue)
                query = query.Where(x => x.ProductId == productId.Value);

            if (brandId.HasValue)
                query = query.Where(x => x.BrandId == brandId.Value);

            if (featuredItemType.HasValue)
                query = query.Where(x => x.FeaturedItemType == featuredItemType.Value);

            var items = query.ToList();

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    _brandFeaturedItemRepository.Delete(item.Id);
                }

                _cacheManager.RemoveByPattern(CacheKey.BRAND_PATTERN_KEY);
            }
        }

        #endregion

        #region Command

        public void ProcessNewBrandFeaturedInCategory(int categoryId, IList<int> brandIdList)
        {
            DeleteFeaturedBrandListInCategory(categoryId, brandIdList);

            for (int i = 0; i < brandIdList.Count; i++)
            {            
                //Insert Brand Featured In Category
                InsertFeaturedBrandInCategory(brandIdList[i], categoryId);
            }
        }

        public void ProcessBrandCategoryRemoval(int brandCategoryId)
        {
            var products = _productRepository.Table.Where(x => x.BrandCategoryId == brandCategoryId).ToList();
            if (products.Count> 0)
            {
                for(int i = 0; i < products.Count; i++)
                {
                    products[i].BrandCategoryId = 0;
                    _productRepository.Update(products[i]);
                }
            }

            _brandCategoryRepository.Delete(brandCategoryId);
        }

        public string ProcessBrandFeaturedItemInsertion(BrandFeaturedItem featuredItem)
        {
            var count = _brandFeaturedItemRepository.TableNoTracking
                .Where(x => x.BrandId == featuredItem.BrandId)
                .Where(x => x.ProductId == featuredItem.ProductId)
                .Where(x => x.FeaturedItemType == featuredItem.FeaturedItemType)
                .Select(x => x.Id)
                .Count();

            if (count == 0)
            {
                _brandFeaturedItemRepository.Create(featuredItem);
                return string.Empty;
            }

            return "Item is already assigned.";
        }

        #endregion

        #region Private methods

        private BrandCategoryOverviewModel BuildBrandCategoryOverviewModel(BrandCategory brandCategory)
        {
            return new BrandCategoryOverviewModel
            {
                Id = brandCategory.Id,
                Name = brandCategory.Name,
                ParentId = brandCategory.ParentId,
                BrandId = brandCategory.BrandId,
                Description = brandCategory.Description,
                ImageUrl = brandCategory.ImageUrl,
                UrlKey = brandCategory.UrlRewrite
            };
        }

        private IList<BrandCategoryOverviewModel> BuildBrandCategoryOverviewModelTree(IList<BrandCategory> categories, int brandCategoryId = -1)
        {
            var models = new List<BrandCategoryOverviewModel>();
            var foundList = categories.Where(c => c.ParentId == brandCategoryId).ToList();
            if (foundList != null)
            {
                foreach (var item in foundList)
                {
                    var model = BuildBrandCategoryOverviewModel(item);
                    model.Children.AddRange(BuildBrandCategoryOverviewModelTree(categories, item.Id));
                    models.Add(model);
                }
            }

            return models;
        }

        private IList<int> GenerateBrandCategoryIdList(IList<BrandCategoryOverviewModel> models)
        {
            var list = new List<int>();
            foreach (var item in models)
            {
                list.Add(item.Id);
                list.AddRange(GenerateBrandCategoryIdList(item.Children));
            }

            return list;
        }

        private IList<int> BuildBrandCategory(int brandCategoryId, IList<int> tree)
        {
            var category = _brandCategoryRepository.TableNoTracking
                .Where(x => x.Id == brandCategoryId)
                .Select(x => new { Id = x.Id, ParentId = x.ParentId })
                .FirstOrDefault();

            if (category == null) return tree;

            tree.Insert(0, category.Id);
            return BuildBrandCategory(category.ParentId, tree);
        }

        #endregion
    }
}
