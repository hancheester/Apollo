using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class OfferAction : BaseEntity
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int OfferRuleId { get; set; }
        [DataMember]
        public int OfferActionAttributeId { get; set; }
        [DataMember]
        public decimal? DiscountAmount  { get; set; }
        [DataMember]
        public bool? FreeProductItself  { get; set; }
        [DataMember]
        public int? FreeProductId { get; set; }
        [DataMember]
        public int? FreeProductPriceId { get; set; }
        [DataMember]
        public int? FreeProductQty { get; set; }
        [DataMember]
        public int? OptionOperatorId { get; set; }
        [DataMember]
        public string OptionOperand { get; set; }
        [DataMember]
        public int? DiscountQtyStep { get; set; }
        [DataMember]
        public decimal? MinimumAmount { get; set; }
        [DataMember]
        public bool FreeShipping { get; set; }
        [DataMember]
        public int RewardPoint { get; set; }
        [DataMember]
        public OfferOperator OptionOperator { get; set; }
        [DataMember]
        public OfferCondition Condition { get; set; }
        [DataMember]
        public int? XValue { get; set; }
        [DataMember]
        public decimal? YValue { get; set; }
        [DataMember]
        public bool? IsCart { get; set; }
        [DataMember]
        public bool? IsCatalog { get; set; }        
    }
}
