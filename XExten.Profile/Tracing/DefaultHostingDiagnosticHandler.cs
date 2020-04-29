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
            var Partial = tracingContext.CreateEntryPartialContext(httpContext.Request.Path, new CarrierHeaderCollection(httpContext));
            Partial.Context.Component = "ASPNETCORE";
            Partial.Context.LayerType = ChannelLayerType.HTTP;
            Partial.Context.Method = httpContext.Request.Method;
            Partial.Context.URL = httpContext.Request.GetDisplayUrl();
            Partial.Context.Path = httpContext.Request.Path;
            Partial.Context.Router = httpContext.Connection.RemoteIpAddress.ToString();
        }

        public void EndRequest(PartialContext partialContext, HttpContext httpContext)
        {
            partialContext.Context.StatusCode = httpContext.Response.StatusCode;
            partialContext.Context.PenddingEnd = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            partialContext.Context.RequestMilliseconds = partialContext.Context.PenddingEnd - partialContext.Context.PenddingStar;
        }

        public bool OnlyMatch(HttpContext httpContext)
        {
            return true;
        }
    }
}
