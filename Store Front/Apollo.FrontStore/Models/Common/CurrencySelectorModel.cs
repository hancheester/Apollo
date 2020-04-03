using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Common
{
    public class CurrencySelectorModel
    {
        public IList<CurrencyModel> AvailableCurrencies { get; set; }
        public int CurrentCurrencyId { get; set; }
        public string CurrencyCurrencyCode { get; set; }

        public CurrencySelectorModel()
        {
            AvailableCurrencies = new List<CurrencyModel>();
        }
    }
}