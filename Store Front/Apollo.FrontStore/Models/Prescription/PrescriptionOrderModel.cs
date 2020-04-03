using Apollo.FrontStore.Validators.Prescription;
using FluentValidation.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Prescription
{
    [Validator(typeof(PrescriptionOrderValidator))]
    public class PrescriptionOrderModel
    {
        [Display(Name = "Do you pay for your NHS prescription?")]
        public bool? HasExemption { get; set; }
        [Display(Name = "Your payment exemption status")]
        public string EnteredExemptionId { get; set; }
        public List<SelectListItem> AvailableExemptions { get; set; }
        public int EnteredQuantity { get; set; }
        [Display(Name = "How many different drugs are on your prescription?")]
        public List<SelectListItem> AllowedQuantities { get; set; }

        public PrescriptionOrderModel()
        {
            AvailableExemptions = new List<SelectListItem>();
            AllowedQuantities = new List<SelectListItem>();
        }
    }
}