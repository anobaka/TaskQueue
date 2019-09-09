using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace TaskQueue
{
    public abstract class TaskQueue<TOptions, TTaskHandler> : ConcurrentQueue<TaskData>, ITaskQueue
        where TOptions : TaskQueueOptions, new() where TTaskHandler : ITaskHandler
    {
        private readonly IOptions<TOptions> _options;
        protected readonly TTaskHandler Handler;
        protected readonly IPoolThreadManager PoolThreadManager;
        public bool Running { get; protected set; }
        private readonly object _lock = new object();
        private int _threadCount;
        protected int ThreadCount => _threadCount;

        protected TaskQueue(IOptions<TOptions> options, TTaskHandler handler, IPoolThreadManager poolThreadManager)
        {
            _options = options;
            Handler = handler;
            PoolThreadManager = poolThreadManager;
        }

        public virtual Task<bool> TryEnqueueTaskData(TaskData data)
        {
            if (Handler.CanHandle(data))
            {
                Enqueue(data);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        private async Task _executeTask(TaskData taskData)
        {
            if (!taskData.ExecuteImmediately)
            {
                await Task.Delay(_options.Value.Interval);
            }

            await Handler.Handle(taskData);
            Interlocked.Decrement(ref _threadCount);
            if (PoolThreadManager != null)
            {
                await PoolThreadManager.Release();
            }
        }

        public virtual async Task Start()
        {
            var canStart = false;
            lock (_lock)
            {
                if (!Running)
                {
                    Running = true;
                    canStart = true;
                }
            }

            if (canStart)
            {
                while (Running)
                {
                    TaskData data = null;
                    lock (_lock)
                    {
                        //todo: the sort of queue data is not stable for now.
                        if (_threadCount < _options.Value.MaxThreads && TryDequeue(out data))
                        {
                            if (PoolThreadManager == null || PoolThreadManager.Lock().ConfigureAwait(false)
                                    .GetAwaiter().GetResult())
                            {
                                Interlocked.Increment(ref _threadCount);
                            }
                            else
                            {
                                Enqueue(data);
                            }
                        }
                    }

                    if (data != null)
                    {
                        // Execute asynchronously.
                        _executeTask(data);
                    }
                    else
                    {
                        // Avoid unnecessary cpu usage.
                        await Task.Delay(1);
                    }
                }
            }
        }

        public virtual Task Shutdown()
        {
            Running = false;
            while (Count > 0)
            {
                TryDequeue(out _);
            }

            return Task.CompletedTask;
        }
    }
}