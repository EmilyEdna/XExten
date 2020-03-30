using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Client.Common
{
    public interface ITraceLogger
    {
        void Info(string message);

        void Error(string message, Exception exception);
    }
}
