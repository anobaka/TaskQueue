using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ProxyProvider.HttpClientProvider;
using TaskQueue.CommonTaskQueues.Handlers.CrawlerTaskHandler;

namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public class DownloadTaskHandler : DownloadTaskHandler<DownloadTaskHandlerOptions, DownloadTaskData,
        DownloadTaskHandlerState>
    {
        public DownloadTaskHandler(IOptions<DownloadTaskHandlerOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider, IDownloadTaskFilter fileFilter) : base(options, taskDistributor,
            httpClientProvider, fileFilter)
        {
        }
    }

    public class DownloadTaskHandler<TTaskData> : DownloadTaskHandler<DownloadTaskHandlerOptions, TTaskData,
        DownloadTaskHandlerState>
        where TTaskData : DownloadTaskData

    {
        public DownloadTaskHandler(IOptions<DownloadTaskHandlerOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider, IDownloadTaskFilter fileFilter) : base(options, taskDistributor,
            httpClientProvider, fileFilter)
        {
        }
    }

    public class
        DownloadTaskHandler<TOptions, TTaskData, TState> : CrawlerTaskHandler<TOptions, TTaskData, TState>
        where TTaskData : DownloadTaskData
        where TOptions : DownloadTaskHandlerOptions, new()
        where TState : DownloadTaskHandlerState, new()
    {
        protected int FilteredCount;
        protected int SkippedCount;
        protected int DownloadedCount;
        protected IDownloadTaskFilter FileFilter { get; }

        public DownloadTaskHandler(IOptions<TOptions> options, ITaskDistributor taskDistributor,
            IHttpClientProvider httpClientProvider, IDownloadTaskFilter fileFilter) : base(options, taskDistributor,
            httpClientProvider)
        {
            FileFilter = fileFilter;
        }

        public override TState State
        {
            get
            {
                var s = base.State;
                s.FilteredCount = FilteredCount;
                s.SkippedCount = SkippedCount;
                s.DownloadedCount = DownloadedCount;
                return s;
            }
        }

        protected virtual Task AfterDownloading(TTaskData taskData, string fileFullname)
        {
            return Task.CompletedTask;
        }

        protected override async Task HandleInternalUnstatable(TTaskData taskData,
            CancellationToken ct)
        {
            var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var filename = Path.GetFileName(taskData.RelativeFilename);
            var newFilename = invalidChars.Aggregate(filename, (current, c) => current.Replace(c.ToString(), ""));
            taskData.RelativeFilename = taskData.RelativeFilename.Replace(filename, newFilename);
            var fullname = Path.Combine(Options.DownloadPath, taskData.RelativeFilename);
            if (!File.Exists(fullname))
            {
                var client = await GetHttpClient();
                var imgRsp = await client.GetAsync(taskData.Url, ct);
                if (imgRsp.StatusCode != HttpStatusCode.NotFound)
                {
                    if (await FileFilter.Filter(imgRsp, out var stream))
                    {
                        Interlocked.Increment(ref FilteredCount);
                    }
                    else
                    {
                        // Seek stream
                        if (stream.Position != 0)
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                        }
                    }

                    var directory = Path.GetDirectoryName(fullname);
                    // Ensure directory exist
                    Directory.CreateDirectory(directory);
                    using (var fs = File.Create(fullname))
                    {
                        await stream.CopyToAsync(fs, (int)stream.Length, ct);
                        await AfterDownloading(taskData, fullname);
                        Interlocked.Increment(ref DownloadedCount);
                    }
                }
            }
            else
            {
                Interlocked.Increment(ref SkippedCount);
            }
        }
    }
}