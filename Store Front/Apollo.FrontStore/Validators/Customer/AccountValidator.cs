using Apollo.FrontStore.Models.Customer;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Customer
{
    public class AccountValidator : BaseApolloValidator<AccountModel>
    {
        public AccountValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Name).Length(1, 200).WithMessage("Maximum length of name is 200 characters.");
            RuleFor(x => x.Email).Length(1, 80).WithMessage("Maximum length of email is 80 characters.");
            RuleFor(x => x.ContactNumber).Length(0, 50).WithMessage("Maximum length of contact number is 50 characters.");
        }
    }
}