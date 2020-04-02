using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.Common;
using Microsoft.Extensions.Logging;

namespace XExten.TracingClient.Core.Common
{
    public class TraceLogger : ITraceLogger
    {
        private readonly ILogger _logger;

        public TraceLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Error(string message, Exception exception)
        {
            _logger.LogError(exception, message);
        }

        public void Info(string message)
        {
            _logger.LogInformation(message);
        }
    }
}
