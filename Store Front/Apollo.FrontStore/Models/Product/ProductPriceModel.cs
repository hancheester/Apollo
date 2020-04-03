using Apollo.FrontStore.Models.Media;
using Apollo.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductPriceModel : BaseEntityModel
    {
        public string Option { get; set; }

        #region For Structured Data

        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string OfferPriceValue { get; set; }
        public string PriceValue { get; set; }
        public IDictionary<SchemaMetaTagModel, SchemaMetaTagModel> SchemaMetaPrices { get; set; }
        public IDictionary<SchemaMetaTagModel, SchemaMetaTagModel> SchemaMetaOfferPrices { get; set; }
        
        #endregion

        public string Price { get; set; }
        public string OfferPrice { get; set; }
        public bool IsPreSelected { get; set; }
        public int OfferRuleId { get; set; }
        public string SavePercentageNote { get; set; }
        public bool StockAvailability { get; set; }
        public bool Visible { get; set; }
        public string MessageAfterHidden { get; set; }
        public bool DisplayRRP { get; set; }
        public int? ProductMediaId { get; set; }
        
        //picture model is used when we want to override a default product picture when an option is selected
        public PictureModel PictureModel { get; set; }

        public ProductPriceModel()
        {
            PictureModel = new PictureModel();
            SchemaMetaPrices = new Dictionary<SchemaMetaTagModel, SchemaMetaTagModel>();
            SchemaMetaOfferPrices = new Dictionary<SchemaMetaTagModel, SchemaMetaTagModel>();
        }
    }
}