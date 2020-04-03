using System;

namespace Apollo.Core.Model.Entity
{
    public class SagePayLog : BaseEntity
    {   
        public int OrderId { get; set; }
        public string NameValue { get; set; }
        public string Status { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
