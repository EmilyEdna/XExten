using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions.Common;

namespace XExten.Profile.Abstractions
{
    public interface ITracingDiagnosticProcessor: IDependency
    {
        string ListenerName { get; }
    }
}
