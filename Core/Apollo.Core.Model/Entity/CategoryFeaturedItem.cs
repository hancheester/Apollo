using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class CategoryFeaturedItem : BaseEntity
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public int FeaturedItemType { get; set; }
    }
}
