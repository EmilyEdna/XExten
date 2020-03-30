using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExten.TracingClient.Client.DataContract;

namespace XExten.TracingClient.Client.Sender
{
    public interface IHttpTraceSender
    {
        Task SendSpanAsync(Span[] spans, CancellationToken cancellationToken = default);
    }
}
