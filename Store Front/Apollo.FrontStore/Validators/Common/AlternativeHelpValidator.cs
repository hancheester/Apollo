using Apollo.FrontStore.Models.Common;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Common
{
    public class AlternativeHelpValidator : BaseApolloValidator<AlternativeHelpModel>
    {
        public AlternativeHelpValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email.");
        }        
    }
}