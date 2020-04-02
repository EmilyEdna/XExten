using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DiagnosticAdapter;
using XExten.TracingClient.Core.Extensions;
using XExten.TracingClient.Core.Tracing.Interface;
using XExten.TracingClient.OpenTracing.Extensions;
using XExten.TracingClient.OpenTracing.Logs;

namespace XExten.TracingClient.Core.Tracing.Implement
{
    public class MvcTracingDiagnosticListener : ITracingDiagnosticListener
    {
        public string ListenerName { get; } = "Microsoft.AspNetCore";

        [DiagnosticName("Microsoft.AspNetCore.Mvc.BeforeAction")]
        public void BeforeAction(ActionDescriptor actionDescriptor, HttpContext httpContext, RouteData routeData)
        {
            Event(httpContext, "AspNetCore.Mvc BeforeAction");
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.AfterAction")]
        public void AfterAction(ActionDescriptor actionDescriptor, HttpContext httpContext, RouteData routeData)
        {
            Event(httpContext, "AspNetCore.Mvc AfterAction");
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.BeforeOnException")]
        public void BeforeOnException(ExceptionContext exceptionContext)
        {
            var httpContext = exceptionContext.HttpContext;
            Event(httpContext, "AspNetCore.Mvc BeforeOnException");
            var span = httpContext.GetSpan();
            span?.Exception(exceptionContext.Exception);
        }

        [DiagnosticName("Microsoft.AspNetCore.Mvc.AfterOnException")]
        public void AfterOnException(ExceptionContext exceptionContext)
        {
            Event(exceptionContext.HttpContext, "AspNetCore.Mvc AfterOnException");
        }

        private void Event(HttpContext httpContext, string Event)
        {
            var span = httpContext.GetSpan();
            span?.Log(LogField.CreateNew().Event(Event));
        }
    }
}
