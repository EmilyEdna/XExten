using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.DataContract;

namespace XExten.TracingClient.Client.Dispatcher.Interface
{
    public interface ITraceDispatcher: IDisposable
    {
        bool Dispatch(Span span);
    }
}
