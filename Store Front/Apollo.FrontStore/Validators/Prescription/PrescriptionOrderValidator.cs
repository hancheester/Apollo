using Apollo.FrontStore.Models.Prescription;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Prescription
{
    public class PrescriptionOrderValidator : BaseApolloValidator<PrescriptionOrderModel>
    {
        public PrescriptionOrderValidator()
        {
            RuleFor(x => x.HasExemption).NotEmpty().WithMessage("Exemption status is required.");
        }
    }
}