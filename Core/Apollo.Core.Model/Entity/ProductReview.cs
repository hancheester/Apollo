using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class ProductReview : BaseEntity
    {
        [DataMember]
        public int ProductId { get; set; }
        [DataMember]
        public int ProfileId { get; set; }
        [DataMember]
        public string Alias { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public bool Approved { get; set; }
        [DataMember]
        public string ProductName { get; set; }
    }
}
