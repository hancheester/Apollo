using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Apollo.Core.Model.Entity
{
    [Serializable]
    [DataContract]
    public class RestrictedGroup : BaseEntity, IEqualityComparer<RestrictedGroup>
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int MaximumQuantity { get; set; }
        [DataMember]
        public string AllowedCountries { get; set; }
        [DataMember]
        public string DisallowedCountries { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public string DisplayName
        {
            get
            {
                return this.Name + ", Max Qty: " + (this.MaximumQuantity == 0 ? string.Empty : this.MaximumQuantity.ToString());
            }
            protected set { }
        }

        public bool Equals(RestrictedGroup x, RestrictedGroup y)
        {
            return x.Id == y.Id && x.Name.CompareTo(y.Name) == 0;
        }

        public int GetHashCode(RestrictedGroup obj)
        {
            return obj.Id.GetHashCode() ^ obj.Name.GetHashCode();
        }
    }
}
