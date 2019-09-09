using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TaskQueue
{
    public class TaskQueuePool<TOptions, TTaskQueue> : List<TTaskQueue>, ITaskDistributor, IPoolThreadManager
        where TTaskQueue : ITaskQueue
        where TOptions : TaskQueuePoolOptions, new()
    {
        private int _threadCount;
        protected readonly IOptions<TOptions> Options;
        protected readonly object LockObject = new object();
        public int ThreadCount => _threadCount;

        public TaskQueuePool(IOptions<TOptions> options, ILoggerFactory loggerFactory)
        {
            Options = options;
        }

        public virtual async Task Stop()
        {
            
        }

        public virtual async Task Start()
        {
        }

        public async Task Distribute(IEnumerable<TaskData> data)
        {
            foreach (var d in data)
            {
                foreach (var q in this)
                {
                    if (await q.TryEnqueueTaskData(d))
                    {
                        break;
                    }
                }
            }
        }

        public Task Release()
        {
            Interlocked.Decrement(ref _threadCount);
            return Task.CompletedTask;
        }

        public Task<bool> Lock()
        {
            lock (LockObject)
            {
                if (_threadCount < Options.Value.MaxThreads)
                {
                    Interlocked.Increment(ref _threadCount);
                    return Task.FromResult(true);
                }

                return Task.FromResult(true);
            }
        }
    }
}