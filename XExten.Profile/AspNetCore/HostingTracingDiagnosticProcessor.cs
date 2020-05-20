using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;
using XExten.Profile.Attributes;

namespace XExten.Profile.AspNetCore
{
    public class HostingTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.MicrosoftAspNetCore;

        private readonly ITracingContext TracingContext;
        private readonly IEntryContextAccessor Accessor;
        private readonly IEnumerable<IHostingDiagnosticHandler> DiagnosticHandlers;

        public HostingTracingDiagnosticProcessor(ITracingContext tracingContext, IEnumerable<IHostingDiagnosticHandler> diagnosticHandlers, IEntryContextAccessor accessor)
        {
            TracingContext = tracingContext;
            DiagnosticHandlers = diagnosticHandlers;
            Accessor = accessor;
        }

        [DiagnosticName(ProcessorName.BeginRequest)]
        public void BeginRequest([Property] HttpContext httpContext)
        {
            foreach (var handler in DiagnosticHandlers)
            {
                if (handler.OnlyMatch(httpContext))
                {
                    handler.BeginRequest(TracingContext, httpContext);
                    return;
                }
            }
        }

        [DiagnosticName(ProcessorName.EndRequest)]
        public void EndRequest([Property] HttpContext httpContext)
        {
            var Context = Accessor.Context;
            if (Context == null) return;
            foreach (var handler in DiagnosticHandlers)
            {
                if (handler.OnlyMatch(httpContext))
                {
                    handler.EndRequest(Context, httpContext);
                    break;
                }
            }
            TracingContext.Release(Context);
        }

        [DiagnosticName(ProcessorName.UnhandledException)]
        public void DiagnosticUnhandledException([Property]HttpContext httpContext, [Property] Exception exception)
        {
            Accessor.Context?.Context?.Add(exception);
        }

        [DiagnosticName(ProcessorName.HostingException)]
        public void HostingUnhandledException([Property]HttpContext httpContext, [Property] Exception exception)
        {
            Accessor.Context?.Context?.Add(exception);
        }
    }
}
