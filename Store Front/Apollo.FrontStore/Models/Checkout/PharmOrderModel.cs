using Apollo.FrontStore.Validators.Checkout;
using FluentValidation.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Checkout
{
    [Validator(typeof(PharmOrderValidator))]
    public class PharmOrderModel
    {
        [Display(Name = "Will all the medication in your basket be taken by you?")]
        public bool TakenByOwner { get; set; }

        [Display(Name = "Please provide information on any known allergies.")]
        public string Allergy { get; set; }

        [Display(Name = "How old are you?")]
        public string OwnerAge { get; set; }

        [Display(Name = "Do you have any existing conditions or are you taking other medication?")]
        public bool HasCondition { get; set; }

        [Display(Name = "Please provide information about your conditions or other medication.")]
        public string OwnerCondition { get; set; }

        public IList<SelectListItem> OwnerAges { get; set; }

        public IList<PharmItemModel> PharmItems { get; set; }

        public PharmOrderModel()
        {
            OwnerAges = new List<SelectListItem>();
            PharmItems = new List<PharmItemModel>();
        }
    }
}