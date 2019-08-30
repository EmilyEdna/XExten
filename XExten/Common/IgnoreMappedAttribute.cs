using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Common
{
    /// <summary>
    /// 忽略映射(Not Map)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreMappedAttribute: Attribute
    {
    }
}
