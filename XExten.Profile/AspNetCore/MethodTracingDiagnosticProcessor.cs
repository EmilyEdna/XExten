using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;
using XExten.Profile.Attributes;

namespace XExten.Profile.AspNetCore
{
    public class MethodTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.MethodClient;
        private readonly ITracingContext TracingContext;
        private readonly ILocalContextAccessor Accessor;
        private readonly IEnumerable<IMethondDiagnosticHandler> MethondDiagnosticHandler;

        public MethodTracingDiagnosticProcessor(ITracingContext tracingContext, ILocalContextAccessor accessor, IEnumerable<IMethondDiagnosticHandler> methondDiagnosticHandler)
        {
            TracingContext = tracingContext;
            Accessor = accessor;
            MethondDiagnosticHandler = methondDiagnosticHandler;
        }

        [DiagnosticName(ProcessorName.MethodBegin)]
        public void MethodBeginInvoke([Object]Object data)
        {
            foreach (var handler in MethondDiagnosticHandler)
            {
                if (handler.OnlyMatch(data))
                {
                    handler.Handle(TracingContext, data);
                    return;
                }
            }
        }

        [DiagnosticName(ProcessorName.MethodEnd)]
        public void MethodEndInvoke([Object]Object data)
        {
            var Context = Accessor.Context;
            if (Context == null) return;
            Context.ResultContext.SetResult(data);
            TracingContext.Release(Context);
        }

        [DiagnosticName(ProcessorName.MethodException)]
        public void MethodExceptionInvoke([Object]Exception exception)
        {
            Accessor.Context?.Context?.Add(exception);
            if (Accessor.Context == null) return;
            TracingContext.Release(Accessor.Context);
        }
    }
}
