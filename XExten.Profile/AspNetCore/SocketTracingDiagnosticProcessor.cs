using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;
using XExten.Profile.Attributes;

namespace XExten.Profile.AspNetCore
{
    public class SocketTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.SocketClient;

        private readonly ITracingContext TracingContext;
        private readonly IEntryContextAccessor Accessor;
        private readonly IEnumerable<ISocketDiagnosticHandler> SocketDiagnosticHandler;
        public SocketTracingDiagnosticProcessor(ITracingContext tracingContext, IEntryContextAccessor accessor, IEnumerable<ISocketDiagnosticHandler> socketDiagnosticHandler)
        {
            TracingContext = tracingContext;
            Accessor = accessor;
            SocketDiagnosticHandler = socketDiagnosticHandler;
        }

        [DiagnosticName(ProcessorName.SocketBegin)]
        public void ExcuteSocketBeginReceive([Object]Object data) 
        {
            foreach (var handler in SocketDiagnosticHandler)
            {
                if (handler.OnlyMatch(data))
                {
                    handler.Handle(TracingContext, data);
                    return;
                }
            }
        }

        [DiagnosticName(ProcessorName.SocketEnd)]
        public void ExcuteSocketBeginEnd([Object]Object data)
        {
            var Context = Accessor.Context;
            if (Context == null) return;
            Context.ResultContext.SetResult(data);
            TracingContext.Release(Context);
        }

        [DiagnosticName(ProcessorName.SocketException)]
        public void SocketException([Object]Exception exception)
        {
            Accessor.Context?.Context?.Add(exception);
        }
    }
}
