using System;
using System.Xml.Linq;

namespace BoardGameGeekApiClient.Interfaces
{
    public interface IApiDownloadService
    {
        XDocument DownloadApiResult(Uri requestUrl);
    }
}