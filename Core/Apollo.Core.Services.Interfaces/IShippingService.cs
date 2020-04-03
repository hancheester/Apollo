using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface IShippingService
    {
        IList<ShippingOptionOverviewModel> GetActiveTrackedDeliveryShippingOptions();
        void DeleteShippingOption(int shippingOptionId);
        IList<Country> GetCountries();
        PagedList<Country> GetPagedCountry(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string name = null,
            string countryCode = null,
            CountrySortingType orderBy = CountrySortingType.IdAsc);
        PagedList<ShippingOptionOverviewModel> GetPagedShippingOptionOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string description = null,
            string name = null,
            string countryName = null,
            ShippingOptionSortingType orderBy = ShippingOptionSortingType.IdAsc);
        USState GetUSStateByCode(string usStateCode);
        IList<USState> GetUSStates();
        int InsertShippingOption(ShippingOption option);
        void UpdateShippingOption(ShippingOption option);
        int InsertCountry(Country country);
        void UpdateCountry(Country country);
        IList<Delivery> GetDeliveryList();
        IList<Country> GetActiveCountries();
        Country GetCountryByCountryCode(string countryCode);
        Country GetCountryById(int countryId);
        USState GetUSStateById(int usStateId);
        ShippingOption GetShippingOptionById(int shippingOptionId);
        IList<ShippingOption> GetShippingOptionByCountryAndEnabled(int countryId, bool enabled);
    }
}
