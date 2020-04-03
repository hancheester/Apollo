using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class Tag : BaseEntity
    {
        [DataMember]
        public string Name { get; set; }        
    }
}
