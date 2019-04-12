using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.CacheFactory.RunTimeCache;
using XExten.CacheFactory.MongoDbCache;
using XExten.CacheFactory.RedisCache;
using XExten.XCore;
using XExten.XExpres;
using System.Linq.Expressions;

namespace XExten.CacheFactory
{
    /// <summary>
    /// 
    /// </summary>
    public class Caches
    {
        /// <summary>
        /// Redis链接字符串
        /// </summary>
        public static string RedisConnectionString
        {
            set{ RedisCaches.RedisConnectionString = value;}
        }
        /// <summary>
        /// MongoDB链接字符串
        /// </summary>
        public static string MongoDBConnectionString
        {
            set
            { MongoDbCaches.MongoDBConnectionString = value;}
          }
        /// <summary>
        /// 缓存类型为MongoDB是必填
        /// </summary>
        public static string DbName
        {
            set { MongoDbCaches.MongoDBName = value; }
        }
        /// <summary>
        /// 添加Memory缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Minutes"></param>
        public static void RunTimeCacheSet<T>(string key, T value, int Minutes = 5)
        {
            MemoryCaches.AddCache<T>(key, value, Minutes);
        }
        /// <summary>
        /// 添加Redis缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Minutes"></param>
        public static void RedisCacheSet<T>(string key, T value, int Minutes = 5)
        {
            RedisCaches.StringSet<T>(key, value, (DateTime.Now.AddMinutes(Minutes) - DateTime.Now));
        }
        /// <summary>
        /// 添加MongoDB缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public static void MongoDBCacheSet<T>(T value)
        {
            MongoDbCaches.Insert<T>(value);
        }
        /// <summary>
        /// 获取Memory缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T RunTimeCacheGet<T>(object key)
        {
            return MemoryCaches.GetCache<T>(key.ToString());
        }
        /// <summary>
        /// 获取Redis缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T RedisCacheGet<T>(object key)
        {
            return RedisCaches.StringGet<T>(key.ToString());
        }
        /// <summary>
        /// 获取MongoDB缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Exp"></param>
        /// <returns></returns>
        public static T MongoDBCacheGet<T>(Expression<Func<T,bool>> Exp)
        {
            return MongoDbCaches.Search<T>(Exp);
        }
        /// <summary>
        ///  删除Memory缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RunTimeCacheRemove(object key)
        {
            MemoryCaches.RemoveCache(key.ToString());
        }
        /// <summary>
        /// 删除Redis缓存
        /// </summary>
        /// <param name="key"></param>
        public static void RedisCacheRemove(object key)
        {
            RedisCaches.KeyDelete(key.ToString());
        }
        /// <summary>
        /// 删除MongoDB缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Exp"></param>
        public static void MongoDBCacheRemove<T>(Expression<Func<T, bool>> Exp)
        {
            MongoDbCaches.Delete<T>(Exp);
        }
    }
}
