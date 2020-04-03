using Apollo.FrontStore.Validators.Customer;
using FluentValidation.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Apollo.FrontStore.Models.Customer
{
    [Validator(typeof(RegisterValidator))]
    public class RegisterModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Repeat email")]
        public string ConfirmEmail { get; set; }

        public int? DateOfBirthDay { get; set; }
        public int? DateOfBirthMonth { get; set; }
        public int? DateOfBirthYear { get; set; }

        [Display(Name = "Contact number")]
        public string ContactNumber { get; set; }
        [Display(Name = "Display contact number on delivery label")]
        public bool DisplayContactNumberInDespatch { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Receive our newsletter")]
        public bool Newsletter { get; set; }
        public bool AcceptPrivacyPolicyEnabled { get; set; }

        public RegisterModel()
        {

        }

        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }
    }
}