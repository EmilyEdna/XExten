﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing.Spans.Implement
{
    public class SpanBuilder : ISpanBuilder
    {
        public string OperationName { get; }

        public DateTimeOffset? StartTimestamp { get; }

        public Baggage Baggage { get; }

        public SpanReferenceCollection References { get; }

        public bool? Sampled => References.FirstOrDefault()?.SpanContext?.Sampled;

        public SpanBuilder(string operationName)
            : this(operationName, null)
        {
        }

        public SpanBuilder(string operationName, DateTimeOffset? startTimestamp)
        {
            OperationName = operationName ?? throw new ArgumentNullException(nameof(operationName));
            StartTimestamp = startTimestamp;
            Baggage = new Baggage();
            References = new SpanReferenceCollection();
        }
    }
}
