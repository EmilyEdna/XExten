using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;

namespace XExten.Profile.AbstractionsDefault
{
    /// <summary>
    /// 空参
    /// </summary>
    public class NullParameterResolver : IParameterResolver
    {
        public object Resolve(object value)
        {
            return null;
        }
    }
}
