using Apollo.FrontStore.Validators.Common;
using FluentValidation.Attributes;

namespace Apollo.FrontStore.Models.Common
{
    [Validator(typeof(AlternativeHelpValidator))]
    public class AlternativeHelpModel
    {
        public string Email { get; set; }
        public int ProductId { get; set; }
    }
}