using Apollo.FrontStore.Models.Common;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Common
{
    public class ContactUsValidator : BaseApolloValidator<ContactUsModel>
    {
        public ContactUsValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email.");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Message).NotEmpty().WithMessage("Message is required.");
        }
    }
}