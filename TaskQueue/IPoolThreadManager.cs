using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskQueue
{
    /// <summary>
    /// <see cref="IPoolThreadManager"/> is used to limit the concurrent execution globally.
    /// </summary>
    public interface IPoolThreadManager
    {
        /// <summary>
        /// Release a thread locked by <see cref="Lock"/>
        /// </summary>
        /// <returns></returns>
        Task Release();
        /// <summary>
        /// Try to lock a virtual thread(can be any mechanism) to start a new task execution.
        /// </summary>
        /// <returns>Return true for start a new task execution.</returns>
        Task<bool> Lock();
    }
}