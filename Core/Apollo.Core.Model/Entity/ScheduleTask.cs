using System;

namespace Apollo.Core.Model.Entity
{
    public partial class ScheduleTask : BaseEntity
    {
        public string Name { get; set; }
        public int Seconds { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }
        public bool StopOnError { get; set; }
        public DateTime? LastStartUtc { get; set; }
        public DateTime? LastEndUtc { get; set; }
        public DateTime? LastSuccessUtc { get; set; }
        public int? Frequency { get; set; }
        public DateTime? NextStartUtc { get; set; }
    }
}
