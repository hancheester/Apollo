using System;
using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class OrderShipment : BaseEntity
    {
        public int OrderId { get; set; }
        public string Carrier { get; set; }
        public string TrackingRef { get; set; }
        public DateTime TimeStamp { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public int CountryId { get; set; }
        public string PostCode { get; set; }
        public int USStateId { get; set; }        
        public IList<ItemShipment> ItemShipmentList  { get; set; }
    }
}
