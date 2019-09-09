namespace TaskQueue.CommonTaskQueues.Handlers.StatableTaskHandler
{
    public interface IStatableTaskHandler : ITaskHandler
    {
        void ResetState();
    }
}
