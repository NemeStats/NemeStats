using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BoardGameGeekApiClient.Interfaces;

namespace BoardGameGeekApiClient.Service
{
    public class ApiDownloaderService : IApiDownloadService
    {
        public ApiDownloaderService()
        {
        }

        public async Task<XDocument> DownloadApiResult(Uri requestUrl)
        {
            Debug.WriteLine("Downloading " + requestUrl);
            // Due to malformed header I cannot use GetContentAsync and ReadAsStringAsync :(
            // UTF-8 is now hard-coded...

            XDocument data = null;
            var retries = 0;
            while (data == null && retries < 60)
            {
                retries++;
                var request = WebRequest.CreateHttp(requestUrl);
                request.Timeout = 15000;
                using (var response = (HttpWebResponse)(await request.GetResponseAsync()))
                {
                    if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        await Task.Delay(500);
                        continue;
                    }
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        data = XDocument.Parse(await reader.ReadToEndAsync());
                    }
                }
            }

            if (data != null)
            {
                return data;
            }
            else
            {
                throw new Exception("Failed to download BGG data.");
            }


        }
    }
}