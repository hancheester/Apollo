using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class Brand : BaseEntity
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string FlashImage { get; set; }
        [DataMember]
        public int FlashImageWidth { get; set; }
        [DataMember]
        public int FlashImageHeight { get; set; }
        [DataMember]
        public string UrlRewrite { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string CssPrefix { get; set; }
        [DataMember]
        public bool HasMicrosite { get; set; }
        [DataMember]
        public string MetaDescription { get; set; }
        [DataMember]
        public string MetaTitle { get; set; }
        [DataMember]
        public string MetaKeywords { get; set; }
        [DataMember]
        public string SecondaryKeywords { get; set; }
        [DataMember]
        public int DeliveryId { get; set; }
        [DataMember]
        public bool EnforceStockCount { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public Delivery Delivery { get; set; }
        [DataMember]
        public IList<BrandMedia> BrandMedias { get; set; }
        [DataMember]
        public int MicrositeType { get; set; }
        [DataMember]
        public IList<BrandFeaturedItem> FeaturedItems { get; set; }
    }
}
