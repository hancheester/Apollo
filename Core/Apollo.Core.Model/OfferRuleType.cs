using System.Runtime.Serialization;

namespace Apollo.Core.Model
{
    [DataContract]
    public enum OfferRuleType
    {
        [EnumMember]
        Catalog,
        [EnumMember]
        Cart
    }
}
