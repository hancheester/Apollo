using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class OfferRule : BaseEntity, ICacheExpirySupported, IEquatable<OfferRule>
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool ProceedForNext { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public string PromoCode { get; set; }
        [DataMember]
        public int UsesPerCustomer { get; set; }
        [DataMember]
        public bool IsCart { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public string HtmlMessage { get; set; }
        [DataMember]
        public bool OfferedItemIncluded { get; set; }
        [DataMember]
        public string Alias { get; set; }
        [DataMember]
        public bool ShowOfferTag { get; set; }
        [DataMember]
        public bool ShowRRP { get; set; }
        [DataMember]
        public string ShortDescription { get; set; }
        [DataMember]
        public string LongDescription { get; set; }
        [DataMember]
        public string SmallImage { get; set; }
        [DataMember]
        public string LargeImage { get; set; }
        [DataMember]
        public string UrlRewrite { get; set; }
        [DataMember]
        public bool ShowInOfferPage { get; set; }
        [DataMember]
        public string OfferUrl { get; set; }
        [DataMember]
        public bool PointSpendable { get; set; }
        [DataMember]
        public bool UseInitialPrice { get; set; }
        [DataMember]
        public bool NewCustomerOnly { get; set; }
        [DataMember]
        public bool ShowCountDown { get; set; }
        [DataMember]
        public string RelatedBrands { get; set; }
        [DataMember]
        public string RelatedCategory { get; set; }
        [DataMember]
        public string RelatedProducts { get; set; }
        [DataMember]
        public bool DisplayOnProductPage { get; set; }
        [DataMember]
        public bool DisplayOnHeaderStrip { get; set; }
        [DataMember]
        public int OfferTypeId { get; set; }
        [DataMember]
        public string OfferLabel { get; set; }
        [DataMember]
        public bool DisableOfferLabel { get; set; }

        [DataMember]
        public OfferCondition Condition { get; set; }
        [DataMember]
        public OfferAction Action { get; set; }
        [DataMember]
        public IList<OfferRelatedItem> RelatedItems { get; set; }
        [DataMember]
        public DateTime? CacheExpiryDate
        {
            get { return this.EndDate; }
            set { }
        }

        public OfferRule()
        {
            StartDate = null;
            EndDate = null;
            Name = string.Empty;
            PromoCode = string.Empty;
            HtmlMessage = string.Empty;
            Alias = string.Empty;
            ShortDescription = string.Empty;
            LongDescription = string.Empty;
            SmallImage = string.Empty;
            LargeImage = string.Empty;
            UrlRewrite = string.Empty;
            OfferUrl = string.Empty;
            RelatedBrands = string.Empty;
            RelatedCategory = string.Empty;
            RelatedProducts = string.Empty;
        }

        public bool Equals(OfferRule other)
        {
            return other.Id == this.Id;
        }
    }
}
