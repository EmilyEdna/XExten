using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XExten.TracingClient.Client.Tracing.Interface;
using XExten.TracingClient.Core.Common;
using XExten.TracingClient.Core.Extensions;
using XExten.TracingClient.Core.Tracing.Interface;
using XExten.TracingClient.OpenTracing.Extensions;
using XExten.TracingClient.OpenTracing.Logs;
using XExten.TracingClient.OpenTracing.Spans.Implement;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.Core.Tracing.Implement
{
    public class RequestTracer : IRequestTracer
    {
        private readonly IServiceTracer _tracer;
        private readonly TraceOptions _options;

        public RequestTracer(IServiceTracer tracer, IOptions<TraceOptions> options)
        {
            _tracer = tracer;
            _options = options.Value;
        }

        public ISpan OnBeginRequest(HttpContext httpContext)
        {
            var patterns = _options.IgnoredRoutesRegexPatterns;
            if (patterns == null || patterns.Any(x => Regex.IsMatch(httpContext.Request.Path, x)))
            {
                return null;
            }

            var spanBuilder = new SpanBuilder($"server {httpContext.Request.Method} {httpContext.Request.Path}");
            if (_tracer.Tracer.TryExtract(out var spanContext, httpContext.Request.Headers, (c, k) => c[k].GetValue(),
                c => c.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.GetValue())).GetEnumerator()))
            {
                spanBuilder.AsChildOf(spanContext);
            }
            var span = _tracer.Start(spanBuilder);
            httpContext.SetSpan(span);
            span.Log(LogField.CreateNew().ServerReceive());
            span.Log(LogField.CreateNew().Event("AspNetCore BeginRequest"));
            span.Tags
             .Server().Component("AspNetCore")
             .HttpMethod(httpContext.Request.Method)
             .HttpUrl($"{httpContext.Request.Scheme}://{httpContext.Request.Host.ToUriComponent()}{httpContext.Request.Path}{httpContext.Request.QueryString}")
             .HttpHost(httpContext.Request.Host.ToUriComponent())
             .HttpPath(httpContext.Request.Path)
             .HttpStatusCode(httpContext.Response.StatusCode)
             .PeerAddress(httpContext.Connection.RemoteIpAddress.ToString())
             .PeerPort(httpContext.Connection.RemotePort);
            _tracer.Tracer.SetCurrentSpan(span);
            return span;
        }

        public void OnEndRequest(HttpContext httpContext)
        {
            var span = httpContext.GetSpan();
            if (span == null)
            {
                return;
            }

            span.Tags.HttpStatusCode(httpContext.Response.StatusCode);

            span.Log(LogField.CreateNew().Event("AspNetCore EndRequest"));
            span.Log(LogField.CreateNew().ServerSend());
            span.Finish();
            _tracer.Tracer.SetCurrentSpan(null);
        }

        public void OnException(HttpContext httpContext, Exception exception, string @event)
        {
            var span = httpContext.GetSpan();
            if (span == null)
            {
                return;
            }
            span.Log(LogField.CreateNew().Event(@event));
            span.Exception(exception);
        }
    }
}
