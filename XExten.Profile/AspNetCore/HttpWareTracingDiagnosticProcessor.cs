using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.AspNetCore.DiagnosticProcessorName;
using XExten.Profile.Attributes;

namespace XExten.Profile.AspNetCore
{
    public class HttpWareTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName { get; } = ProcessorName.HttpClinet;

        private readonly ITracingContext TracingContext;
        private readonly ILocalContextAccessor Accessor;

        public HttpWareTracingDiagnosticProcessor(ITracingContext tracingContext, ILocalContextAccessor accessor)
        {
            TracingContext = tracingContext;
            Accessor = accessor;
        }

        [DiagnosticName(ProcessorName.HttpClientRequest)]
        public void HttpRequest([Property(Name = "Request")] HttpRequestMessage request) 
        { 
        
        }
        [DiagnosticName(ProcessorName.HttpClientResponse)]
        public void HttpResponse([Property(Name = "Response")] HttpResponseMessage response)
        {

        }
        [DiagnosticName(ProcessorName.HttpClientException)]
        public void HttpException([Property(Name = "Request")] HttpRequestMessage request, [Property(Name = "Exception")] Exception exception)
        {

        }
    }
}
