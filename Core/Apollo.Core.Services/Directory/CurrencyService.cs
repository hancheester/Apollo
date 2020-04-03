using Apollo.Core.Caching;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model.Entity;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Apollo.Core.Services.Directory
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<CurrencyCountry> _currencyCountryRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly ICacheManager _cacheManager;
        private readonly CurrencySettings _currencySettings;
        
        public CurrencyService(
            IDbContext dbContext,
            IRepository<Currency> currencyRepository,
            IRepository<CurrencyCountry> currencyCountryRepository,
            IRepository<Country> countryRepository,
            ICacheManager cacheManager,
            CurrencySettings currencySettings)
        {
            _dbContext = dbContext;
            _currencyRepository = currencyRepository;
            _currencyCountryRepository = currencyCountryRepository;
            _countryRepository = countryRepository;
            _cacheManager = cacheManager;
            _currencySettings = currencySettings;
        }

        public int InsertCurrencyCountry(CurrencyCountry currencyCountry)
        {
            return _currencyCountryRepository.Create(currencyCountry);
        }

        public int InsertCurrency(Currency currency)
        {
            return _currencyRepository.Create(currency);
        }

        public void UpdateCurrencyLiveRates()
        {
            var providerUrl = _currencySettings.ExchangeRateProviderLink;
            var factor = _currencySettings.ExchangeRateFactor;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(providerUrl);
            request.Method = WebRequestMethods.Http.Get;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream());

            XmlDocument doc = new XmlDocument();
            doc.Load(sr);

            XmlNode header = doc.SelectSingleNode("/xe-datafeed/header[hname = 'Status']/hvalue");
            string status = header.InnerText;

            if (status.ToLower() == "ok")
            {
                StringBuilder sb = new StringBuilder();
                XmlNodeList nodes = doc.SelectNodes("/xe-datafeed/currency");
                
                for (int i = 0; i < nodes.Count; i++)
                {
                    string symbol = nodes[i]["csymbol"].InnerText;
                    string rate = Math.Round(Convert.ToDecimal(nodes[i]["crate"].InnerText), 2, MidpointRounding.AwayFromZero).ToString();
                    decimal actualRate = Convert.ToDecimal(rate) * factor;

                    sb.AppendFormat("update currencies set exchangerate = {0}, googleexchangerate = {1}, updatedonutc = getdate() where currencycode = '{2}'; ",
                                    Math.Round(actualRate, 2, MidpointRounding.AwayFromZero),
                                    rate,
                                    nodes[i]["csymbol"].InnerText);
                }

                _dbContext.ExecuteSqlCommand(sb.ToString());

                _cacheManager.RemoveByPattern(CacheKey.CURRENCY_PATTERN_KEY);
            }
        }

        public void UpdateCurrency(Currency currency)
        {
            _currencyRepository.Update(currency);
        }

        public Currency GetCurrencyByCurrencyCode(string currencyCode)
        {
            string key = string.Format(CacheKey.CURRENCY_BY_CURRENCY_CODE_KEY, currencyCode);

            var currency = _cacheManager.Get(key, delegate ()
            {
                return _currencyRepository.Table
                    .Join(_currencyCountryRepository.Table, c => c.Id, cc => cc.CurrencyId, (c, cc) => new { c, cc })
                    .Join(_countryRepository.Table, c_cc => c_cc.cc.CountryId, co => co.Id, (c_cc, co) => new { c_cc.c, c_cc.cc, co })
                    .Where(c_cc_co => c_cc_co.c.CurrencyCode == currencyCode)
                    .Select(c_cc_co => c_cc_co.c)
                    .Distinct()
                    .FirstOrDefault();
            });

            return currency;
        }

        public Currency GetCurrency(int currencyId)
        {
            string key = string.Format(CacheKey.CURRENCY_BY_ID_KEY, currencyId);

            var currency = _cacheManager.Get(key, delegate ()
            {
                return _currencyRepository.Table
                    .Join(_currencyCountryRepository.Table, c => c.Id, cc => cc.CurrencyId, (c, cc) => new { c, cc })
                    .Join(_countryRepository.Table, c_cc => c_cc.cc.CountryId, co => co.Id, (c_cc, co) => new { c_cc.c, c_cc.cc, co })
                    .Where(c_cc_co => c_cc_co.c.Id == currencyId)
                    .Select(c_cc_co => c_cc_co.c)
                    .Distinct()
                    .FirstOrDefault();
            });

            return currency;
        }

        public IList<Currency> GetAllCurrency()
        {
            string key = CacheKey.CURRENCY_ALL_KEY;
            var currencies = _cacheManager.Get(key, delegate ()
            {
                return _currencyRepository.Table.OrderBy(x => x.CurrencyCode).ToList();
            });

            return currencies;                
        }

        public PagedList<Currency> GetPagedCurrency(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string currencyCode = null)
        {
            var query = _currencyRepository.Table;

            if (!string.IsNullOrEmpty(currencyCode))
                query = query.Where(c => c.CurrencyCode.Contains(currencyCode));

            int totalRecords = query.Count();

            query = query.OrderBy(c => c.Id);

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var items = query.ToList();

            return new PagedList<Currency>(items, pageIndex, pageSize, totalRecords);
        }

        public IList<CurrencyCountry> GetCurrencyCountryByCurrencyId(int currencyId)
        {
            return _currencyCountryRepository.Table.Where(x => x.CurrencyId == currencyId).ToList();
        }

        public void DeleteCurrencyCountry(int currencyCountryId)
        {
            _currencyCountryRepository.Delete(currencyCountryId);
        }
    }
}
