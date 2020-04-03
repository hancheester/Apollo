using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Apollo.Core.Services.Interfaces
{
    public interface ICategoryService
    {
        IList<Tuple<int, string, int>> GetCategoryFiltersRangeByCategory(IList<int> categoryIds);
        IList<CategoryOverviewModel> GetCategoryOverviewModelForMenu();
        CategoryTemplate GetCategoryTemplateById(int categoryTemplateId);
        CategoryOverviewModel GetActiveCategoryOverviewModel(string topUrlKey, string secondUrlKey = "", string thirdUrlKey = "");
        IList<GoogleTaxonomy> GetGoogleTaxonomyTreeByParentId(int parentId);
        IList<Category> GetCategoryByParent(int parentId);
        void DeleteCategoryFromProduct(int categoryId, int productId);
        void DeleteCategoryFromLargeBanner(int categoryId, int largeBannerId);
        void DeleteCategoryFilterByProductIdList(int categoryFilterId, IList<int> productIds);
        void DeleteCategoryMedia(int categoryMediaId);
        Category GetCategoryByUrlKey(string urlKey);
        CategoryFilter GetCategoryFilterById(int categoryFilterId);
        GoogleTaxonomy GetGoogleTaxonomyById(int googleTaxonomyId);
        IList<int> GetGoogleTaxonomyTreeList(int googleTaxonomyId);
        int GetCategoryTreeLevel(int categoryId);
        Category GetActiveCategory(int categoryId);
        int InsertCategory(Category category);
        int InsertCategoryFilter(CategoryFilter categoryFilter);
        int InsertCategoryMedia(CategoryMedia media);
        void DeleteCategoryWhatsNew(int categoryWhatsNewId);
        int InsertCategoryWhatsNew(CategoryWhatsNew item);
        void ProcessNewGoogleTaxonomy(DataTable dt);
        void ProcessCategoryAssignmentForProduct(int categoryId, int productId);
        void ProcessCategoryAssignmentForLargeBanner(int categoryId, int largeBannerId);
        string ProcessCategoryFeaturedItemInsertion(CategoryFeaturedItem featuredItem);
        void ProcessCategoryFilterRemoval(int categoryFilterId);
        void ProcessProductCategoryFilterInsertion(int categoryFilterId, IList<int> productIds);
        bool ProductHasThisCategory(int productId, int categoryId = 0, int categoryFilterId = 0);
        void ProcessProductCategoryByCategoryIdAndProductIdListInsertion(int categoryId, IList<int> productIds);
        void DeleteCategoryFromProductIdList(int categoryId, IList<int> productIds);
        void UpdateCategoryWhatsNew(CategoryWhatsNew whatsNew);
        void UpdateCategory(Category category);
        void UpdateCategoryFeaturedBrandForPriorityById(int categoryFeaturedBrandId, int priority);
        void UpdateCategoryLargeBannerForDisplayOrder(int categoryLargeBannerMappingId, int displayOrder);
        void DeleteCategoryFeaturedItem(int? categoryFeaturedItemId = null, int? productId = null, int? categoryId = null, int? featuredItemType = null);
        void UpdateCategoryFeaturedItemForPriority(int productId, int categoryId, int featuredItemType, int priority);
        void UpdateCategoryFilterType(string type, int categoryFilterId);
        void ProcessCategoryRemoval(int categoryId);
        IList<CategoryMedia> GetCategoryMediasByCategoryId(int categoryId);
        IList<CategoryTemplate> GetAllCategoryTemplates();
        IList<int> GetTreeList(int categoryId);
        Category GetCategory(int categoryId);
        CategoryWhatsNew GetCategoryWhatsNewById(int categoryWhatsNewById);
        Category GetFirstActiveCategoryByProductId(int productId);
        IList<Category> GetCategoriesByProductId(int productId);
        IList<Category> GetCategoriesByLargeBannerId(int largeBannerId);
        IList<int> GetCategoryIdListByProductId(int productId);
        int InsertCategoryFeaturedItem(CategoryFeaturedItem item);
        IList<KeyValuePair<int, string>> GetTreeListWithName(int categoryId);
        IList<KeyValuePair<int, string>> GetGoogleTaxonomyTreeListWithName(int googleTaxonomyId);
    }
}
