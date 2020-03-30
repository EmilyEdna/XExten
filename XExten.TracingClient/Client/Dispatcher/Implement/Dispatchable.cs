using System;
using System.Collections.Generic;
using System.Text;
using XExten.TracingClient.Client.Dispatcher.Enum;
using XExten.TracingClient.Client.Dispatcher.Interface;
using System.Threading;

namespace XExten.TracingClient.Client.Dispatcher.Implement
{
    public class Dispatchable<T> : IDispatchable
    {
        public DispatchableToken Token { get; }

        public object RawInstance { get; }

        public T Instance { get; }

        public Dispatchable(string token, T instance)
        {
            Token = token;
            RawInstance = Instance = instance;
        }

        private int _counter = 0;

        public SendState State { get; set; } = SendState.Untreated;

        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;

        public int ErrorCount
        {
            get { return _counter; }
        }

        public int Error()=> Interlocked.Increment(ref _counter);

    }
}
