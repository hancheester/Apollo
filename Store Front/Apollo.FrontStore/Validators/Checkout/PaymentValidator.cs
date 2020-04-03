using Apollo.FrontStore.Models.Checkout;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Checkout
{
    public class PaymentValidator : BaseApolloValidator<CheckoutPaymentModel>
    {
        public PaymentValidator()
        {
            //useful links:
            //http://fluentvalidation.codeplex.com/wikipage?title=Custom&referringTitle=Documentation&ANCHOR#CustomValidator
            //http://benjii.me/2010/11/credit-card-validator-attribute-for-asp-net-mvc-3/

            RuleFor(x => x.CardNumber).NotEmpty().WithMessage("Card number is required.");
            RuleFor(x => x.CardCode).NotEmpty().WithMessage("Secure code is required.");

            RuleFor(x => x.CardHolderName).NotEmpty().WithMessage("Cardholder name is required.");            
            RuleFor(x => x.CardNumber).IsCreditCard().WithMessage("Card number is invalid.");
            RuleFor(x => x.CardCode).Matches(@"^[0-9]{3,4}$").WithMessage("Secure code is invalid");
            RuleFor(x => x.ExpireMonth).NotEmpty().WithMessage("Expiry month is required.");
            RuleFor(x => x.ExpireYear).NotEmpty().WithMessage("Expiry year is required.");
        }
    }
}