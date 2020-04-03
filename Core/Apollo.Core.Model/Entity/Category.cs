using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class Category : BaseEntity
    {
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string UrlRewrite { get; set; }
        [DataMember]
        public int TreeLevel { get; set; }
        [DataMember]
        public string ColourScheme { get; set; }
        [DataMember]
        public bool Visible { get; set; }
        [DataMember]
        public int Priority { get; set; }        
        [DataMember]
        public int CategoryTemplateId { get; set; }        
        [DataMember]
        public int? ParentId { get; set; }
        [DataMember]
        public string MetaTitle { get; set; }
        [DataMember]
        public string MetaKeywords { get; set; }
        [DataMember]
        public string MetaDescription { get; set; }
        [DataMember]
        public string SecondaryKeywords { get; set; }
        [DataMember]
        public string H1Title { get; set; }
        [DataMember]
        public string ShortDescription { get; set; }
        [DataMember]
        public string ThumbnailFilename { get; set; }
        [DataMember]
        public int[] ChildrenId { get; set; }
        [DataMember]
        public IList<CategoryMedia> CategoryMedias { get; set; }
        [DataMember]
        public IList<CategoryFeaturedBrand> FeaturedBrands { get; set; }
        [DataMember]
        public IList<CategoryFeaturedItem> FeaturedItems { get; set; }
        [DataMember]
        public IList<CategoryFilter> CategoryFilters { get; set; }
        [DataMember]
        public IList<CategoryWhatsNew> CategoryWhatsNews { get; set; }
        [DataMember]
        public IList<CategoryLargeBannerMapping> Banners { get; set; }

        public Category()
        {
            ChildrenId = new int[0];
            CategoryMedias = new List<CategoryMedia>();
            FeaturedBrands = new List<CategoryFeaturedBrand>();
            FeaturedItems = new List<CategoryFeaturedItem>();
            CategoryFilters = new List<CategoryFilter>();
            CategoryWhatsNews = new List<CategoryWhatsNew>();
            Banners = new List<CategoryLargeBannerMapping>();
        }
    }
}
