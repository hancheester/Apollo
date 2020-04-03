using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class Currency : BaseEntity
    {
        [DataMember]
        public string CurrencyCode { get; set; }
        [DataMember]
        public string HtmlEntity { get; set; }
        [DataMember]
        public decimal ExchangeRate { get; set; }
        [DataMember]
        public string Symbol { get; set; }
        [DataMember]
        public decimal GoogleExchangeRate { get; set; }
        [DataMember]
        public DateTime CreatedOnUtc { get; set; }
        [DataMember]
        public DateTime UpdatedOnUtc { get; set; }
    }
}
