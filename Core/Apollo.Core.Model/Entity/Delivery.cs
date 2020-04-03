using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class Delivery : BaseEntity
    {
        [DataMember]
        public string TimeLine { get; set; }
    }
}
