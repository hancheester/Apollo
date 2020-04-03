using Apollo.FrontStore.Validators.Common;
using FluentValidation.Attributes;

namespace Apollo.FrontStore.Models.Common
{
    [Validator(typeof(ContactUsValidator))]
    public class ContactUsModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}