using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class CategoryWhatsNew : BaseEntity
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string HtmlContent { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
        [DataMember]
        public int Priority { get; set; }
    }
}
