using Apollo.Core.Caching;
using Apollo.Core.Infrastructure;
using System.Web.Mvc;

namespace Apollo.Web.Framework
{
    public static class UrlHelperExtensions
    {
        public static string GenerateAbsoluteUrl(this UrlHelper helper, string path, bool forceHttps = false)
        {
            var cacheManager = EngineContext.Current.ContainerManager.ResolveNamed<ICacheManager>("Apollo_cache_static");

            var key = string.Format(CacheKey.URL_PATH_KEY, path, forceHttps.ToString());
            var url = cacheManager.Get(key, delegate ()
            {
                const string HTTPS = "https";
                var uri = helper.RequestContext.HttpContext.Request.Url;
                var scheme = forceHttps ? HTTPS : uri.Scheme;
                var host = uri.Host;
                var port = (forceHttps || uri.Scheme == HTTPS) ? string.Empty : (uri.Port == 80 ? string.Empty : ":" + uri.Port);

                return string.Format("{0}://{1}{2}/{3}", scheme, host, port, string.IsNullOrEmpty(path) ? string.Empty : path.TrimStart('/'));
            });

            return url;
        }
    }
}
