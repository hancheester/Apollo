using Apollo.FrontStore.Models.Checkout;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Checkout
{
    public class PharmItemValidator : BaseApolloValidator<PharmItemModel>
    {
        public PharmItemValidator()
        {
            RuleFor(x => x.Symptoms).NotEmpty().WithMessage("What symptoms are going to be treated with this medication?");
            RuleFor(x => x.MedForSymptom).NotEmpty().WithMessage("What other medicines has the intended user tried for the symptom?");
            RuleFor(x => x.ActionTaken).NotEmpty().WithMessage("What action has been taken to treat this condition?");
            RuleFor(x => x.PersistedInDays).NotEmpty().WithMessage("How long have the symptoms persisted for?");
            RuleFor(x => x.Age).NotEmpty().WithMessage("What is the age of the person who will be using this product?");
            RuleFor(x => x.LastTimeTakenDay).NotEmpty().WithMessage("When was the last time the intended user took this medication?");
            //RuleFor(x => x.LastTimeTakenMonth).NotEmpty().WithMessage("When was the last time the intended user took this medication?");
            //RuleFor(x => x.LastTimeTakenYear).NotEmpty().WithMessage("When was the last time the intended user took this medication?");

            //Custom(x =>
            //{
            //    var dateOfLastTimeTaken = x.ParseLastTimeTakenDate();
            //    if ((string.IsNullOrEmpty(x.LastTimeTakenYear) || string.IsNullOrEmpty(x.LastTimeTakenMonth) || string.IsNullOrEmpty(x.LastTimeTakenDay)) && dateOfLastTimeTaken == null)
            //    {
            //        return new ValidationFailure("LastTimeTakenDay", "When was the last time the intended user took this medication?");
            //    }
            //    return null;
            //});
        }
    }
}