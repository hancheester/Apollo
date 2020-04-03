using Apollo.Core.Model;
using Apollo.FrontStore.Validators.Customer;
using Apollo.Web.Framework.Mvc;
using FluentValidation.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Common
{
    [Validator(typeof(AddressValidator))]
    public class AddressModel : BaseEntityModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Address line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address line 2")]
        public string AddressLine2 { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "County")]
        public string County { get; set; }
        [Display(Name = "Postcode")]
        public string PostCode { get; set; }

        [Display(Name = "Country")]
        public string CountryId { get; set; }
        [Display(Name = "Country")]
        public string CountryName { get; set; }

        //[Display(Name = "Is billing")]
        public bool IsBilling { get; set; }

        //[Display(Name = "Is shipping")]
        public bool IsShipping { get; set; }

        [Display(Name = "State")]
        public int? USStateId { get; set; }
        [Display(Name = "State")]
        public string USState { get; set; }

        public AddressType AddressType { get; set; }
        
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public AddressModel()
        {
            County = string.Empty;            
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }
    }
}