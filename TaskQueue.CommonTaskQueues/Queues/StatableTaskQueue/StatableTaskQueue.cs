using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TaskQueue.CommonTaskQueues.Handlers.StatableTaskHandler;

namespace TaskQueue.CommonTaskQueues.Queues.StatableTaskQueue
{
    public class StatableTaskQueue<TTaskHandler> : StatableTaskQueue<TaskQueueOptions, TTaskHandler,
        StatableTaskQueueState> where TTaskHandler : IStatableTaskHandler

    {
        public StatableTaskQueue(IOptions<TaskQueueOptions> options, TTaskHandler handler,
            IPoolThreadManager poolThreadManager = null) : base(options, handler, poolThreadManager)
        {
        }
    }

    public class StatableTaskQueue<TOptions, TTaskHandler, TState> : TaskQueue<TOptions, TTaskHandler>
        where TOptions : TaskQueueOptions, new()
        where TState : StatableTaskQueueState, new()
        where TTaskHandler : IStatableTaskHandler
    {
        protected int EnqueuedDataCount;

        public StatableTaskQueue(IOptions<TOptions> options, TTaskHandler handler,
            IPoolThreadManager poolThreadManager = null) : base(options, handler, poolThreadManager)
        {
        }

        public virtual TState State => new TState
        {
            ThreadCount = ThreadCount,
            EnqueuedDataCount = EnqueuedDataCount,
        };

        public override async Task Start()
        {
            if (!Running)
            {
                EnqueuedDataCount = 0;
                Handler.ResetState();
            }

            await base.Start();
        }

        public override async Task<bool> TryEnqueueTaskData(TaskData data)
        {
            var result = await base.TryEnqueueTaskData(data);
            if (result)
            {
                Interlocked.Increment(ref EnqueuedDataCount);
            }

            return result;
        }
    }
}