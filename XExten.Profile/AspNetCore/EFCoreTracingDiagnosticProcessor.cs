using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;

namespace XExten.Profile.AspNetCore
{
    public class EFCoreTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.EntityFrameworkCore;

        private readonly ITracingContext TracingContext;
        private readonly IExitContextAccessor Accessor;
        private readonly IEnumerable<IEFCoreDiagnosticHandler> EFCoreDiagnosticHandler;

        public EFCoreTracingDiagnosticProcessor(ITracingContext tracingContext, IExitContextAccessor accessor, IEnumerable<IEFCoreDiagnosticHandler> EfDiagnosticHandler)
        {
            TracingContext = tracingContext;
            Accessor = accessor;
            EFCoreDiagnosticHandler = EfDiagnosticHandler;
        }

        public 
    }
}
