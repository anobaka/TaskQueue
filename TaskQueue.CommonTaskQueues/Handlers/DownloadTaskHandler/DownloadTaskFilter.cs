using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public class DownloadTaskFilter : IDownloadTaskFilter
    {
        public virtual Task<bool> Filter(HttpResponseMessage message, out Stream stream)
        {
            stream = message.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return Task.FromResult(true);
        }
    }
}