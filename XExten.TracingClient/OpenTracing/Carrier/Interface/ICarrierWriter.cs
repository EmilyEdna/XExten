using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XExten.TracingClient.OpenTracing.Carrier.Interface
{
    public interface ICarrierWriter
    {
        void Write(SpanContextPackage spanContext, ICarrier carrier);

        Task WriteAsync(SpanContextPackage spanContext, ICarrier carrier);
    }
}
