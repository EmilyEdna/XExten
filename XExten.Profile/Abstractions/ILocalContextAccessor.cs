using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions.Common;
using XExten.Profile.Tracing.Entry;

namespace XExten.Profile.Abstractions
{
    public interface ILocalContextAccessor: IDependency
    {
        PartialContext Context { get; set; }
    }
}
