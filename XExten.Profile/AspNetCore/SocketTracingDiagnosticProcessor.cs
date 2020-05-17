using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;

namespace XExten.Profile.AspNetCore
{
    public class SocketTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.SocketClient;

        private readonly ITracingContext TracingContext;
        private readonly ILocalContextAccessor Accessor;

        public SocketTracingDiagnosticProcessor(ITracingContext tracingContext, ILocalContextAccessor accessor)
        {
            TracingContext = tracingContext;
            Accessor = accessor;
        }
    }
}
