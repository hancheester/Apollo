using System.Collections.Generic;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Common
{
    public class CountryPreferenceModel
    {
        public IList<SelectListItem> AvailableCountries { get; set; }        
        public string SelectedCountryName { get; set; }
        public string SelectedCountryCode { get; set; }
        public string Note { get; set; }
        public string FreeDeliveryNote { get; set; }
        public int CountryId { get; set; }
        
        public CountryPreferenceModel()
        {
            AvailableCountries = new List<SelectListItem>();
        }
    }
}