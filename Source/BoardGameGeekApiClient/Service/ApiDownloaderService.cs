using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using BoardGameGeekApiClient.Interfaces;
using Exceptionless;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace BoardGameGeekApiClient.Service
{
    internal class BGGTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex)
        {
            if (ex is WebException)
                return true;
            return false;
        }
    }

    public class ApiDownloaderService : IApiDownloadService
    {
        private readonly RetryPolicy<BGGTransientErrorDetectionStrategy> _retryPolicy;

        public ApiDownloaderService()
        {
            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
            _retryPolicy = new RetryPolicy<BGGTransientErrorDetectionStrategy>(retryStrategy);
        }

        public XDocument DownloadApiResult(Uri requestUrl)
        {
            Debug.WriteLine("Downloading " + requestUrl);

            

            XDocument result = null;
            _retryPolicy.ExecuteAction(() => ReadApiResult(requestUrl, out result));


            return result;
        }

        private static void ReadApiResult(Uri requestUrl, out XDocument data)
        {
            // Due to malformed header I cannot use GetContentAsync and ReadAsStringAsync :(
            // UTF-8 is now hard-coded...

            //--wait 500ms before each read to avoid BGG block
            Thread.Sleep(500);
            data = null;
            var tryCount = 0;
            while (tryCount < 2)
            {
                var request = WebRequest.CreateHttp(requestUrl);
                request.Timeout = 10000;
                //--setting this so BGG can see the request from NemeStats
                request.UserAgent = "NemeStats website/2.96";
                using (var response = (HttpWebResponse)(request.GetResponse()))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var exception = new Exception($"Expected a 200 OK response from BGG when calling '{requestUrl.AbsolutePath}', but got a '{response.StatusCode}' response.");
                        exception.ToExceptionless();
                        tryCount++;
                    }
                    else
                    {
                        using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            data = XDocument.Parse(reader.ReadToEnd());
                            break;
                        }
                    }
                }
            }
        }
    }
}