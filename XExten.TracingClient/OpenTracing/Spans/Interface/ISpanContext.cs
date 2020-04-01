using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.OpenTracing.Spans.Interface
{
    public interface ISpanContext
    {
        string TraceId { get; }

        string SpanId { get; }

        bool Sampled { get; }

        Baggage Baggage { get; }

        SpanReferenceCollection References { get; }
    }
}
