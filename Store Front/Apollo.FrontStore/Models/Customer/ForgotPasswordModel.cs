using Apollo.FrontStore.Validators.Customer;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Apollo.FrontStore.Models.Customer
{
    [Validator(typeof(ForgotPasswordValidator))]
    public class ForgotPasswordModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}