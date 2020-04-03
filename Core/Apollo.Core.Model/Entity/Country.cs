using System;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class Country : BaseEntity
    {
        [DataMember]
        public string ISO3166Code { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsEC { get; set; }
        [DataMember]
        public string DeminimisValue { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public DateTime CreatedOnDate { get; set; }
        [DataMember]
        public DateTime UpdatedOnDate { get; set; }
        
        public Country()
        {
            Name = string.Empty;
            ISO3166Code = string.Empty;
            DeminimisValue = string.Empty;
            CreatedOnDate = DateTime.Now;
            UpdatedOnDate = DateTime.Now;
        }
    }
}
