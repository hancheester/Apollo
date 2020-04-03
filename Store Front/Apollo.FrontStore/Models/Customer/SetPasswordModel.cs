using System.ComponentModel.DataAnnotations;

namespace Apollo.FrontStore.Models.Customer
{
    public class SetPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(999, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]        
        public string ConfirmPassword { get; set; }
    }
}