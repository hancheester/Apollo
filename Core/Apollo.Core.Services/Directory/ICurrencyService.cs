using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Directory
{
    public interface ICurrencyService
    {
        int InsertCurrencyCountry(CurrencyCountry currencyCountry);
        int InsertCurrency(Currency currency);
        IList<Currency> GetAllCurrency();
        void UpdateCurrencyLiveRates();
        void UpdateCurrency(Currency currency);
        Currency GetCurrencyByCurrencyCode(string currencyCode);
        Currency GetCurrency(int currencyId);
        PagedList<Currency> GetPagedCurrency(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string currencyCode = null);
        IList<CurrencyCountry> GetCurrencyCountryByCurrencyId(int currencyId);
        void DeleteCurrencyCountry(int currencyCountryId);
    }
}
