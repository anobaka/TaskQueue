namespace TaskQueue.CommonTaskQueues.Handlers.StatableTaskHandler
{
    public class StatableTaskHandlerState
    {
        public int FailureCount { get; set; }
        public int CompleteCount { get; set; }
    }
}
