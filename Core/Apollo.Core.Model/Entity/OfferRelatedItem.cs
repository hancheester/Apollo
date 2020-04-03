using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class OfferRelatedItem : BaseEntity
    {
        [DataMember]
        public int OfferRuleId { get; set; }
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public string ProductName { get; set; }
    }
}
