using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.Profile.Abstractions
{
    public interface IParameterResolver
    {
        object Resolve(object value);
    }
}
