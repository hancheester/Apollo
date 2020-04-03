using Apollo.Core.Model.OverviewModel;
using Apollo.FrontStore.Models.Media;
using Apollo.Web.Framework.Mvc;
using System;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductBoxModel : BaseEntityModel
    {
        public string Name { get; set; }
        public string UrlKey { get; set; }
        public string DefaultOption { get; set; }
        public string Options { get; set; }
        public string PriceRange { get; set; }
        public string StyleClass { get; set; }
        public int AverageReviewRating { get; set; }
        public int ReviewCount { get; set; }
        public string ProductMark { get; set; }
        public int ProductMarkType { get; set; }
        public DateTime? ProductMarkExpiryDate { get; set; }
        public string Note { get; set; }
        public string AMPNote { get; set; }
        public string ButtonMessage { get; set; }
        public string ShortDescription { get; set; }
        public bool StockAvailability { get; set; }
        public string CurrencyCode { get; set; }        
        public string PriceValue { get; set; }
        public bool DisableBuyButton { get; set; }
        public string ImageLoadType { get; set; }
        public PictureModel Picture { get; set; }
        public IList<OfferRuleOverviewModel> RelatedOffers {get;set;}

        public ProductBoxModel()
        {
            Note = "<img src='/content/img/icon-bag-small.svg' alt='add to cart'/> ADD TO CART";
            AMPNote = "<amp-img width='16' height='19' src='/content/img/icon-bag-small.svg' alt='add to cart'></amp-img> ADD TO CART";
            ButtonMessage = "Add To Cart";
            Picture = new PictureModel();
            RelatedOffers = new List<OfferRuleOverviewModel>();
        }
    }    
}