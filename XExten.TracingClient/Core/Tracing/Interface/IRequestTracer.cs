using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.Core.Tracing.Interface
{
    public interface IRequestTracer
    {
        ISpan OnBeginRequest(HttpContext httpContext);

        void OnEndRequest(HttpContext httpContext);

        void OnException(HttpContext httpContext, Exception exception, string Event);
    }
}
