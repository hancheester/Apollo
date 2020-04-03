using Apollo.FrontStore.Validators.Common;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Apollo.FrontStore.Models.Common
{
    [Validator(typeof(SubscriptionValidator))]
    public class SubscriptionModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}