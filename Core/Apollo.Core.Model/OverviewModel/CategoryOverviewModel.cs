using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Model.OverviewModel
{
    public class CategoryOverviewModel : BaseOverviewModel
    {
        public string Name { get; set; }
        public string UrlKey { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string ThumbnailFilename { get; set; }
        public int CategoryTemplateId { get; set; }
        public string ColourScheme { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string H1Title { get; set; }
        public IList<CategoryOverviewModel> Children { get; set; }
        public IList<CategoryFeaturedItemOverviewModel> FeaturedItems { get; set; }
        public IList<int> FeatureItemTypes { get; set; }
        public IList<CategoryOverviewModel> FeaturedBrands { get; set; }
        public CategoryWhatsNew ActiveWhatsNew { get; set; }
        public IList<LargeBanner> Banners { get; set; }

        public CategoryOverviewModel()
        {
            Children = new List<CategoryOverviewModel>();
            FeaturedItems = new List<CategoryFeaturedItemOverviewModel>();
            FeaturedBrands = new List<CategoryOverviewModel>();
            ActiveWhatsNew = new CategoryWhatsNew();
            Banners = new List<LargeBanner>();
        }
    }
}
