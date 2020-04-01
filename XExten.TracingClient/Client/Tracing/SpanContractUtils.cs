using System.Linq;
using BaggageContract = XExten.TracingClient.Client.DataContract.Baggage;
using LogFieldContract = XExten.TracingClient.Client.DataContract.LogField;
using SpanReferenceContract = XExten.TracingClient.Client.DataContract.SpanReference;
using SpanContract = XExten.TracingClient.Client.DataContract.Span;
using LogContract = XExten.TracingClient.Client.DataContract.Log;
using TagContract = XExten.TracingClient.Client.DataContract.Tag;
using XExten.TracingClient.Client.DataContract;
using XExten.TracingClient.Client.Extensions;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.Client.Tracing
{
    public static class SpanContractUtils
    {
        public static Span CreateFromSpan(ISpan span)
        {
            var spanContract = new SpanContract
            {
                FinishTimestamp = span.FinishTimestamp,
                StartTimestamp = span.StartTimestamp,
                Sampled = span.SpanContext.Sampled,
                SpanId = span.SpanContext.SpanId,
                TraceId = span.SpanContext.TraceId,
                OperationName = span.OperationName,
                Duration = (span.FinishTimestamp - span.StartTimestamp).GetMicroseconds()
            };

            spanContract.Baggages = span.SpanContext.Baggage?.Select(x => new BaggageContract { Key = x.Key, Value = x.Value }).ToList();
            spanContract.Logs = span.Logs?.Select(x =>
                new LogContract
                {
                    Timestamp = x.Timestamp,
                    Fields = x.Fields.Select(f => new LogFieldContract { Key = f.Key, Value = f.Value?.ToString() }).ToList()
                }).ToList();

            spanContract.Tags = span.Tags?.Select(x => new TagContract { Key = x.Key, Value = x.Value }).ToList();

            spanContract.References = span.SpanContext.References?.Select(x =>
                new SpanReferenceContract { ParentId = x.SpanContext.SpanId, Reference = x.SpanReferenceOptions.ToString() }).ToList();

            return spanContract;
        }
    }
}