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

        private readonly ITracingContext _tracingContext;
        private readonly IEnumerable<IHostingDiagnosticHandler> _diagnosticHandlers;

        public HostingTracingDiagnosticProcessor(ITracingContext tracingContext, IEnumerable<IHostingDiagnosticHandler> diagnosticHandlers)
        {
            _tracingContext = tracingContext;
            _diagnosticHandlers = diagnosticHandlers;
        }

        [DiagnosticName(ProcessorName.BeginRequest)]
        public void BeginRequest([Property] HttpContext httpContext)
        {
            foreach (var handler in _diagnosticHandlers)
            {
                if (handler.OnlyMatch(httpContext))
                {
                    handler.BeginRequest(_tracingContext,httpContext);
                    return;
                }
            }
        }

        [DiagnosticName(ProcessorName.EndRequest)]
        public void EndRequest([Property] HttpContext httpContext)
        {

        }

        [DiagnosticName(ProcessorName.UnhandledException)]
        public void DiagnosticUnhandledException([Property]HttpContext httpContext, [Property] Exception exception)
        {

        }

        [DiagnosticName(ProcessorName.HostingException)]
        public void HostingUnhandledException([Property]HttpContext httpContext, [Property] Exception exception)
        {

        }
    }
}
