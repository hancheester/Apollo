using Apollo.Core.Model.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.OverviewModel
{
    [Serializable]
    [DataContract]
    public class ProductOverviewModel : ICacheExpirySupported
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string H1Title { get; set; }
        [DataMember]
        public string UrlKey { get; set; }
        [DataMember]
        public string ShortDescription { get; set; }
        [DataMember]
        public string FullDescription { get; set; }
        [DataMember]
        public string GridMediaFilename { get; set; }
        [DataMember]
        public string ThumbMediaFilename { get; set; }
        [DataMember]
        public IList<ProductMedia> Images { get; set; }
        [DataMember]
        public string DefaultOption { get; set; }
        [DataMember]
        public string Options { get; set; }
        /// <summary>
        /// Format: PriceRange[0] - min price, PriceRange[1] - max price
        /// </summary>
        [DataMember]
        public decimal[] PriceExclTaxRange { get; set; }
        [DataMember]
        public decimal[] PriceInclTaxRange { get; set; }
        [DataMember]
        public string DeliveryTimeLine { get; set; }
        [DataMember]
        public int AverageReviewRating { get; set; }
        [DataMember]
        public int ReviewCount { get; set; }
        [DataMember]
        public int StepQuantity { get; set; }
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public int BrandCategoryId { get; set; }
        [DataMember]
        public bool IsPharmaceutical { get; set; }
        [DataMember]
        public bool Discontinued { get; set; }
        [DataMember]
        public bool ProductEnforcedStockCount { get; set; }
        [DataMember]
        public bool BrandEnforcedStockCount { get; set; }
        [DataMember]
        public bool OpenForOffer { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public OptionType OptionType { get; set; }
        [DataMember]
        public IList<int> AssignedCategoryIds { get; set; }
        [DataMember]
        public string ProductMark { get; set; }
        [DataMember]
        public int ProductMarkType { get; set; }
        [DataMember]
        public DateTime? ProductMarkExpiryDate { get; set; }
        [DataMember]
        public bool StockAvailability { get; set; }
        [DataMember]
        public bool ShowPreOrderButton { get; set; }
        [DataMember]
        public bool IsPhoneOrder { get; set; }
        [DataMember]
        public DateTime? CacheExpiryDate { get; set; }
        [DataMember]
        public int TaxCategoryId { get; set; }
        [DataMember]
        public bool VisibleIndividually { get; set; }
        [DataMember]
        public IList<OfferRuleOverviewModel> RelatedOffers { get; set; }
        [DataMember]
        public string MetaTitle { get; set; }
        [DataMember]
        public string MetaDescription { get; set; }
        [DataMember]
        public string MetaKeywords { get; set; }
        [DataMember]
        public decimal DisplayRank { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public decimal Popularity { get; set; }
        [DataMember]
        public int ApolloRating { get; set; }
    }
}