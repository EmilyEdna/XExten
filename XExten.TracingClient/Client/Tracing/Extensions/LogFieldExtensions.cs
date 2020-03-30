using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Client.Tracing.Extensions
{
    public static class LogFieldExtensions
    {
        public static LogField MethodExecuting(this LogField logField)
        {
            return logField?.Event("Method Executing");
        }

        public static LogField MethodExecuted(this LogField logField)
        {
            return logField?.Event("Method Executed");
        }
    }
}
