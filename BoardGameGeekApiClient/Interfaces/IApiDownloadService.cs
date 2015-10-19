using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BoardGameGeekApiClient.Interfaces
{
    public interface IApiDownloadService
    {
        Task<XDocument> DownloadApiResult(Uri requestUrl);
    }
}