using Apollo.FrontStore.Validators.Checkout;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Checkout
{
    [Validator(typeof(PharmItemValidator))]
    public class PharmItemModel
    {
        public int CartPharmOrderId { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        [Display(Name = "What symptoms are going to be treated with this medication?")]
        public string Symptoms { get; set; }
        [Display(Name = "What is the age of the person who will be using this product?")]
        public string Age { get; set; }
        [Display(Name = "Does the intended user have any existing conditions or taking other medication?")]
        public bool HasOtherCondMed { get; set; }
        [Display(Name = "Please provide information about conditions or other medication.")]
        public string OtherCondMed { get; set; }
        [Display(Name = "How long have the symptoms persisted for?")]
        public string PersistedInDays { get; set; }
        [Display(Name = "What action has been taken to treat this condition?")]
        public string ActionTaken { get; set; }
        [Display(Name = "Has the intended user taken this medication before?")]        
        public bool HasTaken { get; set; }
        [Display(Name = "On how many different occasions has the intended user taken this medication?")]
        public string TakenQuantity { get; set; }
        [Display(Name = "What other medicines has the intended user tried for the symptom?")]
        public string MedForSymptom { get; set; }

        public string LastTimeTakenDay { get; set; }
        public string LastTimeTakenMonth { get; set; }
        public string LastTimeTakenYear { get; set; }
        public string LastTimeTaken
        {
            get
            {
                if (!string.IsNullOrEmpty(LastTimeTakenYear) && !string.IsNullOrEmpty(LastTimeTakenMonth) && !string.IsNullOrEmpty(LastTimeTakenDay))
                {
                    return string.Format("{0}/{1}/{2}", LastTimeTakenDay, LastTimeTakenMonth, LastTimeTakenYear);
                }

                return string.Empty;
            }
        }

        public IList<SelectListItem> AvailablePersistedInDays { get; set; }
        public IList<SelectListItem> AvailableTakenQuantity { get; set; }
        public IList<SelectListItem> AvailableAges { get; set; }
        
        public PharmItemModel()
        {
            AvailablePersistedInDays = new List<SelectListItem>();
            AvailableTakenQuantity = new List<SelectListItem>();
            AvailableAges = new List<SelectListItem>();
        }

        public DateTime? ParseLastTimeTakenDate()
        {
            if (string.IsNullOrEmpty(LastTimeTakenYear) || string.IsNullOrEmpty(LastTimeTakenMonth) || string.IsNullOrEmpty(LastTimeTakenDay))
                return null;

            DateTime? lastTimeTakenDate = null;
            try
            {
                lastTimeTakenDate = new DateTime(Convert.ToInt32(LastTimeTakenYear), Convert.ToInt32(LastTimeTakenMonth), Convert.ToInt32(LastTimeTakenDay));
            }
            catch { }
            return lastTimeTakenDate;
        }
    }
}