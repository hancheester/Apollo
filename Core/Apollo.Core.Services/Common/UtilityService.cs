using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Caching;
using Apollo.Core.Services.Directory;
using Apollo.Core.Services.Directory.IP2Country;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Apollo.Core.Services.Common
{
    public class UtilityService : BaseRepository, IUtilityService
    {
        #region Fields
        
        private readonly IRepository<Testimonial> _testimonialRepository;
        private readonly IRepository<CustomDictionary> _customDictionaryRepository;
        private readonly IRepository<StockNotification> _stockNotificationRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IIPToCountry _ipToCountry;
        private readonly ISettingService _settings;
        private readonly IEmailManager _emailManager;
        private readonly ICacheNotifier _cacheNotifier;
        private readonly IEnumerable<Lazy<ICacheManager, ICacheManagerMetadata>> _cacheManagers;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductBuilder _productBuilder;
        private readonly CacheSettings _cacheSettings;
        private readonly Regex _regexValidIPv4Address = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\b", RegexOptions.Compiled);
        private readonly object _logger;

        #endregion

        #region Ctor

        public UtilityService(            
            IRepository<Testimonial> testimonialRepository,
            IRepository<CustomDictionary> customDictionaryRepository,
            IRepository<StockNotification> stockNotificationRepository,
            IRepository<Product> productRepository,
            IRepository<ProductPrice> productPriceRepository,
            IIPToCountry ipToCountry,
            ILogBuilder logBuilder,
            ISettingService settings,
            IEmailManager emailManager,
            ICacheNotifier cacheNotifier,
            IEnumerable<Lazy<ICacheManager, ICacheManagerMetadata>> cacheManagers,
            ISitemapGenerator sitemapGenerator,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            IProductBuilder productBuilder,
            CacheSettings cacheSettings)
        {
            _testimonialRepository = testimonialRepository;
            _customDictionaryRepository = customDictionaryRepository;
            _stockNotificationRepository = stockNotificationRepository;
            _productRepository = productRepository;
            _productPriceRepository = productPriceRepository;
            _ipToCountry = ipToCountry;
            _settings = settings;
            _emailManager = emailManager;
            _cacheNotifier = cacheNotifier;
            _cacheManagers = cacheManagers;
            _sitemapGenerator = sitemapGenerator;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _productBuilder = productBuilder;
            _cacheSettings = cacheSettings;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion
        
        #region Create
        
        public int InsertCurrency(Currency item)
        {
            return _currencyService.InsertCurrency(item);
        }

        public int InsertCurrencyCountry(CurrencyCountry item)
        {
            return _currencyService.InsertCurrencyCountry(item);
        }
        
        public int InsertTestimonial(Testimonial testimonial)
        {
            return _testimonialRepository.Create(testimonial);
        }

        #endregion

        #region Return

        public IList<CustomDictionary> GetAllCustomDictionary()
        {
            return _customDictionaryRepository.Table.ToList();
        }
        
        public IList<GenericAttribute> GetAttributesForEntity(int entityId, string keyGroup)
        {
            return _genericAttributeService.GetAttributesForEntity(entityId, keyGroup);
        }
        
        public PagedList<Currency> GetPagedCurrency(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string currencyCode = null)
        {
            return _currencyService.GetPagedCurrency(
                pageIndex,
                pageSize,
                currencyCode);
        }

        public IList<CurrencyCountry> GetCurrencyCountryByCurrencyId(int currencyId)
        {
            return _currencyService.GetCurrencyCountryByCurrencyId(currencyId);
        }
        
        public Testimonial GetTestimonial(int testimonialId)
        {
            return _testimonialRepository.Return(testimonialId);
        }

        public PagedList<Testimonial> GetTestimonialLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string comment = null,
            string name = null,
            TestimonialSortingType orderBy = TestimonialSortingType.IdAsc)
        {
            var query = _testimonialRepository.Table;

            if (!string.IsNullOrEmpty(comment))
                query = query.Where(x => x.Comment.Contains(comment));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));

            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case TestimonialSortingType.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case TestimonialSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case TestimonialSortingType.NameAsc:
                    query = query.OrderBy(x => x.Name);
                    break;
                case TestimonialSortingType.NameDesc:
                    query = query.OrderByDescending(x => x.Name);
                    break;
                case TestimonialSortingType.CommentAsc:
                    query = query.OrderBy(x => x.Comment);
                    break;
                case TestimonialSortingType.CommentDesc:
                    query = query.OrderByDescending(x => x.Comment);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList();

            return new PagedList<Testimonial>(list, pageIndex, pageSize, totalRecords);
        }
        
        public Currency GetCurrency(int currencyId)
        {
            return _currencyService.GetCurrency(currencyId);
        }

        public Currency GetCurrencyByCurrencyCode(string currencyCode)
        {
            return _currencyService.GetCurrencyByCurrencyCode(currencyCode);
        }
     
        public IList<Currency> GetAllCurrency()
        {
            return _currencyService.GetAllCurrency();
        }

        #endregion

        #region Update

        public void UpdateTestimonial(Testimonial testimonial)
        {
            _testimonialRepository.Update(testimonial);
        }
        
        public void UpdateCurrency(Currency currency)
        {
            _currencyService.UpdateCurrency(currency);
        }

        #endregion

        #region Command

        public string ConvertIPToCountryName(string ip)
        {
            string country = null;

            if (_regexValidIPv4Address.IsMatch(ip))
                country = _ipToCountry.ConvertIPToCountry(ip);

            return country;
        }

        public void SaveAttribute(int entityId, string entityName, string key, string value, int storeId = 0)
        {
            _genericAttributeService.SaveAttribute(entityId, entityName, key, value, storeId);          
        }

        public void ProcessContactUs(string contactUsEmail, string email, string name, string message)
        {
            if (string.IsNullOrWhiteSpace(contactUsEmail)) return;
            if (string.IsNullOrWhiteSpace(email)) return;
            if (string.IsNullOrWhiteSpace(name)) return;
            if (string.IsNullOrWhiteSpace(message)) return;
                        
            _emailManager.SendEmailAsync(contactUsEmail, 
                                         "Contact Us Message", 
                                         string.Format("From: {0}<br/>Email: {1}<br/>Message:<br/>{2}", name, email, message));
        }

        public void ProcessHelpAlternativeProduct(string contactUsEmail, string email, int productId)
        {
            if (string.IsNullOrWhiteSpace(contactUsEmail)) return;
            if (string.IsNullOrWhiteSpace(email)) return;
            
            _emailManager.SendEmailAsync(contactUsEmail,
                                         "Alternative Product Help Message",
                                         string.Format("Please help this customer for alternative products. <br/><br/>Email: {0}<br/>Product ID: {1}", email, productId));
        }

        public void ProcessHelpStockNofification(string email, int productId, int productPriceId)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            email = email.ToLower();

            var notification = _stockNotificationRepository.Table
                .Where(x => x.Email.ToLower() == email)
                .Where(x => x.ProductId == productId)
                .Where(x => x.ProductPriceId == productPriceId)
                .FirstOrDefault();
               
            if (notification != null)
            {
                notification.Notified = false;
                notification.UpdatedOnDate = DateTime.Now;

                _stockNotificationRepository.Update(notification);
            }
            else
            {
                var newNotification = new StockNotification
                {
                    Email = email,
                    ProductId = productId,
                    ProductPriceId = productPriceId,
                    CreatedOnDate = DateTime.Now,
                    UpdatedOnDate = DateTime.Now
                };

                _stockNotificationRepository.Create(newNotification);
            }            
        }

        public void NotifyUserForStock()
        {
            var items = _stockNotificationRepository.TableNoTracking
                .Join(_productPriceRepository.Table, s => s.ProductId, pp => pp.ProductId, (s, pp) => new { s, pp })
                .Join(_productRepository.Table, x => x.s.ProductId, p => p.Id, (x, p) => new { x.s, p, x.pp })
                .Where(x => x.s.Notified == false)
                .Where(x => x.p.Enabled == true)
                .Where(x => x.pp.Stock > 0)
                .Where(x => x.pp.Enabled == true)
                .Select(x => new { x.s, x.p })
                .ToList()
                .Select(x => new { x.s, p = _productBuilder.Build(x.p) })
                .ToList();

            foreach (var item in items)
            {
                var option = string.Empty;
                var proceed = false;

                if (item.s.ProductPriceId > 0 && item.p.ProductPrices.Where(x => x.Id == item.s.ProductPriceId).Any())
                {
                    option = item.p.ProductPrices.Where(x => x.Id == item.s.ProductPriceId).Select(x => x.Option).FirstOrDefault();
                    proceed = true;
                }
                else if (item.s.ProductPriceId == 0)
                {
                    proceed = true;
                }
                
                if (proceed)
                {
                    item.s.Notified = true;
                    item.s.UpdatedOnDate = DateTime.Now;
                    _emailManager.SendBackInStockEmail(item.s.Email, item.p.Name, item.p.UrlRewrite, option, item.p.ProductMedias[0].MediaFilename);

                    _stockNotificationRepository.Update(item.s);
                }                
            }
                
        }

        public bool RefreshCache(CacheEntityKey keys)
        {
            var cacheManagers = new List<ICacheManager>();
            var memoryCacheManager = _cacheManagers.FirstOrDefault(t => t.Metadata.Type == CacheManagerType.Memory);
            if (memoryCacheManager != null && memoryCacheManager.Value != null) cacheManagers.Add(memoryCacheManager.Value);
            var dacheCacheManager = _cacheManagers.FirstOrDefault(t => t.Metadata.Type == CacheManagerType.Dache);
            if (dacheCacheManager != null && dacheCacheManager.Value != null) cacheManagers.Add(dacheCacheManager.Value);

            if ((keys & CacheEntityKey.Product) == CacheEntityKey.Product)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.PRODUCT_PATTERN_KEY);
                RemoveCacheByPattern(cacheManagers, CacheKey.PRODUCT_PRICE_PATTERN_KEY);
                RemoveCacheByPattern(cacheManagers, CacheKey.COLOUR_PATTERN_KEY);
            }

            if ((keys & CacheEntityKey.Brand) == CacheEntityKey.Brand)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.BRAND_PATTERN_KEY);
                RemoveCacheByPattern(cacheManagers, CacheKey.BRAND_CATEGORY_PATTERN_KEY);

                RemoveCacheByPattern(cacheManagers, CacheKey.PRODUCT_PATTERN_KEY);
                RemoveCacheByPattern(cacheManagers, CacheKey.PRODUCT_PRICE_PATTERN_KEY);
            }

            if ((keys & CacheEntityKey.Category) == CacheEntityKey.Category)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.CATEGORY_PATTERN_KEY);
                RemoveCacheByPattern(cacheManagers, CacheKey.CATEGORY_FILTER_PATTERN_KEY);

                RemoveCacheByPattern(cacheManagers, CacheKey.PRODUCT_PATTERN_KEY);
                RemoveCacheByPattern(cacheManagers, CacheKey.PRODUCT_PRICE_PATTERN_KEY);
            }

            if ((keys & CacheEntityKey.Offer) == CacheEntityKey.Offer)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.OFFER_RULE_PATTERN_KEY);
            }

            if ((keys & CacheEntityKey.Campaign) == CacheEntityKey.Campaign)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.LARGE_BANNER_PATTERN_KEY);
            }
            
            if ((keys & CacheEntityKey.LargeBanner) == CacheEntityKey.LargeBanner)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.LARGE_BANNER_PATTERN_KEY);
            }

            if ((keys & CacheEntityKey.Currency) == CacheEntityKey.Currency)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.CURRENCY_PATTERN_KEY);
            }

            if ((keys & CacheEntityKey.Country) == CacheEntityKey.Country)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.COUNTRY_PATTERN_KEY);
            }

            if ((keys & CacheEntityKey.CustomDictionary) == CacheEntityKey.CustomDictionary)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.CUSTOM_DICTIONARY_PATTERN_KEY);
            }

            if ((keys & CacheEntityKey.Blog) == CacheEntityKey.Blog)
            {
                RemoveCacheByPattern(cacheManagers, CacheKey.BLOG_PATTERN_KEY);
            }

            return _cacheNotifier.RefreshCache(keys);
        }

        public byte[] GenerateSitemap()
        {
            return _sitemapGenerator.BuildSitemap();
        }

        public IDictionary<string, string> GetCachePerformanceData(string type)
        {
            switch (type)
            {
                case "service_memory_cache_manager":
                    var cacheManager1 = _cacheManagers.FirstOrDefault(t => t.Metadata.Type == CacheManagerType.Memory);
                    if (cacheManager1 == null || cacheManager1.Value == null) return new Dictionary<string, string>();
                    return cacheManager1.Value.GetPerformanceData();

                case "service_dache_cache_manager":
                    var cacheManager2 = _cacheManagers.FirstOrDefault(t => t.Metadata.Type == CacheManagerType.Dache);
                    if (cacheManager2 == null || cacheManager2.Value == null) return new Dictionary<string, string>();
                    return cacheManager2.Value.GetPerformanceData();

                case "store_memory_cache_manager":
                    var storeFrontGetCachePerfLink = _cacheSettings.StoreFrontGetPerfDataLink;
                    var token = _cacheSettings.StoreFrontToken;

                    string requestUriString = string.Format("{0}/{1}", storeFrontGetCachePerfLink, token);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);

                    request.Method = WebRequestMethods.Http.Get;
                    //request.Timeout = 30000;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            var objText = reader.ReadToEnd();
                            return (IDictionary<string, string>)js.Deserialize(objText, typeof(IDictionary<string, string>));
                        }
                    }
                    
                default:
                    break;
            }

            return new Dictionary<string, string>();
        }

        public IList<string> GetCacheKeys(string type)
        {
            switch (type)
            {
                case "service_memory_keys_manager":
                    var cacheManager1 = _cacheManagers.FirstOrDefault(t => t.Metadata.Type == CacheManagerType.Memory);
                    if (cacheManager1 == null || cacheManager1.Value == null) return new List<string>();
                    return cacheManager1.Value.GetCacheKeys();

                case "service_dache_keys_manager":
                    var cacheManager2 = _cacheManagers.FirstOrDefault(t => t.Metadata.Type == CacheManagerType.Dache);
                    if (cacheManager2 == null || cacheManager2.Value == null) return new List<string>();
                    return cacheManager2.Value.GetCacheKeys();

                case "store_memory_keys_manager":
                    var storeFrontGetCacheKeysLink = _cacheSettings.StoreFrontGetCacheKeysLink;
                    var token = _cacheSettings.StoreFrontToken;

                    string requestUriString = string.Format("{0}/{1}", storeFrontGetCacheKeysLink, token);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);

                    request.Method = WebRequestMethods.Http.Get;
                    //request.Timeout = 30000;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            var objText = reader.ReadToEnd();
                            return (IList<string>)js.Deserialize(objText, typeof(IList<string>));
                        }
                    }
                default:
                    break;
            }

            return new List<string>();
        }

        #endregion

        #region Delete

        public void DeleteTestimonial(int testimonialId)
        {
            _testimonialRepository.Delete(testimonialId);
        }

        public void DeleteCurrencyCountry(int currencyCountryId)
        {
            _currencyService.DeleteCurrencyCountry(currencyCountryId);
        }
        
        #endregion

        private void RemoveCacheByPattern(IList<ICacheManager> cacheManagers, string pattern)
        {
            if (cacheManagers.Count > 0)
            {
                for(int i = 0; i < cacheManagers.Count; i++)
                {
                    cacheManagers[i].RemoveByPattern(pattern);
                }
            }            
        }
    }
}
