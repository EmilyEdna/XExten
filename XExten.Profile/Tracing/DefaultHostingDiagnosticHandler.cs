using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Tracing.Entry;

namespace XExten.Profile.Tracing
{
    public class DefaultHostingDiagnosticHandler : IHostingDiagnosticHandler
    {
        public void BeginRequest(ITracingContext tracingContext, HttpContext httpContext)
        {
            var context = tracingContext.CreateEntryPartialContext(httpContext.Request.Path, new CarrierHeaderCollection(httpContext));
            context.Context.Component = "ASPNETCORE";
            context.Context.LayerType = ChannelLayerType.HTTP;
            context.Context.Method = httpContext.Request.Method;
            context.Context.URL = httpContext.Request.GetDisplayUrl();
            context.Context.Path = httpContext.Request.Path;
            context.Context.Router = httpContext.Connection.RemoteIpAddress.ToString();
        }

        public void EndRequest(PartialContext partialContext, HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public bool OnlyMatch(HttpContext httpContext)
        {
            return true;
        }
    }
}
