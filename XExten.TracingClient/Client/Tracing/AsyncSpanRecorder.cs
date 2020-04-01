using System;
using XExten.TracingClient.Client.Dispatcher.Interface;
using XExten.TracingClient.OpenTracing.Spans.Interface;

namespace XExten.TracingClient.Client.Tracing
{
    public class AsyncSpanRecorder : ISpanRecorder
    {
        private readonly ITraceDispatcher _dispatcher;

        public AsyncSpanRecorder(ITraceDispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Record(ISpan span)
        {
            _dispatcher.Dispatch(SpanContractUtils.CreateFromSpan(span));
        }
    }
}