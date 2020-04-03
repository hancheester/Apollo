using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class Colour : BaseEntity
    {
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string ColourFilename { get; set; }
        [DataMember]
        public string ThumbnailFilename { get; set; }

        [DataMember]
        public string BrandName { get; set; }
    }
}
