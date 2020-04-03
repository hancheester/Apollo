using Apollo.Web.Framework.Mvc;
using System;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Order
{
    public class CustomerRewardPointsModel
    {
        public IList<RewardPointsHistoryModel> RewardPoints { get; set; }
        public CustomerRewardPointsPagingFilteringModel PagingFilteringContext { get; set; }
        
        public CustomerRewardPointsModel()
        {
            RewardPoints = new List<RewardPointsHistoryModel>();
            PagingFilteringContext = new CustomerRewardPointsPagingFilteringModel();
        }
    }

    #region Nested classes

    public class RewardPointsHistoryModel : BaseEntityModel
    {
        public int? Points { get; set; }
        public int? UsedPoints { get; set; }
        public int PointsBalance { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    #endregion
}