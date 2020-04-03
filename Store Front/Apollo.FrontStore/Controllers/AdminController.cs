using Apollo.Core.Caching;
using Apollo.Web.Framework.Security;
using System.Web;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class AdminController : BasePublicController
    {
        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly ICacheManager _cacheManager;
        private readonly CacheSettings _cacheSettings;

        #endregion
        
        #region Constants

        private const string BRAND = "brand";
        private const string CATEGORY = "category";
        private const string PRODUCT = "product";
        private const string LARGE_BANNER = "largebanner";
        private const string CURRENCY = "currency";
        private const string COUNTRY = "country";
        private const string SETTING = "setting";
        private const string OFFER = "offer";
        private const string SHIPPING_OPTION = "shippingoption";
        private const string BLOG = "blog";
        private const string WIDGET = "widget";        
        
        #endregion

        #region Ctor
        
        public AdminController(
            HttpContextBase httpContext,
            CacheSettings cacheSettings,
            ICacheManager cacheManager)
        {
            _httpContext = httpContext;
            _cacheSettings = cacheSettings;
            _cacheManager = cacheManager;
        }

        #endregion

        [HttpGet]
        public ActionResult RefreshCache(string entity, string token)
        {
            if (string.IsNullOrEmpty(entity)) return InvokeHttp404();
            if (string.IsNullOrEmpty(token)) return InvokeHttp404();

            var appToken = _cacheSettings.StoreFrontToken;

            if (appToken.ToLower() != token.ToLower()) return InvokeHttp404();

            switch (entity)
            {
                case PRODUCT:
                    _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
                    _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PRICE_PATTERN_KEY);
                    _cacheManager.RemoveByPattern(CacheKey.COLOUR_PATTERN_KEY);
                    break;

                case BRAND:
                    _cacheManager.RemoveByPattern(CacheKey.BRAND_PATTERN_KEY);
                    _cacheManager.RemoveByPattern(CacheKey.BRAND_CATEGORY_PATTERN_KEY);

                    _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
                    _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PRICE_PATTERN_KEY);                    
                    break;

                case CATEGORY:
                    _cacheManager.RemoveByPattern(CacheKey.CATEGORY_PATTERN_KEY);
                    _cacheManager.RemoveByPattern(CacheKey.CATEGORY_FILTER_PATTERN_KEY);

                    _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PATTERN_KEY);
                    _cacheManager.RemoveByPattern(CacheKey.PRODUCT_PRICE_PATTERN_KEY);

                    _cacheManager.RemoveByPattern(CacheKey.CHILD_ACTION_CACHE_PATTERN_KEY);

                    // We need to specify parameter to remove output cache item.
                    //_httpContext.Response.RemoveOutputCacheItem(Url.Action("Category", "Category"));
                    //_httpContext.Response.RemoveOutputCacheItem(Url.Action("CategoryWithProducts", "Category"));                    
                    break;

                case SETTING:
                    _cacheManager.RemoveByPattern(CacheKey.SETTING_PATTERN_KEY);
                    break;

                case LARGE_BANNER:
                    _cacheManager.RemoveByPattern(CacheKey.LARGE_BANNER_PATTERN_KEY);
                    _cacheManager.RemoveByPattern(CacheKey.CHILD_ACTION_CACHE_PATTERN_KEY);
                    break;

                case CURRENCY:
                    _cacheManager.RemoveByPattern(CacheKey.CURRENCY_PATTERN_KEY);
                    _cacheManager.RemoveByPattern(CacheKey.CHILD_ACTION_CACHE_PATTERN_KEY);                    
                    break;

                case COUNTRY:
                    _cacheManager.RemoveByPattern(CacheKey.COUNTRY_PATTERN_KEY);
                    break;

                case OFFER:
                    _cacheManager.RemoveByPattern(CacheKey.OFFER_RULE_PATTERN_KEY);
                    break;

                case SHIPPING_OPTION:
                    _cacheManager.RemoveByPattern(CacheKey.SHIPPING_OPTION_PATTERN_KEY);
                    break;

                case BLOG:
                    _cacheManager.RemoveByPattern(CacheKey.BLOG_PATTERN_KEY);
                    break;

                case WIDGET:
                    _cacheManager.RemoveByPattern(CacheKey.WIDGET_PATTERN_KEY);
                    break;

                default:
                    break;
            }

            _cacheManager.RemoveByPattern(CacheKey.URL_PATTERN_KEY);

            ViewBag.Message = string.Format("Entity {0} cache has been refreshed.", entity);

            return View();
        }

        [HttpGet]
        public ActionResult GetCachePerformanceData(string token)
        {
            if (string.IsNullOrEmpty(token)) return InvokeHttp404();

            var appToken = _cacheSettings.StoreFrontToken;

            if (appToken.ToLower() != token.ToLower()) return InvokeHttp404();

            var data = _cacheManager.GetPerformanceData();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCacheKeys(string token)
        {
            if (string.IsNullOrEmpty(token)) return InvokeHttp404();

            var appToken = _cacheSettings.StoreFrontToken;

            if (appToken.ToLower() != token.ToLower()) return InvokeHttp404();

            var data = _cacheManager.GetCacheKeys();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}