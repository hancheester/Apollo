using Apollo.FrontStore.Validators.Customer;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Apollo.FrontStore.Models.Customer
{
    [Validator(typeof(LoginValidator))]
    public class LoginModel
    {
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        
        [Display(Name = "Username")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}