using System;
using System.Collections.Generic;
using System.Text;
using XExten.Profile.Abstractions;

namespace XExten.Profile.Tracing
{
    public class SocketDiagnosticHandler : ISocketDiagnosticHandler
    {
        public void Handle(ITracingContext tracingContext, object Provider)
        {
            
        }

        public bool OnlyMatch(object Data)
        {
            return true;
        }
    }
}
