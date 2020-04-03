using Apollo.Core.Configuration;

namespace Apollo.Core.Domain.Directory
{
    public class CurrencySettings : ISettings
    {
        public int PrimaryStoreCurrencyId { get; set; }

        //TODO: We can only support currency id
        public string PrimaryStoreCurrencyCode { get; set; }

        public string ExchangeRateProviderLink { get; set; }
        public decimal ExchangeRateFactor { get; set; }
    }
}
