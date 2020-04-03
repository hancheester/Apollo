using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class CategoryMedia : BaseEntity
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string MediaFilename { get; set; }
    }
}
