using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XExten.TracingClient.OpenTracing.Carrier.Interface;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing.Tracers
{
    public interface ITracer
    {
        ISpan Start(ISpanBuilder spanBuilder);

        void Inject(ISpanContext spanContext, ICarrierWriter carrierWriter, ICarrier carrier);

        Task InjectAsync(ISpanContext spanContext, ICarrierWriter carrierWriter, ICarrier carrier);

        ISpanContext Extract(ICarrierReader carrierReader, ICarrier carrier);

        Task<ISpanContext> ExtractAsync(ICarrierReader carrierReader, ICarrier carrier);
    }
}
