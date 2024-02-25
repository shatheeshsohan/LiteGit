using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace LiteGit.BaseCache
{
    class SCache
    {
        private static readonly MemoryCache _cache = MemoryCache.Default;

        //Store Stuff in the cache  
        private static void StoreItemsInCache(string key, string value)
        {

            var cacheItemPolicy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTime.Now.AddDays(365)
            };

            _cache.Add(key, value, cacheItemPolicy);
        }

        //Get stuff from the cache
        public static void SetItemsForCache(string key, string value)
        {
            if (!_cache.Contains(key))
            {
                StoreItemsInCache(key, value);
            }
        }

        public static string GetItemsFromCache(string key)
        {
            return (string)_cache.Get(key);
        }

        public static void RemoveItemsFromCache(string _key)
        {
            _cache.Remove(_key);
        }
    }
}
