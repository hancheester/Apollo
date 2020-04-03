using Apollo.FrontStore.Models.Customer;
using Apollo.Web.Framework.Validators;
using FluentValidation;
using FluentValidation.Results;

namespace Apollo.FrontStore.Validators.Customer
{
    public class RegisterValidator : BaseApolloValidator<RegisterModel>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Name).Length(1, 200).WithMessage("Maximum length of name is 200 characters.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email.");
            RuleFor(x => x.Email).Length(1, 80).WithMessage("Maximum length of email is 80 characters.");
            RuleFor(x => x.ConfirmEmail).NotEmpty().WithMessage("Repeat email is required.");
            RuleFor(x => x.ConfirmEmail).EmailAddress().WithMessage("Invalid email.");
            RuleFor(x => x.ConfirmEmail).Equal(x => x.Email).WithMessage("Entered emails do not match.");

            RuleFor(x => x.ContactNumber).Length(0, 50).WithMessage("Maximum length of contact number is 50 characters.");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.Password).Length(8, 999).WithMessage(string.Format("The password should have at least {0} characters.", 8));
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Re-type password is required.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Entered passwords do not match.");

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