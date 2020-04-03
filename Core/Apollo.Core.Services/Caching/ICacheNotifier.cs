using Apollo.Core.Model;

namespace Apollo.Core.Services.Caching
{
    public interface ICacheNotifier
    {
        bool RefreshCache(CacheEntityKey keys);
    }
}
