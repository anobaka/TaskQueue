using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace TaskQueue.CommonTaskQueues.Handlers.StatableTaskHandler
{
    public abstract class StatableTaskHandler<TOptions, TTaskData, TState> : TaskHandler<TOptions, TTaskData>, IStatableTaskHandler
        where TOptions : TaskHandlerOptions, new()
        where TTaskData : TaskData
        where TState : StatableTaskHandlerState, new()
    {
        protected int FailureCount;
        protected int CompleteCount;

        protected StatableTaskHandler(IOptions<TOptions> options, ITaskDistributor taskDistributor) : base(options,
            taskDistributor)
        {
        }

        public virtual TState State => new TState
        {
            CompleteCount = CompleteCount,
            FailureCount = FailureCount
        };

        public virtual void ResetState()
        {
            FailureCount = 0;
            CompleteCount = 0;
        }

        protected abstract Task HandleInternalUnstatable(TTaskData taskData,
            CancellationToken ct);

        protected override Task HandleInternal(TTaskData taskData, CancellationToken ct)
        {
            HandleInternalUnstatable(taskData, ct);
            Interlocked.Increment(ref CompleteCount);
            return Task.CompletedTask;
        }

        protected virtual Task OnExceptionUnstatable(TaskData data, Exception e) => Task.CompletedTask;

        protected override Task OnException(TaskData data, Exception e)
        {
            Interlocked.Increment(ref FailureCount);
            return OnExceptionUnstatable(data, e);
        }
    }
}