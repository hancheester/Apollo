using Apollo.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Brand
{
    public class BrandCategoryModel : BaseEntityModel
    {
        public string Name { get; set; }
        public string UrlKey { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public List<BrandCategoryModel> Children { get; set; }

        public BrandCategoryModel()
        {
            Children = new List<BrandCategoryModel>();
        }
    }
}