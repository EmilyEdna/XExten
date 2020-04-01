using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing.Extensions
{
    public static class SpanContextExtensions
    {
        public static SpanContextPackage Package(this ISpanContext spanContext)
        {
            if (spanContext == null)
            {
                throw new ArgumentNullException(nameof(spanContext));
            }

            return new SpanContextPackage(spanContext.TraceId, spanContext.SpanId, spanContext.Sampled, spanContext.Baggage, null);
        }

        public static ISpanContext SetBaggage(this ISpanContext spanContext, string key, string value)
        {
            if (spanContext == null)
            {
                throw new ArgumentNullException(nameof(spanContext));
            }

            spanContext.Baggage[key] = value;
            return spanContext;
        }
    }
}