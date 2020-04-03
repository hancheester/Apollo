using System;

namespace Apollo.Core.Model.Entity
{
    public class OrderNote : BaseEntity
    {
        public int OrderId { get; set; }
        public string Note { get; set; }
        public DateTime TimeStamp { get; set; }

        public OrderNote()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
