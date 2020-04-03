using Apollo.Core;
using Apollo.Core.Configuration;
using Apollo.Core.Infrastructure;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;

namespace Apollo.Web.Framework.Services.Helpers
{
    /// <summary>
    /// User agent helper
    /// </summary>
    public partial class UserAgentHelper : IUserAgentHelper
    {
        private readonly ApolloConfig _config;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="webHelper">Web helper</param>
        /// <param name="httpContext">HTTP context</param>
        public UserAgentHelper(ApolloConfig config, IWebHelper webHelper, HttpContextBase httpContext)
        {
            this._config = config;
            this._webHelper = webHelper;
            this._httpContext = httpContext;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual BrowscapXmlHelper GetBrowscapXmlHelper()
        {
            if (Singleton<BrowscapXmlHelper>.Instance != null)
                return Singleton<BrowscapXmlHelper>.Instance;

            //no database created
            if (string.IsNullOrEmpty(_config.UserAgentStringsPath))
                return null;

            var filePath = CommonHelper.MapPath(_config.UserAgentStringsPath);
            var bowscapXmlHelper = new BrowscapXmlHelper(filePath);

            Singleton<BrowscapXmlHelper>.Instance = bowscapXmlHelper;
            return Singleton<BrowscapXmlHelper>.Instance;
        }
        
        /// <summary>
        /// Get a value indicating whether the request is made by search engine (web crawler)
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool IsSearchEngine()
        {
            if (_httpContext == null)
                return false;

            bool result = false;
            try
            {
                var bowscapXmlHelper = GetBrowscapXmlHelper();

                //we cannot load parser
                if (bowscapXmlHelper == null)
                    return false;

                var userAgent = _httpContext.Request.UserAgent;
                return bowscapXmlHelper.IsCrawler(userAgent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }

    }
}
