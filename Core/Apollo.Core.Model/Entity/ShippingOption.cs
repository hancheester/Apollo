using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class ShippingOption : BaseEntity
    {
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public decimal Value { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public decimal FreeThreshold { get; set; }
        [DataMember]
        public decimal SingleItemValue { get; set; }
        [DataMember]
        public decimal UpToOneKg { get; set; }
        [DataMember]
        public decimal UpToOneHalfKg { get; set; }
        [DataMember]
        public decimal UpToTwoKg { get; set; }
        [DataMember]
        public decimal UpToTwoHalfKg { get; set; }
        [DataMember]
        public decimal UpToThreeKg { get; set; }
        [DataMember]
        public decimal UpToThreeHalfKg { get; set; }
        [DataMember]
        public decimal UpToFourKg { get; set; }
        [DataMember]
        public decimal UpToFourHalfKg { get; set; }
        [DataMember]
        public decimal UpToFiveKg { get; set; }
        [DataMember]
        public decimal HalfKgRate { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public int Priority { get; set; }
        [DataMember]
        public string Timeline { get; set; }
        [DataMember]
        public Country Country { get; set; }

        public ShippingOption()
        {
            Country = new Country();            
        }
    }
}
