using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.OpenTracing.Enum;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing
{
    public class SpanReference
    {
        public SpanReferenceOptions SpanReferenceOptions { get; }

        public ISpanContext SpanContext { get; }

        public SpanReference(SpanReferenceOptions spanReferenceOptions, ISpanContext spanContext)
        {
            SpanReferenceOptions = spanReferenceOptions;
            SpanContext = spanContext ?? throw new ArgumentNullException(nameof(spanContext));
        }
    }
}
