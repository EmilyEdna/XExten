using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExten.TracingClient.Client.Common;
using XExten.TracingClient.Client.DataContract;
using XExten.TracingClient.Client.Dispatcher.Interface;

namespace XExten.TracingClient.Client.Dispatcher.Implement
{
    public class TraceDispatcher: ITraceDispatcher
    {

        private const int DefaultBoundedCapacity = 1000000;
        private const int DefaultInterval = 5;
        private readonly int DefaultConsumerCount = Environment.ProcessorCount;
        private readonly int _boundedCapacity;
        private readonly int _consumerCount;
        private readonly int _flushInterval;
        private BlockingCollection<IDispatchable> _limitCollection;
        private CancellationTokenSource _cancellationTokenSource;
        private IEnumerable<IDispatchCallback> _callbacks;
        private ICollection<Task> _consumerTasks;
        private ITraceLoggerFactory _loggerFactory;
        private ITraceLogger _logger;

        public TraceDispatcher(IEnumerable<IDispatchCallback> callbacks, ITraceLoggerFactory loggerFactory, int flushInterval, int boundedCapacity, int consumerCount)
        {
            _callbacks = callbacks;
            _consumerCount = consumerCount <= 0 ? DefaultConsumerCount : consumerCount;
            _boundedCapacity = boundedCapacity <= 0 ? DefaultBoundedCapacity : boundedCapacity;
            _flushInterval = flushInterval <= 0 ? DefaultInterval : flushInterval;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateTraceLogger(typeof(TraceDispatcher));
            _cancellationTokenSource = new CancellationTokenSource();
            _limitCollection = InitializationLimitCollection(_boundedCapacity);
            _consumerTasks = InitializationConsumer(_consumerCount);
        }

        private BlockingCollection<IDispatchable> InitializationLimitCollection(int boundedCapacity)
        {
            _logger.Info($"Trace Client initialized LimitQueue with options: BoundedCapacity={boundedCapacity}");
            return new BlockingCollection<IDispatchable>(boundedCapacity);
        }

        private ICollection<Task> InitializationConsumer(int consumerCount)
        {
            var consumerList = new List<Task>(consumerCount);
            for (var i = 0; i < _consumerCount; i++)
            {
                consumerList.Add(CreateConsumer());
            }
            _logger.Info($"Trace Client initialized ConsumerTasks with options: ConsumerCount={_consumerCount} FlushInterval={_flushInterval}");
            return consumerList;
        }

        private Task CreateConsumer()
        {
            return Task.Factory.StartNew(
                Consumer, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public bool Dispatch(Span span)
        {
            if (_limitCollection.IsAddingCompleted)
            {
                return false;
            }

            _limitCollection.Add(new Dispatchable<Span>(DispatchableToken.SpanToken, span));
            return true;
        }

        private void Consumer()
        {
            using (var handler = new TimerDispatchHandler(_callbacks, _loggerFactory, _flushInterval))
            {
                foreach (var consumingItem in _limitCollection.GetConsumingEnumerable(_cancellationTokenSource.Token))
                    handler.Post(consumingItem);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _limitCollection?.CompleteAdding();
        }
    }
}
