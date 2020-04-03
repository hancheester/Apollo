using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class CategoryFeaturedBrand : BaseEntity
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public int BrandId { get; set; }
    }
}
