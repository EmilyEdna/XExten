using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.OpenTracing.Carrier.Interface
{
    public interface ICarrierReader
    {
        ISpanContext Read(ICarrier carrier);

        Task<ISpanContext> ReadAsync(ICarrier carrier);
    }
}
