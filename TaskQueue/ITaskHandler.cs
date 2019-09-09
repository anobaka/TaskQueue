using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskQueue
{
    /// <summary>
    /// Task executor.
    /// </summary>
    public interface ITaskHandler
    {
        /// <summary>
        /// How this handler handle <see cref="TaskData"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task Handle(TaskData data);
        /// <summary>
        /// Whether this handler can handle <see cref="TaskData"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool CanHandle(TaskData data);
    }
}