using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Attributes;

namespace XExten.Profile.Core.Diagnostics
{
    /// <summary>
    /// 追踪方法集合
    /// </summary>
    public class TracingDiagnosticMethodCollection : IEnumerable<TracingDiagnosticMethod>
    {
        private readonly List<TracingDiagnosticMethod> _Method;

        public TracingDiagnosticMethodCollection(ITracingDiagnosticProcessor TracingDiagnosticProcessor) 
        {
            _Method = new List<TracingDiagnosticMethod>();
            foreach (var Method in TracingDiagnosticProcessor.GetType().GetMethods())
            {
                var diagnosticName = Method.GetCustomAttribute<DiagnosticName>();
                if (diagnosticName == null)
                    continue;
                _Method.Add(new TracingDiagnosticMethod(TracingDiagnosticProcessor, Method, diagnosticName.Name));
            }
        }

        public IEnumerator<TracingDiagnosticMethod> GetEnumerator()
        {
            return _Method.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Method.GetEnumerator();
        }
    }
}
