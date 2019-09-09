using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskQueue
{
    /// <summary>
    /// Queue for execution.
    /// </summary>
    public interface ITaskQueue
    {
        /// <summary>
        /// Try to push <see cref="TaskData"/> to the end of queue.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Whether <see cref="TaskData"/> has been enqueued.</returns>
        Task<bool> TryEnqueueTaskData(TaskData data);
        /// <summary>
        /// Start this queue.
        /// </summary>
        /// <returns></returns>
        Task Start();
        /// <summary>
        /// Shutdown this queue.
        /// </summary>
        /// <returns></returns>
        Task Shutdown();
    }
}