using Apollo.FrontStore.Models.Common;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Common
{
    public class SubscriptionValidator : BaseApolloValidator<SubscriptionModel>
    {
        public SubscriptionValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email.");
        }
    }
}