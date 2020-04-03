using Apollo.FrontStore.Validators.Common;
using FluentValidation.Attributes;

namespace Apollo.FrontStore.Models.Common
{
    [Validator(typeof(StockNotiferValidator))]
    public class StockNotifierModel
    {
        public string Email { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
    }
}