using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;

namespace XExten.Profile.Core.Diagnostics
{
    public class TracingDiagnosticMethodCollection : IEnumerable<TracingDiagnosticMethod>
    {
        public TracingDiagnosticMethodCollection(ITracingDiagnosticProcessor TracingDiagnosticProcessor) { 
        
        }

        public IEnumerator<TracingDiagnosticMethod> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
