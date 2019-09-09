using System;
using System.Collections.Generic;
using System.Text;

namespace TaskQueue
{
    public class TaskHandlerOptions
    {
        /// <summary>
        /// Timeout for task handling.
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// Default max times trying to handle same task data.
        /// </summary>
        public int MaxTryTimes { get; set; }
    }
}