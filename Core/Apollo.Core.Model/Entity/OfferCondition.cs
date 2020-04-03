using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class OfferCondition : BaseEntity
    {
        [DataMember]
        public int OfferRuleId { get; set; }
        [DataMember]
        public int OfferActionId { get; set; }
        [DataMember]
        public int ParentId { get; set; }
        [DataMember]
        public bool? IsAny { get; set; }
        [DataMember]
        public bool? IsAll { get; set; }
        [DataMember]
        public bool? Matched { get; set; }
        [DataMember]
        public int? OfferAttributeId { get; set; }
        [DataMember]
        public int? OfferOperatorId { get; set; }
        [DataMember]
        public string Operand { get; set; }
        [DataMember]
        public bool? IsTotalQty { get; set; }
        [DataMember]
        public bool? IsTotalAmount { get; set; }
        [DataMember]
        public bool? ItemFound { get; set; }
        [DataMember]
        public OfferConditionType Type { get; set; }
        [DataMember]
        public List<OfferCondition> ChildOfferConditions { get; set; }
        [DataMember]
        public OfferAttribute OfferAttribute { get; set; }
        [DataMember]
        public OfferOperator OfferOperator { get; set; }
    }
}
