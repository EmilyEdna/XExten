using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Core.Tracing.Interface
{
    [NonAspect]
    public interface ITracingDiagnosticListener
    {
        string ListenerName { get; }
    }
}
