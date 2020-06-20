using System;
using System.Collections.Generic;
using System.Text;
using XExten.XCore;

namespace XExten.SocketProxy.SocketConfig
{
    /// <summary>
    /// 参数序列化
    /// </summary>
    public class SocketSerializeData
    {
        public String Route { get; set; }
        public Dictionary<String, Object> Providor { get; set; }
        /// <summary>
        /// 添加键值对参数
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public SocketSerializeData AppendSerialized(string Key, object Value)
        {
            Providor ??= new Dictionary<String, Object>();
            Providor.Add(Key, Value);
            return this;
        }
        /// <summary>
        /// 添加实体类参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Param"></param>
        /// <returns></returns>
        public SocketSerializeData AppendSerialized<T>(T Param)
        {
            Providor ??= new Dictionary<String, Object>();
            var data = Param.ToDic();
            if (data.Count == 0) return this;
            data.ToEachs(Item =>
            {
                Providor.Add(Item.Key, Item.Value);
            });
            return this;
        }
        /// <summary>
        /// 添加字典类参数
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public SocketSerializeData AppendSerialized(Dictionary<String, Object> Param)
        {
            Providor ??= new Dictionary<String, Object>();
            if (Param.Count == 0) return this;
            Param.ToEachs(Item =>
            {
                Providor.Add(Item.Key, Item.Value);
            });
            return this;
        }
        /// <summary>
        /// 添加请求路由
        /// </summary>
        /// <param name="Router"></param>
        /// <returns></returns>
        public SocketSerializeData AppendRoute(string Router)
        {
            Route = Router.ToLower();
            return this;
        }
    }
}
