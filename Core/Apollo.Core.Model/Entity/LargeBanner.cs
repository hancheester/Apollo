using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class LargeBanner : BaseEntity
    {
        [DataMember]
        public string MediaFilename { get; set; }
        [DataMember]
        public string MediaAlt { get; set; }
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
        [DataMember]
        public bool DisplayOnHomePage { get; set; }        
        [DataMember]
        public bool DisplayOnOffersPage { get; set; }
    }
}
