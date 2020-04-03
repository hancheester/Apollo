using Apollo.Core.Configuration;
using Apollo.Core.Model;

namespace Apollo.Core.Domain.Tax
{
    public class TaxSettings : ISettings
    {
        public TaxDisplayType TaxDisplayType { get; set; }
        public bool PricesIncludeTax { get; set; }
    }
}
