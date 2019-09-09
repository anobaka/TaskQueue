namespace TaskQueue.CommonTaskQueues.Queues.StatableTaskQueue
{
    public class StatableTaskQueueState
    {
        public int EnqueuedDataCount { get; set; }
        public int ThreadCount { get; set; }
    }
}
