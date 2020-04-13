using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;
using XExten.Profile.Attributes;

namespace XExten.Profile
{
    public class TestTracingDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName => "HttpTest";

        [DiagnosticName("HttpTests")]
        public void StarTest([Object]string data)
        {
            Console.WriteLine(data);
        }
    }
}
