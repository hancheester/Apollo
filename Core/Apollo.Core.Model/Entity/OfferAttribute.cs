using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class OfferAttribute : BaseEntity
    {
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool IsCatalog { get; set; }
        [DataMember]
        public bool IsCart { get; set; }

        public OfferAttribute()
        {
            Value = string.Empty;            
        }
    }
}
