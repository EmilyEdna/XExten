using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.Configuration;

namespace XExten.TracingClient.Core.Common
{
    public class TraceOptions : TraceConfig, IOptions<TraceOptions>
    {
        public TraceOptions Value => this;
    }
}
