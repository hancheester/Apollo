using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class ProductMedia : BaseEntity
    {
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public string MediaType { get; set; }
        [DataMember]
        public bool PrimaryImage { get; set; }
        [DataMember]
        public string ThumbnailFilename { get; set; }
        [DataMember]
        public string MediaFilename { get; set; }
        [DataMember]
        public string HighResFilename { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
    }
}
