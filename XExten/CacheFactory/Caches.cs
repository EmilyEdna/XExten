using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.CacheFactory.MemoryCache;
using XExten.CacheFactory.MongoDbCache;
using XExten.CacheFactory.RedisCache;
using XExten.XCore;
using XExten.XExpres;

namespace XExten.CacheFactory
{
    /// <summary>
    /// 
    /// </summary>
    public class Caches
    {
        /// <summary>
        /// 为空默认MemoryCache(值为-1缓存类型为MongoDb,值为1缓存类型为Redis)
        /// </summary>
        public static int? CacheType { get; set; }
        /// <summary>
        /// 默认是Redis和MongoDb必填
        /// </summary>
        public static string ConnectionString { get; set; }
        /// <summary>
        /// 缓存类型为MongoDB是必填
        /// </summary>
        public static string DbName { get; set; }
        /// <summary>
        /// 添加缓存
        /// </summary>
        public static void SetCache<T>(string key, T value, int hours = 2)
        {
            try
            {
                if (CacheType.IsNullOrEmpty())
                {
                    MemoryCaches.AddCache<T>(key, value, 2);
                    return;
                }
                if (CacheType == 1)
                {
                    if (ConnectionString.IsNullOrEmpty())
                        throw new Exception("please input redis connectionstring!");
                    RedisCaches.RedisConnectionString = ConnectionString;
                    RedisCaches.StringSet<T>(key, value, (DateTime.Now.AddHours(hours) - DateTime.Now));
                    return;
                }
                if (CacheType == 2)
                {
                    if (ConnectionString.IsNullOrEmpty())
                        throw new Exception("please input mongodb connectionstring!");
                    if(DbName.IsNullOrEmpty())
                        throw new Exception("please input mongodb name!");
                    MongoDbCaches.Insert<T>(value);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取缓存
        /// (注意当使用mongoDb是不存在key，此时的查询条件Expression《Func《T,bool》》只需传入模型的字段值)
        /// (注意当使用mongoDb，参数PorpertyName必填)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetCache<T>(object key,string PorpertyName=null)
        {
            try
            {
                if (CacheType.IsNullOrEmpty())
                {
                    return MemoryCaches.GetCache<T>(key.ToString());
                }
                if (CacheType == 1)
                {
                    if (ConnectionString.IsNullOrEmpty())
                        throw new Exception("please input redis connectionstring!");
                    RedisCaches.RedisConnectionString = ConnectionString;
                    return RedisCaches.StringGet<T>(key.ToString());
                }
                if (CacheType == 2)
                {
                    if (ConnectionString.IsNullOrEmpty())
                        throw new Exception("please input mongodb connectionstring!");
                    if (DbName.IsNullOrEmpty())
                        throw new Exception("please input mongodb name!");
                    if (PorpertyName.IsNullOrEmpty())
                        throw new Exception("please input PorpertyName!");
                    return MongoDbCaches.Search<T>(XExp.GetExpression<T>(PorpertyName, key, QType.Equals));
                }
                return default;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///  删除某个缓存
        ///  (注意当缓存类型不为mongoDb,泛型参数可随意填写)
        ///  (注意当使用mongoDb，参数PorpertyName必填)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="PorpertyName"></param>
        public static void RemoveCache<T>(string key, string PorpertyName = null)
        {
            try
            {
                if (CacheType.IsNullOrEmpty())
                {
                    MemoryCaches.RemoveCache(key);
                    return;
                }
                if (CacheType == 1)
                {
                    if (ConnectionString.IsNullOrEmpty())
                        throw new Exception("please input redis connectionstring!");
                    RedisCaches.RedisConnectionString = ConnectionString;
                    RedisCaches.KeyDelete(key);
                    return;
                }
                if (CacheType == 2)
                {
                    if (ConnectionString.IsNullOrEmpty())
                        throw new Exception("please input mongodb connectionstring!");
                    if (DbName.IsNullOrEmpty())
                        throw new Exception("please input mongodb name!");
                    if (PorpertyName.IsNullOrEmpty())
                        throw new Exception("please input PorpertyName!");
                    MongoDbCaches.Delete<T>(XExp.GetExpression<T>(PorpertyName, key, QType.Equals));
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
