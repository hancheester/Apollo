using Apollo.Core.Model.OverviewModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class Product : BaseEntity, ICacheExpirySupported
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public int DeliveryId { get; set; }
        [DataMember]
        public int BrandCategoryId { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public bool VisibleIndividually { get; set; }
        [DataMember]
        public bool IsPharmaceutical { get; set; }
        [DataMember]
        public bool HasFreeWrapping { get; set; }
        [DataMember]
        public string H1Title { get; set; }
        [DataMember]
        public string UrlRewrite { get; set; }
        [DataMember]
        public bool OpenForOffer { get; set; }
        [DataMember]
        public bool Discontinued { get; set; }
        [DataMember]
        public bool EnforceStockCount { get; set; }
        [DataMember]
        public bool IsGoogleProductSearchDisabled { get; set; }
        [DataMember]
        public string ProductCode { get; set; }
        [DataMember]
        public bool ShowPreOrderButton { get; set; }
        [DataMember]
        public int StepQuantity { get; set; }
        [DataMember]
        public int Rating { get; set; }
        [DataMember]
        public string MetaTitle { get; set; }
        [DataMember]
        public string MetaDescription { get; set; }
        [DataMember]
        public string MetaKeywords { get; set; }
        [DataMember]
        public string SecondaryKeywords { get; set; }
        [DataMember]
        public string ProductMark { get; set; }
        [DataMember]
        public int ProductMarkType { get; set; }
        [DataMember]
        public DateTime? ProductMarkExpiryDate { get; set; }
        [DataMember]
        public int OptionType { get; set; }
        [DataMember]
        public bool IsPhoneOrder { get; set; }
        [DataMember]
        public DateTime CreatedOnDate { get; set; }
        [DataMember]
        public DateTime UpdatedOnDate { get; set; }
        [DataMember]
        public DateTime? CacheExpiryDate { get; set; }
        [DataMember]
        public int GoogleTaxonomyId { get; set; }

        [DataMember]        
        public Brand Brand { get; set; }
        [DataMember]
        public Delivery Delivery { get; set; }
        [DataMember]
        public TaxCategory TaxCategory { get; set; }
        [DataMember]
        public IList<RestrictedGroup> RestrictedGroups { get; set; }
        [DataMember]
        public IList<ProductMedia> ProductMedias { get; set; }
        [DataMember]
        public IList<ProductPrice> ProductPrices { get; set; }
        [DataMember]
        public IList<ProductTag> ProductTags { get; set; }
        [DataMember]
        public IList<ProductReview> ProductReviews { get; set; }
        [DataMember]
        public ProductGoogleCustomLabelGroupMapping ProductGoogleCustomLabelGroup { get; set; }
        [DataMember]
        public OfferRuleOverviewModel RelatedCatalogOffer { get; set; }
        
        public Product()
        {
            Name = string.Empty;
            Description = string.Empty;
            UrlRewrite = string.Empty;
            H1Title = string.Empty;
            MetaTitle = string.Empty;
            ProductCode = string.Empty;

            ProductMedias = new List<ProductMedia>();
            ProductPrices = new List<ProductPrice>();
            ProductTags = new List<ProductTag>();
            RestrictedGroups = new List<RestrictedGroup>();
            Brand = new Brand();
            Delivery = new Delivery();
            CreatedOnDate = DateTime.Now;
            UpdatedOnDate = DateTime.Now;            
        }
    }
}
