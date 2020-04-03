using Apollo.Core;
using Apollo.Core.Caching;
using System;

namespace Apollo.FrontStore.Infrastructure
{
    public class ApolloSessionState
    {
        private readonly ICacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        public object this[string name]
        {
            get
            {
                object data;

                if (_cacheManager.TryGetCache(string.Format("profile-id-{0}.{1}", _workContext.CurrentProfile.Id, name), out data))                
                    return data;
                
                return null;
            }
            set
            {
                string key = string.Format("profile-id-{0}.{1}", _workContext.CurrentProfile.Id, name);
                if (value == null)                
                    _cacheManager.Remove(key);                
                else
                    _cacheManager.Set(key, value, new DateTimeOffset(DateTime.Now.AddMinutes(5))); // Stored for 5 minutes
            }
        }

        public ApolloSessionState(IWorkContext workContext, ICacheManager cacheManager)
        {
            _workContext = workContext;
            _cacheManager = cacheManager;
        }
    }
}