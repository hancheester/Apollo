using Apollo.Core.Model;
using Apollo.FrontStore.Models.Media;
using Apollo.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductDetailsModel : BaseEntityModel
    {
        public string Name { get; set; }
        public string UrlKey { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string DeliveryTimeLine { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public int AverageReviewRating { get; set; }
        public OptionType OptionType { get; set; }
        public GiftCardModel GiftCard { get; set; }
        public AddToCartModel AddToCart { get; set; }
        public ProductBreadcrumbModel ProductBreadcrumb { get; set; }
        public ProductBrandModel ProductBrand { get; set; }
        public PictureModel DefaultPicture { get; set; }
        public IList<PictureModel> PictureModels { get; set; }
        public ProductPriceModel DefaultProductPrice { get; set; }
        public IList<ProductPriceModel> ProductPrices { get; set; }
        public IList<ProductTagModel> ProductTags { get; set; }
        public IList<ProductReviewModel> Reviews { get; set; }
        public ProductOfferModel ProductOffer { get; set; }
        public bool IsPhoneOrder { get; set; }
        public string PhoneOrderMessage { get; set; }
        public bool Enabled { get; set; }
        public bool Discontinued { get; set; }

        public ProductDetailsModel()
        {
            ProductBrand = new ProductBrandModel();
            DefaultPicture = new PictureModel();
            PictureModels = new List<PictureModel>();
            DefaultProductPrice = new ProductPriceModel();
            ProductPrices = new List<ProductPriceModel>();
            AddToCart = new AddToCartModel();
            GiftCard = new GiftCardModel();
            ProductTags = new List<ProductTagModel>();
            Reviews = new List<ProductReviewModel>();
            ProductBreadcrumb = new ProductBreadcrumbModel();
            ProductOffer = new ProductOfferModel();
        }

        #region Nested Classes
        
        public class GiftCardModel
        {
            public bool IsGiftCard { get; set; }
            
            [AllowHtml]
            public string RecipientName { get; set; }            
            [AllowHtml]
            public string RecipientEmail { get; set; }            
            [AllowHtml]
            public string SenderName { get; set; }            
            [AllowHtml]
            public string SenderEmail { get; set; }            
            [AllowHtml]
            public string Message { get; set; }

            public GiftCardType GiftCardType { get; set; }
        }
        
        #endregion
    }
}