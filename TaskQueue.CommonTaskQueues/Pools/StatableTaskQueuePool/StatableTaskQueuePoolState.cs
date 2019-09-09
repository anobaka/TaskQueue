namespace TaskQueue.CommonTaskQueues.Pools.StatableTaskQueuePool
{
    public class StatableTaskQueuePoolState
    {
        public StatableTaskQueuePoolStatus Status { get; set; }
        public int ThreadCount { get; set; }
    }
}
