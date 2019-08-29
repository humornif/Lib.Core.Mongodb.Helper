//using Microsoft.Extensions.Caching.Memory;

namespace Lib.Core.Mongodb.Helper.Common
{
    //not use yet, for future

    internal class CursorCache<T>
    {
        //private static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        //private CacheItemPolicy _policy = new CacheItemPolicy();

        //public CursorCache()
        //{
        //    _policy.SlidingExpiration = new TimeSpan(0, 5, 0);
        //}

        //internal IFindFluent<T, T> GetCursorCacheData(string key)
        //{
        //    return _cache.Get(key) as IFindFluent<T, T>;
        //}

        //internal void SetCursorCacheData(string key, IFindFluent<T, T> data)
        //{
        //    _cache.Set(key, data, _policy);
        //}
    }
}
