using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.Dispatcher.Enum;

namespace XExten.TracingClient.Client.Dispatcher.Interface
{
    public interface IDispatchable
    {
        DispatchableToken Token { get; }

        object RawInstance { get; }

        SendState State { get; set; }

        int ErrorCount { get; }

        int Error();

        DateTimeOffset Timestamp { get; }
    }
}
