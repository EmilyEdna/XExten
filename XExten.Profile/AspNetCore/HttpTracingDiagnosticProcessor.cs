using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;
using XExten.Profile.Attributes;

namespace XExten.Profile.AspNetCore
{
    public class HttpTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.HttpClinet;

        private readonly ITracingContext TracingContext;
        private readonly ILocalContextAccessor Accessor;
        private readonly IEnumerable<IRequestDiagnosticHandler> RequestDiagnosticHandlers;

        public HttpTracingDiagnosticProcessor(ITracingContext tracingContext, ILocalContextAccessor accessor, IEnumerable<IRequestDiagnosticHandler> requestDiagnosticHandlers)
        {
            TracingContext = tracingContext;
            Accessor = accessor;
            RequestDiagnosticHandlers = requestDiagnosticHandlers;
        }

        [DiagnosticName(ProcessorName.HttpClientRequest)]
        public void HttpRequest([Property(Name = "Request")] HttpRequestMessage request)
        {
            foreach (var handler in RequestDiagnosticHandlers)
            {
                if (handler.OnlyMatch(request))
                {
                    handler.Handle(TracingContext, request);
                    return;
                }
            }
        }
        [DiagnosticName(ProcessorName.HttpClientResponse)]
        public void HttpResponse([Property(Name = "Response")] HttpResponseMessage response)
        {
            var Context = Accessor.Context;
            if (Context == null) return;
            if (response != null)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    Context.Context.Add(new Exception(response.Content.ReadAsStringAsync().Result));
                Context.Context.Add("StatusCode", response.StatusCode.ToString());
            }
            TracingContext.Release(Context);
        }
        [DiagnosticName(ProcessorName.HttpClientException)]
        public void HttpException([Property(Name = "Request")] HttpRequestMessage request, [Property(Name = "Exception")] Exception exception)
        {
            Accessor.Context?.Context?.Add(exception);
            if (Accessor.Context == null) return;
            TracingContext.Release(Accessor.Context);
        }
    }
}
