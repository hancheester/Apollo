using Apollo.FrontStore.Models.Customer;
using Apollo.Web.Framework.Validators;
using FluentValidation;
using FluentValidation.Results;

namespace Apollo.FrontStore.Validators.Customer
{
    public class RegisterExternalValidator : BaseApolloValidator<RegisterExternalModel>
    {
        public RegisterExternalValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Name).Length(1, 200).WithMessage("Maximum length of name is 200 characters.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email.");
            RuleFor(x => x.Email).Length(1, 80).WithMessage("Maximum length of email is 80 characters.");
            
            RuleFor(x => x.ContactNumber).Length(0, 50).WithMessage("Maximum length of contact number is 50 characters.");
            
            Custom(x =>
            {
                var dateOfBirth = x.ParseDateOfBirth();
                if ((x.DateOfBirthDay.Value != 0 || x.DateOfBirthMonth.Value != 0 || x.DateOfBirthYear.Value != 0) && dateOfBirth == null)
                {
                    return new ValidationFailure("DateOfBirthDay", "Date of birth is required.");
                }
                return null;
            });
        }
    }
}