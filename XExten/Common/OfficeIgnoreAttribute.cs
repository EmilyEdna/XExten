using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Common
{
    /// <summary>
    /// 文档特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OfficeAttribute : Attribute
    {
        /// <summary>
        /// 忽略导出
        /// </summary>
        public bool IngoreField { get; set; } = false;

        /// <summary>
        /// 映射字段
        /// </summary>
        public string MapperField { get; set; }

        /// <summary>
        /// bool类型映射枚举
        /// </summary>
        public Type BoolEnum { get; set; }

        /// <summary>
        /// 是否枚举
        /// </summary>
        public bool Enum { get; set; } = false;
    }
}
