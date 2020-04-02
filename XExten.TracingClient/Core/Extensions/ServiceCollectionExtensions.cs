using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.Common;
using XExten.TracingClient.Client.Dispatcher.Interface;
using XExten.TracingClient.Client.Sender;
using XExten.TracingClient.Client.Tracing;
using XExten.TracingClient.Client.Tracing.Interface;
using XExten.TracingClient.OpenTracing.Smaple;
using XExten.TracingClient.OpenTracing.Spans.Interface;
using XExten.TracingClient.OpenTracing.Tracers;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Configuration;
using XExten.TracingClient.Core.Tracing.Interface;
using XExten.TracingClient.Core.Tracing.Implement;
using XExten.TracingClient.Client.Tracing.Implement;
using XExten.TracingClient.Client.Dispatcher.Implement;
using XExten.TracingClient.Core.Common;
using Microsoft.Extensions.Hosting;

namespace XExten.TracingClient.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddButterfly(this IServiceCollection services, Action<TraceOptions> configure)
        {
            return services.AddButterfly().Configure<TraceOptions>(configure);
        }

        private static IServiceCollection AddButterfly(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<HttpTracingHandler>();

            services.TryAddSingleton<ISpanContextFactory, SpanContextFactory>();
            services.TryAddSingleton<ISampler, FullSampler>();
            services.TryAddSingleton<ITracer, Tracer>();
            services.TryAddSingleton<IServiceTracerProvider, ServiceTracerProvider>();
            services.TryAddSingleton<ISpanRecorder, AsyncSpanRecorder>();
            services.TryAddSingleton<ITraceDispatcherProvider, TraceDispatcherProvider>();
            services.TryAddSingleton<IHttpTraceSenderProvider, TraceSenderProvider>();
            services.TryAddSingleton<ITraceIdGenerator, TraceIdGenerator>();

            services.AddSingleton<IServiceTracer>(provider => provider.GetRequiredService<IServiceTracerProvider>().GetServiceTracer());
            services.AddSingleton<ITraceDispatcher>(provider => provider.GetRequiredService<ITraceDispatcherProvider>().GetDispatcher());
            services.AddSingleton<IHostedService, TraceHostedService>();
            services.AddSingleton<ITracingDiagnosticListener, HttpRequestDiagnosticListener>();
            services.AddSingleton<ITracingDiagnosticListener, MvcTracingDiagnosticListener>();
            services.AddSingleton<IRequestTracer, RequestTracer>();
            services.AddSingleton<IDispatchCallback, SpanDispatchCallback>();
            services.AddSingleton<ITraceLoggerFactory, TraceLoggerFactory>();

            services.ConfigureDynamicProxy(option =>
            {
                option.NonAspectPredicates.AddNamespace("XExten.*");
            });

            return services;
        }
    }
}
