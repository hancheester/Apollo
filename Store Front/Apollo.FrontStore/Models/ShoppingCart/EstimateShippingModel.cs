using Apollo.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.ShoppingCart
{
    public class EstimateShippingModel
    {
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<ShippingOptionModel> ShippingOptions { get; set; }
        public IList<string> Warnings { get; set; }
        public int CountryId { get; set; }
        public bool Enabled { get; set; }
        
        public EstimateShippingModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            ShippingOptions = new List<ShippingOptionModel>();
            Warnings = new List<string>();
        }

        #region Nested Classes

        public partial class ShippingOptionModel : BaseEntityModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Cost { get; set; }
            public bool Selected { get; set; }
        }

        #endregion
    }
}