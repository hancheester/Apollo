using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class BrandMedia : BaseEntity
    {
        [DataMember]
        public int BrandId { get; set; }
        [DataMember]
        public string MediaFilename { get; set; }
        [DataMember]
        public string Alt { get; set; }
        [DataMember]
        public string Link { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
    }
}
