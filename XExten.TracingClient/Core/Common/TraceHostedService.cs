using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExten.TracingClient.Client.Dispatcher.Interface;
using XExten.TracingClient.Core.Tracing.Interface;

namespace XExten.TracingClient.Core.Common
{
    public class TraceHostedService : IHostedService
    {
        private readonly ITraceDispatcher _dispatcher;

        public TraceHostedService(ITraceDispatcher dispatcher, IEnumerable<ITracingDiagnosticListener> tracingDiagnosticListeners, DiagnosticListener diagnosticListener)
        {
            _dispatcher = dispatcher;
            foreach (var tracingDiagnosticListener in tracingDiagnosticListeners)
            {
                diagnosticListener.SubscribeWithAdapter(tracingDiagnosticListener);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _dispatcher.Dispose();
            return Task.CompletedTask;
        }
    }
}
