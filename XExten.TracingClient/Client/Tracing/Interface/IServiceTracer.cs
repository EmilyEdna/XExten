﻿using AspectCore.DynamicProxy;
using XExten.TracingClient.OpenTracing.Spans.Interface;
using XExten.TracingClient.OpenTracing.Tracers;

namespace XExten.TracingClient.Client.Tracing.Interface
{
    [NonAspect]
    public interface IServiceTracer
    {
        ITracer Tracer { get; }
        
        string ServiceName { get; }

        string Environment { get; }

        string Identity { get; }

        ISpan Start(ISpanBuilder spanBuilder);
    }
}