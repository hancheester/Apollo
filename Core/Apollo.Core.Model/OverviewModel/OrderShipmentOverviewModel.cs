using System;

namespace Apollo.Core.Model.OverviewModel
{
    public class OrderShipmentOverviewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Carrier { get; set; }
        public string TrackingRef { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
