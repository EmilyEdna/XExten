using System;
using System.Collections;

#if NET461

using System.Web;
using System.Web.Caching;

#elif NETSTANDARD2_1
using Microsoft.Extensions.Caching.Memory;
#endif

namespace XExten.CacheFactory.RunTimeCache
{
    /// <summary>
    ///
    /// </summary>
    public class MemoryCaches
    {
#if NETSTANDARD2_1
        public static IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
#endif

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="Time"></param>
        /// <param name="UseSecond"></param>
        public static void AddCache<T>(String Key, T Value, int Time, bool UseSecond = false)
        {
#if NET461
            HttpRuntime.Cache.Insert(Key, Value, null,(UseSecond? DateTime.Now.AddSeconds(Time) : DateTime.Now.AddMinutes(Time)), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
#elif NETSTANDARD2_1
            Cache.Set(Key, Value, new DateTimeOffset((UseSecond ? DateTime.Now.AddSeconds(Time) : DateTime.Now.AddMinutes(Time))));
#endif
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static T GetCache<T>(String Key)
        {
#if NET461
            return HttpRuntime.Cache.Get(Key) == null ? default(T) : (T)HttpRuntime.Cache.Get(Key);
#elif NETSTANDARD2_1
            return Cache.Get(Key) == null ? default(T) : (T)Cache.Get(Key);
#endif
        }

        /// <summary>
        /// 删除指定缓存
        /// </summary>
        /// <param name="Key"></param>
        public static void RemoveCache(String Key)
        {
#if NET461
            HttpRuntime.Cache.Remove(Key);
#elif NETSTANDARD2_1
            Cache.Remove(Key);
#endif
        }

#if NET461

        public static void RemoveAllCache()
        {
            var CacheEnum = HttpRuntime.Cache.GetEnumerator();
            ArrayList arr = new ArrayList();
            while (CacheEnum.MoveNext())
            {
                arr.Add(CacheEnum.Key);
            }
            foreach (string key in arr)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }

#endif
    }
}