using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TaskQueue.CommonTaskQueues.Pools.StatableTaskQueuePool
{
    public class StatableTaskQueuePool : StatableTaskQueuePool<TaskQueuePoolOptions>
    {
        public StatableTaskQueuePool(IOptions<TaskQueuePoolOptions> options, ILoggerFactory loggerFactory) : base(
            options, loggerFactory)
        {
        }
    }

    public class
        StatableTaskQueuePool<TOptions> : StatableTaskQueuePool<TOptions, ITaskQueue, StatableTaskQueuePoolState>
        where TOptions : TaskQueuePoolOptions, new()
    {
        public StatableTaskQueuePool(IOptions<TOptions> options, ILoggerFactory loggerFactory) :
            base(options, loggerFactory)
        {
        }
    }

    public class StatableTaskQueuePool<TOptions, TTaskQueue, TState> : TaskQueuePool<TOptions, TTaskQueue>
        where TState : StatableTaskQueuePoolState, new()
        where TTaskQueue : ITaskQueue
        where TOptions : TaskQueuePoolOptions, new()
    {
        protected StatableTaskQueuePoolStatus Status { get; set; }

        public StatableTaskQueuePool(IOptions<TOptions> options, ILoggerFactory loggerFactory) : base(
            options,
            loggerFactory)
        {
        }

        public virtual TState State => new TState
        {
            Status = Status,
            ThreadCount = ThreadCount
        };

        protected virtual async Task BeforeStart()
        {

        }

        public override async Task Stop()
        {
            await base.Stop();
            Status = StatableTaskQueuePoolStatus.Idle;
        }

        public override async Task Start()
        {
            var startNew = false;
            lock (LockObject)
            {
                if (Status != StatableTaskQueuePoolStatus.Running)
                {
                    Status = StatableTaskQueuePoolStatus.Running;
                    startNew = true;
                }
            }

            if (startNew)
            {
                await base.Start();
            }
        }
    }
}