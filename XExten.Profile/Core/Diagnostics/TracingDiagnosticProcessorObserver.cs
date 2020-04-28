using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Abstractions.Common;
using XExten.Profile.Core.Common;

namespace XExten.Profile.Core.Diagnostics
{
    /// <summary>
    /// 追踪执行器
    /// </summary>
    public class TracingDiagnosticProcessorObserver : IObserver<DiagnosticListener>, ISingletonDependency
    {
        private readonly IEnumerable<ITracingDiagnosticProcessor> _TracingDiagnosticProcessors;
        public TracingDiagnosticProcessorObserver(IEnumerable<ITracingDiagnosticProcessor> TracingDiagnosticProcessors)
        {
            _TracingDiagnosticProcessors = TracingDiagnosticProcessors;
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(DiagnosticListener Listener)
        {
            foreach (var item in _TracingDiagnosticProcessors.Distinct(t => t.ListenerName))
            {
                if (Listener.Name == item.ListenerName)
                    Listener.Subscribe(new TracingDiagnosticObserver(item));
            }
        }

    }
}
