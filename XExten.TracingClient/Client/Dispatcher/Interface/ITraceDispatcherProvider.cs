using System;
using System.Collections.Generic;
using System.Text;

namespace XExten.TracingClient.Client.Dispatcher.Interface
{
    public interface ITraceDispatcherProvider
    {
        ITraceDispatcher GetDispatcher();
    }
}
