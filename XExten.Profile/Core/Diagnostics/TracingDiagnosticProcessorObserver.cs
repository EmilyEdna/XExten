using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using XExten.Profile.Abstractions;

namespace XExten.Profile.Core.Diagnostics
{
    public class TracingDiagnosticProcessorObserver : IObserver<DiagnosticListener>
    {
        private readonly IEnumerable<ITracingDiagnosticProcessor> _TracingDiagnosticProcessors;
        public TracingDiagnosticProcessorObserver(IEnumerable<ITracingDiagnosticProcessor> TracingDiagnosticProcessors)
        {
            _TracingDiagnosticProcessors = TracingDiagnosticProcessors;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(DiagnosticListener value)
        {
            
        }
        protected virtual void Subscribe(DiagnosticListener Listener, ITracingDiagnosticProcessor TracingDiagnosticProcessor)
        {
            Listener.Subscribe(new TracingDiagnosticObserver(TracingDiagnosticProcessor));
        }
    }
}
