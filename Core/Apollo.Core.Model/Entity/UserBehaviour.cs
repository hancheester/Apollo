using System;

namespace Apollo.Core.Model.Entity
{
    public class UserBehaviour : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime LogTime { get; set; }
        public string UserFlow { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public int ProductId { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int CountryId { get; set; }
        public int OfferRuleId { get; set; }
    }
}
