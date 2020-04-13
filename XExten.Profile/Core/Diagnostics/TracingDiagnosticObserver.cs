using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.XPlus;

namespace XExten.Profile.Core.Diagnostics
{
    /// <summary>
    /// 探针值
    /// </summary>
    public class TracingDiagnosticObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly TracingDiagnosticMethodCollection  _TracingDiagnosticMethodCollection;
        public TracingDiagnosticObserver(ITracingDiagnosticProcessor TracingDiagnosticProcessor)
        {
            _TracingDiagnosticMethodCollection = new TracingDiagnosticMethodCollection(TracingDiagnosticProcessor);
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
            foreach (var method in _TracingDiagnosticMethodCollection)
            {
                XPlusEx.XTry(() => method.Invoke(value.Key, value.Value), ex => Console.WriteLine(ex));
            }
        }
    }
}
