using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.OpenTracing.Spans.Interface
{
    public interface ISpanBuilder
    {
        SpanReferenceCollection References { get; }

        string OperationName { get; }

        DateTimeOffset? StartTimestamp { get; }

        Baggage Baggage { get; }

        bool? Sampled { get; }
    }
}
