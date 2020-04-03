using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class ProductTag : BaseEntity
    {
        [DataMember]
        public int TagId { get; set; }
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public Tag Tag { get; set; }
    }
}
