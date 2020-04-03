using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class CategoryFilter : BaseEntity
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string Type { get; set; }        
    }
}
