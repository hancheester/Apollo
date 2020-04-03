using Apollo.FrontStore.Models.Customer;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Customer
{
    public class LoginValidator : BaseApolloValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}