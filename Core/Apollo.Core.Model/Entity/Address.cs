using System;

namespace Apollo.Core.Model.Entity
{
    public class Address : BaseEntity
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }        
        public string County { get; set; }
        public int CountryId { get; set; }
        public string PostCode { get; set; }
        public bool IsBilling { get; set; }
        public bool IsShipping { get; set; }
        public int USStateId { get; set; }
        public Country Country { get; set; }
        public USState USState { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime UpdatedOnDate { get; set; }
    }
}
