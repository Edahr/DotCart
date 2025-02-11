namespace Sheyaaka.Infrastructure.Cache
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
        bool TryGet<T>(string key, out T value);
        void Remove(string key);
    }
}
