using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.OpenTracing.Enum;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing.Extensions
{
    public static class SpanBuilderExtensions
    {
        public static ISpanBuilder AsChildOf(this ISpanBuilder spanBuilder, ISpanContext spanContext)
        {
            if (spanBuilder == null)
            {
                throw new ArgumentNullException(nameof(spanBuilder));
            }
            spanBuilder.References.Add(new SpanReference(SpanReferenceOptions.ChildOf, spanContext));
            return spanBuilder;
        }

        public static ISpanBuilder FollowsFrom(this ISpanBuilder spanBuilder, ISpanContext spanContext)
        {
            if (spanBuilder == null)
            {
                throw new ArgumentNullException(nameof(spanBuilder));
            }
            spanBuilder.References.Add(new SpanReference(SpanReferenceOptions.FollowsFrom, spanContext));
            return spanBuilder;
        }
    }
}
