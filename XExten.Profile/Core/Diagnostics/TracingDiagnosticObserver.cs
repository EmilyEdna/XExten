using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;

namespace XExten.Profile.Core.Diagnostics
{
    public class TracingDiagnosticObserver : IObserver<KeyValuePair<string, object>>
    {
        public TracingDiagnosticObserver(ITracingDiagnosticProcessor TracingDiagnosticProcessor)
        {
            //_eventCollection = new DiagnosticEventCollection(diagnosticProcessor);
        }


        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            throw new NotImplementedException();
        }
    }
}
