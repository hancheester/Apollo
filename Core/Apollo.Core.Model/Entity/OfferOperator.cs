using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class OfferOperator : BaseEntity
    {
        [DataMember]
        public string Operator { get; set; }               
    }
}
