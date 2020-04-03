using System;

namespace Apollo.Core.Model.Entity
{
    public class spReport_GetRegistrations_Result
    {
        public Nullable<System.DateTime> TheDay { get; set; }
        public Nullable<int> TotalRegistered { get; set; }
        public int TotalOrders { get; set; }
    }
}
