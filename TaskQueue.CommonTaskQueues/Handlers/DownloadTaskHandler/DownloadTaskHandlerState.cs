using TaskQueue.CommonTaskQueues.Handlers.StatableTaskHandler;

namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public class DownloadTaskHandlerState : StatableTaskHandlerState
    {
        public int FilteredCount { get; set; }
        public int SkippedCount { get; set; }
        public int DownloadedCount { get; set; }
    }
}