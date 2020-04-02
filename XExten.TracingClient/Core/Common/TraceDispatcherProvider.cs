using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.Common;
using XExten.TracingClient.Client.Dispatcher.Implement;
using XExten.TracingClient.Client.Dispatcher.Interface;

namespace XExten.TracingClient.Core.Common
{
    public class TraceDispatcherProvider: ITraceDispatcherProvider
    {
        private readonly IEnumerable<IDispatchCallback> _dispatchCallbacks;
        private readonly TraceOptions _options;
        private readonly ITraceLoggerFactory _loggerFactory;

        public TraceDispatcherProvider(IEnumerable<IDispatchCallback> dispatchCallbacks, ITraceLoggerFactory loggerFactory, IOptions<TraceOptions> options)
        {
            _dispatchCallbacks = dispatchCallbacks;
            _loggerFactory = loggerFactory;
            _options = options.Value;
        }

        public ITraceDispatcher GetDispatcher()
        {
            return new TraceDispatcher(_dispatchCallbacks, _loggerFactory, _options.FlushInterval, _options.BoundedCapacity, _options.ConsumerCount);
        }
    }
}
