using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.Sender;

namespace XExten.TracingClient.Core.Common
{
    public class TraceSenderProvider : IHttpTraceSenderProvider
    {
        private readonly TraceOptions _options;

        public TraceSenderProvider(IOptions<TraceOptions> options)
        {
            _options = options.Value;
        }

        public IHttpTraceSender GetSender()
        {
            return new HttpTraceSender(_options.CollectorUrl);
        }
    }
}
