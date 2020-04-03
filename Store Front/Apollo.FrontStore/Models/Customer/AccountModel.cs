using Apollo.FrontStore.Validators.Customer;
using Apollo.Web.Framework.Mvc;
using FluentValidation.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Apollo.FrontStore.Models.Customer
{
    [Validator(typeof(AccountValidator))]
    public class AccountModel : BaseEntityModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Contact number")]
        public string ContactNumber { get; set; }
        [Display(Name = "Display contact number on delivery label")]
        public bool DisplayContactNumberInDespatch { get; set; }
        public bool HasPassword { get; set; }
        public IDictionary<string, string> Credentials { get; set; }
    }
}