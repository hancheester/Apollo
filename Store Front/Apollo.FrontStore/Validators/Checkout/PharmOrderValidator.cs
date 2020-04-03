using Apollo.FrontStore.Models.Checkout;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Checkout
{
    public class PharmOrderValidator : BaseApolloValidator<PharmOrderModel>
    {
        public PharmOrderValidator()
        {
            RuleFor(x => x.Allergy).NotEmpty().WithMessage("Please provide information on any known allergies.");
            RuleFor(x => x.OwnerAge).NotEmpty().WithMessage("How old are you?");
        }
    }
}