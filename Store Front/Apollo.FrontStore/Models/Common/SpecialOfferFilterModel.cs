using Apollo.Core.Model.OverviewModel;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Common
{
    public class SpecialOfferFilterModel
    {
        public bool IsForMobile { get; set; }
        public IList<OfferTypeOverviewModel> OfferTypes { get; set; }
        public int SelectedOfferTypeId { get; set; }

        public SpecialOfferFilterModel()
        {
            OfferTypes = new List<OfferTypeOverviewModel>();
        }
    }
}