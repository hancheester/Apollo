using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface IUtilityService
    {
        byte[] GenerateSitemap();
        string ConvertIPToCountryName(string ip);
        void ProcessContactUs(string contactUsEmail, string email, string name, string message);
        void ProcessHelpAlternativeProduct(string contactUsEmail, string email, int productId);
        void ProcessHelpStockNofification(string email, int productId, int productPriceId);
        int InsertCurrency(Currency item);
        void DeleteCurrencyCountry(int currencyCountryId);
        int InsertCurrencyCountry(CurrencyCountry item);
        void UpdateCurrency(Currency currency);
        PagedList<Currency> GetPagedCurrency(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string currencyCode = null);
        IList<CurrencyCountry> GetCurrencyCountryByCurrencyId(int currencyId);
        void UpdateTestimonial(Testimonial testimonial);
        Testimonial GetTestimonial(int testimonialId);
        int InsertTestimonial(Testimonial testimonial);
        PagedList<Testimonial> GetTestimonialLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string comment = null,
            string name = null,
            TestimonialSortingType orderBy = TestimonialSortingType.IdAsc);
        void DeleteTestimonial(int testimonialId);
        IList<Currency> GetAllCurrency();
        bool RefreshCache(CacheEntityKey keys);
        IList<GenericAttribute> GetAttributesForEntity(int entityId, string keyGroup);
        void SaveAttribute(int entityId, string entityName, string key, string value, int storeId = 0);
        Currency GetCurrency(int currencyId);
        Currency GetCurrencyByCurrencyCode(string currencyCode);
        IList<CustomDictionary> GetAllCustomDictionary();
        IDictionary<string, string> GetCachePerformanceData(string type);
        IList<string> GetCacheKeys(string type);
        void NotifyUserForStock();
    }
}
