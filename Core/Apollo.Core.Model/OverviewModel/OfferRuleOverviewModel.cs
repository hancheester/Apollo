using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.OverviewModel
{
    [Serializable]
    [DataContract]
    public class OfferRuleOverviewModel
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string UrlKey { get; set; }
        [DataMember]
        public string OfferTag { get; set; }
        [DataMember]        
        public OfferRuleType OfferRuleType { get; set; }
        [DataMember]
        public string ShortDescription { get; set; }
        [DataMember]
        public bool DisableOfferLabel { get; set; }
        [DataMember]
        public bool ShowInOfferPage { get; set; }
    }
}
