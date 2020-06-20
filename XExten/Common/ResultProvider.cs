using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.XCore;

namespace XExten.Common
{
    /// <summary>
    /// 序列化中间结果
    /// </summary>
    public class ResultProvider
    {
        /// <summary>
        /// 对象结果
        /// </summary>
        public Object ObjectProvider { get; set; }
        /// <summary>
        /// 运行时结果
        /// </summary>
        public dynamic DynamicProvider { get; set; }
        /// <summary>
        /// 字典结果
        /// </summary>
        public Dictionary<String, Object> DictionaryStringProvider { get; set; }
        /// <summary>
        /// 设值
        /// </summary>
        /// <param name="ObjKey"></param>
        /// <param name="DicKey"></param>
        /// <returns></returns>
        public static ResultProvider SetValue(Object ObjKey = null, Dictionary<String, Object> DicKey = null)
        {
            DicKey.Where(Item => Item.Value is JObject).Select(Item => Item.Key).ToList().ForEach(Item =>
            {
                DicKey[Item] = DicKey[Item].ToJson().ToModel<Dictionary<String, Object>>();
            });
            return new ResultProvider { ObjectProvider = ObjKey, DictionaryStringProvider = DicKey };
        }
        /// <summary>
        /// 设值
        /// </summary>
        /// <param name="ObjKey"></param>
        /// <param name="DynKey"></param>
        /// <returns></returns>
        public static ResultProvider SetValue(Object ObjKey = null, dynamic DynKey = null)
        {
            return new ResultProvider { ObjectProvider = ObjKey, DynamicProvider = DynKey };
        }
    }
}
