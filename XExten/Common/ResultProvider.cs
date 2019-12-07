using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Dictionary<Object, Object> DictionaryObjectProvider { get; set; }
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
        public static ResultProvider SetValue(Object ObjKey = null, Dictionary<Object, Object> DicKey = null)
        {
            return new ResultProvider { ObjectProvider = ObjKey, DictionaryObjectProvider = DicKey };
        }
        /// <summary>
        /// 设值
        /// </summary>
        /// <param name="ObjKey"></param>
        /// <param name="DicKey"></param>
        /// <returns></returns>
        public static ResultProvider SetValue(Object ObjKey = null, Dictionary<String, Object> DicKey = null)
        {
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
