using AspectCore.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.XCore;

namespace XExten.Profile.Attributes
{
    public class PropertyAttribute : ParameterResolverAttribute
    {
        public string Name { get; set; }

        public override object Resolve(object value)
        {
            if (value == null || Name.IsNullOrEmpty()) return null;
            var Property = value.GetType().GetProperty(Name);
            return Property?.GetReflector()?.GetValue(value);
        }
    }
}
