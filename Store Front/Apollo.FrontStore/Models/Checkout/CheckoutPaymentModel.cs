using Apollo.FrontStore.Validators.Checkout;
using FluentValidation.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Checkout
{
    [Validator(typeof(PaymentValidator))]
    public class CheckoutPaymentModel
    {
        public string OrderTotal { get; set; }

        [Display(Name = "Card type")]
        public string CardType { get; set; }
        [Display(Name = "Card type")]
        public IList<SelectListItem> CardTypes { get; set; }

        [DataType(DataType.CreditCard)]
        [Display(Name = "Card number")]
        public string CardNumber { get; set; }
        [Display(Name = "Cardholder name")]
        public string CardHolderName { get; set; }
        
        /// <summary>
        /// 3-digits code behind card.
        /// </summary>
        [Display(Name = "Secure code")]
        public string CardCode { get; set; }

        [Display(Name = "Expiry date")]
        public string ExpireMonth { get; set; }
        [Display(Name = "Expiry date")]
        public string ExpireYear { get; set; }
        public IList<SelectListItem> ExpireMonths { get; set; }
        public IList<SelectListItem> ExpireYears { get; set; }

        public bool DisableProceed { get; set; }

        public CheckoutPaymentModel()
        {
            CardTypes = new List<SelectListItem>();
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
        }
    }
}