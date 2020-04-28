using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExten.Profile.Abstractions.Common;
using XExten.Profile.Core.Diagnostics;

namespace XExten.Profile
{
    public class InstrumentationHostedService : IHostedService, IDependency
    {
        private readonly TracingDiagnosticProcessorObserver _observer;

        public InstrumentationHostedService(TracingDiagnosticProcessorObserver observer)
        {
            _observer = observer;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            DiagnosticListener.AllListeners.Subscribe(_observer);
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
