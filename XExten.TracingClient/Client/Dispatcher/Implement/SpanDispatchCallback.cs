using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XExten.TracingClient.Client.Common;
using XExten.TracingClient.Client.DataContract;
using XExten.TracingClient.Client.Dispatcher.Enum;
using XExten.TracingClient.Client.Dispatcher.Interface;
using XExten.TracingClient.Client.Extensions;
using XExten.TracingClient.Client.Sender;

namespace XExten.TracingClient.Client.Dispatcher.Implement
{
    public class SpanDispatchCallback : IDispatchCallback
    {
        private const int DefaultChunked = 500;
        private readonly IHttpTraceSender _butterflySender;
        private readonly Func<DispatchableToken, bool> _filter;
        private readonly ITraceLogger _logger;

        public SpanDispatchCallback(IHttpTraceSenderProvider senderProvider, ITraceLoggerFactory loggerFactory)
        {
            _butterflySender = senderProvider.GetSender();
            _logger = loggerFactory.CreateTraceLogger(typeof(SpanDispatchCallback));
            _filter = token => token == DispatchableToken.SpanToken;
        }

        public Func<DispatchableToken, bool> Filter => _filter;

        public async Task Accept(IEnumerable<IDispatchable> dispatchables)
        {
            foreach(var block in dispatchables.Chunked(DefaultChunked))
            {
                try
                {
                    await _butterflySender.SendSpanAsync(block.Select(x => x.RawInstance).OfType<Span>().ToArray());
                }
                catch(Exception exception)
                {
                    foreach(var item in block)
                    {
                        item.State = SendState.Untreated;
                        item.Error();
                    }
                    _logger.Error("Flush span to collector error.", exception);
                }
            }
        }
    }
}
