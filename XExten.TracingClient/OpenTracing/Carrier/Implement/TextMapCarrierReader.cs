using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XExten.TracingClient.OpenTracing.Carrier.Interface;
using XExten.TracingClient.OpenTracing.Extensions;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing.Carrier.Implement
{
    public class TextMapCarrierReader : ICarrierReader
    {
        private readonly ISpanContextFactory _spanContextFactory;

        public TextMapCarrierReader(ISpanContextFactory spanContextFactory)
        {
            _spanContextFactory = spanContextFactory ?? throw new ArgumentNullException(nameof(spanContextFactory));
        }

        public ISpanContext Read(ICarrier carrier)
        {
            var textMapCarrier = carrier as ITextMapCarrier;
            if (textMapCarrier == null)
            {
                return null;
            }

            var traceId = textMapCarrier.GetTraceId();
            var spanId = textMapCarrier.GetSpanId();
            var sampled = textMapCarrier.GetSampled();

            if (traceId == null || spanId == null)
            {
                return null;
            }

            return _spanContextFactory.Create(new SpanContextPackage(traceId, spanId, sampled, textMapCarrier.GetBaggage()));
        }

        public Task<ISpanContext> ReadAsync(ICarrier carrier)
        {
            return Task.FromResult(Read(carrier));
        }
    }
}
