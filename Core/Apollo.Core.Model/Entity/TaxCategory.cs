using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class TaxCategory : BaseEntity
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Rate { get; set; }
        [DataMember]
        public bool Default { get; set; }
    }
}
