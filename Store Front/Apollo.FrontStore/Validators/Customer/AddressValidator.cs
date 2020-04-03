using Apollo.Core.Infrastructure;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Models.Common;
using Apollo.Web.Framework.Validators;
using FluentValidation;
using FluentValidation.Results;
using System.Linq;

namespace Apollo.FrontStore.Validators.Customer
{
    public class AddressValidator : BaseApolloValidator<AddressModel>
    {
        #region Fields

        private readonly string[] _countries_without_postcode = {
            "AO",
            "AE",
            "AG",
            "AN",
            "AW",
            "BF",
            "BI",
            "BJ",
            "BS",
            "BW",
            "BZ",
            "CD",
            "CF",
            "CG",
            "CI",
            "CK",
            "CM",
            "DJ",
            "DM",
            "ER",
            "FJ",
            "GD",
            "GH",
            "GM",
            "GN",
            "GQ",
            "GY",
            "HK",
            "JM",
            "KE",
            "KI",
            "KM",
            "KN",
            "KP",
            "LC",
            "ML",
            "MO",
            "MR",
            "MS",
            "MU",
            "MW",
            "NR",
            "NU",
            "PA",
            "QA",
            "RW",
            "SA",
            "SB",
            "SC",
            "SL",
            "SO",
            "SR",
            "ST",
            "SY",
            "TF",
            "TK",
            "TL",
            "TO",
            "TT",
            "TV",
            "TZ",
            "UG",
            "VU",
            "YE",
            "ZA",
            "ZW"};

        #endregion

        public AddressValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Name).Length(1, 100).WithMessage("Maximum length of name is 100 characters.");
            RuleFor(x => x.AddressLine1).NotEmpty().WithMessage("Address line 1 is required.");
            RuleFor(x => x.AddressLine1).Length(1, 256).WithMessage("Maximum length of address line 1 is 256 characters.");
            RuleFor(x => x.AddressLine2).Length(0, 256).WithMessage("Maximum length of address line 2 is 256 characters.");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.City).Length(1, 100).WithMessage("Maximum length of city is 100 characters.");
            RuleFor(x => x.County).Length(0, 100).WithMessage("Maximum length of county is 100 characters.");
            RuleFor(x => x.PostCode).Length(0, 50).WithMessage("Maximum length of post code is 50 characters.");
            RuleFor(x => x.CountryId).NotEmpty().WithMessage("Country is required.");

            Custom(x =>
            {
                var countryId = 0;
                if (int.TryParse(x.CountryId, out countryId) == false)
                    return new ValidationFailure("CountryId", "Country is required.");
                
                return null;
            });

            Custom(x =>
            {
                var countryId = 0;
                if (int.TryParse(x.CountryId, out countryId) == false)
                    return new ValidationFailure("CountryId", "Country is required.");

                var service = EngineContext.Current.Resolve<IShippingService>();
                var country = service.GetCountryById(countryId);

                if (country == null)
                    return new ValidationFailure("CountryId", "Country is required.");

                if (_countries_without_postcode.Contains(country.ISO3166Code) == false && string.IsNullOrEmpty(x.PostCode))
                    return new ValidationFailure("PostCode", " Post code is required.");

                return null;
            });
        }
    }
}