using TaskQueue.CommonTaskQueues.Handlers.CrawlerTaskHandler;

namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public class DownloadTaskHandlerOptions : CrawlerTaskHandlerOptions
    {
        public string DownloadPath { get; set; }
    }
}