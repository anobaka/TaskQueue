using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ProxyProvider.HttpClientProvider;
using TaskQueue.CommonTaskQueues.Handlers.StatableTaskHandler;

namespace TaskQueue.CommonTaskQueues.Handlers.CrawlerTaskHandler
{
    public abstract class
        CrawlerTaskHandler<TTaskData> : CrawlerTaskHandler<CrawlerTaskHandlerOptions, TTaskData,
            StatableTaskHandlerState> where TTaskData : TaskData

    {
        protected CrawlerTaskHandler(IOptions<CrawlerTaskHandlerOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider) : base(options, taskDistributor, httpClientProvider)
        {
        }
    }

    public abstract class
        CrawlerTaskHandler<TOptions, TTaskData> : CrawlerTaskHandler<TOptions, TTaskData, StatableTaskHandlerState>
        where TTaskData : TaskData
        where TOptions : CrawlerTaskHandlerOptions, new()
    {
        protected CrawlerTaskHandler(IOptions<TOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider) : base(options, taskDistributor, httpClientProvider)
        {
        }
    }

    /// <summary>
    /// Provide proxy client for each request.
    /// </summary>
    /// <typeparam name="TTaskData"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public abstract class
        CrawlerTaskHandler<TOptions, TTaskData, TState> : StatableTaskHandler<TOptions, TTaskData, TState>
        where TTaskData : TaskData
        where TOptions : CrawlerTaskHandlerOptions, new()
        where TState : StatableTaskHandlerState, new()
    {
        protected IHttpClientProvider HttpClientProvider { get; }

        protected CrawlerTaskHandler(IOptions<TOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider) : base(options,
            taskDistributor)
        {
            HttpClientProvider = httpClientProvider;
        }

        protected virtual async Task<HttpClient> GetHttpClient() =>
            await HttpClientProvider.GetClient(Options.HttpClientPurpose);
    }
}