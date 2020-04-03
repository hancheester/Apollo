using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class BrandFeaturedItem : BaseEntity
    {
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public int FeaturedItemType { get; set; }
    }
}
