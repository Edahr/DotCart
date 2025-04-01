using Microsoft.Extensions.Caching.Memory;

namespace DotCart.Infrastructure.Cache
{
    public class MemoryCacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key)
        {
            return _memoryCache.Get<T?>(key);
        }

        public void Set<T>(string key, T? value, TimeSpan expiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expiration); // Use sliding expiration
            _memoryCache.Set(key, value, cacheEntryOptions);
        }

        public bool TryGet<T>(string key, out T? value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
