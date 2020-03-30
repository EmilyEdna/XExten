using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Client.Common
{
    public interface ITraceLoggerFactory
    {
        ITraceLogger CreateTraceLogger(Type type);
    }
}
