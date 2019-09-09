namespace TaskQueue.CommonTaskQueues.Handlers.CrawlerTaskHandler
{
    public class CrawlerTaskHandlerOptions : TaskHandlerOptions
    {
        /// <summary>
        /// Purpose for <see cref="ProxyProvider.HttpClientProvider.IHttpClientProvider"/>
        /// </summary>
        public string HttpClientPurpose { get; set; }
    }
}
