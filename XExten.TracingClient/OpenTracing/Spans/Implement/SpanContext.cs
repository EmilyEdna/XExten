using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing.Spans.Implement
{
    public class SpanContext : ISpanContext
    {
        public string TraceId { get; }

        public string SpanId { get; }

        public bool Sampled { get; }

        public Baggage Baggage { get; }

        public SpanReferenceCollection References { get; }

        public SpanContext(string traceId, string spanId, bool sampled, Baggage baggage, SpanReferenceCollection references)
        {
            TraceId = traceId;
            SpanId = spanId;
            Sampled = sampled;
            Baggage = baggage ?? throw new ArgumentNullException(nameof(baggage));
            References = references ?? SpanReferenceCollection.Empty;
        }
    }
}
