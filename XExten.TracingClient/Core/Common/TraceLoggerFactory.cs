using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.Common;

namespace XExten.TracingClient.Core.Common
{
    public class TraceLoggerFactory : ITraceLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        public ITraceLogger CreateTraceLogger(Type type)
        {
            return new TraceLogger(_loggerFactory.CreateLogger(type));
        }
        public TraceLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
    }
}
