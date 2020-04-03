using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core.Services.Shipping
{
    public class ShippingService : BaseRepository, IShippingService
    {
        #region Fields

        private readonly IRepository<ShippingOption> _shippingOptionRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<USState> _usStateRepository;
        private readonly IRepository<Delivery> _deliveryRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public ShippingService(IRepository<ShippingOption> shippingOptionRepository,
                               IRepository<Country> countryRepository,
                               IRepository<USState> usStateRepository,
                               IRepository<Delivery> deliveryRepository,
                               ICacheManager cacheManager,
                               ILogBuilder logBuilder)
        {
            _shippingOptionRepository = shippingOptionRepository;
            _countryRepository = countryRepository;
            _usStateRepository = usStateRepository;
            _deliveryRepository = deliveryRepository;
            _cacheManager = cacheManager;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion
        
        #region Return

        public IList<Country> GetCountries()
        {
            return _countryRepository.Table.OrderBy(c => c.Name).ToList();
        }

        public USState GetUSStateByCode(string usStateCode)
        {
            return _usStateRepository.Table.Where(u => u.Code == usStateCode).FirstOrDefault();
        }

        public IList<USState> GetUSStates()
        {
            return _usStateRepository.Table.ToList();
        }

        public IList<Delivery> GetDeliveryList()
        {
            return _deliveryRepository.Table.ToList();
        }

        public IList<Country> GetActiveCountries()
        {
            return _countryRepository.Table.Where(c => c.Enabled == true).OrderBy(c => c.Name).ToList();
        }

        public USState GetUSStateById(int usStateId)
        {
            var key = string.Format(CacheKey.USSTATE_BY_ID_KEY, usStateId);

            return _cacheManager.Get(key, delegate ()
            {
                return _usStateRepository.Return(usStateId);
            });            
        }

        public ShippingOption GetShippingOptionById(int shippingOptionId)
        {
            var key = string.Format(CacheKey.SHIPPING_OPTION_BY_ID_KEY, shippingOptionId);

            var shippingOption = _cacheManager.Get(key, delegate ()
            {
                var option = _shippingOptionRepository.Return(shippingOptionId);

                if (option != null)                
                    option.Country = GetCountryById(option.CountryId);
                
                return option;
            });

            return shippingOption;
        }

        public PagedList<ShippingOptionOverviewModel> GetPagedShippingOptionOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string description = null,
            string name = null,
            string countryName = null,
            ShippingOptionSortingType orderBy = ShippingOptionSortingType.IdAsc)
        {
            var query = _shippingOptionRepository.Table
                .Join(_countryRepository.Table, s => s.CountryId, c => c.Id, (s, c) => new { s, c });

            if (!string.IsNullOrEmpty(description))
                query = query.Where(s_c => s_c.s.Description.Contains(description));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(s_c => s_c.s.Name.Contains(name));

            if (!string.IsNullOrEmpty(countryName))
                query = query.Where(s_c => s_c.c.Name.Contains(countryName));

            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case ShippingOptionSortingType.IdAsc:
                    query = query.OrderBy(s_c => s_c.s.Id);
                    break;
                case ShippingOptionSortingType.IdDesc:
                    query = query.OrderByDescending(s_c => s_c.s.Id);
                    break;
                case ShippingOptionSortingType.NameAsc:
                    query = query.OrderBy(s_c => s_c.s.Name);
                    break;
                case ShippingOptionSortingType.NameDesc:
                    query = query.OrderByDescending(s_c => s_c.s.Name);
                    break;
                case ShippingOptionSortingType.DescriptionAsc:
                    query = query.OrderBy(s_c => s_c.s.Description);
                    break;
                case ShippingOptionSortingType.DescriptionDesc:
                    query = query.OrderByDescending(s_c => s_c.s.Description);
                    break;
                case ShippingOptionSortingType.PriorityAsc:
                    query = query.OrderBy(s_c => s_c.s.Priority);
                    break;
                case ShippingOptionSortingType.PriorityDesc:
                    query = query.OrderByDescending(s_c => s_c.s.Priority);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var items = query.ToList()
                .Select(x => new ShippingOptionOverviewModel
                {
                    Id = x.s.Id,
                    Description = x.s.Description,
                    Name = x.s.Name,
                    Enabled = x.s.Enabled,
                    Priority = x.s.Priority,
                    CountryId = x.s.CountryId,
                    FreeThreshold = x.s.FreeThreshold,
                    Timeline = x.s.Timeline
                }
                );

            return new PagedList<ShippingOptionOverviewModel>(items, pageIndex, pageSize, totalRecords);
        }

        public Country GetCountryById(int countryId)
        {
            var key = string.Format(CacheKey.COUNTRY_BY_COUNTRY_ID_KEY, countryId);

            return _cacheManager.Get(key, delegate ()
            {
                return _countryRepository.Return(countryId);
            });
        }

        public PagedList<Country> GetPagedCountry(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string name = null,
            string countryCode = null,
            CountrySortingType orderBy = CountrySortingType.IdAsc)
        {
            var query = _countryRepository.Table;

            if (!string.IsNullOrEmpty(name))
                query = query.Where(c => c.Name.Contains(name));

            if (!string.IsNullOrEmpty(countryCode))
                query = query.Where(c => c.ISO3166Code.Contains(countryCode));

            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case CountrySortingType.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case CountrySortingType.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case CountrySortingType.NameAsc:
                    query = query.OrderBy(x => x.Name);
                    break;
                case CountrySortingType.NameDesc:
                    query = query.OrderByDescending(x => x.Name);
                    break;
                case CountrySortingType.CountryCodeAsc:
                    query = query.OrderBy(x => x.ISO3166Code);
                    break;
                case CountrySortingType.CountryCodeDesc:
                    query = query.OrderByDescending(x => x.ISO3166Code);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var items = query.ToList();

            return new PagedList<Country>(items, pageIndex, pageSize, totalRecords);
        }

        public IList<ShippingOption> GetShippingOptionByCountryAndEnabled(int countryId, bool enabled)
        {
            var options = _shippingOptionRepository.Table
                .Where(p => p.CountryId == countryId)
                .Where(p => p.Enabled == enabled)
                .OrderBy(p => p.Priority)
                .ToList();

            for (int i = 0; i < options.Count; i++)
            {
                options[i].Country = _countryRepository.Return(options[i].CountryId);
            }

            return options;
        }

        public IList<ShippingOptionOverviewModel> GetActiveTrackedDeliveryShippingOptions()
        {
            var options = _shippingOptionRepository.Table
                .Join(_countryRepository.Table, s => s.CountryId, c => c.Id, (s, c) => new { s, c })
                .Where(x => x.s.Name == "Tracked Delivery")
                .Where(x => x.s.Enabled == true)
                .Where(x => x.c.Enabled == true)
                .OrderBy(x => x.c.Name)
                .Select(x => new ShippingOptionOverviewModel
                {
                    Id = x.s.Id,
                    Description = x.s.Description,
                    Name = x.s.Name,
                    Cost = x.s.Value,
                    Enabled = x.s.Enabled,
                    Priority = x.s.Priority,
                    CountryId = x.s.CountryId,
                    FreeThreshold = x.s.FreeThreshold,
                    Timeline = x.s.Timeline
                })
                .ToList();

            return options;
        }

        public Country GetCountryByCountryCode(string countryCode)
        {
            var country = _countryRepository.Table.Where(c => c.ISO3166Code == countryCode).FirstOrDefault();
            return country;
        }

        #endregion

        #region Create

        public int InsertShippingOption(ShippingOption option)
        {
            return _shippingOptionRepository.Create(option);
        }

        public int InsertCountry(Country country)
        {
            return _countryRepository.Create(country);
        }

        #endregion

        #region Delete

        public void DeleteShippingOption(int shippingOptionId)
        {
            _shippingOptionRepository.Delete(shippingOptionId);
        }

        #endregion

        #region Update
        
        public void UpdateShippingOption(ShippingOption option)
        {
            _shippingOptionRepository.Update(option);
        }

        public void UpdateCountry(Country country)
        {
            _countryRepository.Update(country);
        }

        #endregion
    }
}
