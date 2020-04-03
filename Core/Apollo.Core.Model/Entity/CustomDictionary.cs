using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class CustomDictionary : BaseEntity
    {
        [DataMember]
        public string Word { get; set; }
    }
}
