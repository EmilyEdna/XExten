using System;

namespace XExten.Common
{
    /// <summary>
    /// 忽略映射(Not Map)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreMappedAttribute : Attribute
    {
        /// <summary>
        /// 是否忽略
        /// </summary>
        public bool? Ingore { get; set; }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="ingore"></param>
        public IgnoreMappedAttribute(bool? ingore)
        {
            Ingore = ingore;
        }
    }
}