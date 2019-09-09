using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace TaskQueue
{
    public abstract class TaskHandler<TOptions, TTaskData> : ITaskHandler
        where TOptions : TaskHandlerOptions, new() where TTaskData : TaskData
    {
        protected TOptions Options { get; }
        public ITaskDistributor TaskDistributor { get; }

        protected TaskHandler(IOptions<TOptions> options, ITaskDistributor taskDistributor)
        {
            Options = options.Value;
            TaskDistributor = taskDistributor;
        }


        protected abstract Task HandleInternal(TTaskData taskData,
            CancellationToken ct);

        public virtual async Task Handle(TaskData taskData)
        {
            taskData.TryTimes++;
            CancellationToken ct;
            if (Options.Timeout > 0)
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(Options.Timeout);
                ct = cts.Token;
            }
            else
            {
                ct = CancellationToken.None;
            }

            try
            {
                await HandleInternal((TTaskData) taskData, ct);
            }
            catch (Exception e)
            {
                await OnException(taskData, e);
                if (Options.MaxTryTimes == 0 || taskData.TryTimes < Options.MaxTryTimes)
                {
                    await TaskDistributor.Distribute(new List<TaskData> {taskData});
                }
            }
        }

        protected virtual Task OnException(TaskData data, Exception e) => Task.CompletedTask;

        public virtual bool CanHandle(TaskData data)
        {
            return data is TTaskData;
        }
    }
}