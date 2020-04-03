using Apollo.Core.Model.OverviewModel;
using Apollo.FrontStore.Models.Media;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Common
{
    public class SpecialOfferModel
    {
        public IList<SingleOfferModel> Offers { get; set; }
        public IList<OfferTypeOverviewModel> OfferTypes { get; set; }
        public int SelectedOfferTypeId { get; set; }
        public string SelectedOfferType { get; set; }
        public int SelectedOfferId { get; set; }
        public IList<BannerModel> Banners { get; set; }

        public SpecialOfferModel()
        {
            Offers = new List<SingleOfferModel>();
            OfferTypes = new List<OfferTypeOverviewModel>();
            Banners = new List<BannerModel>();
        }

        #region Nested Classes

        public class SingleOfferModel
        {
            public int Id { get; set; }
            public string Alias { get; set; }
            public int OfferTypeId { get; set; }
            public string UrlKey { get; set; }
            public PictureModel Image { get; set; }
            public string Description { get; set; }

            public SingleOfferModel()
            {
                Image = new PictureModel();
            }
        }

        #endregion
    }
}