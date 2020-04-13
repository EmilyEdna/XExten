using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;

namespace XExten.Profile.Attributes
{
    public abstract class ParameterResolverAttribute : Attribute, IParameterResolver
    {
        public abstract object Resolve(object value);
    }
}
