using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using System;
using System.Net;

namespace Apollo.Core.Services.Caching
{
    public class CacheNotifier : ICacheNotifier
    {
        #region Constants

        private const string PRODUCT = "product";
        private const string BRAND = "brand";
        private const string CATEGORY = "category";
        private const string OFFER = "offer";
        private const string CAMPAIGN = "campaign";
        private const string SETTING = "setting";
        private const string LARGE_BANNER = "largebanner";
        private const string CURRENCY = "currency";
        private const string COUNTRY = "country";
        private const string SHIPPING_OPTION = "shippingoption";
        private const string BLOG = "blog";
        private const string WIDGET = "widget";

        #endregion

        private readonly ILogger _logger;
        private readonly CacheSettings _cacheSettings;

        public CacheNotifier(CacheSettings cacheSettings, ILogBuilder logBuilder)
        {
            if (logBuilder == null) throw new ArgumentException("logBuilder");
            _logger = logBuilder.CreateLogger(GetType().FullName);
            _cacheSettings = cacheSettings;
        }

        public bool RefreshCache(CacheEntityKey keys)
        {
            bool result = true;
            bool lastResult = true;
            if ((keys & CacheEntityKey.Product) == CacheEntityKey.Product) result = Call(PRODUCT);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Brand) == CacheEntityKey.Brand) result = Call(BRAND);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Category) == CacheEntityKey.Category) result = Call(CATEGORY);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Offer) == CacheEntityKey.Offer) result = Call(OFFER);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Campaign) == CacheEntityKey.Campaign) result = Call(CAMPAIGN);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Setting) == CacheEntityKey.Setting) result = Call(SETTING);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.LargeBanner) == CacheEntityKey.LargeBanner) result = Call(LARGE_BANNER);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Currency) == CacheEntityKey.Currency) result = Call(CURRENCY);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Country) == CacheEntityKey.Country) result = Call(COUNTRY);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.ShippingOption) == CacheEntityKey.ShippingOption) result = Call(SHIPPING_OPTION);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Blog) == CacheEntityKey.Blog) result = Call(BLOG);
            if (!result) lastResult = result;
            if ((keys & CacheEntityKey.Widget) == CacheEntityKey.Blog) result = Call(WIDGET);
            if (!result) lastResult = result;

            return lastResult;
        }

        private bool Call(string entity)
        {
            string storeFrontRefreshCacheLink = _cacheSettings.StoreFrontRefreshCacheLink;
            if (string.IsNullOrEmpty(storeFrontRefreshCacheLink)) throw new ApolloException("Setting for storeFrontRefreshCacheURL is empty.");
            string storeFrontToken = _cacheSettings.StoreFrontToken;
            if (string.IsNullOrEmpty(storeFrontToken)) throw new ApolloException("Setting StoreFrontToken is empty.");

            try
            {
                string requestUriString = string.Format(storeFrontRefreshCacheLink + "/{0}/{1}", entity, storeFrontToken);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                request.Method = WebRequestMethods.Http.Get;
                //request.Timeout = 30000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return true;
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Failed to call store front refresh cache URL. Store Front Refresh Cache URL={{{0}}}, Entity={{{1}}}, Token={{{2}}}", storeFrontRefreshCacheLink, entity, storeFrontToken), ex);
                return false;
            }
        }
    }
}
