using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskQueue
{
    /// <summary>
    /// Distribute list of <see cref="TaskData"/> to others queue.
    /// </summary>
    public interface ITaskDistributor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task Distribute(IEnumerable<TaskData> data);
    }
}
