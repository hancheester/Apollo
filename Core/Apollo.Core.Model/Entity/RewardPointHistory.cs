using System;

namespace Apollo.Core.Model.Entity
{
    public class RewardPointHistory : BaseEntity
    {
        public int AccountId { get; set; }
        public int? Points { get; set; }
        public int PointsBalance { get; set; }
        public int? UsedPoints { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int? UsedWithOrderId { get; set; }

        public RewardPointHistory()
        {
            this.CreatedOnDate = DateTime.Now;
        }
    }
}
