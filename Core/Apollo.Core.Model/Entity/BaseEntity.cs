using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public abstract class BaseEntity
    {
        [DataMember]
        public int Id { get; set; }
    }
}
