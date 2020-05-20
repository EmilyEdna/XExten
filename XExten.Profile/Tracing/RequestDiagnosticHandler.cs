using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Tracing.Entry.Enum;

namespace XExten.Profile.Tracing
{
    public class RequestDiagnosticHandler : IRequestDiagnosticHandler
    {
        public void Handle(ITracingContext tracingContext, HttpRequestMessage request)
        {
            var Partial = tracingContext.CreateLocalPartialContext(request.RequestUri.ToString());
            Partial.Context.Component = "HTTPCLIENT";
            Partial.Context.LayerType = ChannelLayerType.HTTP;
            Partial.Context.Add("Method", request.Method.ToString());
            Partial.Context.Add("URL", request.RequestUri.ToString());
            Partial.Context.Add("Path", $"{request.RequestUri.Host}:{request.RequestUri.Port}");
            Partial.Context.Add("Router", request.RequestUri.Host);
        }

        public bool OnlyMatch(HttpRequestMessage request)
        {
            return true;
        }
    }
}
