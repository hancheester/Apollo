using Apollo.Core.Model.OverviewModel;
using Apollo.FrontStore.Models.Media;
using Apollo.FrontStore.Models.Product;
using Apollo.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Common
{
    public class IndividualOfferModel : BaseEntityModel
    {
        public IList<OfferTypeOverviewModel> OfferTypes { get; set; }        
        public string Alias { get; set; }
        public PictureModel Image { get; set; }
        public string Description { get; set; }
        public int OfferTypeId { get; set; }
        public string UrlKey { get; set; }
        public string OfferUrl { get; set; }
        public IList<ProductBoxModel> Products { get; set; }

        public IndividualOfferModel()
        {
            Image = new PictureModel();
            OfferTypes = new List<OfferTypeOverviewModel>();
            Products = new List<ProductBoxModel>();
        }
    }
}