using Apollo.FrontStore.Models.Customer;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Customer
{
    public class ForgotPasswordValidator : BaseApolloValidator<ForgotPasswordModel>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email.");
        }
    }
}