using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XExten.TracingClient.Client.Tracing.Implement;
using XExten.TracingClient.Client.Tracing.Interface;
using XExten.TracingClient.OpenTracing.Tracers;

namespace XExten.TracingClient.Core.Common
{
    public class ServiceTracerProvider: IServiceTracerProvider
    {
        private readonly ITracer _tracer;
        private readonly TraceOptions _options;
        private readonly IHostEnvironment _hostingEnvironment;

        public ServiceTracerProvider(ITracer tracer, IHostEnvironment hostingEnvironment, IOptions<TraceOptions> options)
        {
            _tracer = tracer;
            _options = options.Value;
            _hostingEnvironment = hostingEnvironment;
        }

        public IServiceTracer GetServiceTracer()
        {
            var service = _options.Service ?? _hostingEnvironment.ApplicationName;
            var environmentName = _hostingEnvironment.EnvironmentName;
            var host = Dns.GetHostName();
            var identity = string.IsNullOrEmpty(_options.ServiceIdentity) ? $"{service}@{host}" : _options.ServiceIdentity;
            return new ServiceTracer(_tracer, service, environmentName, identity, host);
        }
    }
}
