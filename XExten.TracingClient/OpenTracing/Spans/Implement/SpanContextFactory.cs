using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing.Spans.Implement
{
    internal class SpanContextFactory : ISpanContextFactory
    {
        public ISpanContext Create(SpanContextPackage spanContextPackage)
        {
            return new SpanContext(
                spanContextPackage.TraceId ?? RandomUtils.NextLong().ToString(),
                spanContextPackage.SpanId ?? RandomUtils.NextLong().ToString(),
                spanContextPackage.Sampled,
                spanContextPackage.Baggage ?? new Baggage(),
                spanContextPackage.References);
        }
    }
}
