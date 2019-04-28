using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace XExten.CacheFactory.RedisCache
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisCaches
    {
        #region Redis
        public static ConfigurationOptions Options => new ConfigurationOptions(){EndPoints = { RedisConnectionString },AllowAdmin = true};
        private static readonly object locker = new object();
        private static ConnectionMultiplexer instance;
        /// <summary>
        /// 链接字符串
        /// </summary>
        public static string RedisConnectionString { get; set; }
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = GetInstace();
                    }
                }
                return instance;
            }
        }
        private static ConnectionMultiplexer GetInstace()
        {
            var connect = ConnectionMultiplexer.Connect(Options);
            #region 注册事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            #endregion

            return connect;
        }
        #endregion

        #region 管理员方法 删除所有redis数据库
        /// <summary>
        /// 删除所有redis库
        /// </summary>
        public static void DeleteRedisDataBase()
        {
            Instance.GetServer(RedisConnectionString).FlushAllDatabases();
        }
        /// <summary>
        /// 异步删除所有redis库
        /// </summary>
        public static async Task DeleteRedisDataBaseAsync()
        {
            await Instance.GetServer(RedisConnectionString).FlushAllDatabasesAsync();
        }
        #endregion

        #region Redis事件
        /// <summary>
        /// 内部异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            throw new Exception("内部异常：" + e.Exception.Message);
        }

        /// <summary>
        /// 集群更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            throw new Exception("新集群：" + e.NewEndPoint + "旧集群：" + e.OldEndPoint);
        }

        /// <summary>
        /// 配置更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            throw new Exception("配置更改：" + e.EndPoint);
        }

        /// <summary>
        /// 错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            throw new Exception("异常信息：" + e.Message);
        }

        /// <summary>
        /// 重连错误事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            throw new Exception("重连错误" + e.EndPoint);
        }

        /// <summary>
        /// 连接失败事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            throw new Exception("连接异常" + e.EndPoint + "，类型为" + e.FailureType + (e.Exception == null ? "" : ("，异常信息是" + e.Exception.Message)));
        }
        #endregion

        #region redis方法
        /// <summary>
        /// 保存通用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static T Save<T>(Func<IDatabase, T> exp)
        {
            return exp(Instance.GetDatabase());
        }
        /// <summary>
        /// Redis转String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ConvertToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// Redis值转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertToObj<T>(RedisValue value)
        {
            return value.IsNull ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
        /// <summary>
        /// Redis值数组转list集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static List<T> ConvertList<T>(RedisValue[] val)
        {
            List<T> result = new List<T>();
            foreach (var item in val)
            {
                var model = ConvertToObj<T>(item);
                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// 集合转key
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static RedisKey[] ConvertRedisKeys(List<string> val)
        {
            return val.Select(key => (RedisKey)key).ToArray();
        }
        #endregion

        #region Redis String
        #region 同步执行
        /// <summary>
        /// 单个保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">值</param>
        /// <param name="exp">过期时间</param>
        /// <returns></returns>
        public static bool StringSet(string key, string val, TimeSpan? exp = default(TimeSpan?))
        {
            return Save(db => db.StringSet(key, val, exp));
        }
        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool StringSet<T>(string key, T obj, TimeSpan? exp = default(TimeSpan?))
        {
            return Save(db => db.StringSet(key, ConvertToJson(obj), exp));
        }
        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string StringGet(string key)
        {
            return Save(db => db.StringGet(key));
        }
        /// <summary>
        /// 获取单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T StringGet<T>(string key)
        {
            if (!string.IsNullOrEmpty(key))
                return ConvertToObj<T>(Save(db => db.StringGet(key)));
            else
                return default(T);
        }
        #endregion
        #region 异步执行
        /// <summary>
        /// 异步保存单个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static async Task<bool> StringSetAsync(string key, string val, TimeSpan? exp = default(TimeSpan?))
        {
            return await Save(db => db.StringSetAsync(key, val, exp));
        }
        /// <summary>
        /// 异步保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static async Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? exp = default(TimeSpan?))
        {
            return await Save(db => db.StringSetAsync(key, ConvertToJson(obj), exp));
        }
        /// <summary>
        /// 异步获取单个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<string> StringGetAsync(string key)
        {
            return await Save(db => db.StringGetAsync(key));
        }
        /// <summary>
        /// 异步获取单个对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> StringGetAsync<T>(string key)
        {
            return ConvertToObj<T>(await Save(db => db.StringGetAsync(key)));
        }
        #endregion
        #endregion

        #region  Redis Key
        #region 同步执行
        /// <summary>
        /// 删除单个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyDelete(string key)
        {
            return Save(db => db.KeyDelete(key));
        }
        /// <summary>
        /// 删除多个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long KeyDelete(List<string> key)
        {
            return Save(db => db.KeyDelete(ConvertRedisKeys(key)));
        }
        /// <summary>
        /// 重命名Key
        /// </summary>
        /// <param name="key">old key name</param>
        /// <param name="newKey">new key name</param>
        /// <returns></returns>
        public static bool KeyRename(string key, string newKey)
        {
            return Save(db => db.KeyRename(key, newKey));
        }
        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool KeyExpire(string key, TimeSpan? exp = default(TimeSpan?))
        {
            return Save(db => db.KeyExpire(key, exp));
        }
        #endregion
        #region 异步执行
        /// <summary>
        /// 异步删除单个key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<bool> KeyDeleteAsync(string key)
        {
            return await Save(db => db.KeyDeleteAsync(key));
        }
        /// <summary>
        /// 异步删除多个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<long> KeyDeleteAsync(List<string> key)
        {
            return await Save(db => db.KeyDeleteAsync(ConvertRedisKeys(key)));
        }
        /// <summary>
        ///  异步重命名Key
        /// </summary>
        /// <param name="key">old key name</param>
        /// <param name="newKey">new key name</param>
        /// <returns></returns>
        public static async Task<bool> KeyRenameAsync(string key, string newKey)
        {
            return await Save(db => db.KeyRenameAsync(key, newKey));
        }
        /// <summary>
        /// 异步设置Key的时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static async Task<bool> KeyExpireAsync(string key, TimeSpan? exp = default(TimeSpan?))
        {
            return await Save(db => db.KeyExpireAsync(key, exp));
        }
        #endregion
        #endregion

        #region Redis List
        #region 同步执行
        /// <summary>
        /// 移除List内部指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void ListRemove<T>(string key, T val)
        {
            Save(db => db.ListRemove(key, ConvertToJson(val)));
        }
        /// <summary>
        /// 获取指定Key的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> ListRange<T>(string key)
        {
            return Save(db => { return ConvertList<T>(db.ListRange(key)); });
        }
        /// <summary>
        /// 插入（入队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void ListRightPush<T>(string key, T val)
        {

            Save(db => db.ListRightPush(key, ConvertToJson(val)));
        }
        /// <summary>
        /// 取出（出队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T ListRightPop<T>(string key)
        {
            return Save(db => { return ConvertToObj<T>(db.ListRightPop(key)); });
        }
        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void ListLeftPush<T>(string key, T val)
        {
            Save(db => db.ListLeftPush(key, ConvertToJson(val)));
        }
        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T ListLeftPop<T>(string key)
        {
            return Save(db => { return ConvertToObj<T>(db.ListLeftPop(key)); });
        }
        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long GetListLength(string key)
        {
            return Save(db => db.ListLength(key));
        }
        #endregion
        #region 异步执行
        /// <summary>
        /// 异步移除List内部指定的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static async Task<long> ListRemoveAsync<T>(string key, T val)
        {
            return await Save(db => db.ListRemoveAsync(key, ConvertToJson(val)));
        }
        /// <summary>
        /// 异步获取指定Key的List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<List<T>> ListRangeAsync<T>(string key)
        {
            return ConvertList<T>(await Save(db => db.ListRangeAsync(key)));
        }
        /// <summary>
        /// 异步插入（入队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static async Task<long> ListRightPushAsync<T>(string key, T val)
        {
            return await Save(db => db.ListRightPushAsync(key, ConvertToJson(val)));
        }
        /// <summary>
        /// 异步取出（出队）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> ListRightPopAsync<T>(string key)
        {
            return ConvertToObj<T>(await Save(db => db.ListRightPopAsync(key)));
        }
        /// <summary>
        /// 异步入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static async Task<long> ListLeftPushAsync<T>(string key, T val)
        {
            return await Save(db => db.ListLeftPushAsync(key, ConvertToJson(val)));
        }
        /// <summary>
        /// 异步出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T> ListLeftPopAsync<T>(string key)
        {
            return ConvertToObj<T>(await Save(db => db.ListLeftPopAsync(key)));
        }
        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<long> GetListLengthAsync(string key)
        {
            return await Save(db => db.ListLengthAsync(key));
        }
        #endregion
        #endregion

        #region Redis Set
        #region 同步执行
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool SetAdd(string key, string val)
        {
            return Save(db => db.SetAdd(key, val));
        }
        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long SetLength(string key)
        {
            return Save(db => db.SetLength(key));
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool SetContains(string key, string val)
        {
            return Save(db => db.SetContains(key, val));
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool SetRemove(string key, string val)
        {
            return Save(db => db.SetRemove(key, val));
        }
        #endregion
        #region 异步执行
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static async Task<bool> SetAddAsync(string key, string val)
        {
            return await Save(db => db.SetAddAsync(key, val));
        }
        /// <summary>
        /// 获取长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<long> SetLengthAsync(string key)
        {
            return await Save(db => db.SetLengthAsync(key));
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static async Task<bool> SetContainsAsync(string key, string val)
        {
            return await Save(db => db.SetContainsAsync(key, val));
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static async Task<bool> SetRemoveAsync(string key, string val)
        {
            return await Save(db => db.SetRemoveAsync(key, val));
        }
        #endregion
        #endregion

        #region Redis Hash
        #region 同步执行
        /// <summary>
        /// 是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool HashExists(string key, string dataKey)
        {
            return Save(db => db.HashExists(key, dataKey));
        }
        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool HashSet<T>(string key, string dataKey, T val)
        {
            return Save(db => { return db.HashSet(key, dataKey, ConvertToJson(val)); });
        }
        /// <summary>
        /// 从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool HashDelete(string key, string dataKey)
        {
            return Save(db => db.HashDelete(key, dataKey));
        }
        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static long HashRemove(string key, List<RedisValue> dataKey)
        {
            return Save(db => db.HashDelete(key, dataKey.ToArray()));
        }
        /// <summary>
        /// 从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static T HashGet<T>(string key, string dataKey)
        {
            return Save(db => { return ConvertToObj<T>(db.HashGet(key, dataKey)); });
        }
        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> HashKeys<T>(string key)
        {
            return Save(db => { return ConvertList<T>(db.HashKeys(key)); });
        }
        #endregion
        #region 异步执行
        /// <summary>
        /// 异步是否被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static async Task<bool> HashExistsAsync(string key, string dataKey)
        {
            return await Save(db => db.HashExistsAsync(key, dataKey));
        }
        /// <summary>
        /// 异步存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static async Task<bool> HashSetAsync<T>(string key, string dataKey, T val)
        {
            return await Save(db => { return db.HashSetAsync(key, dataKey, ConvertToJson(val)); });
        }
        /// <summary>
        /// 异步从hash表中移除数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static async Task<bool> HashDeleteAsync(string key, string dataKey)
        {
            return await Save(db => db.HashDeleteAsync(key, dataKey));
        }
        /// <summary>
        /// 异步移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static async Task<long> HashRemoveAsync(string key, List<RedisValue> dataKey)
        {
            return await Save(db => db.HashDeleteAsync(key, dataKey.ToArray()));
        }
        /// <summary>
        /// 从hash表中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static async Task<T> HashGetAsync<T>(string key, string dataKey)
        {
            return ConvertToObj<T>(await Save(db => db.HashGetAsync(key, dataKey)));
        }
        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<List<T>> HashKeysAsync<T>(string key)
        {
            return ConvertList<T>(await Save(db => db.HashKeysAsync(key)));
        }
        #endregion
        #endregion

        #region Redis SortedSet
        #region 同步执行
        /// <summary>
        /// 无序添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public static bool SortedSetAdd<T>(string key, T val, double score)
        {
            return Save(db => db.SortedSetAdd(key, ConvertToJson<T>(val), score));
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool SortedSetRemove<T>(string key, T val)
        {
            return Save(db => db.SortedSetRemove(key, ConvertToJson<T>(val)));
        }
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> SortedSetRangeByRank<T>(string key)
        {
            return Save(db => { return ConvertList<T>(db.SortedSetRangeByRank(key)); });
        }
        /// <summary>
        ///  获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long SortedSetLength(string key)
        {
            return Save(db => db.SortedSetLength(key));
        }
        #endregion
        #region 异步执行
        /// <summary>
        /// 异步无序添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public static async Task<bool> SortedSetAddAsync<T>(string key, T val, double score)
        {
            return await Save(db => db.SortedSetAddAsync(key, ConvertToJson<T>(val), score));
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static async Task<bool> SortedSetRemoveAsync<T>(string key, T val)
        {
            return await Save(db => db.SortedSetRemoveAsync(key, ConvertToJson<T>(val)));
        }
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            return ConvertList<T>(await Save(db => db.SortedSetRangeByRankAsync(key)));
        }
        /// <summary>
        ///  获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<long> SortedSetLengthAsync(string key)
        {
            return await Save(db => db.SortedSetLengthAsync(key));
        }
        #endregion
        #endregion
    }
}
