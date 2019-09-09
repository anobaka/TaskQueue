using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TaskQueue.CommonTaskQueues.Handlers.DownloadTaskHandler
{
    public interface IDownloadTaskFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rsp"></param>
        /// <param name="stream"></param>
        /// <returns>False for ignore</returns>
        Task<bool> Filter(HttpResponseMessage rsp, out Stream stream);
    }
}
