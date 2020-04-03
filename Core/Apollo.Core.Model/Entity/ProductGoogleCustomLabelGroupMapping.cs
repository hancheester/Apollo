using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class ProductGoogleCustomLabelGroupMapping : BaseEntity
    {
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public string CustomLabel1 { get; set; }
        [DataMember]
        public string Value1 { get; set; }
        [DataMember]
        public string CustomLabel2 { get; set; }
        [DataMember]
        public string Value2 { get; set; }
        [DataMember]
        public string CustomLabel3 { get; set; }
        [DataMember]
        public string Value3 { get; set; }
        [DataMember]
        public string CustomLabel4 { get; set; }
        [DataMember]
        public string Value4 { get; set; }
        [DataMember]
        public string CustomLabel5 { get; set; }
        [DataMember]
        public string Value5 { get; set; }
    }
}
