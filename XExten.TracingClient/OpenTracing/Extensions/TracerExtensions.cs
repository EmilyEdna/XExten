using System;
using System.Collections;
using System.Collections.Generic;
using XExten.TracingClient.OpenTracing.Carrier.Implement;
using XExten.TracingClient.OpenTracing.Spans.Implement;
using XExten.TracingClient.OpenTracing.Spans.Interface;
using XExten.TracingClient.OpenTracing.Tracers;

namespace XExten.TracingClient.OpenTracing.Extensions
{
    public static class TracerExtensions
    {
        public static ISpan GetCurrentSpan(this ITracer tracer)
        {
            return SpanLocal.Current;
        }

        public static void SetCurrentSpan(this ITracer tracer, ISpan spanContext)
        {
            SpanLocal.Current = spanContext;
        }
        
        public static void Inject<T>(this ITracer tracer, ISpanContext spanContext, T carrier, Action<T, string, string> injector)
            where T : class, IEnumerable
        {
            if (tracer == null)
            {
                throw new ArgumentNullException(nameof(tracer));
            }

            tracer.Inject(spanContext, new TextMapCarrierWriter(), new DelegatingCarrier<T>(carrier, injector));
        }

        public static bool TryExtract<T>(this ITracer tracer, out ISpanContext spanContext, T carrier, Func<T, string, string> extractor, Func<T, IEnumerator<KeyValuePair<string, string>>> enumerator = null)
            where T : class, IEnumerable
        {
            spanContext = tracer.Extract(new TextMapCarrierReader(new SpanContextFactory()), new DelegatingCarrier<T>(carrier, extractor, enumerator));
            return spanContext != null;
        }
    }
}