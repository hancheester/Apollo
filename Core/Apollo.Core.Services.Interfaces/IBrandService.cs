using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface IBrandService
    {
        int InsertBrand(Brand brand);
        int InsertBrandMedia(BrandMedia brandMedia);        
        int InsertBrandCategory(BrandCategory brandCategory);
        int InsertBrandFeaturedItem(BrandFeaturedItem brandFeaturedItem);

        IList<Brand> GetActiveBrands();
        IList<int> GetBrandCategoryIdList(
            int brandId,
            string topUrlKey = null,
            string secondUrlKey = null,
            string thirdUrlKey = null);
        Brand GetBrandById(int brandId);
        Brand GetBrandByName(string name);
        IList<Brand> GetBrandList();        
        IList<int> GetBrandCategoryTreeList(int brandCategoryId);        
        BrandCategory GetBrandCategoryByUrlKey(string urlKey);
        BrandCategory GetBrandCategoryById(int id);
        IList<BrandCategory> GetBrandCategoriesByBrandParent(int brandId, int parentId);
        IList<BrandCategory> GetBrandCategoriesByParentId(int parentId);
        IList<Brand> GetActiveBrandsByFirstLetter(string letter);
        IList<BrandCategoryOverviewModel> GetActiveBrandCategoryOverviewModelTree(int brandId);
        IList<Tuple<int, string, int>> GetBrandRangeByCategory(IList<int> categoryIds);        
        PagedList<BrandOverviewModel> GetPagedBrandOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> brandIds = null,
            string name = null,
            bool? isCategoryFeaturedBrand = null,
            int? categoryId = null,
            bool? hideDisabled = null,
            BrandSortingType orderBy = BrandSortingType.IdAsc);
        Brand GetBrandByUrlKey(string urlKey);
        BrandMedia GetBrandMediaById(int id);

        void UpdateProductWithBrand(int brandId, IList<int> productList);
        void UpdateBrand(Brand brand);
        void UpdateBrandMedia(BrandMedia brandMedia);
        void UpdateBrandCategory(BrandCategory brandCategory);
        void UpdateProductWithBrandCategory(int brandCategoryId, IList<int> productList);
        void UpdateBrandFeaturedItemForPriority(int productId, int brandId, int featuredItemType, int priority);
        
        void DeleteBrandMedia(int id);
        void DeleteBrand(int id);
        void DeleteFeaturedBrandListInCategory(int categoryId, IList<int> brandList);
        void DeleteBrandFeaturedItem(int? brandFeaturedItemId = null, int? productId = null, int? brandId = null, int? featuredItemType = null);

        void ProcessNewBrandFeaturedInCategory(int categoryId, IList<int> brandIdList);
        void ProcessBrandCategoryRemoval(int id);
        string ProcessBrandFeaturedItemInsertion(BrandFeaturedItem featuredItem);
    }
}
