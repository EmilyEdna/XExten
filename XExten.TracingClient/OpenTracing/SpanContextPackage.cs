using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.OpenTracing
{
    public struct SpanContextPackage
    {
        public string TraceId { get; }

        public string SpanId { get; }

        public Baggage Baggage { get; }

        public bool Sampled { get; }

        public SpanReferenceCollection References { get; }

        public SpanContextPackage(string traceId, string spanId, bool sampled, Baggage baggage, SpanReferenceCollection references = null)
        {
            TraceId = traceId;
            SpanId = spanId;
            Sampled = sampled;
            Baggage = baggage;
            References = references;
        }
    }
}
